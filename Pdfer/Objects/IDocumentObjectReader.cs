using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public interface IDocumentObjectReader<T> : IDocumentObjectReader
  where T : DocumentObject
{
  new Task<T> Read(Stream stream, IObjectRepository objectRepository);
}

public interface IDocumentObjectReader
{
  Task<DocumentObject> Read(Stream stream, IObjectRepository objectRepository);
}