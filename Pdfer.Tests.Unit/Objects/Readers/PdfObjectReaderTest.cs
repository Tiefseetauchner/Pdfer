using System.IO;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Pdfer.Common.Tests;
using Pdfer.Objects;
using Pdfer.Objects.Readers;

namespace Pdfer.Tests.Unit.Objects.Readers;

public class PdfObjectReaderTest
{
  private Mock<IObjectRepository> _objectRepository;

  private PdfObjectReader _reader;

  [SetUp]
  public void SetUp()
  {
    _objectRepository = new Mock<IObjectRepository>();

    _reader = PdfObjectReaderFactory.Create();
  }

  [Test]
  public async Task Read_ArrayObject_DifferentObjects()
  {
    using var stream = new MemoryStream("[ /Value1 (Value2) 3 ]after"u8.ToArray());
    var result = await _reader.Read(stream, _objectRepository.Object);

    Assert.Multiple(() =>
    {
      TypeAssert.VerifyInstanceOf<ArrayObject>(result, array =>
      {
        var i = 0;
        TypeAssert.VerifyInstanceOf<NameObject>(array.Value[i++], name => Assert.That(name.Value, Is.EqualTo("Value1")));
        TypeAssert.VerifyInstanceOf<StringObject>(array.Value[i++], name => Assert.That(name.Value, Is.EqualTo("(Value2)")));
        TypeAssert.VerifyInstanceOf<IntegerObject>(array.Value[i++], name => Assert.That(name.Value, Is.EqualTo(3)));
        Assert.That(array.Value, Has.Length.EqualTo(i));
      });
    });
    VerifyStreamStoppedCorrectly(stream);
  }

  [Test]
  public async Task Read_ArrayObject_NoSpaces()
  {
    using var stream = new MemoryStream("[/Value1/Value2/Value3]after"u8.ToArray());
    var result = await _reader.Read(stream, _objectRepository.Object);

    Assert.Multiple(() =>
    {
      TypeAssert.VerifyInstanceOf<ArrayObject>(result, array =>
      {
        var i = 0;
        TypeAssert.VerifyInstanceOf<NameObject>(array.Value[i++], name => Assert.That(name.Value, Is.EqualTo("Value1")));
        TypeAssert.VerifyInstanceOf<NameObject>(array.Value[i++], name => Assert.That(name.Value, Is.EqualTo("Value2")));
        TypeAssert.VerifyInstanceOf<NameObject>(array.Value[i++], name => Assert.That(name.Value, Is.EqualTo("Value3")));
        Assert.That(array.Value, Has.Length.EqualTo(i));
      });
    });
    VerifyStreamStoppedCorrectly(stream);
  }

  [Test]
  public async Task Read_ArrayObject_EmptyArray_NoSpaces()
  {
    using var stream = new MemoryStream("[]after"u8.ToArray());
    var result = await _reader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<ArrayObject>(result, array => { Assert.That(array.Value, Is.Empty); });
    VerifyStreamStoppedCorrectly(stream);
  }

  [Test]
  public async Task Read_ArrayObject_EmptyArray_MultipleSpaces()
  {
    using var stream = new MemoryStream("[      ]after"u8.ToArray());
    var result = await _reader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<ArrayObject>(result, array => { Assert.That(array.Value, Is.Empty); });
    VerifyStreamStoppedCorrectly(stream);
  }

  [Test]
  public async Task Read_BooleanObject_True()
  {
    using var stream = new MemoryStream("trueafter"u8.ToArray());
    var result = await _reader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<BooleanObject>(result, boolean => { Assert.That(boolean.Value, Is.True); });
    VerifyStreamStoppedCorrectly(stream);
  }

  [Test]
  public async Task Read_BooleanObject_False()
  {
    using var stream = new MemoryStream("falseafter"u8.ToArray());
    var result = await _reader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<BooleanObject>(result, boolean => { Assert.That(boolean.Value, Is.False); });
    VerifyStreamStoppedCorrectly(stream);
  }

