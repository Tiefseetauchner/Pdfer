using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class NumberObjectReader(StreamHelper streamHelper) : IDocumentObjectReader<NumberObject>
{
  public async Task<NumberObject> Read(Stream stream, IObjectRepository objectRepository, ObjectIdentifier objectIdentifier)
  {
    var rawNumber = await streamHelper.ReadStreamTo("endobj", stream);
    var numberString = Encoding.UTF8.GetString(rawNumber).Trim();

    var rawDataStream = new MemoryStream();
    rawDataStream.Write(objectIdentifier.GetHeaderBytes());
    rawDataStream.Write(rawNumber);
    rawDataStream.Write("endobj"u8);

    if (numberString.Contains('.'))
      return new FloatObject(float.Parse(numberString), rawDataStream.ToArray(), objectIdentifier);

    return new IntegerObject(long.Parse(numberString), rawDataStream.ToArray(), objectIdentifier);
  }
}