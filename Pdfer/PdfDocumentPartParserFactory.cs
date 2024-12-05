using Pdfer.Objects;

namespace Pdfer;

public class PdfDocumentPartParserFactory : IPdfDocumentPartParserFactory
{
  public PdfDocumentPartParser Create()
  {
    var streamHelper = new StreamHelper();

    var documentObjectReaderRepository = new DocumentObjectReaderRepository();

    var pdfObjectReader = new PdfObjectReader(
      streamHelper,
      documentObjectReaderRepository);

    var pdfDictionaryHelper = new PdfDictionaryHelper(streamHelper, pdfObjectReader);
    var dictionaryObjectReader = new DictionaryObjectReader(pdfDictionaryHelper);

    documentObjectReaderRepository.AddReader<ArrayObject>(new ArrayObjectReader(streamHelper, pdfObjectReader));
    documentObjectReaderRepository.AddReader<DictionaryObject>(dictionaryObjectReader);
    documentObjectReaderRepository.AddReader<IndirectObject>(new IndirectObjectReader());
    documentObjectReaderRepository.AddReader<NameObject>(new NameObjectReader());
    documentObjectReaderRepository.AddReader<NumericObject>(new NumericObjectReader());
    documentObjectReaderRepository.AddReader<StreamObject>(new StreamObjectReader(dictionaryObjectReader, streamHelper));
    documentObjectReaderRepository.AddReader<StringObject>(new StringObjectReader());


    return new PdfDocumentPartParser(
      streamHelper,
      pdfDictionaryHelper,
      pdfObjectReader);
  }
}

public interface IPdfDocumentPartParserFactory
{
  PdfDocumentPartParser Create();
}