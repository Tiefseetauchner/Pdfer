using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class DictionaryObjectReader(IPdfDictionaryHelper pdfDictionaryHelper) : IDocumentObjectReader<DictionaryObject>
{
  public async Task<DictionaryObject> Read(Stream stream)
  {
    var dictionary = await pdfDictionaryHelper.ReadDictionary(stream);

    return new DictionaryObject(dictionary);
  }
}