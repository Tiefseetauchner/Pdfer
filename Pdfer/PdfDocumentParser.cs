using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer;

public class PdfDocumentParser(PdfDocumentPartParser pdfDocumentPartParser)
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
    var header = await GetHeader(stream);

    var parts = await pdfDocumentPartParser.Parse(stream);

    return new PdfDocument(header, parts);
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
}