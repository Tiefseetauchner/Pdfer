using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class IndirectPdfObjectReader(IPdfObjectReader pdfObjectReader) : IIndirectPdfObjectReader
{
  public async Task<DocumentObject> Read(Stream stream, XRefEntry xRefEntry, IObjectRepository objectRepository)
  {
    stream.Position = xRefEntry.Position;

    var objectIdentifierString = Encoding.Default.GetString(await StreamHelper.ReadStreamTo("\n", stream));

    if (!ObjectIdentifier.TryParseIdentifier(objectIdentifierString, out _))
      throw new PdfInvalidIndirectObjectReferenceParsingException("Indirect object did not start with an object identifier.");

    var documentObject = await pdfObjectReader.Read(stream, objectRepository);

    return documentObject;
  }
}