using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer;

public interface IPdfDictionaryHelper
{
  Task<(Dictionary<string, string> dictionary, byte[] bytes)> ReadDictionary(Stream stream);
}