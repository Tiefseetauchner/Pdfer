using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class ArrayObjectReader(IStreamHelper streamHelper, PdfObjectReader pdfObjectReader) : IDocumentObjectReader
{
  public async Task<DocumentObject> Read(Stream stream)
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

    return new ArrayObject(objects.ToArray());
  }
}