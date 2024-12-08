using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class NullObjectSerializer : IDocumentObjectSerializer<NullObject>
{
  async Task IDocumentObjectSerializer.Serialize(Stream stream, DocumentObject documentObject) =>
    await Serialize(stream, (NullObject)documentObject);

  public async Task Serialize(Stream stream, NullObject documentObject)
  {
    await stream.WriteAsync("null"u8.ToArray());
  }
}