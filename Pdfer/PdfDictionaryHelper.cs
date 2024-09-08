using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer;

public class PdfDictionaryHelper(IStreamHelper streamHelper) : IPdfDictionaryHelper
{
  public async Task<(Dictionary<string, string> dictionary, byte[] bytes)> ReadDictionary(Stream stream)
  {
    var dictionary = new Dictionary<string, string>();
    using var rawBytes = new MemoryStream();

    var dictionaryDepth = 0;
    var bufferStringBuilder = new StringBuilder();

    var firstChar = streamHelper.ReadChar(stream);
    bufferStringBuilder.Append(firstChar);
    rawBytes.WriteByte((byte)firstChar);

    var arrayKeyReading = false;
    string? arrayKey = null;
    var buffer = new byte[1];

    do
    {
      if (await stream.ReadAsync(buffer) == 0)
        throw new IOException("Unexpected end of stream");

      var character = (char)buffer[0];

      rawBytes.Write(buffer);

      if (dictionaryDepth == 1)
      {
        switch (character)
        {
          case '/' when !arrayKeyReading && (!string.IsNullOrWhiteSpace(bufferStringBuilder.ToString()) || arrayKey == null):
            arrayKeyReading = true;

            if (arrayKey != null)
              dictionary[arrayKey] = bufferStringBuilder.ToString().Trim();

            bufferStringBuilder = new StringBuilder();
            break;
          case '/' when arrayKeyReading:
          case ' ' when arrayKeyReading:
          case '\n' when arrayKeyReading:
          case '\r' when arrayKeyReading:
          case '[' when arrayKeyReading:
          case '(' when arrayKeyReading:
            arrayKeyReading = false;
            arrayKey = bufferStringBuilder.ToString().Trim();
            bufferStringBuilder = new StringBuilder();
            break;
        }
      }

      switch (character)
      {
        case '<' when (bufferStringBuilder.Length > 0 && bufferStringBuilder[^1] == '<'):
          dictionaryDepth++;
          break;
        case '>' when (bufferStringBuilder.Length > 0 && bufferStringBuilder[^1] == '>'):
          dictionaryDepth--;
          break;
      }

      bufferStringBuilder.Append(character);
    } while (dictionaryDepth > 0);


    if (arrayKey != null)
      dictionary[arrayKey] = bufferStringBuilder.ToString()[..^2].Trim();

    return (dictionary, rawBytes.ToArray());
  }
}