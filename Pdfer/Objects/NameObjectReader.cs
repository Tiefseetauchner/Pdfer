using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class NameObjectReader : IDocumentObjectReader<NameObject>
{
  public async Task<NameObject> Read(Stream stream)
  {
    var name = new StringBuilder();
    var nextByte = new byte[1];

    while (await stream.ReadAsync(nextByte) != 0)
    {
      // TODO (lena.tauchner): Decode #XX

      if (nextByte[0] == '/' || nextByte[0] == ' ' || nextByte[0] == '\n' || nextByte[0] == '\r')
        break;

      name.Append((char)nextByte[0]);
    }

    return new NameObject(name.ToString());
  }
}