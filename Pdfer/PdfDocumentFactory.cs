using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pdfer;

public class PdfDocumentFactory
{
  public static PdfDocumentFactory Instance { get; } = new PdfDocumentFactory();
  
  private const int HeaderLengthInBytes = 8;
  private const string HeaderBytes = "%PDF-";


  public PdfDocument Parse(byte[] bytes)
  {
    using var stream = new MemoryStream(bytes);
    return Parse(stream);
  }

  public PdfDocument Parse(Stream stream)
  {
    var header = GetHeader(stream);
    //var trailer = GetTrailer(stream);

    return new PdfDocument(header, new Body(new Dictionary<ObjectIdentifier, DocumentObject>()), new XRefTable([]), new Trailer(new Dictionary<string, string>(), 0));
  }

  private Header GetHeader(Stream stream)
  {
    var buffer = new byte[HeaderLengthInBytes];
    var bytesRead = stream.Read(buffer);
    
    if (bytesRead != HeaderLengthInBytes)
      throw new IOException("Unexpected end of stream");

    if (HeaderBytes
        .Where((c, i) => c != buffer[i])
        .Any())
      throw new InvalidOperationException("Invalid header");

    if (buffer[5] != '1' && buffer[6] != '.')
      throw new InvalidOperationException($"Invalid version number '{System.Text.Encoding.UTF8.GetString(buffer[5..])}'");

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
      _ => throw new InvalidOperationException($"Invalid version number '{System.Text.Encoding.UTF8.GetString(buffer[5..])}'")
    };
    
    return new Header(pdfVersion, true);
  }

  //private Trailer GetTrailer(Stream stream)
  //{
  //}
}