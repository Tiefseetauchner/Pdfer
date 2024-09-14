using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer;

public class EncodingDetector : IEncodingDetector
{
  public async Task<Encoding> DetectEncoding(Stream stream)
  {
    var bytes = new byte[3];

    if (await stream.ReadAsync(bytes) < 3)
      throw new IOException("Unexpected end of stream");

    if (bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
      return Encoding.UTF8;
    if (bytes[0] == 0xFE && bytes[1] == 0xFF)
      return Encoding.BigEndianUnicode;
    if (bytes[0] == 0xFF && bytes[1] == 0xFE)
      return Encoding.Unicode;

    return Encoding.Default;
  }
}

public interface IEncodingDetector
{
  Task<Encoding> DetectEncoding(Stream stream);
}