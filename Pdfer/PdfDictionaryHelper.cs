using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Pdfer.Objects;

namespace Pdfer;

public class PdfDictionaryHelper(
  IStreamHelper streamHelper,
  IPdfObjectReader pdfObjectReader) : IPdfDictionaryHelper
{
  public async Task<Dictionary<string, DocumentObject>> ReadDictionary(Stream stream)
  {
    var dictionary = new Dictionary<string, DocumentObject>();

    var buffer = new byte[2];
    string? key = null;

    while (await streamHelper.Peak(stream, buffer) != 2 && buffer[0] == '>' && buffer[1] == '>')
    {
      var nextObject = await pdfObjectReader.Read(stream);

      if (key == null)
      {
        if (nextObject is not NameObject nameObject)
          throw new InvalidOperationException("Key in Dictionary was not a NameObject.");

        key = nameObject.Value;
      }
      else
      {
        dictionary[key] = nextObject;
        key = null;
      }

      await streamHelper.SkipWhiteSpaceCharacters(stream);
    }

    return dictionary;
  }

  public static async Task<byte[]> GetDictionaryBytes(Dictionary<string, DocumentObject> dictionary, IDocumentObjectSerializerRepository documentObjectSerializerRepository)
  {
    using var memoryStream = new MemoryStream();
    await WriteDictionary(memoryStream, dictionary, documentObjectSerializerRepository);
    return memoryStream.ToArray();
  }

  public static async Task WriteDictionary(Stream stream, Dictionary<string, DocumentObject> dictionary, IDocumentObjectSerializerRepository documentObjectSerializerRepository)
  {
    await stream.WriteAsync("<<"u8.ToArray());

    foreach (var (key, value) in dictionary)
    {
      await stream.WriteAsync("\n"u8.ToArray());
      await stream.WriteAsync(Encoding.UTF8.GetBytes(key));
      await stream.WriteAsync(" "u8.ToArray());
      await documentObjectSerializerRepository.GetSerializer(value).Serialize(stream, value);
    }

    await stream.WriteAsync(">>"u8.ToArray());
  }
}