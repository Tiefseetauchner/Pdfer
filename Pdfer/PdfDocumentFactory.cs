using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer;

public class PdfDocumentFactory
{
  public static PdfDocumentFactory Instance { get; } = new PdfDocumentFactory();

  private const int HeaderLengthInBytes = 8;
  private const string HeaderBytes = "%PDF-";


  public async Task<PdfDocument> Parse(byte[] bytes)
  {
    using var stream = new MemoryStream(bytes);
    return await Parse(stream);
  }

  public async Task<PdfDocument> Parse(Stream stream)
  {
    using var streamReader = new StreamReader(stream);

    var header = await GetHeader(stream);
    var trailer = await GetTrailer(stream, streamReader);
    var xrefTable = await GetXrefTable(streamReader, trailer.XRefByteOffset);
    var body = await GetBody(streamReader, xrefTable);

    return new PdfDocument(header, new Body(new Dictionary<ObjectIdentifier, DocumentObject>()), xrefTable, trailer);
  }

  // TODO (lena): Operate on Stream
  private async Task<Header> GetHeader(Stream stream)
  {
    var buffer = new byte[HeaderLengthInBytes];
    var bytesRead = await stream.ReadAsync(buffer);

    if (bytesRead != HeaderLengthInBytes)
      throw new IOException("Unexpected end of stream");

    if (HeaderBytes
        .Where((c, i) => c != buffer[i])
        .Any())
      throw new InvalidOperationException("Invalid header");

    if (buffer[5] != '1' || buffer[6] != '.')
      throw new InvalidOperationException($"Invalid version number '{Encoding.UTF8.GetString(buffer[5..])}'");

    var pdfVersion = GetPdfVersion(buffer);

    // TODO (lena): do containsBinaryDataHeader check
    return new Header(pdfVersion, true);
  }

  private static PdfVersion GetPdfVersion(byte[] buffer) =>
    (char)buffer[7] switch
    {
      '0' => PdfVersion.Pdf10,
      '1' => PdfVersion.Pdf11,
      '2' => PdfVersion.Pdf12,
      '3' => PdfVersion.Pdf13,
      '4' => PdfVersion.Pdf14,
      '5' => PdfVersion.Pdf15,
      '6' => PdfVersion.Pdf16,
      '7' => PdfVersion.Pdf17,
      _ => throw new InvalidOperationException($"Invalid version number '{Encoding.UTF8.GetString(buffer[5..])}'")
    };

  private async Task<Trailer> GetTrailer(Stream stream, StreamReader streamReader)
  {
    await using var reverseStream = new ReverseStream(stream);
    reverseStream.Position = 0;
    using var reverseStreamReader = new StreamReader(reverseStream);

    await VerifyEofExists(reverseStreamReader);

    var xrefOffset = await GetXrefOffset(reverseStreamReader);

    await VerifyStartxrefExists(reverseStreamReader);

    var trailerDictionary = await GetTrailerDictionary(reverseStream, stream, streamReader);

    return new Trailer(trailerDictionary, xrefOffset);
  }

  private async Task VerifyEofExists(StreamReader streamReader)
  {
    var eofFound = false;
    while (!eofFound)
    {
      var line = await ReadReverseLine(streamReader);

      if (string.IsNullOrWhiteSpace(line))
        continue;

      if (line != "%%EOF")
        throw new InvalidOperationException("No EOF found");

      eofFound = true;
    }
  }

  private async Task<long> GetXrefOffset(StreamReader streamReader)
  {
    var xrefOffsetString = await ReadReverseLine(streamReader);
    var xrefOffsetParsed = long.TryParse(xrefOffsetString, out var xrefOffset);
    if (!xrefOffsetParsed)
      throw new InvalidOperationException("xref offset is not a number");
    return xrefOffset;
  }

  private async Task VerifyStartxrefExists(StreamReader streamReader)
  {
    var startXref = await ReadReverseLine(streamReader);

    if (startXref != "startxref")
      throw new InvalidOperationException("No 'startxref' keyword found");
  }

