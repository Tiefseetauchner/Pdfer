using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class ArrayObjectSerializer(IPdfArrayHelper pdfArrayHelper) : IDocumentObjectSerializer<ArrayObject>
{
  public async Task<byte[]> Serialize(ArrayObject documentObject)
  {
    using var memoryStream = new MemoryStream();

    await memoryStream.WriteAsync(documentObject.ObjectIdentifier.GetHeaderBytes());
    await pdfArrayHelper.WriteArray(memoryStream, documentObject.Value);
    await memoryStream.WriteAsync("\nendobj"u8.ToArray());

    return memoryStream.ToArray();

  }
}