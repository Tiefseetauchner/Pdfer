using System;
using System.Collections.Generic;

namespace Pdfer.Objects.Readers;

public class DocumentObjectReaderRepository : IDocumentObjectReaderRepository
{
  private readonly Dictionary<Type, IDocumentObjectReader> _objectReaders = [];

  public IDocumentObjectReader<TObjectType> GetReader<TObjectType>() where TObjectType : DocumentObject
  {
    if (!_objectReaders.TryGetValue(typeof(TObjectType), out var reader))
      throw new ArgumentException($"No reader found for type {typeof(TObjectType)}");

    return (IDocumentObjectReader<TObjectType>)reader;
  }

  public void AddReader<TObjectType>(IDocumentObjectReader<TObjectType> documentObjectReader) where TObjectType : DocumentObject =>
    _objectReaders[typeof(TObjectType)] = documentObjectReader;
}