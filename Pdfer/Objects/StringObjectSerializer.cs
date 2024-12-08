using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class StringObjectSerializer : IDocumentObjectSerializer<StringObject>
{
  async Task IDocumentObjectSerializer.Serialize(Stream stream, DocumentObject documentObject) =>
    await Serialize(stream, (StringObject)documentObject);

  public async Task Serialize(Stream stream, StringObject documentObject)
  {
    await stream.WriteAsync(Encoding.UTF8.GetBytes(documentObject.Value));
  }
}