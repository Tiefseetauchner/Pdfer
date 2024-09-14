using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class StringObjectSerializer : IDocumentObjectSerializer<StringObject>
{
  public async Task Serialize(Stream stream, StringObject documentObject)
  {
    await stream.WriteAsync(documentObject.ObjectIdentifier.GetHeaderBytes());
    await stream.WriteAsync("\n"u8.ToArray());
    await stream.WriteAsync(Encoding.UTF8.GetBytes(documentObject.Value));
    await stream.WriteAsync("\nendobj"u8.ToArray());
  }
}