  [Test]
  public async Task Read_DictionaryObject_DifferentObjects()
  {
    using var stream = new MemoryStream("<< /Key1 (Value1) /Key2 3 >>after"u8.ToArray());
    var result = await _reader.Read(stream, _objectRepository.Object);

    Assert.Multiple(() =>
    {
      TypeAssert.VerifyInstanceOf<DictionaryObject>(result, dictionary =>
      {
        TypeAssert.VerifyInstanceOf<StringObject>(dictionary.Value["Key1"], name => Assert.That(name.Value, Is.EqualTo("(Value1)")));
        TypeAssert.VerifyInstanceOf<IntegerObject>(dictionary.Value["Key2"], name => Assert.That(name.Value, Is.EqualTo(3)));
      });
    });
    VerifyStreamStoppedCorrectly(stream);
  }

  [Test]
  public async Task Read_DictionaryObject_NoSpaces()
  {
    using var stream = new MemoryStream("<</Key1/Value1/Key2/Value2>>after"u8.ToArray());
    var result = await _reader.Read(stream, _objectRepository.Object);

    Assert.Multiple(() =>
    {
      TypeAssert.VerifyInstanceOf<DictionaryObject>(result, dictionary =>
      {
        TypeAssert.VerifyInstanceOf<NameObject>(dictionary.Value["Key1"], name => Assert.That(name.Value, Is.EqualTo("Value1")));
        TypeAssert.VerifyInstanceOf<NameObject>(dictionary.Value["Key2"], name => Assert.That(name.Value, Is.EqualTo("Value2")));
      });
    });
    VerifyStreamStoppedCorrectly(stream);
  }

  [Test]
  public async Task Read_DictionaryObject_Empty_NoSpaces()
  {
    using var stream = new MemoryStream("<<>>after"u8.ToArray());
    var result = await _reader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<DictionaryObject>(result, dictionary => { Assert.That(dictionary.Value, Is.Empty); });
    VerifyStreamStoppedCorrectly(stream);
  }

  [Test]
  public async Task Read_DictionaryObject_Empty_MultipleSpaces()
  {
    using var stream = new MemoryStream("<<     >>after"u8.ToArray());
    var result = await _reader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<DictionaryObject>(result, dictionary => { Assert.That(dictionary.Value, Is.Empty); });
    VerifyStreamStoppedCorrectly(stream);
  }

  [Test]
  public async Task Read_NameObject()
  {
    using var stream = new MemoryStream("/NameObject after"u8.ToArray());
    var result = await _reader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<NameObject>(result, name => { Assert.That(name.Value, Is.EqualTo("NameObject")); });
    VerifyStreamStoppedCorrectly(stream, " after"u8.ToArray());
  }

  [Test]
  public async Task Read_NullObject()
  {
    using var stream = new MemoryStream("nullafter"u8.ToArray());
    var result = await _reader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<NullObject>(result);
    VerifyStreamStoppedCorrectly(stream);
  }

  [Test]
  public async Task Read_NumericObject_Float()
  {
    using var stream = new MemoryStream("1.56after"u8.ToArray());
    var result = await _reader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<FloatObject>(result, _ => Assert.That(_.Value, Is.EqualTo(1.56)));
    VerifyStreamStoppedCorrectly(stream);
  }

  [Test]
  public async Task Read_NumericObject_Integer()
  {
    using var stream = new MemoryStream("1after"u8.ToArray());
    var result = await _reader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<IntegerObject>(result, _ => Assert.That(_.Value, Is.EqualTo(1)));
    VerifyStreamStoppedCorrectly(stream);
  }

  [Test]
  public async Task Read_NumericObject_LongInteger()
  {
    using var stream = new MemoryStream("123456789after"u8.ToArray());
    var result = await _reader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<IntegerObject>(result, _ => Assert.That(_.Value, Is.EqualTo(123456789)));
    VerifyStreamStoppedCorrectly(stream);
  }

  [Test]
  public async Task Read_ReferenceObject()
  {
    using var stream = new MemoryStream("17 0 Rafter"u8.ToArray());
    var referencedObject = new IntegerObject(16);
    _objectRepository.Setup(_ => _.RetrieveObject<DocumentObject>(new ObjectIdentifier(17, 0), stream)).ReturnsAsync(referencedObject);

    var result = await _reader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<ReferenceObject>(result, _ => Assert.That(_.Value, Is.SameAs(referencedObject)));
    VerifyStreamStoppedCorrectly(stream);
  }

