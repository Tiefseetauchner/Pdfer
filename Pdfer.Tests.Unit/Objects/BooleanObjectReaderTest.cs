using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Pdfer.Objects;

namespace Pdfer.Tests.Unit.Objects;

public class BooleanObjectReaderTest
{
  [Test]
  public async Task Read_True()
  {
    using var stream = new MemoryStream("true"u8.ToArray());

    var reader = new BooleanObjectReader(new StreamHelper());

    var result = await reader.Read(stream, null!);

    TypeAssert.VerifyInstanceOf<BooleanObject>(result, _ => Assert.That(_.Value, Is.True));
  }

  [Test]
  public async Task Read_False()
  {
    using var stream = new MemoryStream("false"u8.ToArray());

    var reader = new BooleanObjectReader(new StreamHelper());

    var result = await reader.Read(stream, null!);

    TypeAssert.VerifyInstanceOf<BooleanObject>(result, _ => Assert.That(_.Value, Is.False));
  }

  [Test]
  [TestCase("fabse", "Expected 'false' but got 'fabse'")]
  [TestCase("trub", "Expected 'true' but got 'trub'")]
  [TestCase("sass", "Expected the object to start with 't' or 'f' but got 's'")]
  [TestCase("", "Expected the object to start with 't' or 'f' but got '\uffff'")]
  public void Read_Invalid_Throws(string value, string exceptionMessage)
  {
    using var stream = new MemoryStream(Encoding.Default.GetBytes(value));

    var reader = new BooleanObjectReader(new StreamHelper());

    var exception = Assert.ThrowsAsync<PdfInvalidBooleanValueParsingException>(() => reader.Read(stream, null!));

    Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
  }
}