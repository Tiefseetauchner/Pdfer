using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class DictionaryObjectReader(IStreamHelper streamHelper, IPdfDictionaryHelper pdfDictionaryHelper) : IDocumentObjectReader<DictionaryObject>
{
  public async Task<DictionaryObject> Read(Stream stream)
  {
    await streamHelper.ReadStreamTo("<<", stream);

    var dictionary = await pdfDictionaryHelper.ReadDictionary(stream);

    return new DictionaryObject(dictionary);
  }
}