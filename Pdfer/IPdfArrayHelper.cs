using System.IO;
using System.Threading.Tasks;

namespace Pdfer;

public interface IPdfArrayHelper
{
  Task<(string[] Array, byte[] RawBytes)> ReadArray(Stream stream);
  Task WriteArray(Stream stream, string[] array);
}