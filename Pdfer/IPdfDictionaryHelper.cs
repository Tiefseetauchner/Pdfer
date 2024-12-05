using System.IO;
using System.Threading.Tasks;
using Pdfer.Objects;

namespace Pdfer;

public interface IPdfDictionaryHelper
{
  Task<PdfDictionary> ReadDictionary(Stream stream);
}