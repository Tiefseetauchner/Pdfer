using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Pdfer.Objects;

namespace Pdfer;

public class PdfObjectReader(
  IStreamHelper streamHelper,
  IObjectRepository objectRepository,
  IDocumentObjectReader<DictionaryObject> dictionaryObjectReader,
  IDocumentObjectReader<StringObject> stringObjectReader,
  IDocumentObjectReader<StreamObject> streamObjectReader) : IPdfObjectReader
{
  // TODO (lena): Deal with NameObjects
  // TODO (lena): Deal with BooleanObjects
  // TODO (lena): Deal with NullObjects
  // TODO (lena): change to stream
  public async Task<DocumentObject> Read(Stream stream, long xrefOffset)
  {
    stream.Position = xrefOffset;

    await streamHelper.ReadStreamTo("\n", stream);

    var objectStartBuffer = new byte[2];
    var objectStart = await stream.ReadAsync(objectStartBuffer);
    stream.Position -= objectStart;

    if (objectStart != 2)
      throw new IOException("Unexpected end of stream");

    if (objectStartBuffer[0] == '<' &&
        objectStartBuffer[0] != '<' ||
        objectStartBuffer[0] == '(')
      return await stringObjectReader.Read(stream);

    if (objectStartBuffer[0] == '<' &&
        objectStartBuffer[0] == '<')
    {
      var dictionary = await dictionaryObjectReader.Read(stream);

      var nextLine = Encoding.UTF8.GetString(await streamHelper.ReadStreamTo("\n", stream)).Trim();

      if (nextLine == "stream")
      {
        if (!dictionary.Value.TryGetValue("Length", out string? value))
          throw new ArgumentException("Length must be specified for stream objects.");

        if (long.TryParse(value, out var length))
          return await streamObjectReader.Read(stream, length);
        else
        {
          var lengthObject = objectRepository.RetrieveObject<IntegerObject>(ObjectIdentifier.ParseReference(value));
          
          
        }
      }

      return dictionary;
    }

    // check if next line equals "stream"
    //   then read object key "Length"
    //     then check whether reference and retrieve object
    //   then read that amount of bytes.
    // Find next endobj
    // 
    return null!;
  }
}