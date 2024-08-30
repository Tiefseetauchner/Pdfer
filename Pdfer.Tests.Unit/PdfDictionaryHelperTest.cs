using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Pdfer.Tests.Unit;

public class PdfDictionaryHelperTest
{
  [Test]
  public async Task Read_ShouldReturnDictionary()
  {
    var bytes = "/TestKey TestValue"u8.ToArray();

    var result = await new PdfDictionaryHelper(new StreamHelper()).ReadDictionary(bytes);

    Assert.That(result, Is.EqualTo(new Dictionary<string, string>
    {
      { "TestKey", "TestValue" }
    }));
  }

  [Test]
  public async Task Read_WithValueWithSpaces()
  {
    var bytes = "/TestKey Test Value with Spaces"u8.ToArray();

    var result = await new PdfDictionaryHelper(new StreamHelper()).ReadDictionary(bytes);

    Assert.That(result, Is.EqualTo(new Dictionary<string, string>
    {
      { "TestKey", "Test Value with Spaces" }
    }));
  }

  [Test]
  [TestCase("/TestKey")]
  [TestCase("/TestKey     ")]
  [TestCase("/TestKey     \n\n")]
  public async Task Read_NoValue_ReturnsEmpty(string key)
  {
    var bytes = Encoding.UTF8.GetBytes(key);

    var result = await new PdfDictionaryHelper(new StreamHelper()).ReadDictionary(bytes);

    Assert.That(result, Is.Empty);
  }
}