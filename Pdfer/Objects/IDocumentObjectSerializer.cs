using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public interface IDocumentObjectSerializer<in T> : IDocumentObjectSerializer where T : DocumentObject
{
  Task Serialize(Stream stream, T documentObject);
}

public interface IDocumentObjectSerializer
{
  Task Serialize(Stream stream, DocumentObject documentObject);
}