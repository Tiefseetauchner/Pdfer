using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer;

public class PdfDictionaryHelper(IStreamHelper streamHelper) : IPdfDictionaryHelper
{
  public Task<Dictionary<string, string>> ReadDictionary(Stream stream)
  {
    var dictionary = new Dictionary<string, string>();
    
    Console.WriteLine($"  Starting Dict Read at Pos {stream.Position}");

    var dictionaryDepth = 1;
    var bufferString = "";
    var arrayKeyReading = false;
    var arrayKey = "";

    while (stream.Position < stream.Length && dictionaryDepth > 0)
    {
      var currentChar = streamHelper.ReadChar(stream);

      switch (currentChar)
      {
        case '/':
          switch (arrayKeyReading)
          {
            case false when bufferString.Length > 0 && dictionaryDepth == 1:
              dictionary[arrayKey] = bufferString.Trim();

              bufferString = "";
              arrayKeyReading = true;
              break;

            case false when bufferString.Length == 0:
              arrayKeyReading = true;
              break;
            case false when dictionaryDepth > 1:
            case true:
              bufferString += currentChar;
              break;
          }

          break;
        case ' ':
        case '<':
          if (arrayKeyReading)
          {
            arrayKey = bufferString;
            arrayKeyReading = false;
            bufferString = currentChar.ToString();
          }
          else
            bufferString += currentChar;

          break;
        default:
          bufferString += currentChar;
          break;
      }
      
      if (bufferString.Length >= 2)
      {
        switch (bufferString[^2..])
        {
          case "<<":
            dictionaryDepth++;
            break;
          case ">>":
            dictionaryDepth--;

            if (dictionaryDepth < 1 && !arrayKeyReading && arrayKey.Length > 0 && !string.IsNullOrWhiteSpace(bufferString[..^2]))
              dictionary[arrayKey] = bufferString[..^2].Trim();

            break;
        }
      }
    }

    Console.WriteLine($"  Finished Dict Read at Pos {stream.Position}");
    return Task.FromResult(dictionary);
  }
}