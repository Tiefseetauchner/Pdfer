using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class DictionaryObjectSerializer(IPdfDictionaryHelper pdfDictionaryHelper) : IDocumentObjectSerializer<DictionaryObject>
{
  public async Task Serialize(Stream stream, DictionaryObject documentObject)
  {
    await stream.WriteAsync(documentObject.ObjectIdentifier.GetHeaderBytes());
    await pdfDictionaryHelper.WriteDictionary(stream, documentObject.Value);
    await stream.WriteAsync("\nendobj"u8.ToArray());
  }
}