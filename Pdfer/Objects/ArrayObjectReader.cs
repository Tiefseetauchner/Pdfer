using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class ArrayObjectReader(IPdfArrayHelper pdfArrayHelper) : IDocumentObjectReader<ArrayObject>
{
  public async Task<ArrayObject> Read(Stream stream, IObjectRepository objectRepository)
  {
    var array = await pdfArrayHelper.ReadArray(stream, objectRepository);

    return new ArrayObject(array);
  }
}