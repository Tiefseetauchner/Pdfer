using System.IO;
using System.Threading.Tasks;

namespace Pdfer;

public interface IStreamHelper
{
  Task<string> ReadReverseLine(StreamReader streamReader);
  Task<byte[]> ReadStreamTo(string s, Stream stream);
  char ReadChar(Stream stream);
  char PeakChar(Stream stream);
  Task<int> Peak(Stream stream, byte[] buffer);
  Task<byte[]> SkipWhiteSpaceCharacters(Stream stream);
}