using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class ArrayObjectSerializer(IPdfArrayHelper pdfArrayHelper) : IDocumentObjectSerializer<ArrayObject>
{
  public async Task Serialize(Stream stream, ArrayObject documentObject)
  {
    await pdfArrayHelper.WriteArray(stream, documentObject.Value);
  }
}