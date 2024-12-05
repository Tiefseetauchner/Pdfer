using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Pdfer.Objects;

namespace Pdfer;

public class ObjectRepository(
  IPdfObjectReader pdfObjectReader) : IObjectRepository
{
  public Dictionary<ObjectIdentifier, DocumentObject> Objects { get; } = new();

  public async Task<T?> RetrieveObject<T>(ObjectIdentifier objectIdentifier, Stream stream)
    where T : DocumentObject
  {
    if (Objects.TryGetValue(objectIdentifier, out var obj))
      return obj as T;

    var pdfObject = await pdfObjectReader.Read(stream);

    Objects.Add(objectIdentifier, pdfObject);

    return pdfObject as T;
  }
}