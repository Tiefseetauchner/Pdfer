namespace Pdfer.Objects;

public interface IDocumentObjectReaderRepository
{
  IDocumentObjectReader GetReader<TObjectType>() where TObjectType : DocumentObject;
  void AddReader<TObjectType>(IDocumentObjectReader documentObjectReader) where TObjectType : DocumentObject;
}