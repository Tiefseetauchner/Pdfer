using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class PdfObjectReader(
  IStreamHelper streamHelper,
  IDocumentObjectReader<DictionaryObject> dictionaryObjectReader,
  IDocumentObjectReader<StringObject> stringObjectReader,
  IDocumentObjectReader<StreamObject> streamObjectReader,
  IDocumentObjectReader<NumericObject> numberObjectReader,
  IDocumentObjectReader<NameObject> nameObjectReader,
  IDocumentObjectReader<ArrayObject> arrayObjectReader) : IPdfObjectReader
{
  // TODO (lena): Deal with BooleanObjects
  // TODO (lena): Deal with NullObjects
  public async Task<DocumentObject> Read(Stream stream)
  {
    var objectStartBuffer = new byte[2];
    var objectStart = await stream.ReadAsync(objectStartBuffer);
    stream.Position -= objectStart;
    var streamPositionAfterObjectStart = stream.Position;

    if (objectStart != 2)
      throw new IOException("Unexpected end of stream");

    if (objectStartBuffer[0] == '<' &&
        objectStartBuffer[1] != '<' ||
        objectStartBuffer[0] == '(')
      return await stringObjectReader.Read(stream);

    if (objectStartBuffer[0] == '[')
      return await arrayObjectReader.Read(stream);

    if (objectStartBuffer[0] == '<' &&
        objectStartBuffer[1] == '<')
    {
      // IMPROVE (lena): This is not a good way to check if it is a stream. We're reading the whole dictionary twice.
      await new PdfDictionaryHelper(streamHelper, this).ReadDictionary(stream);

      await streamHelper.SkipWhiteSpaceCharacters(stream);

      var buffer = new byte[7];
      _ = await stream.ReadAsync(buffer);
      var contentAfterDictionary = Encoding.UTF8.GetString(buffer);
      stream.Position = streamPositionAfterObjectStart;

      if (contentAfterDictionary.Trim().StartsWith("stream"))
        return await streamObjectReader.Read(stream);

      return await dictionaryObjectReader.Read(stream);
    }

    if (char.IsNumber((char)objectStartBuffer[0]))
      return await numberObjectReader.Read(stream);

    if ((char)objectStartBuffer[0] == '/')
      return await nameObjectReader.Read(stream);

    throw new NotImplementedException("The object type passed was not yet implemented");
  }
}