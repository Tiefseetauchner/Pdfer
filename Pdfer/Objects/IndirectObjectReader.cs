using System;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class IndirectObjectReader : IDocumentObjectReader
{
  public Task<DocumentObject> Read(Stream stream)
  {
    throw new NotImplementedException();
  }
}