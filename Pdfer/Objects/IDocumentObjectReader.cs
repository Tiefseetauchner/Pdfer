using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public interface IDocumentObjectReader<T> where T : DocumentObject
{
  Task<T> Read(Stream stream, IObjectRepository objectRepository);
}