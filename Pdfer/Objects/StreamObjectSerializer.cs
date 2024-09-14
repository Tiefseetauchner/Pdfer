using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class StreamObjectSerializer(PdfDictionaryHelper pdfDictionaryHelper) : IDocumentObjectSerializer<StreamObject>
{
  public async Task Serialize(Stream stream, StreamObject documentObject)
  {
    await stream.WriteAsync(documentObject.ObjectIdentifier.GetHeaderBytes());
    await pdfDictionaryHelper.WriteDictionary(stream, documentObject.Dictionary);
    await stream.WriteAsync("\nstream\n"u8.ToArray());
    await stream.WriteAsync(documentObject.Value);
    await stream.WriteAsync("\nendstream"u8.ToArray());
    await stream.WriteAsync("\nendobj"u8.ToArray());
  }
}