namespace Pdfer;

public class PdfDocumentWriterFactory
{
  public PdfDocumentWriter Create()
  {
    var streamHelper = new StreamHelper();
    var pdfDictionaryHelper = new PdfDictionaryHelper(streamHelper);
    return new PdfDocumentWriter(pdfDictionaryHelper);
  }
}