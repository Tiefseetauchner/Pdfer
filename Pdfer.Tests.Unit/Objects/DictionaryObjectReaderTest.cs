using System.IO;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Pdfer.Objects;

namespace Pdfer.Tests.Unit.Objects;

public class DictionaryObjectReaderTest
{
  private Mock<IObjectRepository> _objectRepository;

  private DictionaryObjectReader _dictionaryObjectReader;

  [SetUp]
  public void Setup()
  {
    _objectRepository = new Mock<IObjectRepository>();

    var pdfObjectReader = PdfObjectReaderFactory.Create();

    _dictionaryObjectReader = new DictionaryObjectReader(
      new PdfDictionaryHelper(
        new StreamHelper(),
        pdfObjectReader));
  }

  [Test]
  public async Task Read()
  {
    using var stream = new MemoryStream(
      """
        <</TestKey /TestValue
        /TestKey2 /TestValue2
        >>
        """u8.ToArray());

    var result = await _dictionaryObjectReader.Read(stream, _objectRepository.Object);

    Assert.Multiple(() =>
    {
      TypeAssert.VerifyInstanceOf<NameObject>(result.Value["TestKey"], _ => Assert.That(_.Value, Is.EqualTo("TestValue")));
      TypeAssert.VerifyInstanceOf<NameObject>(result.Value["TestKey2"], _ => Assert.That(_.Value, Is.EqualTo("TestValue2")));
    });
  }

  [Test]
  public async Task Read_NameObject()
  {
    using var stream = new MemoryStream(
      "<</TestKey /TestValue>>"u8.ToArray());

    var result = await _dictionaryObjectReader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<NameObject>(result.Value["TestKey"], _ => Assert.That(_.Value, Is.EqualTo("TestValue")));
  }

  [Test]
  public async Task Read_NameObject_WithoutSpace()
  {
    using var stream = new MemoryStream(
      "<</TestKey/TestValue>>"u8.ToArray());

    var result = await _dictionaryObjectReader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<NameObject>(result.Value["TestKey"], _ => Assert.That(_.Value, Is.EqualTo("TestValue")));
  }

  [Test]
  public async Task Read_StringObject()
  {
    using var stream = new MemoryStream(
      @"<</TestKey (Test\)\(\<\>\>)>>"u8.ToArray());

    var result = await _dictionaryObjectReader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<StringObject>(result.Value["TestKey"], _ => Assert.That(_.Value, Is.EqualTo(@"(Test\)\(\<\>\>)")));
  }

  [Test]
  public async Task Read_StringObject_WithoutSpace()
  {
    using var stream = new MemoryStream(
      @"<</TestKey(Test\)\(\<\>\>)>>"u8.ToArray());

    var result = await _dictionaryObjectReader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<StringObject>(result.Value["TestKey"], _ => Assert.That(_.Value, Is.EqualTo(@"(Test\)\(\<\>\>)")));
  }

  [Test]
  public async Task Read_HexStringObject()
  {
    using var stream = new MemoryStream(
      "<</TestKey <A7895t287639>>>"u8.ToArray());

    var result = await _dictionaryObjectReader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<StringObject>(result.Value["TestKey"], _ => Assert.That(_.Value, Is.EqualTo("<A7895t287639>")));
  }

  [Test]
  public async Task Read_HexStringObject_WithoutSpace()
  {
    using var stream = new MemoryStream(
      "<</TestKey<A7895t287639>>>"u8.ToArray());

    var result = await _dictionaryObjectReader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<StringObject>(result.Value["TestKey"], _ => Assert.That(_.Value, Is.EqualTo("<A7895t287639>")));
  }

