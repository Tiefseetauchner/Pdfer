using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class IndirectObjectSerializer : IDocumentObjectSerializer<IndirectObject>
{
  async Task IDocumentObjectSerializer.Serialize(Stream stream, DocumentObject documentObject) =>
    await Serialize(stream, (IndirectObject)documentObject);

  public async Task Serialize(Stream stream, IndirectObject documentObject)
  {
    await stream.WriteAsync(documentObject.ObjectIdentifier.GetReferenceBytes());
  }
}