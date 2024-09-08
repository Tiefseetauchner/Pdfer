using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class DictionaryObjectSerializer(IPdfDictionaryHelper pdfDictionaryHelper) : IDocumentObjectSerializer<DictionaryObject>
{
  public async Task<byte[]> Serialize(DictionaryObject documentObject)
  {
    using var memoryStream = new MemoryStream();

    await memoryStream.WriteAsync(documentObject.ObjectIdentifier.GetHeaderBytes());
    await pdfDictionaryHelper.WriteDictionary(memoryStream, documentObject.Value);
    await memoryStream.WriteAsync("\nendobj"u8.ToArray());

    return memoryStream.ToArray();
  }
}