using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Pdfer.Objects;

namespace Pdfer.Tests.Unit.Objects;

public class DictionaryObjectReaderTest
{
  [Test]
  public async Task Read_ShouldReturnDictionary()
  {
    using var stream = new MemoryStream(
      """
        <</TestKey TestValue
        /TestKey2 TestValue2
        >>
        endobj
        """u8.ToArray());

    var result = await new DictionaryObjectReader(new StreamHelper(), new PdfDictionaryHelper(new StreamHelper())).Read(stream, null!, new ObjectIdentifier(1, 0));

    Assert.That(result.Value, Is.EquivalentTo(new Dictionary<string, string>()
    {
      { "/TestKey", "TestValue" },
      { "/TestKey2", "TestValue2" }
    }));
  }

  [Test]
  public async Task Read_ShouldReturnDictionary_NameObject()
  {
    using var stream = new MemoryStream(
      """
        <</TestKey /TestValue>>
        endobj
        """u8.ToArray());

    var result = await new DictionaryObjectReader(new StreamHelper(), new PdfDictionaryHelper(new StreamHelper())).Read(stream, null!, new ObjectIdentifier(1, 0));

    Assert.That(result.Value, Is.EquivalentTo(new Dictionary<string, string>()
    {
      { "/TestKey", "/TestValue" },
    }));
  }

  [Test]
  public async Task Read_ShouldReturnDictionary_NameObject_WithoutSpace()
  {
    using var stream = new MemoryStream(
      """
        <</TestKey/TestValue>>
        endobj
        """u8.ToArray());

    var result = await new DictionaryObjectReader(new StreamHelper(), new PdfDictionaryHelper(new StreamHelper())).Read(stream, null!, new ObjectIdentifier(1, 0));

    Assert.That(result.Value, Is.EquivalentTo(new Dictionary<string, string>()
    {
      { "/TestKey", "/TestValue" },
    }));
  }

  [Test]
  public async Task Read_ShouldReturnDictionary_StringObject()
  {
    using var stream = new MemoryStream(
      """
        <</TestKey (Test\)\(\<\>\>)>>
        endobj
        """u8.ToArray());

    var result = await new DictionaryObjectReader(new StreamHelper(), new PdfDictionaryHelper(new StreamHelper())).Read(stream, null!, new ObjectIdentifier(1, 0));

    Assert.That(result.Value, Is.EquivalentTo(new Dictionary<string, string>()
    {
      { "/TestKey", @"(Test\)\(\<\>\>)" },
    }));
  }

  [Test]
  public async Task Read_ShouldReturnDictionary_StringObject_WithoutSpace()
  {
    using var stream = new MemoryStream(
      """
        <</TestKey(Test\)\(\<\>\>)>>
        endobj
        """u8.ToArray());

    var result = await new DictionaryObjectReader(new StreamHelper(), new PdfDictionaryHelper(new StreamHelper())).Read(stream, null!, new ObjectIdentifier(1, 0));

    Assert.That(result.Value, Is.EquivalentTo(new Dictionary<string, string>()
    {
      { "/TestKey", @"(Test\)\(\<\>\>)" },
    }));
  }

  [Test]
  public async Task Read_ShouldReturnDictionary_HexStringObject()
  {
    using var stream = new MemoryStream(
      """
        <</TestKey <A7895t287639>>>
        endobj
        """u8.ToArray());

    var result = await new DictionaryObjectReader(new StreamHelper(), new PdfDictionaryHelper(new StreamHelper())).Read(stream, null!, new ObjectIdentifier(1, 0));

    Assert.That(result.Value, Is.EquivalentTo(new Dictionary<string, string>()
    {
      { "/TestKey", "<A7895t287639>" },
    }));
  }

  [Test]
  public async Task Read_ShouldReturnDictionary_HexStringObject_WithoutSpace()
  {
    using var stream = new MemoryStream(
      """
        <</TestKey<A7895t287639>>>
        endobj
        """u8.ToArray());

    var result = await new DictionaryObjectReader(new StreamHelper(), new PdfDictionaryHelper(new StreamHelper())).Read(stream, null!, new ObjectIdentifier(1, 0));

    Assert.That(result.Value, Is.EquivalentTo(new Dictionary<string, string>()
    {
      { "/TestKey", "<A7895t287639>" },
    }));
  }

  [Test]
  public async Task Read_ShouldReturnDictionary_Dictionary()
  {
    using var stream = new MemoryStream(
      """
        <</TestKey <</Beep/Boop/Type/Asdf /Asdf (aaaaa) /Pasdfghj <234123>>>>>
        endobj
        """u8.ToArray());

    var result = await new DictionaryObjectReader(new StreamHelper(), new PdfDictionaryHelper(new StreamHelper())).Read(stream, null!, new ObjectIdentifier(1, 0));

    Assert.That(result.Value, Is.EquivalentTo(new Dictionary<string, string>()
    {
      { "/TestKey", "<</Beep/Boop/Type/Asdf /Asdf (aaaaa) /Pasdfghj <234123>>>" },
    }));
  }

  [Test]
  public async Task Read_ShouldReturnDictionary_Dictionary_WithoutSpace()
  {
    using var stream = new MemoryStream(
      """
        <</TestKey<</Beep/Boop/Type/Asdf /Asdf (aaaaa) /Pasdfghj <234123>>>>>
        endobj
        """u8.ToArray());

    var result = await new DictionaryObjectReader(new StreamHelper(), new PdfDictionaryHelper(new StreamHelper())).Read(stream, null!, new ObjectIdentifier(1, 0));

    Assert.That(result.Value, Is.EquivalentTo(new Dictionary<string, string>()
    {
      { "/TestKey", "<</Beep/Boop/Type/Asdf /Asdf (aaaaa) /Pasdfghj <234123>>>" },
    }));
  }

  [Test]
  public async Task Read_ShouldReturnDictionary_NumberObject()
  {
    using var stream = new MemoryStream(
      """
        <</TestKey 4637>>
        endobj
        """u8.ToArray());

    var result = await new DictionaryObjectReader(new StreamHelper(), new PdfDictionaryHelper(new StreamHelper())).Read(stream, null!, new ObjectIdentifier(1, 0));

    Assert.That(result.Value, Is.EquivalentTo(new Dictionary<string, string>()
    {
      { "/TestKey", "4637" },
    }));
  }
}