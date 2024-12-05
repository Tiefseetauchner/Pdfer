using System;
using System.IO;
using System.Threading.Tasks;
using Pdfer.Objects;

namespace Pdfer;

public class PdfDictionaryHelper(
  IStreamHelper streamHelper,
  IPdfObjectReader pdfObjectReader) : IPdfDictionaryHelper
{
  public async Task<PdfDictionary> ReadDictionary(Stream stream)
  {
    var dictionary = new PdfDictionary();

    var buffer = new byte[2];
    NameObject? key = null;

    if (await stream.ReadAsync(buffer) != 2 || buffer[0] != '<' || buffer[1] != '<')
      throw new InvalidOperationException("Dictionary does not start with '<<'");

    while (await streamHelper.Peak(stream, buffer) == 2 && !(buffer[0] == '>' && buffer[1] == '>'))
    {
      var nextObject = await pdfObjectReader.Read(stream);

      if (key == null)
      {
        if (nextObject is not NameObject nameObject)
          throw new InvalidOperationException("Key in Dictionary was not a NameObject.");

        key = nameObject;
      }
      else
      {
        dictionary[key] = nextObject;
        key = null;
      }

      await streamHelper.SkipWhiteSpaceCharacters(stream);
    }

    // NOTE: We have to skip the closing '>>' character here.
    var read = await stream.ReadAsync(buffer);

    if (read != 2 || buffer[0] != '>' || buffer[1] != '>')
      throw new InvalidOperationException("Dictionary does not end with '>>'");

    return dictionary;
  }

  public static async Task<byte[]> GetDictionaryBytes(PdfDictionary dictionary, IDocumentObjectSerializerRepository documentObjectSerializerRepository)
  {
    using var memoryStream = new MemoryStream();
    await WriteDictionary(memoryStream, dictionary, documentObjectSerializerRepository);
    return memoryStream.ToArray();
  }

  public static async Task WriteDictionary(Stream stream, PdfDictionary dictionary, IDocumentObjectSerializerRepository documentObjectSerializerRepository)
  {
    await stream.WriteAsync("<<"u8.ToArray());

    foreach (var (key, value) in dictionary)
    {
      await stream.WriteAsync("\n"u8.ToArray());
      await documentObjectSerializerRepository.GetSerializer<NameObject>().Serialize(stream, key);
      await stream.WriteAsync(" "u8.ToArray());
      await documentObjectSerializerRepository.GetSerializer(value).Serialize(stream, value);
    }

    await stream.WriteAsync(">>"u8.ToArray());
  }
}