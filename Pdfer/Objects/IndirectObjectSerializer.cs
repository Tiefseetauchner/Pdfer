using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class IndirectObjectSerializer : IDocumentObjectSerializer<IndirectObject>
{
  public async Task Serialize(Stream stream, IndirectObject documentObject)
  {
    await stream.WriteAsync(documentObject.ObjectIdentifier.GetReferenceBytes());
  }
}