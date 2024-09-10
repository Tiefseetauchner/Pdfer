using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class ArrayObjectReader(IPdfArrayHelper pdfArrayHelper) : IDocumentObjectReader<ArrayObject>
{
  public async Task<ArrayObject> Read(Stream stream, IObjectRepository objectRepository, ObjectIdentifier objectIdentifier)
  {
    var rawData = new MemoryStream();
    rawData.Write(objectIdentifier.GetHeaderBytes());

    var (array, rawArrayBytes) = await pdfArrayHelper.ReadArray(stream);

    rawData.Write(rawArrayBytes);

    rawData.Write("\nendobj"u8.ToArray());

    return new ArrayObject(array, rawData.ToArray(), objectIdentifier);
  }
}