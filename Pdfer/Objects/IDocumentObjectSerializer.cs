using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public interface IDocumentObjectSerializer<T> where T : DocumentObject
{
  Task Serialize(Stream stream, T documentObject);
}