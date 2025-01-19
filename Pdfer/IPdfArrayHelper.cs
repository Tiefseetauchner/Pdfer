using Pdfer.Objects;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer;

public interface IPdfArrayHelper
{
  Task WriteArray(Stream stream, DocumentObject[] array);
}