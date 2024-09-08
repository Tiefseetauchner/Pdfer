using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class StringObjectSerializer : IDocumentObjectSerializer<StringObject>
{
  public async Task<byte[]> Serialize(StringObject documentObject)
  {
    using var memoryStream = new MemoryStream();

    await memoryStream.WriteAsync(documentObject.ObjectIdentifier.GetHeaderBytes());
    await memoryStream.WriteAsync("\n"u8.ToArray());
    await memoryStream.WriteAsync(Encoding.UTF8.GetBytes(documentObject.Value));
    await memoryStream.WriteAsync("\nendobj"u8.ToArray());

    return memoryStream.ToArray();
  }
}