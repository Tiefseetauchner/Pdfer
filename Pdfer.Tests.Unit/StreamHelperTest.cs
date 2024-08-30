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
}