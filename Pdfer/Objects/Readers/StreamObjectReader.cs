using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects.Readers;

public class StreamObjectReader(
  IDocumentObjectReader<DictionaryObject> dictionaryObjectReader) : IDocumentObjectReader<StreamObject>
{
  async Task<DocumentObject> IDocumentObjectReader.Read(Stream stream, IObjectRepository objectRepository) =>
    await Read(stream, objectRepository);

  public async Task<StreamObject> Read(Stream stream, IObjectRepository objectRepository)
  {
    var dictionaryObject = await dictionaryObjectReader.Read(stream, objectRepository);

    var oldPosition = stream.Position;
    var lengthObject = dictionaryObject.Value["Length"];
    var length = lengthObject switch
    {
      IntegerObject integerObject => integerObject.Value,
      ReferenceObject referenceObject => (referenceObject.Value as IntegerObject)?.Value
                                         ?? throw new PdfInvalidIndirectObjectReferenceParsingException($"Object referenced by key '/Length' of stream object was of type {referenceObject.Value?.GetType()} but expected {typeof(IntegerObject)}."),
      _ => throw new PdfInvalidIndirectObjectReferenceParsingException($"Key '/Length' of stream object was of type {lengthObject.GetType()} but expected {typeof(IntegerObject)}.")
    };
    stream.Position = oldPosition;

    await StreamHelper.ReadStreamTo("stream", stream);
    await StreamHelper.SkipWhiteSpaceCharacters(stream);

    var buffer = new byte[length];
    var bytesRead = await stream.ReadAsync(buffer);

    if (bytesRead != length)
      throw new IOException("Unexpected end of stream");

    await ReadEndStream(stream);

    return new StreamObject(buffer, dictionaryObject);
  }

  private static async Task ReadEndStream(Stream stream)
  {
    await StreamHelper.SkipWhiteSpaceCharacters(stream);

    var buffer = new byte[9];
    var read = await stream.ReadAsync(buffer);
    var bufferText = Encoding.Default.GetString(buffer);
    if (read != 9 || bufferText != "endstream")
      throw new PdfInvalidStreamEndParsingException($"Expected 'endstream' but got '{bufferText}'");
  }
}