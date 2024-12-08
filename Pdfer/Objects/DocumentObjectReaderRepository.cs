using System;
using System.Collections.Generic;

namespace Pdfer.Objects;

public class DocumentObjectReaderRepository : IDocumentObjectReaderRepository
{
  private readonly Dictionary<Type, IDocumentObjectReader> _objectReaders = [];

  public IDocumentObjectReader<TObjectType> GetReader<TObjectType>() where TObjectType : DocumentObject =>
    _objectReaders[typeof(TObjectType)] as IDocumentObjectReader<TObjectType>
    ?? throw new ArgumentException($"No reader found for type {typeof(TObjectType)}");

  public void AddReader<TObjectType>(IDocumentObjectReader<TObjectType> documentObjectReader) where TObjectType : DocumentObject =>
    _objectReaders[typeof(TObjectType)] = documentObjectReader;
}