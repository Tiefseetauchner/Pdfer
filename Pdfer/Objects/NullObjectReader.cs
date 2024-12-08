using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class NullObjectReader : IDocumentObjectReader<NullObject>
{
  async Task<DocumentObject> IDocumentObjectReader.Read(Stream stream, ObjectRepository objectRepository) =>
    await Read(stream, objectRepository);

  public async Task<NullObject> Read(Stream stream, ObjectRepository objectRepository)
  {
    var buffer = new byte[4];

    var read = await stream.ReadAsync(buffer);
    var bufferText = Encoding.Default.GetString(buffer);

    if (read != 4 || bufferText != "null")
      throw new InvalidDataException($"Expected 'null' but got {bufferText}");

    return new NullObject();
  }
}