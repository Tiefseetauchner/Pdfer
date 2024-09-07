using System;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class StreamObjectReader(IPdfDictionaryHelper dictionaryHelper, IStreamHelper streamHelper) : IDocumentObjectReader<StreamObject>
{
  public async Task<StreamObject> Read(Stream stream, IObjectRepository objectRepository)
  {
    using var rawStream = new MemoryStream();
    
    var (dictionary, rawBytes) = await dictionaryHelper.ReadDictionary(stream);
    rawStream.Write(rawBytes);
    
    var length = long.Parse(dictionary["Length"]);
    
    var buffer = new byte[length];
    var bytesRead = await stream.ReadAsync(buffer);
    
    if (bytesRead != length)
      throw new IOException("Unexpected end of stream");
    
    rawStream.Write(buffer);
    rawStream.Write(await streamHelper.ReadStreamTo("endstream", stream));
    rawStream.Write("endstream"u8.ToArray());
    
    return new StreamObject(buffer, rawStream.ToArray());
  }
}