using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class StringObjectSerializer : IDocumentObjectSerializer<StringObject>
{
  public async Task Serialize(Stream stream, StringObject documentObject)
  {
    await stream.WriteAsync(Encoding.UTF8.GetBytes(documentObject.Value));
  }
}