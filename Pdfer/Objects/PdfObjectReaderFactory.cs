using Pdfer.Objects;

namespace Pdfer;

public static class PdfObjectReaderFactory
{
  public static IPdfObjectReader Create()
  {
    var streamHelper = new StreamHelper();

    var documentObjectReaderRepository = new DocumentObjectReaderRepository();

    var pdfObjectReader = new PdfObjectReader(
      streamHelper,
      documentObjectReaderRepository);

    var pdfDictionaryHelper = new PdfDictionaryHelper(streamHelper, pdfObjectReader);
    var dictionaryObjectReader = new DictionaryObjectReader(pdfDictionaryHelper);

    documentObjectReaderRepository.AddReader(new ArrayObjectReader(streamHelper, pdfObjectReader));
    documentObjectReaderRepository.AddReader(new BooleanObjectReader(streamHelper));
    documentObjectReaderRepository.AddReader(dictionaryObjectReader);
    documentObjectReaderRepository.AddReader(new IndirectObjectReader());
    documentObjectReaderRepository.AddReader(new NameObjectReader());
    documentObjectReaderRepository.AddReader(new NullObjectReader());
    documentObjectReaderRepository.AddReader(new NumericObjectReader());
    documentObjectReaderRepository.AddReader(new StreamObjectReader(dictionaryObjectReader, streamHelper));
    documentObjectReaderRepository.AddReader(new StringObjectReader());

    return pdfObjectReader;
  }
}