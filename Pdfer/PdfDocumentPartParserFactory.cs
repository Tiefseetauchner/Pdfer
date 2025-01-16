using Pdfer.Objects.ObjectReaders;
using Pdfer.Objects.Readers;

namespace Pdfer;

public class PdfDocumentPartParserFactory : IPdfDocumentPartParserFactory
{
  public IPdfDocumentPartParser Create()
  {
    var pdfObjectReader = PdfObjectReaderFactory.Create();

    var indirectPdfObjectReaderAdapter = new IndirectPdfObjectReaderAdapter(
      pdfObjectReader);

    var pdfDictionaryHelper = new PdfDictionaryHelper(pdfObjectReader);

    return new PdfDocumentPartParser(
      pdfDictionaryHelper,
      indirectPdfObjectReaderAdapter);
  }
}