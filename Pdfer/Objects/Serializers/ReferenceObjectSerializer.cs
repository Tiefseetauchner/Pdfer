using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects.Serializers;

public class ReferenceObjectSerializer : IDocumentObjectSerializer<ReferenceObject>
{
  async Task IDocumentObjectSerializer.Serialize(Stream stream, DocumentObject documentObject) =>
    await Serialize(stream, (ReferenceObject)documentObject);

  public async Task Serialize(Stream stream, ReferenceObject documentObject)
  {
    await stream.WriteAsync(documentObject.ObjectIdentifier.GetReferenceBytes());
  }
}