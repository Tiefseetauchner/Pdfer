namespace Pdfer.Objects.Readers;

public interface IDocumentObjectReaderRepository
{
  IDocumentObjectReader<TObjectType> GetReader<TObjectType>() where TObjectType : DocumentObject;

  void AddReader<TObjectType>(IDocumentObjectReader<TObjectType> documentObjectReader) where TObjectType : DocumentObject;
}