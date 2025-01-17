using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects.Serializers;

public class ArrayObjectSerializer(IDocumentObjectSerializerRepository objectSerializerRepository) : IDocumentObjectSerializer<ArrayObject>
{
  async Task IDocumentObjectSerializer.Serialize(Stream stream, DocumentObject documentObject) =>
    await Serialize(stream, (ArrayObject)documentObject);

  public async Task Serialize(Stream stream, ArrayObject documentObject)
  {
    await stream.WriteAsync("[ "u8.ToArray());

    foreach (var value in documentObject.Value)
    {
      await objectSerializerRepository.GetSerializer(value).Serialize(stream, value);
      await stream.WriteAsync(" "u8.ToArray());
    }

    await stream.WriteAsync("]"u8.ToArray());
  }
}