using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class DictionaryObjectSerializer(IDocumentObjectSerializerRepository objectSerializerRepository) : IDocumentObjectSerializer<DictionaryObject>
{
  async Task IDocumentObjectSerializer.Serialize(Stream stream, DocumentObject documentObject) =>
    await Serialize(stream, (DictionaryObject)documentObject);

  public async Task Serialize(Stream stream, DictionaryObject documentObject)
  {
    await PdfDictionaryHelper.WriteDictionary(stream, documentObject.Value, objectSerializerRepository);
  }
}