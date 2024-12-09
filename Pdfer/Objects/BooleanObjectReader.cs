using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class BooleanObjectReader(IStreamHelper streamHelper) : IDocumentObjectReader<BooleanObject>
{
  async Task<DocumentObject> IDocumentObjectReader.Read(Stream stream, IObjectRepository objectRepository) =>
    await Read(stream, objectRepository);

  public async Task<BooleanObject> Read(Stream stream, IObjectRepository objectRepository)
  {
    var firstChar = streamHelper.ReadChar(stream);

    switch (firstChar)
    {
      case 't':
      {
        var buffer = new byte[3];
        var read = await stream.ReadAsync(buffer);
        var bufferText = Encoding.Default.GetString(buffer);

        if (read != 3 || bufferText != "rue")
          throw new PdfInvalidBooleanValueParsingException($"Expected 'true' but got '{firstChar + bufferText}'");

        return new BooleanObject(true);
      }
      case 'f':
      {
        var buffer = new byte[4];
        var read = await stream.ReadAsync(buffer);
        var bufferText = Encoding.Default.GetString(buffer);

        if (read != 4 || bufferText != "alse")
          throw new PdfInvalidBooleanValueParsingException($"Expected 'false' but got '{firstChar + bufferText}'");

        return new BooleanObject(false);
      }
      default:
        throw new PdfInvalidBooleanValueParsingException($"Expected the object to start with 't' or 'f' but got '{firstChar}'");
    }
  }
}