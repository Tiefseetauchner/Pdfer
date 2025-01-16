using NUnit.Framework;
using Pdfer.Objects;
using Pdfer.Objects.ObjectReaders;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Tests.Unit.Objects.Readers;

[TestFixture]
public class StringObjectReaderTests
{
  private StringObjectReader _reader;

  [SetUp]
  public void SetUp()
  {
    _reader = new StringObjectReader();
  }

  [Test]
  public async Task ReadLiteralString_ValidInput_ReturnsExpectedString()
  {
    using var stream = new MemoryStream(Encoding.UTF8.GetBytes("(Hello, World!)"));
    var result = await _reader.Read(stream, null!);

    Assert.That(result.Value, Is.EqualTo("(Hello, World!)"));
  }

  [Test]
  public async Task ReadLiteralString_EscapedCharacters_ReturnsDecodedString()
  {
    using var stream = new MemoryStream(Encoding.UTF8.GetBytes("(Line1\\nLine2\\tTabbed)"));
    var result = await _reader.Read(stream, null!);

    Assert.That(result.Value, Is.EqualTo("(Line1\\nLine2\\tTabbed)"));
  }

  [Test]
  public void ReadLiteralString_UnmatchedParentheses_ThrowsException()
  {
    using var stream = new MemoryStream(Encoding.UTF8.GetBytes("(Unmatched (parentheses)"));

    Assert.ThrowsAsync<PdfInvalidLiteralStringValueParsingException>(() => _reader.Read(stream, null!));
  }

  [Test]
  public async Task ReadHexString_ValidInput_ReturnsExpectedString()
  {
    using var stream = new MemoryStream(Encoding.UTF8.GetBytes("<48656C6C6F2C20576F726C6421>"));
    var result = await _reader.Read(stream, null!);

    Assert.That(result.Value, Is.EqualTo("<48656C6C6F2C20576F726C6421>"));
  }

  [Test]
  public void ReadHexString_InvalidHex_ThrowsException()
  {
    using var stream = new MemoryStream(Encoding.UTF8.GetBytes("<48656G6F2C20576F726C6421>"));

    Assert.ThrowsAsync<PdfInvalidHexStringValueParsingException>(() => _reader.Read(stream, null!));
  }

  [Test]
  public async Task ReadHexString_EmptyInput_ReturnsEmptyString()
  {
    using var stream = new MemoryStream(Encoding.UTF8.GetBytes("<>"));
    var result = await _reader.Read(stream, null!);

    Assert.That(result.Value, Is.EqualTo("<>"));
  }
}