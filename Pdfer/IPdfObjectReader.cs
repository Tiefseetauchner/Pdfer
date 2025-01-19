using Pdfer.Objects;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer;

public interface IPdfObjectReader
{
  Task<DocumentObject> Read(Stream stream, IObjectRepository objectRepository);
}