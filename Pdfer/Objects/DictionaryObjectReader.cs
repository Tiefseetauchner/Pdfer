using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class DictionaryObjectReader(IPdfDictionaryHelper pdfDictionaryHelper) : IDocumentObjectReader
{
  public async Task<DocumentObject> Read(Stream stream)
  {
    var dictionary = await pdfDictionaryHelper.ReadDictionary(stream);

    return new DictionaryObject(dictionary);
  }
}