using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pdfer;

public static class StreamHelper
{
  public static async Task<string> ReadReverseLine(StreamReader streamReader) =>
    string.Concat((await streamReader.ReadLineAsync())?.Reverse() ?? throw new IOException("Unexpected end of stream"));

  public static async Task<byte[]> ReadStreamTo(string s, Stream stream)
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

  public static char ReadChar(Stream stream) =>
    (char)stream.ReadByte();

  public static char PeakChar(Stream stream)
  {
    var oldPosition = stream.Position;
    var character = (char)stream.ReadByte();
    stream.Position = oldPosition;
    return character;
  }

  public static async Task<int> Peak(Stream stream, byte[] buffer)
  {
    var oldPosition = stream.Position;
    var bytesRead = await stream.ReadAsync(buffer);
    stream.Position = oldPosition;

    return bytesRead;
  }

  public static async Task<byte[]> SkipWhiteSpaceCharacters(Stream stream)
  {
    using var rawBytes = new MemoryStream();

    var buffer = new byte[1];

    while (stream.Read(buffer) == 1 && char.IsWhiteSpace((char)buffer[0]) || buffer[0] == '\r' || buffer[0] == '\n')
    {
      await rawBytes.WriteAsync(buffer);
    }

    stream.Position--;

    return rawBytes.ToArray();
  }
}