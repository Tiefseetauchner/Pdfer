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
    var indirectPdfObjectReaderAdapter = new IndirectPdfObjectReaderAdapter(
      pdfObjectReader,
      streamHelper);

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


    return new PdfDocumentPartParser(
      streamHelper,
      pdfDictionaryHelper,
      indirectPdfObjectReaderAdapter);
  }
}

public interface IPdfDocumentPartParserFactory
{
  PdfDocumentPartParser Create();
}