using System.IO;
using System.Threading.Tasks;
using Pdfer.Objects;

namespace Pdfer;

public interface IPdfArrayHelper
{
  Task WriteArray(Stream stream, DocumentObject[] array);
}