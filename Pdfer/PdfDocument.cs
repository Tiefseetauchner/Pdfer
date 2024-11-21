using System.Collections.Generic;

namespace Pdfer;

public class PdfDocument(
  Header pdfHeader,
  List<PdfDocumentPart> documentParts)
{
  public Header PdfHeader => pdfHeader;
  public List<PdfDocumentPart> DocumentParts => documentParts;
}