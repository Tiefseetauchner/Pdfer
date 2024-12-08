using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class StreamObjectSerializer(IDocumentObjectSerializerRepository objectSerializerRepository) : IDocumentObjectSerializer<StreamObject>
{
  async Task IDocumentObjectSerializer.Serialize(Stream stream, DocumentObject documentObject) =>
    await Serialize(stream, (StreamObject)documentObject);

  public async Task Serialize(Stream stream, StreamObject documentObject)
  {
    await PdfDictionaryHelper.WriteDictionary(stream, documentObject.Dictionary.Value, objectSerializerRepository);
    await stream.WriteAsync("\nstream\n"u8.ToArray());
    await stream.WriteAsync(documentObject.Value);
    await stream.WriteAsync("\nendstream"u8.ToArray());
  }
}