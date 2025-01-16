using Pdfer.Objects;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer;

public interface IObjectRepository
{
  Dictionary<ObjectIdentifier, DocumentObject> Objects { get; }

  Task<T?> RetrieveObject<T>(ObjectIdentifier objectIdentifier, Stream stream)
    where T : DocumentObject;
}