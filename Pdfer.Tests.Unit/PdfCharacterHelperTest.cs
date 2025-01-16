using NUnit.Framework;

namespace Pdfer.Tests.Unit
{
  public class PdfCharacterHelperTest
  {
    [Test]
    [TestCase('(')]
    [TestCase(')')]
    [TestCase('<')]
    [TestCase('>')]
    [TestCase('[')]
    [TestCase(']')]
    [TestCase('{')]
    [TestCase('}')]
    [TestCase('/')]
    [TestCase('%')]
    [TestCase(' ')]
    [TestCase('\t')]
    [TestCase('\n')]
    [TestCase('\r')]
    public void IsDelimitingCharacter(char character) =>
      Assert.That(PdfCharacterHelper.IsDelimitingCharacter(character), Is.True);

    [Test]
    [TestCase('A')]
    [TestCase('H')]
    [TestCase('#')]
    [TestCase('*')]
    [TestCase('-')]
    [TestCase('9')]
    [TestCase('=')]
    [TestCase(';')]
    [TestCase(':')]
    [TestCase('"')]
    [TestCase('\'')]
    public void IsDelimitingCharacter_NotADelimitingCharacter(char character) =>
      Assert.That(PdfCharacterHelper.IsDelimitingCharacter(character), Is.False);
  }
}