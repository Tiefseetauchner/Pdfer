using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class DictionaryObjectSerializer(IDocumentObjectSerializerRepository objectSerializerRepository) : IDocumentObjectSerializer<DictionaryObject>
{
  public async Task Serialize(Stream stream, DictionaryObject documentObject)
  {
    await PdfDictionaryHelper.WriteDictionary(stream, documentObject.Value, objectSerializerRepository);
  }
}