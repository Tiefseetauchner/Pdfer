using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer;

public class PdfArrayHelper(IStreamHelper streamHelper) : IPdfArrayHelper
{
  public async Task<(string[] Array, byte[] RawBytes)> ReadArray(Stream stream)
  {
    var array = new List<string>();
    using var rawBytes = new MemoryStream();

    var bracketDepth = 1;
    var bufferStringBuilder = new StringBuilder();

    var firstChar = streamHelper.ReadChar(stream);
    bufferStringBuilder.Append(firstChar);
    rawBytes.WriteByte((byte)firstChar);

    var buffer = new byte[1];

    if (firstChar != '[')
      throw new IOException($"Could not parse array: Expected '[' but got '{firstChar}'");

    while (bracketDepth > 0)
    {
      if (await stream.ReadAsync(buffer) == 0)
        throw new IOException("Unexpected end of stream");

      var character = (char)buffer[0];

      rawBytes.Write(buffer);

      switch (character)
      {
        case ' ' when bracketDepth == 1 && bufferStringBuilder.ToString().Trim().Length == 0:
          array.Add(bufferStringBuilder.ToString().Trim());
          break;
        case '[':
          bracketDepth++;
          break;
        case ']':
          bracketDepth--;
          break;
      }

      bufferStringBuilder.Append(character);
    }

    var bufferString = bufferStringBuilder.ToString().Trim();

    if (bufferString.Length > 0)
      array.Add(bufferString);

    return (array.ToArray(), rawBytes.ToArray());
  }

  public async Task WriteArray(Stream stream, string[] array)
  {
    await stream.WriteAsync("[ "u8.ToArray());

    foreach (var value in array)
    {
      await stream.WriteAsync(Encoding.UTF8.GetBytes(value));
      await stream.WriteAsync(" "u8.ToArray());
    }

    await stream.WriteAsync("]"u8.ToArray());
  }
}