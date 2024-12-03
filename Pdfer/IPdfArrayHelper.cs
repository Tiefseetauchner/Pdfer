using System.IO;
using System.Threading.Tasks;
using Pdfer.Objects;

namespace Pdfer;

public interface IPdfArrayHelper
{
  Task<DocumentObject[]> ReadArray(Stream stream, IObjectRepository objectRepository);
  Task WriteArray(Stream stream, DocumentObject[] array);
}