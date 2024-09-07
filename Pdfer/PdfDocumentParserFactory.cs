using Pdfer.Objects;

namespace Pdfer;

public class PdfDocumentParserFactory : IPdfDocumentParserFactory
{
  public PdfDocumentParser Create()
  {
    var streamHelper = new StreamHelper();
    var pdfDictionaryHelper = new PdfDictionaryHelper(streamHelper);
    var dictionaryObjectReader = new DictionaryObjectReader(
      streamHelper,
      pdfDictionaryHelper);
    var pdfObjectReader = new PdfObjectReader(
      dictionaryObjectReader,
      new StringObjectReader());

    return new PdfDocumentParser(
      streamHelper,
      dictionaryObjectReader,
      new PdfDictionaryHelper(streamHelper),
      pdfObjectReader);
  }
}

public interface IPdfDocumentParserFactory
{
  PdfDocumentParser Create();
}