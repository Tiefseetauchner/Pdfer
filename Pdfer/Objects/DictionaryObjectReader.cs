using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class DictionaryObjectReader(IStreamHelper streamHelper, IPdfDictionaryHelper pdfDictionaryHelper) : IDocumentObjectReader<DictionaryObject>
{
  public async Task<DictionaryObject> Read(Stream stream, IObjectRepository objectRepository, byte[] objectIdentifier)
  {
    using var rawData = new MemoryStream();

    rawData.Write(objectIdentifier);

    var (dictionary, dictionaryBytes) = await pdfDictionaryHelper.ReadDictionary(stream);
    rawData.Write(dictionaryBytes);

    rawData.Write(await streamHelper.ReadStreamTo("endobj", stream));

    rawData.Write("endobj"u8.ToArray());

    return new DictionaryObject(dictionary, rawData.ToArray());
  }
}