using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Pdfer.Objects;

namespace Pdfer;

public class PdfArrayHelper(IStreamHelper streamHelper) : IPdfArrayHelper
{
  public async Task<DocumentObject[]> ReadArray(Stream stream, IObjectRepository objectRepository)
  {
    var objects = new List<DocumentObject>();

    var firstChar = streamHelper.ReadChar(stream);

    if (firstChar != '[')
      throw new IOException($"Could not parse array: Expected '[' but got '{firstChar}'");

    while (streamHelper.PeakChar(stream) != ']')
    {
      await streamHelper.SkipWhiteSpaceCharacters(stream);
      objects.Add(await pdfObjectReader.Read(stream, objectRepository));
      await streamHelper.SkipWhiteSpaceCharacters(stream);
    }

    return objects.ToArray();
  }

  public async Task WriteArray(Stream stream, DocumentObject[] array)
  {
    await stream.WriteAsync("[ "u8.ToArray());

    foreach (var value in array)
    {
      // TODO (lena.tauchner): DocumentObjectWriter
      await stream.WriteAsync(Encoding.UTF8.GetBytes(value));
      await stream.WriteAsync(" "u8.ToArray());
    }

    await stream.WriteAsync("]"u8.ToArray());
  }
}