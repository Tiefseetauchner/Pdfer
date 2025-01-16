using NUnit.Framework;
using Pdfer.Objects.ObjectReaders;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Tests.Unit.Objects.Readers
{
  public class NameObjectReaderTest
  {
    private NameObjectReader _reader;

    [SetUp]
    public void SetUp()
    {
      _reader = new NameObjectReader();
    }

    [Test]
    public async Task Read()
    {
      using var stream = new MemoryStream("/NameObject"u8.ToArray());
      var result = await _reader.Read(stream, null!);

      Assert.That(result.Value, Is.EqualTo("NameObject"));
    }

    private static readonly char[] s_delimitingCharacters = PdfCharacterHelper.DelimiterCharacters;

    [Test]
    [TestCaseSource(nameof(s_delimitingCharacters))]
    public async Task Read_EndsWithCharacter(char endCharacter)
    {
      using var stream = new MemoryStream("/NameObject"u8.ToArray().Append((byte)endCharacter).Concat(Encoding.ASCII.GetBytes("NotContainedInNameObject")).ToArray());
      var result = await _reader.Read(stream, null!);

      Assert.That(result.Value, Is.EqualTo("NameObject"));
    }

    [Test]
    public async Task Read_EndsWithWhiteSpace()
    {
      using var stream = new MemoryStream("/NameObject "u8.ToArray());
      var result = await _reader.Read(stream, null!);

      Assert.That(result.Value, Is.EqualTo("NameObject"));
    }

    [Test]
    public async Task Read_ContainsEncodedCharacter()
    {
      using var stream = new MemoryStream("/NameObject#28#FF#59"u8.ToArray());
      var result = await _reader.Read(stream, null!);

      Assert.That(result.Value, Is.EqualTo("NameObject#28#FF#59"));
    }

    [Test]
    public async Task Read_ContainsSpecialCharacter()
    {
      using var stream = new MemoryStream("/A;Name_With-Various***Characters?"u8.ToArray());
      var result = await _reader.Read(stream, null!);

      Assert.That(result.Value, Is.EqualTo("A;Name_With-Various***Characters?"));
    }
  }
}