namespace Pdfer.Objects.ObjectReaders;

public interface IDocumentObjectReaderRepository
{
  IDocumentObjectReader<TObjectType> GetReader<TObjectType>() where TObjectType : DocumentObject;

  void AddReader<TObjectType>(IDocumentObjectReader<TObjectType> documentObjectReader) where TObjectType : DocumentObject;
}