using Pdfer.Objects;

namespace Pdfer;

public class PdfDocumentPartParserFactory : IPdfDocumentPartParserFactory
{
  public IPdfDocumentPartParser Create()
  {
    var streamHelper = new StreamHelper();

    var pdfObjectReader = PdfObjectReaderFactory.Create();

    var indirectPdfObjectReaderAdapter = new IndirectPdfObjectReaderAdapter(
      pdfObjectReader,
      streamHelper);

    var pdfDictionaryHelper = new PdfDictionaryHelper(streamHelper, pdfObjectReader);

    return new PdfDocumentPartParser(
      streamHelper,
      pdfDictionaryHelper,
      indirectPdfObjectReaderAdapter);
  }
}