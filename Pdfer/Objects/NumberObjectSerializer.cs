using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class NumberObjectSerializer() : IDocumentObjectSerializer<NumberObject>
{
  public async Task Serialize(Stream stream, NumberObject documentObject)
  {
    await stream.WriteAsync(documentObject.ObjectIdentifier.GetHeaderBytes());

    switch (documentObject)
    {
      case FloatObject floatObject:
        await stream.WriteAsync(Encoding.UTF8.GetBytes(floatObject.Value.ToString(CultureInfo.InvariantCulture)));
        break;
      case IntegerObject integerObject:
        await stream.WriteAsync(Encoding.UTF8.GetBytes(integerObject.Value.ToString()));
        break;
    }

    await stream.WriteAsync("\nendobj"u8.ToArray());
  }
}