using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class NameObjectSerializer : IDocumentObjectSerializer<NameObject>
{
  public async Task Serialize(Stream stream, NameObject documentObject)
  {
    await stream.WriteAsync(documentObject.ObjectIdentifier.GetHeaderBytes());
    await stream.WriteAsync(Encoding.UTF8.GetBytes(documentObject.Value));
    await stream.WriteAsync("\nendobj"u8.ToArray());
  }
}