using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public interface IIndirectPdfObjectReaderAdapter
{
  Task<DocumentObject> Read(Stream stream, XRefEntry xRefEntry, ObjectRepository objectRepository);
}