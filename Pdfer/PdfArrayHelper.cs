using System.IO;
using System.Threading.Tasks;
using Pdfer.Objects;

namespace Pdfer;

public static class PdfArrayHelper
{
  public static async Task WriteArray(Stream stream, DocumentObject[] array, IDocumentObjectSerializerRepository documentObjectSerializerRepository)
  {
    await stream.WriteAsync("[ "u8.ToArray());

    foreach (var value in array)
    {
      await documentObjectSerializerRepository.GetSerializer(value).Serialize(stream, value);
      await stream.WriteAsync(" "u8.ToArray());
    }

    await stream.WriteAsync("]"u8.ToArray());
  }
}