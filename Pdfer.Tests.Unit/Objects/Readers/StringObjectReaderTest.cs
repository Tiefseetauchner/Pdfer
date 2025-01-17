using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Pdfer.Objects;
using Pdfer.Objects.Readers;

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
    using var stream = new MemoryStream("(Hello, World!)"u8.ToArray());
    var result = await _reader.Read(stream, null!);

    Assert.That(result.Value, Is.EqualTo("(Hello, World!)"));
  }

  [Test]
  public async Task ReadLiteralString_EscapedCharacters_ReturnsDecodedString()
  {
    using var stream = new MemoryStream(@"(Line1\nLine2\tTabbed)"u8.ToArray());
    var result = await _reader.Read(stream, null!);

    Assert.That(result.Value, Is.EqualTo("(Line1\\nLine2\\tTabbed)"));
  }

  [Test]
  public void ReadLiteralString_UnmatchedParentheses_ThrowsException()
  {
    using var stream = new MemoryStream("(Unmatched (parentheses)"u8.ToArray());

    Assert.ThrowsAsync<PdfInvalidLiteralStringValueParsingException>(() => _reader.Read(stream, null!));
  }

  [Test]
  public async Task ReadHexString_ValidInput_ReturnsExpectedString()
  {
    using var stream = new MemoryStream("<48656C6C6F2C20576F726C6421>"u8.ToArray());
    var result = await _reader.Read(stream, null!);

    Assert.That(result.Value, Is.EqualTo("<48656C6C6F2C20576F726C6421>"));
  }

  [Test]
  public void ReadHexString_InvalidHex_ThrowsException()
  {
    using var stream = new MemoryStream("<48656G6F2C20576F726C6421>"u8.ToArray());

    Assert.ThrowsAsync<PdfInvalidHexStringValueParsingException>(() => _reader.Read(stream, null!));
  }

  [Test]
  public async Task ReadHexString_EmptyInput_ReturnsEmptyString()
  {
    using var stream = new MemoryStream("<>"u8.ToArray());
    var result = await _reader.Read(stream, null!);

    Assert.That(result.Value, Is.EqualTo("<>"));
  }

  [Test]
  public void Read_UnexpectedEndOfStream_Throws()
  {
    using var stream = new MemoryStream([]);
    var exception = Assert.ThrowsAsync<IOException>(() => _reader.Read(stream, null!));

    Assert.That(exception.Message, Is.EqualTo("Unexpected end of stream"));
  }

  [Test]
  public void ReadHexString_NotClosed_Throws()
  {
    using var stream = new MemoryStream("<ABCDEF"u8.ToArray());
    var exception = Assert.ThrowsAsync<PdfInvalidHexStringValueParsingException>(() => _reader.Read(stream, null!));

    Assert.That(exception.Message, Is.EqualTo("Hexadecimal string was not closed."));
  }

  [Test]
  public void ReadLiteralString_NotClosed_Throws()
  {
    using var stream = new MemoryStream("(A literal string"u8.ToArray());
    var exception = Assert.ThrowsAsync<PdfInvalidLiteralStringValueParsingException>(() => _reader.Read(stream, null!));

    Assert.That(exception.Message, Is.EqualTo("Literal string was not closed."));
  }
}