using System.IO;
using System.Threading.Tasks;
using Pdfer.Objects;

namespace Pdfer;

public interface IPdfObjectReader
{
  Task<DocumentObject> Read(Stream stream, IObjectRepository objectRepository);
}