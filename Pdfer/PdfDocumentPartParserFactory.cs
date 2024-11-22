using Pdfer.Objects;

namespace Pdfer;

public class PdfDocumentPartParserFactory : IPdfDocumentPartParserFactory
{
  public PdfDocumentPartParser Create()
  {
    var streamHelper = new StreamHelper();
    var pdfDictionaryHelper = new PdfDictionaryHelper(streamHelper);
    var pdfArrayHelper = new PdfArrayHelper(streamHelper);

    var pdfObjectReader = new PdfObjectReader(
      pdfDictionaryHelper,
      streamHelper,
      new DictionaryObjectReader(
        streamHelper,
        pdfDictionaryHelper),
      new StringObjectReader(),
      new StreamObjectReader(pdfDictionaryHelper, streamHelper),
      new NumberObjectReader(streamHelper),
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