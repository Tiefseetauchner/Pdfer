using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class ArrayObjectReader(IStreamHelper streamHelper, PdfObjectReader pdfObjectReader) : IDocumentObjectReader<ArrayObject>
{
  async Task<DocumentObject> IDocumentObjectReader.Read(Stream stream) =>
    await Read(stream);

  public async Task<ArrayObject> Read(Stream stream)
  {
    var objects = new List<DocumentObject>();

    var firstChar = streamHelper.ReadChar(stream);

    if (firstChar != '[')
      throw new IOException($"Could not parse array: Expected '[' but got '{firstChar}'");

    while (streamHelper.PeakChar(stream) != ']')
    {
      await streamHelper.SkipWhiteSpaceCharacters(stream);
      objects.Add(await pdfObjectReader.Read(stream));
      await streamHelper.SkipWhiteSpaceCharacters(stream);
    }

    // NOTE: We have to skip the closing ']' character here.
    streamHelper.ReadChar(stream);

    return new ArrayObject(objects.ToArray());
  }
}