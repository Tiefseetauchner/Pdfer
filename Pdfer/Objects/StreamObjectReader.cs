using System;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class StreamObjectReader(
  IDocumentObjectReader dictionaryObjectReader,
  IStreamHelper streamHelper) : IDocumentObjectReader<StreamObject>
{
  async Task<DocumentObject> IDocumentObjectReader.Read(Stream stream, ObjectRepository objectRepository) =>
    await Read(stream, objectRepository);

  public async Task<StreamObject> Read(Stream stream, ObjectRepository objectRepository)
  {
    var dictionary = await dictionaryObjectReader.Read(stream, objectRepository);

    if (dictionary is not DictionaryObject dictionaryObject)
      throw new InvalidOperationException("Stream did not start with a dictionary object.");

    var oldPosition = stream.Position;
    var lengthObject = dictionaryObject.Value["Length"];
    var length = lengthObject switch
    {
      IntegerObject integerObject => integerObject.Value,
      IndirectObject indirectObject => (indirectObject.Value as IntegerObject)?.Value
                                       ?? throw new InvalidOperationException($"Object referenced by key '/Length' of stream object was of type {indirectObject.Value?.GetType()} but expected {typeof(IntegerObject)}."),
      _ => throw new InvalidOperationException($"Key '/Length' of stream object was of type {lengthObject.GetType()} but expected {typeof(IntegerObject)}.")
    };
    stream.Position = oldPosition;

    await streamHelper.ReadStreamTo("stream", stream);
    await streamHelper.SkipWhiteSpaceCharacters(stream);

    var buffer = new byte[length];
    var bytesRead = await stream.ReadAsync(buffer);

    if (bytesRead != length)
      throw new IOException("Unexpected end of stream");

    return new StreamObject(buffer, dictionaryObject);
  }
}