  [Test]
  public async Task Read_ReferenceObject_ReturnsNull()
  {
    using var stream = new MemoryStream("17 0 Rafter"u8.ToArray());
    _objectRepository.Setup(_ => _.RetrieveObject<DocumentObject>(new ObjectIdentifier(17, 0), stream)).ReturnsAsync((DocumentObject?)null);

    var result = await _reader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<ReferenceObject>(result, _ => Assert.That(_.Value, Is.Null));
    VerifyStreamStoppedCorrectly(stream);
  }

  [Test]
  public async Task Read_StreamObject()
  {
    using var stream = new MemoryStream("""
                                        <</Length 1/Filter/FlateDecode>>
                                        stream
                                        1
                                        endstreamafter
                                        """u8.ToArray());
    var result = await _reader.Read(stream, _objectRepository.Object);

    Assert.Multiple(() =>
    {
      TypeAssert.VerifyInstanceOf<StreamObject>(result, _ =>
      {
        Assert.That(_.Value, Has.Length.EqualTo(1));
        Assert.That(_.Value[0], Is.EqualTo('1'));

        Assert.That(_.Dictionary.Value, Has.Count.EqualTo(2));
        TypeAssert.VerifyInstanceOf<IntegerObject>(_.Dictionary.Value["Length"], length => Assert.That(length.Value, Is.EqualTo(1)));
        TypeAssert.VerifyInstanceOf<NameObject>(_.Dictionary.Value["Filter"], filter => Assert.That(filter.Value, Is.EqualTo("FlateDecode")));
      });
      VerifyStreamStoppedCorrectly(stream);
    });
  }

  [Test]
  public async Task Read_StreamObject_LongerStream()
  {
    using var stream = new MemoryStream("""
                                        <</Length 16/Filter/FlateDecode>>
                                        stream
                                        0123456789012345
                                        endstreamafter
                                        """u8.ToArray());
    var result = await _reader.Read(stream, _objectRepository.Object);

    Assert.Multiple(() =>
    {
      TypeAssert.VerifyInstanceOf<StreamObject>(result, _ =>
      {
        Assert.That(_.Value, Is.EqualTo("0123456789012345"u8.ToArray()));

        Assert.That(_.Dictionary.Value, Has.Count.EqualTo(2));
        TypeAssert.VerifyInstanceOf<IntegerObject>(_.Dictionary.Value["Length"], length => Assert.That(length.Value, Is.EqualTo(16)));
        TypeAssert.VerifyInstanceOf<NameObject>(_.Dictionary.Value["Filter"], filter => Assert.That(filter.Value, Is.EqualTo("FlateDecode")));
      });
      VerifyStreamStoppedCorrectly(stream);
    });
  }

  [Test]
  public void Read_StreamObject_MismatchedLength_Throws()
  {
    using var stream = new MemoryStream("""
                                        <</Length 16/Filter/FlateDecode>>
                                        stream
                                        012345678901234endstreamafter
                                        """u8.ToArray());
    var exception = Assert.ThrowsAsync<PdfInvalidStreamEndParsingException>(() => _reader.Read(stream, _objectRepository.Object));

    Assert.That(exception.Message, Is.EqualTo("Expected 'endstream' but got 'ndstreama'"));
  }

  [Test]
  public async Task Read_StringObject_Literal()
  {
    using var stream = new MemoryStream("(A hell of a literal string!)after"u8.ToArray());
    var result = await _reader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<StringObject>(result, _ => Assert.That(_.Value, Is.EqualTo("(A hell of a literal string!)")));
    VerifyStreamStoppedCorrectly(stream);
  }

  [Test]
  public async Task Read_StringObject_Hexadecimal()
  {
    using var stream = new MemoryStream("<123456ABCD>after"u8.ToArray());
    var result = await _reader.Read(stream, _objectRepository.Object);

    TypeAssert.VerifyInstanceOf<StringObject>(result, _ => Assert.That(_.Value, Is.EqualTo("<123456ABCD>")));
    VerifyStreamStoppedCorrectly(stream);
  }

  private static void VerifyStreamStoppedCorrectly(Stream stream, byte[]? expected = null)
  {
    expected ??= "after"u8.ToArray();

    var buffer = new byte[expected.Length];
    var read = stream.Read(buffer);

    Assert.Multiple(() =>
    {
      Assert.That(read, Is.EqualTo(expected.Length));
      Assert.That(buffer, Is.EqualTo(expected));
    });
  }
}