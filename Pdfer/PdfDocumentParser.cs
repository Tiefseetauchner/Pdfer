using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pdfer.Objects;

namespace Pdfer;

public class PdfDocumentParser(
  IStreamHelper streamHelper,
  IDocumentObjectReader<DictionaryObject> dictionaryObjectReader,
  IPdfDictionaryHelper pdfDictionaryHelper,
  IPdfObjectReader pdfObjectReader)
{
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
    var body = await GetBody(streamReader, xrefTable, trailer);

    return new PdfDocument(header, body, xrefTable, trailer);
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
      var line = await streamHelper.ReadReverseLine(streamReader);

      if (string.IsNullOrWhiteSpace(line))
        continue;

      if (line != "%%EOF")
        throw new InvalidOperationException("No EOF found");

      eofFound = true;
    }
  }

  private async Task<long> GetXrefOffset(StreamReader streamReader)
  {
    var xrefOffsetString = await streamHelper.ReadReverseLine(streamReader);
    var xrefOffsetParsed = long.TryParse(xrefOffsetString, out var xrefOffset);
    if (!xrefOffsetParsed)
      throw new InvalidOperationException("xref offset is not a number");
    return xrefOffset;
  }

  private async Task VerifyStartxrefExists(StreamReader streamReader)
  {
    var startXref = await streamHelper.ReadReverseLine(streamReader);

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

    await streamHelper.ReadStreamTo("<<", stream);

    return await pdfDictionaryHelper.ReadDictionary(stream);
  }


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

  private async Task<Body> GetBody(StreamReader streamReader, XRefTable xRefTable, Trailer trailer)
  {
    var objectDictionary = new Dictionary<ObjectIdentifier, DocumentObject>();

    var usedXrefEntries = xRefTable
      .Where(entry => entry.Value.Flag == XRefEntryType.Used);
    
    foreach (var xRefEntry in usedXrefEntries)
    {
      var pdfObject = await pdfObjectReader.Read(streamReader, xRefEntry.Value.Position);
      objectDictionary.Add(xRefEntry.Key, pdfObject);
    }

    return new Body(objectDictionary);
  }

  private async Task<DocumentObject> GetObject(Stream stream, XRefTable xRefTable, ObjectIdentifier objectIdentifier)
  {
    stream.Position = xRefTable[objectIdentifier].Position;

    return await dictionaryObjectReader.Read(stream);
  }
}