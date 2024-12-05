using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class IndirectPdfObjectReaderAdapter(IPdfObjectReader adaptee, IStreamHelper streamHelper) : IIndirectPdfObjectReaderAdapter
{
  public async Task<DocumentObject> Read(Stream stream, XRefEntry xRefEntry)
  {
    stream.Position = xRefEntry.Position;

    var objectIdentifierString = Encoding.Default.GetString(await streamHelper.ReadStreamTo("\n", stream));

    if (!ObjectIdentifier.TryParseIdentifier(objectIdentifierString, out var objectIdentifier))
      throw new InvalidOperationException("Indirect object did not start with an object identifier.");

    var documentObject = await adaptee.Read(stream);

    return new IndirectObject(documentObject, objectIdentifier);
  }
}

public interface IIndirectPdfObjectReaderAdapter
{
  Task<DocumentObject> Read(Stream stream, XRefEntry xRefEntry);
}