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
    var header = await GetHeader(stream);
    var trailer = await GetTrailer(stream);
    //var xrefTable = await GetXrefTable(stream);

    return new PdfDocument(header, new Body(new Dictionary<ObjectIdentifier, DocumentObject>()), new XRefTable([]), trailer);
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

    if (buffer[5] != '1' && buffer[6] != '.')
      throw new InvalidOperationException($"Invalid version number '{Encoding.UTF8.GetString(buffer[5..])}'");

    var pdfVersion = (char)buffer[7] switch
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

    return new Header(pdfVersion, true);
  }

  private async Task<Trailer> GetTrailer(Stream stream)
  {
    await using var reverseStream = new ReverseStream(stream);
    reverseStream.Position = 0;
    using var streamReader = new StreamReader(reverseStream);

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

    var xrefOffset = await ReadReverseLine(streamReader);
    var startXref = await ReadReverseLine(streamReader);

    if (startXref != "startxref")
      throw new InvalidOperationException("No 'startxref' keyword found");

    // TODO (tiefseetauchner): Parse Trailer Dictionary

    return new Trailer([], long.Parse(xrefOffset));
  }

  private async Task<string> ReadReverseLine(StreamReader streamReader) =>
    string.Concat((await streamReader.ReadLineAsync())?.Reverse() ?? throw new IOException("Unexpected end of stream"));
}