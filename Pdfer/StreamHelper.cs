using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pdfer;

public class StreamHelper : IStreamHelper
{
  public async Task<string> ReadReverseLine(StreamReader streamReader) =>
    string.Concat((await streamReader.ReadLineAsync())?.Reverse() ?? throw new IOException("Unexpected end of stream"));

  public async Task<byte[]> ReadStreamTo(string s, Stream stream)
  {
    var outputBuffer = new List<byte>();

    var buffer = new byte[s.Length];
    var bytesRead = stream.Read(buffer);

    while (bytesRead == s.Length && !buffer.SequenceEqual(s.ToCharArray().Select(_ => (byte)_)))
    {
      outputBuffer.Add(buffer[0]);

      stream.Position -= bytesRead - 1;

      bytesRead = await stream.ReadAsync(buffer);
    }

    return outputBuffer.ToArray();
  }

  public char ReadChar(Stream stream) =>
    (char)stream.ReadByte();
}