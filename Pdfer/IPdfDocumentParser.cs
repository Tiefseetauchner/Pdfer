using System.IO;
using System.Threading.Tasks;

namespace Pdfer;

public interface IPdfDocumentParser
{
  Task<PdfDocument> Parse(byte[] bytes);
  Task<PdfDocument> Parse(Stream stream);
}