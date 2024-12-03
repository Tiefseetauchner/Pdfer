using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public interface IDocumentObjectSerializer<in T> where T : DocumentObject
{
  Task Serialize(Stream stream, T documentObject);
}