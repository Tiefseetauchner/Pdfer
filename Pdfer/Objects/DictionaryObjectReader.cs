using System;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class DictionaryObjectReader(IStreamHelper streamHelper, IPdfDictionaryHelper pdfDictionaryHelper) : IDocumentObjectReader<DictionaryObject>
{
  public async Task<DictionaryObject> Read(Stream stream, IObjectRepository objectRepository)
  {
    using var rawData = new MemoryStream();
    
    var (dictionary, dictionaryBytes) = await pdfDictionaryHelper.ReadDictionary(stream);
    rawData.Write(dictionaryBytes);
    
    rawData.Write(await streamHelper.ReadStreamTo("endobj", stream));
    
    rawData.Write("enobj"u8.ToArray());

    return new DictionaryObject(dictionary, rawData.ToArray());
  }
}