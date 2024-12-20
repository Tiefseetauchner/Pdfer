using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class DictionaryObjectReader(IPdfDictionaryHelper pdfDictionaryHelper) : IDocumentObjectReader<DictionaryObject>
{
  async Task<DocumentObject> IDocumentObjectReader.Read(Stream stream, IObjectRepository objectRepository) =>
    await Read(stream, objectRepository);

  public async Task<DictionaryObject> Read(Stream stream, IObjectRepository objectRepository)
  {
    var dictionary = await pdfDictionaryHelper.ReadDictionary(stream, objectRepository);

    return new DictionaryObject(dictionary);
  }
}