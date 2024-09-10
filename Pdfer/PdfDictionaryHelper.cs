using System;
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

    var bracketDepth = 1;
    var bufferStringBuilder = new StringBuilder();

    var firstChar = streamHelper.ReadChar(stream);
    bufferStringBuilder.Append(firstChar);
    rawBytes.WriteByte((byte)firstChar);

    var arrayKeyReading = false;
    string? arrayKey = null;
    var buffer = new byte[1];

    while (bracketDepth > 0)
    {
      if (await stream.ReadAsync(buffer) == 0)
        throw new IOException("Unexpected end of stream");

      var character = (char)buffer[0];

      rawBytes.Write(buffer);

      if (bracketDepth == 2)
      {
        switch (character)
        {
          case '/' when !arrayKeyReading && (!string.IsNullOrWhiteSpace(bufferStringBuilder.ToString()) || arrayKey == null):
            arrayKeyReading = true;

            if (arrayKey != null)
              AddDictionaryEntry(dictionary, arrayKey, bufferStringBuilder.ToString());

            bufferStringBuilder = new StringBuilder();
            break;
          case '/' when arrayKeyReading:
          case ' ' when arrayKeyReading:
          case '\n' when arrayKeyReading:
          case '\r' when arrayKeyReading:
          case '[' when arrayKeyReading:
          case '(' when arrayKeyReading:
          case '<' when arrayKeyReading:
          case '>' when arrayKeyReading:
            arrayKeyReading = false;
            arrayKey = bufferStringBuilder.ToString().Trim();
            bufferStringBuilder = new StringBuilder();
            break;
        }
      }

      switch (character)
      {
        case '<' when bufferStringBuilder.Length == 0 || bufferStringBuilder[^1] != '\\':
          bracketDepth++;
          break;
        case '>' when bufferStringBuilder.Length == 0 || bufferStringBuilder[^1] != '\\':
          bracketDepth--;
          break;
      }

      bufferStringBuilder.Append(character);
    }


    if (arrayKey != null)
      AddDictionaryEntry(dictionary, arrayKey, bufferStringBuilder.ToString()[..^2]);

    return (dictionary, rawBytes.ToArray());
  }

  private static void AddDictionaryEntry(Dictionary<string, string> dictionary, string arrayKey, string bufferString)
  {
    if (dictionary.ContainsKey(arrayKey))
      throw new InvalidOperationException($"Duplicate key '{arrayKey}' in dictionary");
    if (bufferString.Trim().Length == 0)
      throw new InvalidOperationException($"Empty value for key '{arrayKey}' in dictionary");

    dictionary[arrayKey] = bufferString.ToString().Trim();
  }

  public async Task<byte[]> GetDictionaryBytes(Dictionary<string, string> dictionary)
  {
    using var memoryStream = new MemoryStream();
    await WriteDictionary(memoryStream, dictionary);
    return memoryStream.ToArray();
  }

  public async Task WriteDictionary(Stream stream, Dictionary<string, string> dictionary)
  {
    await stream.WriteAsync("<<"u8.ToArray());
    foreach (var (key, value) in dictionary)
    {
      await stream.WriteAsync("\n"u8.ToArray());
      await stream.WriteAsync(Encoding.ASCII.GetBytes(key));
      await stream.WriteAsync(" "u8.ToArray());
      await stream.WriteAsync(Encoding.ASCII.GetBytes(value));
    }

    await stream.WriteAsync(">>"u8.ToArray());
  }
}