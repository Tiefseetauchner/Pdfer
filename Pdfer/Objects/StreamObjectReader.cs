using System;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class StreamObjectReader(IPdfDictionaryHelper dictionaryHelper, IStreamHelper streamHelper) : IDocumentObjectReader<StreamObject>
{
  public async Task<StreamObject> Read(Stream stream, IObjectRepository objectRepository, ObjectIdentifier objectIdentifier)
  {
    using var rawStream = new MemoryStream();
    rawStream.Write(objectIdentifier.GetHeaderBytes());

    var (dictionary, rawBytes) = await dictionaryHelper.ReadDictionary(stream);
    rawStream.Write(rawBytes);

    var oldPosition = stream.Position;
    var length = long.TryParse(dictionary["/Length"], out var lengthNumber)
      ? lengthNumber
      : (await objectRepository.RetrieveObject<IntegerObject>(ObjectIdentifier.ParseReference(dictionary["/Length"]), stream))?.Value
        ?? throw new ArgumentException("Invalid length of stream object");
    stream.Position = oldPosition;

    var buffer = new byte[length];
    var bytesRead = await stream.ReadAsync(buffer);

    if (bytesRead != length)
      throw new IOException("Unexpected end of stream");

    rawStream.Write(buffer);
    rawStream.Write(await streamHelper.ReadStreamTo("endstream", stream));
    rawStream.Write("endstream\n"u8.ToArray());
    rawStream.Write("endobj"u8.ToArray());

    return new StreamObject(buffer, rawStream.ToArray(), objectIdentifier);
  }
}