using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class DictionaryObjectSerializer(IPdfDictionaryHelper pdfDictionaryHelper) : IDocumentObjectSerializer<DictionaryObject>
{
  public async Task Serialize(Stream stream, DictionaryObject documentObject)
  {
    await pdfDictionaryHelper.WriteDictionary(stream, documentObject.Value);
  }
}