  [Test]
  public async Task Read_NestedDictionary()
  {
    using var stream = new MemoryStream(
      "<</TestKey <</Beep/Boop/Type/Asdf /Asdf (aaaaa) /Pasdfghj <234123>>>>>"u8.ToArray());

    var result = await _dictionaryObjectReader.Read(stream, _objectRepository.Object);

    Assert.Multiple(() =>
    {
      TypeAssert.VerifyInstanceOf<DictionaryObject>(result.Value["TestKey"], nestedDictionary =>
      {
        TypeAssert.VerifyInstanceOf<NameObject>(nestedDictionary.Value["Beep"], _ => Assert.That(_.Value, Is.EqualTo("Boop")));
        TypeAssert.VerifyInstanceOf<NameObject>(nestedDictionary.Value["Type"], _ => Assert.That(_.Value, Is.EqualTo("Asdf")));
        TypeAssert.VerifyInstanceOf<StringObject>(nestedDictionary.Value["Asdf"], _ => Assert.That(_.Value, Is.EqualTo("(aaaaa)")));
        TypeAssert.VerifyInstanceOf<StringObject>(nestedDictionary.Value["Pasdfghj"], _ => Assert.That(_.Value, Is.EqualTo("<234123>")));
      });
    });
  }

  [Test]
  public async Task Read_NestedDictionary_WithoutSpace()
  {
    using var stream = new MemoryStream(
      "<</TestKey<</Beep/Boop/Type/Asdf /Asdf (aaaaa) /Pasdfghj <234123>>>>>"u8.ToArray());

    var result = await _dictionaryObjectReader.Read(stream, _objectRepository.Object);

    Assert.Multiple(() =>
    {
      TypeAssert.VerifyInstanceOf<DictionaryObject>(result.Value["TestKey"], nestedDictionary =>
      {
        TypeAssert.VerifyInstanceOf<NameObject>(nestedDictionary.Value["Beep"], _ => Assert.That(_.Value, Is.EqualTo("Boop")));
        TypeAssert.VerifyInstanceOf<NameObject>(nestedDictionary.Value["Type"], _ => Assert.That(_.Value, Is.EqualTo("Asdf")));
        TypeAssert.VerifyInstanceOf<StringObject>(nestedDictionary.Value["Asdf"], _ => Assert.That(_.Value, Is.EqualTo("(aaaaa)")));
        TypeAssert.VerifyInstanceOf<StringObject>(nestedDictionary.Value["Pasdfghj"], _ => Assert.That(_.Value, Is.EqualTo("<234123>")));
      });
    });
  }

  [Test]
  public async Task Read_NumberObject()
  {
    using var stream = new MemoryStream(
      "<</TestKey 4637>>"u8.ToArray());

    var result = await _dictionaryObjectReader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<IntegerObject>(result.Value["TestKey"], _ => Assert.That(_.Value, Is.EqualTo(4637)));
  }

  [Test]
  public async Task Read_MultipleWhiteSpaces()
  {
    using var stream = new MemoryStream(
      "<<  \r \n   /TestKey \r\n\r\t     4637   \t \t\r\r\r\n>>"u8.ToArray());

    var result = await _dictionaryObjectReader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<IntegerObject>(result.Value["TestKey"], _ => Assert.That(_.Value, Is.EqualTo(4637)));
  }

  [Test]
  public async Task Read_IndirectObject_CallsObjectRepository()
  {
    var arrayObject = "<< /IndirectObject 1 0 R >>"u8.ToArray();
    using var stream = new MemoryStream(arrayObject);

    var indirectObjectValue = new IntegerObject(1234);
    _objectRepository.Setup(_ => _.RetrieveObject<DocumentObject>(It.IsAny<ObjectIdentifier>(), It.IsAny<Stream>()))
      .ReturnsAsync(indirectObjectValue)
      .Verifiable();

    var arrayObjectResult = await _dictionaryObjectReader.Read(stream, _objectRepository.Object);

    Assert.Multiple(() =>
    {
      Assert.That(arrayObjectResult.Value, Has.Count.EqualTo(1));

      TypeAssert.VerifyInstanceOf<IndirectObject>(arrayObjectResult.Value["IndirectObject"], _ =>
      {
        Assert.That(_.ObjectIdentifier, Is.EqualTo(new ObjectIdentifier(1, 0)));
        Assert.That(_.Value, Is.SameAs(indirectObjectValue));
      });
    });

    _objectRepository.Verify();
    _objectRepository.VerifyNoOtherCalls();
  }
}