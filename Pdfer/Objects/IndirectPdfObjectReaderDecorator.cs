using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class IIndirectPdfObjectReaderAdapterAdapter(IPdfObjectReader adaptee, IStreamHelper streamHelper) : IIndirectPdfObjectReaderAdapter
{
  public async Task<DocumentObject> Read(Stream stream, XRefEntry xRefEntry, IObjectRepository objectRepository)
  {
    stream.Position = xRefEntry.Position;

    var objectIdentifierString = Encoding.Default.GetString(await streamHelper.ReadStreamTo("\n", stream));

    if (!ObjectIdentifier.TryParseIdentifier(objectIdentifierString, out var objectIdentifier))
      throw new InvalidOperationException("Indirect object did not start with an object identifier.");

    var documentObject = await adaptee.Read(stream, objectRepository);

    // TODO (lena.tauchner): Set RawValue
    return new IndirectObject(documentObject, objectIdentifier);
  }
}

public interface IIndirectPdfObjectReaderAdapter
{
  Task<DocumentObject> Read(Stream stream, XRefEntry xRefEntry, IObjectRepository objectRepository);
}