using System.IO;
using System.Threading.Tasks;
using Pdfer.Objects;

namespace Pdfer;

public class PdfObjectReader : IPdfObjectReader
{
  // TODO (lena): Deal with NameObjects
  // TODO (lena): Deal with BooleanObjects
  // TODO (lena): Deal with NullObjects
  public Task<DocumentObject> Read(Stream stream, long xrefOffset)
  {
    // check whether obj starts with <[^<] or ( => It's a string, read to the next > or ) which is not prefixed by a \
    // else check whether obj starts with <<, then read the dictionary and stop stream at >>
    // check if next line equals "stream"
    //   then read object key "Length"
    //     then check whether reference and retrieve object
    //   then read that amount of bytes.
    // Find next endobj
    // 

    return null!;
  }
}