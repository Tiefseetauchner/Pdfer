using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pdfer;

public interface IPdfDictionaryHelper
{
  Task<Dictionary<string, string>> ReadDictionary(byte[] bytes);
}