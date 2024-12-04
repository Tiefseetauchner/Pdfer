using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class ArrayObjectReader(StreamHelper streamHelper) : IDocumentObjectReader<ArrayObject>
{
  public async Task<ArrayObject> Read(Stream stream)
  {
    var array = await new PdfArrayHelper(streamHelper).ReadArray(stream);

    return new ArrayObject(array);
  }
}