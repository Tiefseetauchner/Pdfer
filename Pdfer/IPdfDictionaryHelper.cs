using Pdfer.Objects;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer;

public interface IPdfDictionaryHelper
{
  Task<PdfDictionary> ReadDictionary(Stream stream, IObjectRepository objectRepository);
}