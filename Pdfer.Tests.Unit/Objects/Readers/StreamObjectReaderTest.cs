using System.IO;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Pdfer.Common.Tests;
using Pdfer.Objects;
using Pdfer.Objects.Readers;

namespace Pdfer.Tests.Unit.Objects.Readers;

public class StreamObjectReaderTest
{
  private DictionaryObjectReader _dictionaryObjectReader;

  private StreamObjectReader _reader;
  private Mock<IObjectRepository> _objectRepository;

  [SetUp]
  public void Setup()
  {
    var pdfDictionaryHelper = new PdfDictionaryHelper(PdfObjectReaderFactory.Create());
    _dictionaryObjectReader = new DictionaryObjectReader(pdfDictionaryHelper);
    _objectRepository = new Mock<IObjectRepository>();

    _reader = new StreamObjectReader(_dictionaryObjectReader);
  }

  [Test]
  public async Task Read()
  {
    using var stream = new MemoryStream(@"<</Filter/FlateDecode/Length 32>>stream
1234567890ABCDEF1234567890ABCDEF
endstream"u8.ToArray());
    var result = await _reader.Read(stream, _objectRepository.Object);

    Assert.Multiple(() =>
    {
      TypeAssert.VerifyInstanceOf<IntegerObject>(result.Dictionary.Value["Length"], length =>
        Assert.That(length.Value, Is.EqualTo(32)));
      TypeAssert.VerifyInstanceOf<NameObject>(result.Dictionary.Value["Filter"], length =>
        Assert.That(length.Value, Is.EqualTo("FlateDecode")));
      Assert.That(result.Value, Is.EqualTo("1234567890ABCDEF1234567890ABCDEF"u8.ToArray()));
    });
  }

  [Test]
  public async Task Read_WithReference()
  {
    _objectRepository.Setup(_ => _.RetrieveObject<DocumentObject>(new ObjectIdentifier(1, 0).MatchesEqual(), It.IsAny<Stream>()))
      .ReturnsAsync(new IntegerObject(32));

    using var stream = new MemoryStream(@"<</Filter/FlateDecode/Length 1 0 R>>stream
1234567890ABCDEF1234567890ABCDEF
endstream"u8.ToArray());
    var result = await _reader.Read(stream, _objectRepository.Object);

    Assert.Multiple(() =>
    {
      TypeAssert.VerifyInstanceOf<ReferenceObject>(result.Dictionary.Value["Length"], lengthReference =>
      {
        Assert.That(lengthReference.ObjectIdentifier.ObjectNumber, Is.EqualTo(1));
        Assert.That(lengthReference.ObjectIdentifier.Generation, Is.EqualTo(0));
        Assert.That(lengthReference.Value, Is.Not.Null);
        TypeAssert.VerifyInstanceOf<IntegerObject>(lengthReference.Value!, length =>
          Assert.That(length.Value, Is.EqualTo(32)));
      });
      TypeAssert.VerifyInstanceOf<NameObject>(result.Dictionary.Value["Filter"], length =>
        Assert.That(length.Value, Is.EqualTo("FlateDecode")));
      Assert.That(result.Value, Is.EqualTo("1234567890ABCDEF1234567890ABCDEF"u8.ToArray()));
    });
  }

  [Test]
  public void Read_LengthNotInteger()
  {
    using var stream = new MemoryStream(@"<</Filter/FlateDecode/Length/32>>stream
1234567890ABCDEF1234567890ABCDEF
endstream"u8.ToArray());

    Assert.ThrowsAsync<PdfInvalidIndirectObjectReferenceParsingException>(() => _reader.Read(stream, _objectRepository.Object));
  }

  [Test]
  public void Read_WithReference_LengthNotInteger()
  {
    _objectRepository.Setup(_ => _.RetrieveObject<DocumentObject>(new ObjectIdentifier(1, 0).MatchesEqual(), It.IsAny<Stream>()))
      .ReturnsAsync(new NameObject("32"));

    using var stream = new MemoryStream(@"<</Filter/FlateDecode/Length 1 0 R>>stream
1234567890ABCDEF1234567890ABCDEF
endstream"u8.ToArray());

    Assert.ThrowsAsync<PdfInvalidIndirectObjectReferenceParsingException>(() => _reader.Read(stream, _objectRepository.Object));
  }
}