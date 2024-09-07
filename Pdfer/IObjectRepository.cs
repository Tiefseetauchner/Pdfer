using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Pdfer.Objects;

namespace Pdfer;

public interface IObjectRepository
{
  Dictionary<ObjectIdentifier, DocumentObject> Objects { get; }
  Task<T?> RetrieveObject<T>(ObjectIdentifier objectIdentifier, XRefEntry xRefEntry, Stream stream)
    where T : DocumentObject;
}

public class ObjectRepository(
  IPdfObjectReader pdfObjectReader) : IObjectRepository
{
  public Dictionary<ObjectIdentifier, DocumentObject> Objects => _objects;
  private readonly Dictionary<ObjectIdentifier, DocumentObject> _objects = new();

  public async Task<T?> RetrieveObject<T>(ObjectIdentifier objectIdentifier, XRefEntry xRefEntry, Stream stream)
    where T : DocumentObject
  {
    if (_objects.TryGetValue(objectIdentifier, out var obj))
      return obj as T;
    
    var pdfObject = await pdfObjectReader.Read(stream, xRefEntry, this);
    
    _objects.Add(objectIdentifier, pdfObject);

    return pdfObject as T;
  }

}