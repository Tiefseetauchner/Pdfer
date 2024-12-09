using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class PdfObjectReader(
  IStreamHelper streamHelper,
  IDocumentObjectReaderRepository documentObjectReaderRepository) : IPdfObjectReader
{
  public async Task<DocumentObject> Read(Stream stream, IObjectRepository objectRepository)
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
      return await documentObjectReaderRepository.GetReader<StringObject>().Read(stream, objectRepository);

    if (objectStartBuffer[0] == '[')
      return await documentObjectReaderRepository.GetReader<ArrayObject>().Read(stream, objectRepository);

    if (objectStartBuffer[0] == '<' &&
        objectStartBuffer[1] == '<')
    {
      // IMPROVE (lena): This is not a good way to check if it is a stream. We're reading the whole dictionary twice.
      await new PdfDictionaryHelper(streamHelper, this).ReadDictionary(stream, objectRepository);

      await streamHelper.SkipWhiteSpaceCharacters(stream);

      var buffer = new byte[7];
      _ = await stream.ReadAsync(buffer);
      var contentAfterDictionary = Encoding.UTF8.GetString(buffer);
      stream.Position = streamPositionAfterObjectStart;

      if (contentAfterDictionary.Trim().StartsWith("stream"))
        return await documentObjectReaderRepository.GetReader<StreamObject>().Read(stream, objectRepository);

      return await documentObjectReaderRepository.GetReader<DictionaryObject>().Read(stream, objectRepository);
    }

    if (char.IsNumber((char)objectStartBuffer[0]) || objectStartBuffer[0] == '-')
      return await ParseNumericOrIndirectObject(stream, objectRepository);

    if ((char)objectStartBuffer[0] == '/')
      return await documentObjectReaderRepository.GetReader<NameObject>().Read(stream, objectRepository);

    if (objectStartBuffer[0] == 'n' && objectStartBuffer[1] == 'u')
      return await documentObjectReaderRepository.GetReader<NullObject>().Read(stream, objectRepository);

    if ((objectStartBuffer[0] == 't' && objectStartBuffer[1] == 'r') || (objectStartBuffer[0] == 'f' && objectStartBuffer[1] == 'a'))
      return await documentObjectReaderRepository.GetReader<BooleanObject>().Read(stream, objectRepository);

    throw new NotImplementedException("The object type passed was not yet implemented.");
  }

  private async Task<DocumentObject> ParseNumericOrIndirectObject(Stream stream, IObjectRepository objectRepository)
  {
    var oldPosition = stream.Position;

    try
    {
      return await documentObjectReaderRepository.GetReader<IndirectObject>().Read(stream, objectRepository);
    }
    catch (PdfInvalidIndirectObjectReferenceParsingException)
    {
      stream.Position = oldPosition;

      return await documentObjectReaderRepository.GetReader<NumericObject>().Read(stream, objectRepository);
    }
  }
}