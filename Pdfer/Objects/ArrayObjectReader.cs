using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class ArrayObjectReader(IStreamHelper streamHelper, IPdfObjectReader pdfObjectReader) : IDocumentObjectReader<ArrayObject>
{
  async Task<DocumentObject> IDocumentObjectReader.Read(Stream stream, IObjectRepository objectRepository) =>
    await Read(stream, objectRepository);

  public async Task<ArrayObject> Read(Stream stream, IObjectRepository objectRepository)
  {
    var objects = new List<DocumentObject>();

    var firstChar = streamHelper.ReadChar(stream);

    if (firstChar != '[')
      throw new IOException($"Could not parse array: Expected '[' but got '{firstChar}'");

    await streamHelper.SkipWhiteSpaceCharacters(stream);

    while (streamHelper.PeakChar(stream) != ']')
    {
      await streamHelper.SkipWhiteSpaceCharacters(stream);
      objects.Add(await pdfObjectReader.Read(stream, objectRepository));
      await streamHelper.SkipWhiteSpaceCharacters(stream);
    }

    // NOTE: We have to skip the closing ']' character here.
    streamHelper.ReadChar(stream);

    return new ArrayObject(objects.ToArray());
  }
}