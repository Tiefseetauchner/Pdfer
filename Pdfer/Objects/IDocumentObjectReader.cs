using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public interface IDocumentObjectReader<T> : IDocumentObjectReader
  where T : DocumentObject
{
  new Task<T> Read(Stream stream);
}

public interface IDocumentObjectReader
{
  Task<DocumentObject> Read(Stream stream);
}