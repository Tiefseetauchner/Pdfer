using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class DictionaryObjectReader(IPdfDictionaryHelper pdfDictionaryHelper) : IDocumentObjectReader<DictionaryObject>
{
  async Task<DocumentObject> IDocumentObjectReader.Read(Stream stream) =>
    await Read(stream);

  public async Task<DictionaryObject> Read(Stream stream)
  {
    var dictionary = await pdfDictionaryHelper.ReadDictionary(stream);

    return new DictionaryObject(dictionary);
  }
}