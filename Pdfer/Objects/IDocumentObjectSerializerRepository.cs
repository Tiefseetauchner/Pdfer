namespace Pdfer.Objects;

public interface IDocumentObjectSerializerRepository
{
  IDocumentObjectSerializer GetSerializer(DocumentObject documentObject);
  IDocumentObjectSerializer<TObjectType> GetSerializer<TObjectType>() where TObjectType : DocumentObject;
  void AddSerializer<TObjectType>(IDocumentObjectSerializer<TObjectType> documentObjectReader) where TObjectType : DocumentObject;
}