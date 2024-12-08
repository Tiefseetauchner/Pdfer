using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class NumericObjectSerializer() : IDocumentObjectSerializer<NumericObject>
{
  async Task IDocumentObjectSerializer.Serialize(Stream stream, DocumentObject documentObject) =>
    await Serialize(stream, (NumericObject)documentObject);

  public async Task Serialize(Stream stream, NumericObject documentObject)
  {
    switch (documentObject)
    {
      case FloatObject floatObject:
        await stream.WriteAsync(Encoding.UTF8.GetBytes(floatObject.Value.ToString(CultureInfo.InvariantCulture)));
        break;
      case IntegerObject integerObject:
        await stream.WriteAsync(Encoding.UTF8.GetBytes(integerObject.Value.ToString()));
        break;
    }
  }
}