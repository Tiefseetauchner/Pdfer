using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class NumberObjectReader(StreamHelper streamHelper) : IDocumentObjectReader<NumberObject>
{
  public async Task<NumberObject> Read(Stream stream, IObjectRepository objectRepository)
  {
    var rawNumber = await streamHelper.ReadStreamTo("endobj", stream);
    var numberString = Encoding.UTF8.GetString(rawNumber).Trim();
    
    var rawDataStream = new MemoryStream();
    rawDataStream.Write(rawNumber);

    if (numberString.Contains('.'))
      return new FloatObject(float.Parse(numberString), rawNumber);
    
    return new IntegerObject(long.Parse(numberString), rawNumber);
  }
}