  private async Task<Dictionary<string, string>> GetTrailerDictionary(Stream reverseStream, Stream stream, StreamReader streamReader)
  {
    reverseStream.Position = 0;

    var buffer = new byte[7];
    var bytesRead = await reverseStream.ReadAsync(buffer);
    while (bytesRead == 7 && !buffer.Reverse().SequenceEqual("trailer".ToCharArray().Select(_ => (byte)_)))
    {
      reverseStream.Position -= 6;
      bytesRead = await reverseStream.ReadAsync(buffer);
    }

    if (bytesRead != 7)
      throw new InvalidOperationException("No 'trailer' keyword found");

    var trailerDictionary = new Dictionary<string, string>();
    ReadStreamTo("<<", stream);
    using var memoryStream = new MemoryStream(ReadStreamTo(">>", stream));
    ReadStreamTo("/", memoryStream); // NOTE (lena): Skip to the first entry
    Console.WriteLine(memoryStream.Position);
    while (memoryStream.Position < memoryStream.Length && ReadStreamTo("/", memoryStream) is { } dictEntry)
    {
      var dictEntryString = Encoding.UTF8.GetString(dictEntry);
      var dictEntrySplit = dictEntryString.Split(' ', 2);

      // NOTE (lena): Due to some fun producers of PDFs, we might get a non-standard conforming PDF.
      //              This might mean there's a DocChecksum element with the value starting with a /.
      //              We could handle that. Ooooor we don't. My choice here is to ignore it. :)
      if (dictEntrySplit.Length != 2)
        continue;

      trailerDictionary.Add(dictEntryString.Split(' ')[0].Trim(), dictEntrySplit[1].Trim());
    }

    return trailerDictionary;
  }

  private byte[] ReadStreamTo(string s, Stream stream)
  {
    var outputBuffer = new List<byte>();

    var buffer = new byte[s.Length];
    var bytesRead = stream.Read(buffer);

    while (bytesRead == s.Length && !buffer.SequenceEqual(s.ToCharArray().Select(_ => (byte)_)))
    {
      outputBuffer.Add(buffer[0]);

      stream.Position -= bytesRead - 1;

      bytesRead = stream.Read(buffer);
    }

    return outputBuffer.ToArray();
  }

  private async Task<string> ReadReverseLine(StreamReader streamReader) =>
    string.Concat((await streamReader.ReadLineAsync())?.Reverse() ?? throw new IOException("Unexpected end of stream"));

  private async Task<XRefTable> GetXrefTable(StreamReader streamReader, long xrefOffset)
  {
    streamReader.BaseStream.Position = xrefOffset;
    streamReader.DiscardBufferedData();

    var xRefTable = new XRefTable();
    var line = await streamReader.ReadLineAsync();

    if (line != "xref")
      throw new InvalidOperationException("No 'xref' keyword found");

    while ((line = await streamReader.ReadLineAsync()) != null && line != "trailer")
    {
      var (objectNumber, objectsInSection) = ParseSubsectionHeader(line);

      await AddXRefEntriesInSubsection(objectsInSection, streamReader, objectNumber, xRefTable);
    }

    return xRefTable;
  }

  private async Task AddXRefEntriesInSubsection(int objectsInSection, StreamReader streamReader, int objectNumber, XRefTable xRefTable)
  {
    for (var i = 0; i < objectsInSection; i++)
    {
      var line = await streamReader.ReadLineAsync();

      if (string.IsNullOrWhiteSpace(line))
        throw new IOException("Unexpected end of stream");

      var (objectIdentifier, xRefEntry) = GetXrefEntry(line, objectNumber);
      xRefTable.Add(objectIdentifier, xRefEntry);
      objectNumber++;
    }
  }

  private (int ObjectNumber, int RemainingInSection) ParseSubsectionHeader(string line)
  {
    var subsectionHeaderParts = line.Split(' ');
    return (int.Parse(subsectionHeaderParts[0]), int.Parse(subsectionHeaderParts[1]));
  }

  private (ObjectIdentifier, XRefEntry) GetXrefEntry(string line, int objectNumber)
  {
    var objectEntryParts = line.Split(' ');
    var offset = long.Parse(objectEntryParts[0]);
    var generationNumber = int.Parse(objectEntryParts[1]);
    var type = objectEntryParts[2] switch
    {
      "f" => XRefEntryType.Free,
      "n" => XRefEntryType.Used,
      _ => throw new InvalidOperationException($"Invalid xref entry type '{objectEntryParts[2]}'")
    };
    return (new ObjectIdentifier(objectNumber, generationNumber), new XRefEntry(offset, type));
  }

  private async Task<Body> GetBody(StreamReader streamReader, XRefTable xRefTable)
  {
    foreach (var (objectIdentifier, xRefEntry) in xRefTable)
    {
      if (xRefEntry.Flag == XRefEntryType.Free)
        continue;

      streamReader.BaseStream.Position = xRefEntry.Position;
      streamReader.DiscardBufferedData();

      var objectIdentifierString = await streamReader.ReadLineAsync();

      if (objectIdentifierString == null)
        throw new IOException("Unexpected end of stream");
    }

    return new Body(new Dictionary<ObjectIdentifier, DocumentObject>());
  }
}