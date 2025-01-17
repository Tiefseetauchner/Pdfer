using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Pdfer.Objects;
using Pdfer.Objects.Readers;

namespace Pdfer.Tests.Unit.Objects.Readers;

public class NullObjectReaderTest
{
  [Test]
  public async Task Read()
  {
    var stream = new MemoryStream("null"u8.ToArray());

    await new NullObjectReader().Read(stream, null!);
  }

  [Test]
  public void Read_Invalid_ThrowsException()
  {
    var stream = new MemoryStream("noll"u8.ToArray());

    var exception = Assert.ThrowsAsync<PdfInvalidNullObjectValueParsingException>(() => new NullObjectReader().Read(stream, null!));

    Assert.That(exception.Message, Is.EqualTo("Expected 'null' but got 'noll'"));
  }
}