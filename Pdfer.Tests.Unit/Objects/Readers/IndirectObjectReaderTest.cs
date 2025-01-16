using Moq;
using NUnit.Framework;
using Pdfer.Objects;
using Pdfer.Objects.ObjectReaders;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Tests.Unit.Objects.Readers;

[TestFixture]
public class IndirectObjectReaderTests
{
  private Mock<IObjectRepository> _objectRepository;
  private IndirectObjectReader _indirectObjectReader;

  [SetUp]
  public void Setup()
  {
    _objectRepository = new Mock<IObjectRepository>();
    _indirectObjectReader = new IndirectObjectReader();
  }

  [Test]
  public async Task Read_ValidIndirectObject_ReturnsExpectedResult()
  {
    using var stream = new MemoryStream("1 0 R"u8.ToArray());
    var mockObject = new IntegerObject(1234);

    _objectRepository
      .Setup(repo => repo.RetrieveObject<DocumentObject>(It.IsAny<ObjectIdentifier>(), It.IsAny<Stream>()))
      .ReturnsAsync(mockObject);

    var result = await _indirectObjectReader.Read(stream, _objectRepository.Object);

    Assert.Multiple(() =>
    {
      Assert.That(result.ObjectIdentifier, Is.EqualTo(new ObjectIdentifier(1, 0)));
      Assert.That(result.Value, Is.SameAs(mockObject));
    });
  }

  [Test]
  public void Read_InvalidSuffix_ThrowsException()
  {
    using var stream = new MemoryStream("1 0 X"u8.ToArray());

    Assert.ThrowsAsync<PdfInvalidIndirectObjectReferenceParsingException>(
      () => _indirectObjectReader.Read(stream, _objectRepository.Object));
  }

  [Test]
  public void Read_IncompleteObjectReference_ThrowsException()
  {
    using var stream = new MemoryStream("1 0"u8.ToArray());

    Assert.ThrowsAsync<PdfInvalidIndirectObjectReferenceParsingException>(
      () => _indirectObjectReader.Read(stream, _objectRepository.Object));
  }

  [Test]
  public void Read_NonNumericObjectNumber_ThrowsException()
  {
    using var stream = new MemoryStream("a 0 R"u8.ToArray());

    Assert.ThrowsAsync<PdfInvalidIndirectObjectReferenceParsingException>(
      () => _indirectObjectReader.Read(stream, _objectRepository.Object));
  }

  [Test]
  public void Read_NonNumericGenerationNumber_ThrowsException()
  {
    using var stream = new MemoryStream("1 a R"u8.ToArray());

    Assert.ThrowsAsync<PdfInvalidIndirectObjectReferenceParsingException>(
      () => _indirectObjectReader.Read(stream, _objectRepository.Object));
  }
}