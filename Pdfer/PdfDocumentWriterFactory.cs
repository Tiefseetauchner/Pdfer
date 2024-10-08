using Pdfer.Objects;

namespace Pdfer;

public class PdfDocumentWriterFactory
{
  public PdfDocumentWriter Create()
  {
    var streamHelper = new StreamHelper();
    var pdfDictionaryHelper = new PdfDictionaryHelper(streamHelper);
    var pdfArrayHelper = new PdfArrayHelper(streamHelper);
    return new PdfDocumentWriter(pdfDictionaryHelper,
      new DictionaryObjectSerializer(pdfDictionaryHelper),
      new NumberObjectSerializer(),
      new StreamObjectSerializer(pdfDictionaryHelper),
      new StringObjectSerializer(),
      new NameObjectSerializer(),
      new ArrayObjectSerializer(pdfArrayHelper));
  }
}