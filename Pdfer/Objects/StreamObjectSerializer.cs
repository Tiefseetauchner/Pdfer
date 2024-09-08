using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class StreamObjectSerializer(PdfDictionaryHelper pdfDictionaryHelper) : IDocumentObjectSerializer<StreamObject>
{
  public async Task<byte[]> Serialize(StreamObject documentObject)
  {
    using var memoryStream = new MemoryStream();

    await memoryStream.WriteAsync(documentObject.ObjectIdentifier.GetHeaderBytes());
    await pdfDictionaryHelper.WriteDictionary(memoryStream, documentObject.Dictionary);
    await memoryStream.WriteAsync(documentObject.Value);
    await memoryStream.WriteAsync("\nendstream"u8.ToArray());
    await memoryStream.WriteAsync("\nendobj"u8.ToArray());

    return memoryStream.ToArray();
  }
}