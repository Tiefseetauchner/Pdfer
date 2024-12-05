namespace Pdfer.Objects;

public interface IDocumentObjectSerializerRepository
{
  IDocumentObjectSerializer<TObjectType> GetSerializer<TObjectType>(TObjectType documentObject) where TObjectType : DocumentObject;
  IDocumentObjectSerializer<TObjectType> GetSerializer<TObjectType>() where TObjectType : DocumentObject;
  void AddSerializer<TObjectType>(IDocumentObjectSerializer<TObjectType> documentObjectReader) where TObjectType : DocumentObject;
}