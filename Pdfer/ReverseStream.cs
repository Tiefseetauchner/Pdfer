using System;
using System.IO;

namespace Pdfer;

public class ReverseStream : Stream
{
  readonly Stream stream;

  public ReverseStream(Stream stream)
  {
    if (!stream.CanSeek) throw new Exception("Stream cannot seek");

    stream.Seek(stream.Position, SeekOrigin.End);
    this.stream = stream;
  }

  public override bool CanRead => true;

  public override bool CanSeek => true;

  public override bool CanWrite => false;

  public override long Length => stream.Length;

  public override long Position
  {
    get
    {
      var position = stream.Length - stream.Position;
      return position;
    }

    set => stream.Position = stream.Length - value;
  }

  public override int Read(byte[] buffer, int offset, int count)
  {
    if (stream.Position == 0) return 0;

    var startReadFrom = stream.Position - count;
    if (startReadFrom < 0)
    {
      count += (int)startReadFrom;
      startReadFrom = 0;
    }

    stream.Seek(startReadFrom, SeekOrigin.Begin);
    var bytesRead = stream.Read(buffer, offset, count);
    stream.Seek(startReadFrom, SeekOrigin.Begin);

    Array.Reverse(buffer, offset, bytesRead);

    // Adjust for \r\n
    for (var i = offset; i < offset + bytesRead - 1; i++)
    {
      if (buffer[i] != '\n' || buffer[i + 1] != '\r')
        continue;

      (buffer[i], buffer[i + 1]) = (buffer[i + 1], buffer[i]);
    }

    return bytesRead;
  }

  public override long Seek(long offset, SeekOrigin origin)
  {
    switch (origin)
    {
      case SeekOrigin.Begin:
        stream.Seek(offset, SeekOrigin.End);
        break;

      case SeekOrigin.End:
        stream.Seek(offset, SeekOrigin.Begin);
        break;

      case SeekOrigin.Current:
        stream.Seek(-offset, SeekOrigin.Current);
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
    }

    return Position;
  }

  public override void SetLength(long value)
  {
  }

  public override void Write(byte[] buffer, int offset, int count)
  {
  }

  public override void Flush()
  {
  }
}