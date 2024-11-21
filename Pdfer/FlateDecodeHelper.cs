using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Pdfer;

public static class FlateDecodeHelper
{
  public static async Task<byte[]> Encode(byte[] input)
  {
    using var inputStream = new MemoryStream(input);

    return await Encode(inputStream);
  }

  public static async Task<byte[]> Encode(Stream inputStream)
  {
    using var outputStream = new MemoryStream();

    await Encode(inputStream, outputStream);

    return outputStream.ToArray();
  }

  public static async Task Encode(Stream inputStream, Stream outputStream)
  {
    await using var encodeStream = new ZLibStream(outputStream, CompressionMode.Compress);
    await inputStream.CopyToAsync(encodeStream);
  }

  public static async Task<byte[]> Decode(byte[] input)
  {
    using var inputStream = new MemoryStream(input);

    return await Decode(inputStream);
  }

  public static async Task<byte[]> Decode(Stream inputStream)
  {
    using var outputStream = new MemoryStream();

    await Decode(inputStream, outputStream);

    return outputStream.ToArray();
  }

  public static async Task Decode(Stream inputStream, Stream outputStream)
  {
    await using var encodeStream = new ZLibStream(inputStream, CompressionMode.Decompress);
    await encodeStream.CopyToAsync(outputStream);
  }
}