using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects.Readers;

public class ArrayObjectReader(IPdfObjectReader pdfObjectReader) : IDocumentObjectReader<ArrayObject>
{
  async Task<DocumentObject> IDocumentObjectReader.Read(Stream stream, IObjectRepository objectRepository) =>
    await Read(stream, objectRepository);

  public async Task<ArrayObject> Read(Stream stream, IObjectRepository objectRepository)
  {
    var objects = new List<DocumentObject>();

    var firstChar = StreamHelper.ReadChar(stream);

    if (firstChar != '[')
      throw new IOException($"Could not parse array: Expected '[' but got '{firstChar}'");

    await StreamHelper.SkipWhiteSpaceCharacters(stream);

    while (StreamHelper.PeakChar(stream) != ']')
    {
      await StreamHelper.SkipWhiteSpaceCharacters(stream);
      objects.Add(await pdfObjectReader.Read(stream, objectRepository));
      await StreamHelper.SkipWhiteSpaceCharacters(stream);
    }

    // NOTE: We have to skip the closing ']' character here.
    StreamHelper.ReadChar(stream);

    return new ArrayObject(objects.ToArray());
  }
}