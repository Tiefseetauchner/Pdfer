using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class NumberObjectSerializer() : IDocumentObjectSerializer<NumberObject>
{
  public async Task<byte[]> Serialize(NumberObject documentObject)
  {
    using var memoryStream = new MemoryStream();

    await memoryStream.WriteAsync(documentObject.ObjectIdentifier.GetHeaderBytes());

    switch (documentObject)
    {
      case FloatObject floatObject:
        await memoryStream.WriteAsync(Encoding.ASCII.GetBytes(floatObject.Value.ToString(CultureInfo.InvariantCulture)));
        break;
      case IntegerObject integerObject:
        await memoryStream.WriteAsync(Encoding.ASCII.GetBytes(integerObject.Value.ToString()));
        break;
    }

    await memoryStream.WriteAsync("\nendobj"u8.ToArray());

    return memoryStream.ToArray();
  }
}