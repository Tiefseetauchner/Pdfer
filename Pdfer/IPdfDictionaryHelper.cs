using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Pdfer.Objects;

namespace Pdfer;

public interface IPdfDictionaryHelper
{
  Task<Dictionary<string, DocumentObject>> ReadDictionary(Stream stream);
}