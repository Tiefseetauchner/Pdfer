using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Objects.ObjectReaders;

public class IndirectPdfObjectReader(IPdfObjectReader adaptee) : IIndirectPdfObjectReader
{
  public async Task<DocumentObject> Read(Stream stream, XRefEntry xRefEntry, ObjectRepository objectRepository)
  {
    stream.Position = xRefEntry.Position;

    var objectIdentifierString = Encoding.Default.GetString(await StreamHelper.ReadStreamTo("\n", stream));

    if (!ObjectIdentifier.TryParseIdentifier(objectIdentifierString, out var objectIdentifier))
      throw new InvalidOperationException("Indirect object did not start with an object identifier.");

    var documentObject = await adaptee.Read(stream, objectRepository);

    return documentObject;
  }
}