using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class StringObjectReader() : IDocumentObjectReader<StringObject>
{
  public async Task<StringObject> Read(Stream stream, IObjectRepository objectRepository, byte[] objectIdentifier)
  {
    var stringBuilder = new StringBuilder();
    using var memoryStream = new MemoryStream();
    memoryStream.Write(objectIdentifier);

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

    memoryStream.Write(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
    memoryStream.Write("endobj"u8);

    return new StringObject(stringBuilder.ToString(), memoryStream.ToArray());
  }
}