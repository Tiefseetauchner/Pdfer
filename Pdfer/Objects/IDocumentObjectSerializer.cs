using System.Threading.Tasks;

namespace Pdfer.Objects;

public interface IDocumentObjectSerializer<T> where T : DocumentObject
{
  Task<byte[]> Serialize(T documentObject);
}