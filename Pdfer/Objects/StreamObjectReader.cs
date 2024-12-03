using System;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class StreamObjectReader(
  IDocumentObjectReader<DictionaryObject> dictionaryObjectReader,
  IStreamHelper streamHelper) : IDocumentObjectReader<StreamObject>
{
  public async Task<StreamObject> Read(Stream stream, IObjectRepository objectRepository)
  {
    var dictionary = await dictionaryObjectReader.Read(stream, objectRepository);

    var oldPosition = stream.Position;
    var lengthObject = dictionary.Value["/Length"];
    var length = lengthObject switch
    {
      IntegerObject integerObject => integerObject.Value,
      IndirectObject indirectObject => (indirectObject.Value as IntegerObject)?.Value ?? throw new InvalidOperationException($"Key '/Length' of stream object was of type {indirectObject.Value.GetType()} but expected {typeof(IntegerObject)}."),
      _ => throw new InvalidOperationException($"Key '/Length' of stream object was of type {lengthObject.GetType()} but expected {typeof(IntegerObject)}.")
    };
    stream.Position = oldPosition;

    await streamHelper.ReadStreamTo("stream", stream);
    await streamHelper.SkipWhiteSpaceCharacters(stream);

    var buffer = new byte[length];
    var bytesRead = await stream.ReadAsync(buffer);

    if (bytesRead != length)
      throw new IOException("Unexpected end of stream");

    return new StreamObject(buffer, dictionary);
  }
}