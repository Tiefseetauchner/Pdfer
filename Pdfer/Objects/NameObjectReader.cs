using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class NameObjectReader : IDocumentObjectReader<NameObject>
{
  public async Task<NameObject> Read(Stream stream, IObjectRepository objectRepository, ObjectIdentifier objectIdentifier)
  {
    using var rawData = new MemoryStream();

    rawData.Write(objectIdentifier.GetHeaderBytes());

    var name = new StringBuilder();
    var nextByte = new byte[1];

    while (await stream.ReadAsync(nextByte) != 0)
    {
      if (nextByte[0] == ' ' || nextByte[0] == '\n' || nextByte[0] == '\r')
        break;

      rawData.Write(nextByte);

      name.Append((char)nextByte[0]);
    }

    rawData.Write("\nendobj"u8);

    return new NameObject(name.ToString(), rawData.ToArray(), objectIdentifier);
  }
}