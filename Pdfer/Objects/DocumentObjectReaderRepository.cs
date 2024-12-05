using System;
using System.Collections.Generic;

namespace Pdfer.Objects;

public class DocumentObjectReaderRepository : IDocumentObjectReaderRepository
{
  private readonly Dictionary<Type, IDocumentObjectReader> _objectReaders = [];

  public IDocumentObjectReader GetReader<TObjectType>() where TObjectType : DocumentObject =>
    _objectReaders[typeof(TObjectType)];

  public void AddReader<TObjectType>(IDocumentObjectReader documentObjectReader) where TObjectType : DocumentObject =>
    _objectReaders[typeof(TObjectType)] = documentObjectReader;
}