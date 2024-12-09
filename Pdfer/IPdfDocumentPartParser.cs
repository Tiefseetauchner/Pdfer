using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer;

public interface IPdfDocumentPartParser
{
  Task<List<PdfDocumentPart>> Parse(Stream stream);
}