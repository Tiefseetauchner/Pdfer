using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class ArrayObjectSerializer(IDocumentObjectSerializerRepository objectSerializerRepository) : IDocumentObjectSerializer<ArrayObject>
{
  public async Task Serialize(Stream stream, ArrayObject documentObject)
  {
    await PdfArrayHelper.WriteArray(stream, documentObject.Value, objectSerializerRepository);
  }
}