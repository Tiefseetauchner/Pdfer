using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class DictionaryObjectReader(IPdfDictionaryHelper pdfDictionaryHelper) : IDocumentObjectReader<DictionaryObject>
{
  public async Task<DictionaryObject> Read(Stream stream, IObjectRepository objectRepository)
  {
    var dictionary = await pdfDictionaryHelper.ReadDictionary(stream, objectRepository);

    return new DictionaryObject(dictionary);
  }
}