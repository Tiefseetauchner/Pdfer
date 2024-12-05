using System;
using System.Collections.Generic;

namespace Pdfer.Objects;

public class DocumentObjectSerializerRepository : IDocumentObjectSerializerRepository
{
  private readonly Dictionary<Type, IDocumentObjectSerializer> _serializers = [];

  public IDocumentObjectSerializer<TObjectType> GetSerializer<TObjectType>(TObjectType documentObject) where TObjectType : DocumentObject =>
    _serializers[documentObject.GetType()] as IDocumentObjectSerializer<TObjectType>
    ?? throw new InvalidOperationException($"The serializer for '{typeof(TObjectType)}' could not be found in the {nameof(DocumentObjectSerializerRepository)}");

  public void AddSerializer<TObjectType>(IDocumentObjectSerializer<TObjectType> documentObjectReader) where TObjectType : DocumentObject =>
    _serializers[typeof(TObjectType)] = documentObjectReader;
}