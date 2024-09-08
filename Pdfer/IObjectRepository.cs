using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Pdfer.Objects;

namespace Pdfer;

public interface IObjectRepository
{
  Dictionary<ObjectIdentifier, DocumentObject> Objects { get; }

  Task<T?> RetrieveObject<T>(ObjectIdentifier objectIdentifier, Stream stream)
    where T : DocumentObject;
}