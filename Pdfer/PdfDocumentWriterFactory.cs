using Pdfer.Objects.Serializers;

namespace Pdfer;

public static class PdfDocumentWriterFactory
{
  public static PdfDocumentWriter Create()
  {
    var objectSerializerRepository = DocumentObjectSerializerRepositoryFactory.CreateForAllSerializers();

    return new PdfDocumentWriter(objectSerializerRepository);
  }
}