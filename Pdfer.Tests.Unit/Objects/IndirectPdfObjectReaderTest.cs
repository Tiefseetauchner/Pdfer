using System.IO;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Pdfer.Common.Tests;
using Pdfer.Objects;
using Pdfer.Objects.Readers;

namespace Pdfer.Tests.Unit.Objects;

public class IndirectPdfObjectReaderTest
{
  [Test]
  public async Task Read_ArrayObject()
  {
    using var stream = new MemoryStream(@"1 0 obj
[ (String Object) 123 /NameObject ]
endobj"u8.ToArray());
    var objectRepository = new Mock<IObjectRepository>();
    var reader = new IndirectPdfObjectReader(PdfObjectReaderFactory.Create());

    var result = await reader.Read(stream, new XRefEntry(0, XRefEntryType.Used), objectRepository.Object);

    TypeAssert.VerifyInstanceOf<ArrayObject>(result, arrayObject =>
    {
      var i = 0;
      TypeAssert.VerifyInstanceOf<StringObject>(arrayObject.Value[i++], _ => Assert.That(_.Value, Is.EqualTo("(String Object)")));
      TypeAssert.VerifyInstanceOf<IntegerObject>(arrayObject.Value[i++], _ => Assert.That(_.Value, Is.EqualTo(123)));
      TypeAssert.VerifyInstanceOf<NameObject>(arrayObject.Value[i++], _ => Assert.That(_.Value, Is.EqualTo("NameObject")));
      Assert.That(arrayObject.Value, Has.Length.EqualTo(i));
    });
  }

  [Test]
  public async Task Read_BooleanObject()
  {
    using var stream = new MemoryStream(@"1 0 obj
true
endobj"u8.ToArray());
    var objectRepository = new Mock<IObjectRepository>();
    var reader = new IndirectPdfObjectReader(PdfObjectReaderFactory.Create());

    var result = await reader.Read(stream, new XRefEntry(0, XRefEntryType.Used), objectRepository.Object);

    TypeAssert.VerifyInstanceOf<BooleanObject>(result, stringObject =>
      Assert.That(stringObject.Value, Is.EqualTo(true)));
  }

  [Test]
  public async Task Read_DictionaryObject()
  {
    using var stream = new MemoryStream(@"1 0 obj
<< /Item1/Value1/Item2 true /Item3 (String Object) >>
endobj"u8.ToArray());
    var objectRepository = new Mock<IObjectRepository>();
    var reader = new IndirectPdfObjectReader(PdfObjectReaderFactory.Create());

    var result = await reader.Read(stream, new XRefEntry(0, XRefEntryType.Used), objectRepository.Object);

    TypeAssert.VerifyInstanceOf<DictionaryObject>(result, dictionaryObject =>
    {
      TypeAssert.VerifyInstanceOf<NameObject>(dictionaryObject.Value["Item1"], _ => Assert.That(_.Value, Is.EqualTo("Value1")));
      TypeAssert.VerifyInstanceOf<BooleanObject>(dictionaryObject.Value["Item2"], _ => Assert.That(_.Value, Is.EqualTo(true)));
      TypeAssert.VerifyInstanceOf<StringObject>(dictionaryObject.Value["Item3"], _ => Assert.That(_.Value, Is.EqualTo("(String Object)")));
    });
  }

  [Test]
  public async Task Read_NameObject()
  {
    using var stream = new MemoryStream(@"1 0 obj
/NameObject
endobj"u8.ToArray());
    var objectRepository = new Mock<IObjectRepository>();
    var reader = new IndirectPdfObjectReader(PdfObjectReaderFactory.Create());

    var result = await reader.Read(stream, new XRefEntry(0, XRefEntryType.Used), objectRepository.Object);

    TypeAssert.VerifyInstanceOf<NameObject>(result, stringObject =>
      Assert.That(stringObject.Value, Is.EqualTo("NameObject")));
  }

  [Test]
  public async Task Read_NullObject()
  {
    using var stream = new MemoryStream(@"1 0 obj
null
endobj"u8.ToArray());
    var objectRepository = new Mock<IObjectRepository>();
    var reader = new IndirectPdfObjectReader(PdfObjectReaderFactory.Create());

    var result = await reader.Read(stream, new XRefEntry(0, XRefEntryType.Used), objectRepository.Object);

    TypeAssert.VerifyInstanceOf<NullObject>(result);
  }

  [Test]
  public async Task Read_IntegerObject()
  {
    using var stream = new MemoryStream(@"1 0 obj
123456789
endobj"u8.ToArray());
    var objectRepository = new Mock<IObjectRepository>();
    var reader = new IndirectPdfObjectReader(PdfObjectReaderFactory.Create());

    var result = await reader.Read(stream, new XRefEntry(0, XRefEntryType.Used), objectRepository.Object);

    TypeAssert.VerifyInstanceOf<IntegerObject>(result, integerObject =>
      Assert.That(integerObject.Value, Is.EqualTo(123456789)));
  }

  [Test]
  public async Task Read_FloatObject()
  {
    using var stream = new MemoryStream(@"1 0 obj
123.456
endobj"u8.ToArray());
    var objectRepository = new Mock<IObjectRepository>();
    var reader = new IndirectPdfObjectReader(PdfObjectReaderFactory.Create());

    var result = await reader.Read(stream, new XRefEntry(0, XRefEntryType.Used), objectRepository.Object);

    TypeAssert.VerifyInstanceOf<FloatObject>(result, floatObject =>
      Assert.That(floatObject.Value, Is.EqualTo(123.456)));
  }

  [Test]
  public async Task Read_ReferenceObject()
  {
    using var stream = new MemoryStream(@"1 0 obj
236 123 R
endobj"u8.ToArray());
    var objectRepository = new Mock<IObjectRepository>();
    var reader = new IndirectPdfObjectReader(PdfObjectReaderFactory.Create());

    var result = await reader.Read(stream, new XRefEntry(0, XRefEntryType.Used), objectRepository.Object);

    TypeAssert.VerifyInstanceOf<ReferenceObject>(result, referenceObject =>
    {
      Assert.Multiple(() =>
      {
        Assert.That(referenceObject.Value, Is.EqualTo(null));
        Assert.That(referenceObject.ObjectIdentifier, Is.EqualTo(new ObjectIdentifier(236, 123)));
      });
    });
  }

  [Test]
  public async Task Read_StreamObject()
  {
    using var stream = new MemoryStream(@"1 0 obj
<</Filter/FlateDecode/Length 16>>stream
0123456789ABCDEF
endstream
endobj"u8.ToArray());
    var objectRepository = new Mock<IObjectRepository>();
    var reader = new IndirectPdfObjectReader(PdfObjectReaderFactory.Create());

    var result = await reader.Read(stream, new XRefEntry(0, XRefEntryType.Used), objectRepository.Object);

    TypeAssert.VerifyInstanceOf<StreamObject>(result, streamObject =>
    {
      Assert.Multiple(() =>
      {
        TypeAssert.VerifyInstanceOf<NameObject>(streamObject.Dictionary.Value["Filter"], _ => Assert.That(_.Value, Is.EqualTo("FlateDecode")));
        TypeAssert.VerifyInstanceOf<IntegerObject>(streamObject.Dictionary.Value["Length"], _ => Assert.That(_.Value, Is.EqualTo(16)));
        Assert.That(streamObject.Value, Is.EqualTo("0123456789ABCDEF"u8.ToArray()));
      });
    });
  }

  [Test]
  public async Task Read_StringObject_LiteralString()
  {
    using var stream = new MemoryStream(@"1 0 obj
(My String\( Obj\)ect!!)
endobj"u8.ToArray());
    var objectRepository = new Mock<IObjectRepository>();
    var reader = new IndirectPdfObjectReader(PdfObjectReaderFactory.Create());

    var result = await reader.Read(stream, new XRefEntry(0, XRefEntryType.Used), objectRepository.Object);

    TypeAssert.VerifyInstanceOf<StringObject>(result, stringObject =>
      Assert.That(stringObject.Value, Is.EqualTo(@"(My String\( Obj\)ect!!)")));
  }

  [Test]
  public async Task Read_StringObject_HexString()
  {
    using var stream = new MemoryStream(@"1 0 obj
<AFFE1234CAFFEE>
endobj"u8.ToArray());
    var objectRepository = new Mock<IObjectRepository>();
    var reader = new IndirectPdfObjectReader(PdfObjectReaderFactory.Create());

    var result = await reader.Read(stream, new XRefEntry(0, XRefEntryType.Used), objectRepository.Object);

    TypeAssert.VerifyInstanceOf<StringObject>(result, stringObject =>
      Assert.That(stringObject.Value, Is.EqualTo(@"<AFFE1234CAFFEE>")));
  }

  [Test]
  public void Read_DoesNotBeginWithObject()
  {
    using var stream = new MemoryStream(@"1 0 2 obj
endobj"u8.ToArray());
    var objectRepository = new Mock<IObjectRepository>();
    var reader = new IndirectPdfObjectReader(PdfObjectReaderFactory.Create());

    var exception = Assert.ThrowsAsync<PdfInvalidIndirectObjectReferenceParsingException>(() => reader.Read(stream, new XRefEntry(0, XRefEntryType.Used), objectRepository.Object));

    Assert.That(exception.Message, Is.EqualTo("Indirect object did not start with an object identifier."));
  }
}