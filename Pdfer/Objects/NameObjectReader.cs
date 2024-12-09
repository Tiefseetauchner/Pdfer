using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class NameObjectReader : IDocumentObjectReader<NameObject>
{
  async Task<DocumentObject> IDocumentObjectReader.Read(Stream stream, IObjectRepository objectRepository) =>
    await Read(stream, objectRepository);

  public async Task<NameObject> Read(Stream stream, IObjectRepository objectRepository)
  {
    var name = new StringBuilder();
    var nextByte = new byte[1];

    var firstChar = await stream.ReadAsync(nextByte);

    if (firstChar < 1 || nextByte[0] != 47)
      throw new IOException("Name Object is not a valid name.");

    while (await stream.ReadAsync(nextByte) != 0)
    {
      // TODO (lena.tauchner): Decode #XX

      if (nextByte[0] == '/' || char.IsWhiteSpace((char)nextByte[0]) || nextByte[0] == '(' || nextByte[0] == '[' || nextByte[0] == '<' || nextByte[0] == '>' || nextByte[0] == ')' || nextByte[0] == ']')
        break;

      name.Append((char)nextByte[0]);
    }

    stream.Position -= 1;

    return new NameObject(name.ToString());
  }
}