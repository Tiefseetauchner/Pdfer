using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects.Readers;

public class StringObjectReader() : IDocumentObjectReader<StringObject>
{
  private static readonly char[] c_validHexCharacters = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'a', 'b', 'c', 'd', 'e', 'f',];

  async Task<DocumentObject> IDocumentObjectReader.Read(Stream stream, IObjectRepository objectRepository) =>
    await Read(stream, objectRepository);

  public async Task<StringObject> Read(Stream stream, IObjectRepository objectRepository)
  {
    var nextByte = new byte[1];

    if (await stream.ReadAsync(nextByte) < 1)
      throw new IOException("Unexpected end of stream");

    var isHexString = nextByte[0] == '<';

    var value = isHexString ? await ReadHexString(stream) : await ReadLiteralString(stream);

    return new StringObject(value);
  }

  private static async Task<string> ReadLiteralString(Stream stream)
  {
    var nextByte = new byte[1];
    var stringBuilder = new StringBuilder();
    var escaped = false;
    stringBuilder.Append('(');

    while (await stream.ReadAsync(nextByte) != 0)
    {
      var nextCharacter = (char)nextByte[0];
      stringBuilder.Append(nextCharacter);

      if (escaped)
        escaped = false;
      else if (nextCharacter == '\\')
        escaped = true;
      else if (nextCharacter == '(' || nextCharacter == '\\')
        throw new PdfInvalidLiteralStringValueParsingException($"Character '{nextCharacter}' was not escaped but has to be.");
      else if (nextCharacter == ')')
        break;
    }

    return stringBuilder.ToString();
  }

  private static async Task<string> ReadHexString(Stream stream)
  {
    var nextByte = new byte[1];
    var stringBuilder = new StringBuilder();
    stringBuilder.Append('<');

    while (await stream.ReadAsync(nextByte) != 0)
    {
      var nextCharacter = (char)nextByte[0];

      stringBuilder.Append(nextCharacter);

      if (nextCharacter == '>')
        break;

      if (!c_validHexCharacters.Contains(nextCharacter))
        throw new PdfInvalidHexStringValueParsingException($"Character '{nextCharacter}' is not valid in the context of a hexadecimal string.");
    }

    return stringBuilder.ToString();
  }
}