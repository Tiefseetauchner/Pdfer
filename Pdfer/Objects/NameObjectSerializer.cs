using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class NameObjectSerializer : IDocumentObjectSerializer<NameObject>
{
  public async Task<byte[]> Serialize(NameObject documentObject)
  {
    using var memoryStream = new MemoryStream();

    await memoryStream.WriteAsync(documentObject.ObjectIdentifier.GetHeaderBytes());
    await memoryStream.WriteAsync(Encoding.ASCII.GetBytes(documentObject.Value));
    await memoryStream.WriteAsync("\nendobj"u8.ToArray());

    return memoryStream.ToArray();
  }
}