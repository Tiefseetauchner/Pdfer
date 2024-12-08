using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class BooleanObjectReader(IStreamHelper streamHelper) : IDocumentObjectReader<BooleanObject>
{
  async Task<DocumentObject> IDocumentObjectReader.Read(Stream stream, ObjectRepository objectRepository) =>
    await Read(stream, objectRepository);

  public async Task<BooleanObject> Read(Stream stream, ObjectRepository objectRepository)
  {
    var firstChar = streamHelper.ReadChar(stream);

    if (firstChar == 't')
    {
      var buffer = new byte[3];
      var read = await stream.ReadAsync(buffer);
      var bufferText = Encoding.Default.GetString(buffer);

      if (read != 3 || bufferText != "rue")
        throw new InvalidDataException($"Expected 'true' but got {firstChar + bufferText}");

      return new BooleanObject(true);
    }

    if (firstChar == 'f')
    {
      var buffer = new byte[4];
      var read = await stream.ReadAsync(buffer);
      var bufferText = Encoding.Default.GetString(buffer);

      if (read != 4 || bufferText != "alse")
        throw new InvalidDataException($"Expected 'false' but got {firstChar + bufferText}");

      return new BooleanObject(false);
    }

    throw new InvalidDataException($"Expected the object to start with 't' or 'f' but got {firstChar}");
  }
}