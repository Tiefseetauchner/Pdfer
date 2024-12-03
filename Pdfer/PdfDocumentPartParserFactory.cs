using Pdfer.Objects;

namespace Pdfer;

public class PdfDocumentPartParserFactory : IPdfDocumentPartParserFactory
{
  public PdfDocumentPartParser Create()
  {
    var streamHelper = new StreamHelper();
    var pdfDictionaryHelper = new PdfDictionaryHelper(streamHelper);
    var pdfArrayHelper = new PdfArrayHelper(streamHelper);
    var dictionaryObjectReader = new DictionaryObjectReader(pdfDictionaryHelper);

    var pdfObjectReader = new PdfObjectReader(
      pdfDictionaryHelper,
      streamHelper,
      dictionaryObjectReader,
      new StringObjectReader(),
      new StreamObjectReader(dictionaryObjectReader, streamHelper),
      new NumericObjectReader(),
      new NameObjectReader(),
      new ArrayObjectReader(pdfArrayHelper));

    return new PdfDocumentPartParser(
      streamHelper,
      new PdfDictionaryHelper(streamHelper),
      pdfObjectReader);
  }
}

public interface IPdfDocumentPartParserFactory
{
  PdfDocumentPartParser Create();
}