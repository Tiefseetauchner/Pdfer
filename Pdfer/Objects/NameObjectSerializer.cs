using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class NameObjectSerializer : IDocumentObjectSerializer<NameObject>
{
  public async Task Serialize(Stream stream, NameObject documentObject) =>
    await stream.WriteAsync(Encoding.UTF8.GetBytes(documentObject.Value));
}