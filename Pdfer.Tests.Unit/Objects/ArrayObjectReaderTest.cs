using System.IO;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Pdfer.Objects;

namespace Pdfer.Tests.Unit.Objects;

public class ArrayObjectReaderTest
{
  private ArrayObjectReader _arrayObjectReader;
  private Mock<IObjectRepository> _objectRepository;


  [SetUp]
  public void Setup()
  {
    _objectRepository = new Mock<IObjectRepository>();
    var pdfObjectReader = PdfObjectReaderFactory.Create();

    _arrayObjectReader = new ArrayObjectReader(
      new StreamHelper(),
      pdfObjectReader);
  }

  [Test]
  public async Task Read()
  {
    var arrayObject = "[ 100 3 0 R [ (Array in Array) <1234> ] (Asdf) ]"u8.ToArray();

    using var stream = new MemoryStream(arrayObject);

    var indirectObjectValue = new IntegerObject(1234);
    _objectRepository.Setup(x => x.RetrieveObject<DocumentObject>(It.IsAny<ObjectIdentifier>(), It.IsAny<Stream>()))
      .ReturnsAsync(indirectObjectValue)
      .Verifiable();

    var arrayObjectResult = await _arrayObjectReader.Read(stream, _objectRepository.Object);

    var i = 0;
    Assert.Multiple(() =>
    {
      TypeAssert.VerifyInstanceOf<IntegerObject>(arrayObjectResult.Value[i++], _ => Assert.That(_.Value, Is.EqualTo(100)));

      TypeAssert.VerifyInstanceOf<IndirectObject>(arrayObjectResult.Value[i++], _ =>
      {
        Assert.That(_.ObjectIdentifier, Is.EqualTo(new ObjectIdentifier(3, 0)));
        Assert.That(_.Value, Is.SameAs(indirectObjectValue));
      });

      TypeAssert.VerifyInstanceOf<ArrayObject>(arrayObjectResult.Value[i++], _ =>
      {
        var j = 0;
        TypeAssert.VerifyInstanceOf<StringObject>(_.Value[j++], stringObject => Assert.That(stringObject.Value, Is.EqualTo("(Array in Array)")));
        TypeAssert.VerifyInstanceOf<StringObject>(_.Value[j++], stringObject => Assert.That(stringObject.Value, Is.EqualTo("<1234>")));
        Assert.That(_.Value, Has.Length.EqualTo(j));
      });

      TypeAssert.VerifyInstanceOf<StringObject>(arrayObjectResult.Value[i++], stringObject => Assert.That(stringObject.Value, Is.EqualTo("(Asdf)")));
      Assert.That(arrayObjectResult.Value, Has.Length.EqualTo(i));
    });

    _objectRepository.Verify();
    _objectRepository.VerifyNoOtherCalls();
  }

  [Test]
  public async Task Read_EmptyArray()
  {
    var arrayObject = "[ ]"u8.ToArray();
    using var stream = new MemoryStream(arrayObject);

    var arrayObjectResult = await _arrayObjectReader.Read(stream, _objectRepository.Object);

    Assert.That(arrayObjectResult.Value, Is.Empty);
  }

  [Test]
  public async Task Read_IndirectObject_CallsObjectRepository()
  {
    var arrayObject = "[ 1 0 R ]"u8.ToArray();
    using var stream = new MemoryStream(arrayObject);

    var indirectObjectValue = new IntegerObject(1234);
    _objectRepository.Setup(_ => _.RetrieveObject<DocumentObject>(It.IsAny<ObjectIdentifier>(), It.IsAny<Stream>()))
      .ReturnsAsync(indirectObjectValue)
      .Verifiable();

    var arrayObjectResult = await _arrayObjectReader.Read(stream, _objectRepository.Object);

    Assert.Multiple(() =>
    {
      Assert.That(arrayObjectResult.Value, Has.Length.EqualTo(1));

      TypeAssert.VerifyInstanceOf<IndirectObject>(arrayObjectResult.Value[0], _ =>
      {
        Assert.That(_.ObjectIdentifier, Is.EqualTo(new ObjectIdentifier(1, 0)));
        Assert.That(_.Value, Is.SameAs(indirectObjectValue));
      });
    });

    _objectRepository.Verify();
    _objectRepository.VerifyNoOtherCalls();
  }

  [Test]
  public async Task Read_MultipleWhiteSpaces()
  {
    var arrayObject = "[ 100     200  \r\n\t\t\r\n      300 ]"u8.ToArray();
    using var stream = new MemoryStream(arrayObject);

    var arrayObjectResult = await _arrayObjectReader.Read(stream, _objectRepository.Object);

    Assert.Multiple(() =>
    {
      var i = 0;

      TypeAssert.VerifyInstanceOf<IntegerObject>(arrayObjectResult.Value[i++], _ => Assert.That(_.Value, Is.EqualTo(100)));
      TypeAssert.VerifyInstanceOf<IntegerObject>(arrayObjectResult.Value[i++], _ => Assert.That(_.Value, Is.EqualTo(200)));
      TypeAssert.VerifyInstanceOf<IntegerObject>(arrayObjectResult.Value[i++], _ => Assert.That(_.Value, Is.EqualTo(300)));

      Assert.That(arrayObjectResult.Value, Has.Length.EqualTo(i));
    });
  }
}