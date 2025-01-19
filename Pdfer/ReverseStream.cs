using System;
using System.IO;

namespace Pdfer;

public class ReverseStream : Stream
{
  private readonly Stream _stream;

  public ReverseStream(Stream stream)
  {
    if (!stream.CanSeek) throw new Exception("Stream cannot seek");

    stream.Seek(stream.Position, SeekOrigin.End);
    this._stream = stream;
  }

  public override bool CanRead => true;

  public override bool CanSeek => true;

  public override bool CanWrite => false;

  public override long Length => _stream.Length;

  public override long Position
  {
    get
    {
      var position = _stream.Length - _stream.Position;
      return position;
    }

    set => _stream.Position = _stream.Length - value;
  }

  public override int Read(byte[] buffer, int offset, int count)
  {
    if (_stream.Position == 0) return 0;

    var startReadFrom = _stream.Position - count;
    if (startReadFrom < 0)
    {
      count += (int)startReadFrom;
      startReadFrom = 0;
    }

    _stream.Seek(startReadFrom, SeekOrigin.Begin);
    var bytesRead = _stream.Read(buffer, offset, count);
    _stream.Seek(startReadFrom, SeekOrigin.Begin);

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
        _stream.Seek(offset, SeekOrigin.End);
        break;

      case SeekOrigin.End:
        _stream.Seek(offset, SeekOrigin.Begin);
        break;

      case SeekOrigin.Current:
        _stream.Seek(-offset, SeekOrigin.Current);
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