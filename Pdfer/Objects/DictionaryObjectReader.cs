using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class DictionaryObjectReader(IStreamHelper streamHelper, IPdfDictionaryHelper pdfDictionaryHelper) : IDocumentObjectReader<DictionaryObject>
{
  public async Task<DictionaryObject> Read(Stream stream, long? length = null)
  {
    await streamHelper.ReadStreamTo("<<", stream);

    var dictionary = await pdfDictionaryHelper.ReadDictionary(stream);
    
    // NOTE (lena): We want to skip to the next line here
    await streamHelper.ReadStreamTo("\n", stream);

    return new DictionaryObject(dictionary);
  }
}