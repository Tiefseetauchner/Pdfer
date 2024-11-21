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

    var bufferStringBuilder = new StringBuilder();

    var firstChar = streamHelper.ReadChar(stream);
    bufferStringBuilder.Append(firstChar);
    rawBytes.WriteByte((byte)firstChar);

    var keyReading = false;
    string? key = null;
    var buffer = new byte[1];

    var openingBracketStack = new Stack<char>();
    openingBracketStack.Push(firstChar);

    var escaped = false;

    while (openingBracketStack.Count != 0)
    {
      if (await stream.ReadAsync(buffer) == 0)
        throw new IOException("Unexpected end of stream");

      var character = (char)buffer[0];

      rawBytes.Write(buffer);


      if (escaped)
      {
        bufferStringBuilder.Append(character);
        escaped = false;
        continue;
      }

      if (character == '\\')
        escaped = true;

      if (openingBracketStack.Count == 2)
      {
        switch (character)
        {
          case '/' when !keyReading && (!string.IsNullOrWhiteSpace(bufferStringBuilder.ToString()) || key == null):
            keyReading = true;

            if (key != null)
              AddDictionaryEntry(dictionary, key, bufferStringBuilder.ToString());

            bufferStringBuilder = new StringBuilder();
            break;
          case '/' when keyReading:
          case ' ' when keyReading:
          case '\n' when keyReading:
          case '\r' when keyReading:
          case '[' when keyReading:
          case '(' when keyReading:
          case '<' when keyReading:
          case '>' when keyReading:
            keyReading = false;
            key = bufferStringBuilder.ToString().Trim();
            bufferStringBuilder = new StringBuilder();
            break;
        }
      }

      switch (character)
      {
        case '<' when openingBracketStack.Peek() != '(':
        case '[' when openingBracketStack.Peek() != '(':
        case '(':
          openingBracketStack.Push(character);
          break;
        case '>' when openingBracketStack.Peek() == '<':
        case ']' when openingBracketStack.Peek() == '[':
        case ')' when openingBracketStack.Peek() == '(':
          openingBracketStack.Pop();
          break;
      }

      bufferStringBuilder.Append(character);
    }


    if (key != null)
      AddDictionaryEntry(dictionary, key, bufferStringBuilder.ToString()[..^2]);

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
      await stream.WriteAsync(Encoding.UTF8.GetBytes(key));
      await stream.WriteAsync(" "u8.ToArray());
      await stream.WriteAsync(Encoding.UTF8.GetBytes(value));
    }

    await stream.WriteAsync(">>"u8.ToArray());
  }
}