using System;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class StreamObjectReader : IDocumentObjectReader<StreamObject>
{
  public async Task<StreamObject> Read(Stream stream, long? length = null)
  {
    if (length == null)
      throw new ArgumentException("Length must be specified for stream objects.");
    
    var buffer = new byte[length.Value];
    var bytesRead = await stream.ReadAsync(buffer);
    
    if (bytesRead != length)
      throw new IOException("Unexpected end of stream");
    
    return new StreamObject(buffer);
  }
}