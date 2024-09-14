using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class ArrayObjectSerializer(IPdfArrayHelper pdfArrayHelper) : IDocumentObjectSerializer<ArrayObject>
{
  public async Task Serialize(Stream stream, ArrayObject documentObject)
  {
    await stream.WriteAsync(documentObject.ObjectIdentifier.GetHeaderBytes());
    await pdfArrayHelper.WriteArray(stream, documentObject.Value);
    await stream.WriteAsync("\nendobj"u8.ToArray());
  }
}