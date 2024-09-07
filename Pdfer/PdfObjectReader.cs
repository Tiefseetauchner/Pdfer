using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Pdfer.Objects;

namespace Pdfer;

public class PdfObjectReader(
  IStreamHelper streamHelper,
  IDocumentObjectReader<DictionaryObject> dictionaryObjectReader,
  IDocumentObjectReader<StringObject> stringObjectReader,
  IDocumentObjectReader<StreamObject> streamObjectReader, 
  IDocumentObjectReader<NumberObject> numberObjectReader) : IPdfObjectReader
{
  // TODO (lena): Deal with NameObjects
  // TODO (lena): Deal with BooleanObjects
  // TODO (lena): Deal with NullObjects
  // TODO (lena): change to stream
  public async Task<DocumentObject> Read(Stream stream, XRefEntry xRefEntry, IObjectRepository objectRepository)
  {
    stream.Position = xRefEntry.Position;

    await streamHelper.ReadStreamTo("\n", stream);

    var objectStartBuffer = new byte[2];
    var objectStart = await stream.ReadAsync(objectStartBuffer);
    stream.Position -= objectStart;

    if (objectStart != 2)
      throw new IOException("Unexpected end of stream");

    if (objectStartBuffer[0] == '<' &&
        objectStartBuffer[0] != '<' ||
        objectStartBuffer[0] == '(')
      return await stringObjectReader.Read(stream, objectRepository);

    if (objectStartBuffer[0] == '<' &&
        objectStartBuffer[0] == '<')
    {
      var dictionary = await dictionaryObjectReader.Read(stream, objectRepository);

      await streamHelper.ReadStreamTo("\n", stream);
      stream.Position += 1;
      var nextLine = Encoding.UTF8.GetString(await streamHelper.ReadStreamTo("\n", stream)).Trim();

      if (nextLine != "stream") 
        return dictionary;
      // NOTE (lena): We need to restart the stream so we can get the whole raw Data
      //              Here we're currently then reading the dictionary twice. This is not ideal, but it works for now.
      stream.Position = xRefEntry.Position;
        
      return await streamObjectReader.Read(stream, objectRepository);

    }
    
    if (char.IsNumber((char)objectStartBuffer[0]))
      return await numberObjectReader.Read(stream, objectRepository);

    return null!;
  }
}