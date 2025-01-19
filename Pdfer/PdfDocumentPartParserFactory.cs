using Pdfer.Objects;
using Pdfer.Objects.Readers;

namespace Pdfer;

public class PdfDocumentPartParserFactory : IPdfDocumentPartParserFactory
{
  public IPdfDocumentPartParser Create()
  {
    var pdfObjectReader = PdfObjectReaderFactory.Create();

    var indirectPdfObjectReaderAdapter = new IndirectPdfObjectReader(
      pdfObjectReader);

    var pdfDictionaryHelper = new PdfDictionaryHelper(pdfObjectReader);

    return new PdfDocumentPartParser(
      pdfDictionaryHelper,
      indirectPdfObjectReaderAdapter);
  }
}