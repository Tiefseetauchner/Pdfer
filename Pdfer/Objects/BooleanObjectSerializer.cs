using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class BooleanObjectSerializer : IDocumentObjectSerializer<BooleanObject>
{
  async Task IDocumentObjectSerializer.Serialize(Stream stream, DocumentObject documentObject) =>
    await Serialize(stream, (BooleanObject)documentObject);

  public async Task Serialize(Stream stream, BooleanObject documentObject) =>
    await stream.WriteAsync((documentObject.Value ? "true"u8 : "false"u8).ToArray());
}