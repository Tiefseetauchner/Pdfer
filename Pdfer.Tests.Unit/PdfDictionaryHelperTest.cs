using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Pdfer.Tests.Unit;

public class PdfDictionaryHelperTest
{
  [Test]
  public async Task Read_ShouldReturnDictionary()
  {
    using var stream = new MemoryStream("<</TestKey TestValue>>Irrellevant Text"u8.ToArray());

    var result = await new PdfDictionaryHelper(new StreamHelper()).ReadDictionary(stream);

    Assert.That(result.dictionary, Is.EquivalentTo(new Dictionary<string, string>
    {
      { "/TestKey", "TestValue" }
    }));
  }

  [Test]
  public async Task Read_WithValueWithSpaces()
  {
    using var stream = new MemoryStream("<</TestKey Test Value with Spaces>>Irrellevant Text"u8.ToArray());

    var result = await new PdfDictionaryHelper(new StreamHelper()).ReadDictionary(stream);

    Assert.That(result.dictionary, Is.EquivalentTo(new Dictionary<string, string>
    {
      { "/TestKey", "Test Value with Spaces" }
    }));
  }

  [Test]
  public async Task Read_NestedDictionary()
  {
    using var stream = new MemoryStream("<</TestKey<</NestedTestKey/L2R>>>>Irrellevant Text"u8.ToArray());

    var result = await new PdfDictionaryHelper(new StreamHelper()).ReadDictionary(stream);

    Assert.That(result.dictionary, Is.EquivalentTo(new Dictionary<string, string>
    {
      { "/TestKey", "<</NestedTestKey/L2R>>" }
    }));
  }

  [Test]
  public async Task Read_MultipleEntries()
  {
    using var stream = new MemoryStream("<</TestKey Test Value 1 with Spaces/TestKey2 Test Value 2 with Spaces/TestKey3 Test Value 3 with Spaces>>Irrellevant Text"u8.ToArray());

    var result = await new PdfDictionaryHelper(new StreamHelper()).ReadDictionary(stream);

    Assert.That(result.dictionary, Is.EquivalentTo(new Dictionary<string, string>
    {
      { "/TestKey", "Test Value 1 with Spaces" },
      { "/TestKey2", "Test Value 2 with Spaces" },
      { "/TestKey3", "Test Value 3 with Spaces" }
    }));
  }

  [Test]
  [TestCase("<</TestKey>>Irrellevant Text")]
  [TestCase("<</TestKey     >>Irrellevant Text")]
  [TestCase("<</TestKey     \n\n>>Irrellevant Text")]
  public void Read_NoValue_ReturnsEmpty(string key)
  {
    using var stream = new MemoryStream(Encoding.UTF8.GetBytes(key));

    var exception = Assert.ThrowsAsync<InvalidOperationException>(() => new PdfDictionaryHelper(new StreamHelper()).ReadDictionary(stream));

    Assert.That(exception.Message, Is.EqualTo($"Empty value for key '/TestKey' in dictionary"));
  }
}