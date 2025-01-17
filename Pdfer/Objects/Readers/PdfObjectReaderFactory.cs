namespace Pdfer.Objects.Readers;

public static class PdfObjectReaderFactory
{
  public static PdfObjectReader Create()
  {
    var documentObjectReaderRepository = new DocumentObjectReaderRepository();

    var pdfObjectReader = new PdfObjectReader(
      documentObjectReaderRepository);

    var pdfDictionaryHelper = new PdfDictionaryHelper(pdfObjectReader);
    var dictionaryObjectReader = new DictionaryObjectReader(pdfDictionaryHelper);

    documentObjectReaderRepository.AddReader(new ArrayObjectReader(pdfObjectReader));
    documentObjectReaderRepository.AddReader(new BooleanObjectReader());
    documentObjectReaderRepository.AddReader(dictionaryObjectReader);
    documentObjectReaderRepository.AddReader(new ReferenceObjectReader());
    documentObjectReaderRepository.AddReader(new NameObjectReader());
    documentObjectReaderRepository.AddReader(new NullObjectReader());
    documentObjectReaderRepository.AddReader(new NumericObjectReader());
    documentObjectReaderRepository.AddReader(new StreamObjectReader(dictionaryObjectReader));
    documentObjectReaderRepository.AddReader(new StringObjectReader());

    return pdfObjectReader;
  }
}