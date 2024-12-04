using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class StringObjectReader() : IDocumentObjectReader<StringObject>
{
  public async Task<StringObject> Read(Stream stream)
  {
    var stringBuilder = new StringBuilder();

    var nextByte = new byte[1];

    if (await stream.ReadAsync(nextByte) < 1)
      throw new IOException("Unexpected end of stream");

    var endChar = nextByte[0] == '<' ? '>' : ')';
    stringBuilder.Append((char)nextByte[0]);

    var escaped = false;

    while (await stream.ReadAsync(nextByte) != 0)
    {
      var nextCharacter = (char)nextByte[0];
      stringBuilder.Append(nextCharacter);

      if (escaped)
        escaped = false;
      else if (nextCharacter == '\\')
        escaped = true;
      else if (nextCharacter == endChar)
        break;
    }

    return new StringObject(stringBuilder.ToString());
  }
}