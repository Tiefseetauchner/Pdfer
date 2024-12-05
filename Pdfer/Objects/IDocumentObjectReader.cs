using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public interface IDocumentObjectReader
{
  Task<DocumentObject> Read(Stream stream);
}