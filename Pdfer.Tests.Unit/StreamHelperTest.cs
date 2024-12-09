using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Pdfer.Tests.Unit;

public class StreamHelperTest
{
  [Test]
  public async Task ReadReverseLine_ShouldReturnLineInReverse()
  {
    var bytes = "Hello World\nLine2\nLine3"u8.ToArray();
    using var stream = new MemoryStream(bytes);
    await using var reverseStream = new ReverseStream(stream);
    reverseStream.Position = 0;
    using var streamReader = new StreamReader(reverseStream);

    var line3 = await new StreamHelper().ReadReverseLine(streamReader);
    var line2 = await new StreamHelper().ReadReverseLine(streamReader);
    var line1 = await new StreamHelper().ReadReverseLine(streamReader);

    Assert.Multiple(() =>
    {
      Assert.That(line3, Is.EqualTo("Line3"));
      Assert.That(line2, Is.EqualTo("Line2"));
      Assert.That(line1, Is.EqualTo("Hello World"));
    });
  }

  [Test]
  public async Task ReadStreamTo_ShouldReturnBytesUntilEndOfStream()
  {
    var bytes = "Hello World\nLine2\r\nLine3"u8.ToArray();
    using var stream = new MemoryStream(bytes);
    var bytesRead = await new StreamHelper().ReadStreamTo("World", stream);

    Assert.That(bytesRead, Is.EqualTo("Hello "u8.ToArray()));
  }

  [Test]
  public void ReadChar_ShouldReturnChar()
  {
    var bytes = "Hello World\nLine2\r\nLine3"u8.ToArray();
    using var stream = new MemoryStream(bytes);

    var character = new StreamHelper().ReadChar(stream);

    Assert.That(character, Is.EqualTo('H'));
  }

  [Test]
  public void PeakChar_ShouldReturnChar()
  {
    var bytes = "Hello World\nLine2\r\nLine3"u8.ToArray();
    using var stream = new MemoryStream(bytes);

    var oldPosition = stream.Position;
    var character = new StreamHelper().PeakChar(stream);

    Assert.Multiple(() =>
    {
      Assert.That(character, Is.EqualTo('H'));
      Assert.That(stream.Position, Is.EqualTo(oldPosition));
    });
  }

  [Test]
  public async Task Peak_ShouldReturnBytes()
  {
    var bytes = "Hello World\nLine2\r\nLine3"u8.ToArray();
    using var stream = new MemoryStream(bytes);

    var oldPosition = stream.Position;
    var buffer = new byte[5];
    var bytesRead = await new StreamHelper().Peak(stream, buffer);

    Assert.Multiple(() =>
    {
      Assert.That(bytesRead, Is.EqualTo(5));
      Assert.That(buffer, Is.EqualTo("Hello"u8.ToArray()));
      Assert.That(stream.Position, Is.EqualTo(oldPosition));
    });
  }

  [Test]
  public async Task SkipWhiteSpaceCharacters_ShouldSkipWhiteSpaceCharacters()
  {
    var bytes = "   \t \r\n\n\r    \t\nLine2\r\nLine3"u8.ToArray();
    using var stream = new MemoryStream(bytes);

    var whiteSpaceCharacters = await new StreamHelper().SkipWhiteSpaceCharacters(stream);

    Assert.Multiple(() =>
    {
      Assert.That(whiteSpaceCharacters, Is.EqualTo("   \t \r\n\n\r    \t\n"u8.ToArray()));
      Assert.That(stream.Position, Is.EqualTo(15));
    });
  }

  [Test]
  public async Task SkipWhiteSpaceCharacters_NoWhiteSpaces_ShouldReturnEmpty()
  {
    var bytes = "Hello World\nLine2\r\nLine3"u8.ToArray();
    using var stream = new MemoryStream(bytes);

    var whiteSpaceCharacters = await new StreamHelper().SkipWhiteSpaceCharacters(stream);

    Assert.Multiple(() =>
    {
      Assert.That(whiteSpaceCharacters, Is.Empty);
      Assert.That(stream.Position, Is.EqualTo(0));
    });
  }
}