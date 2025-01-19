using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Pdfer.Objects;
using Pdfer.Objects.Readers;

namespace Pdfer.Tests.Unit.Objects.Readers;

public class NumericObjectReaderTest
{
  private NumericObjectReader _objectReader;

  [SetUp]
  public void SetUp()
  {
    _objectReader = new NumericObjectReader();
  }

  [Test]
  public async Task Read_ShouldReturnIntegerObject()
  {
    using var stream = new MemoryStream(
      "123"u8.ToArray());

    var result = await _objectReader.Read(stream, null!);

    Assert.Multiple(() =>
    {
      Assert.That(result, Is.TypeOf<IntegerObject>());
      Assert.That(((IntegerObject)result).Value, Is.EqualTo(123));
    });
  }

  [Test]
  public async Task Read_Negative_ShouldReturnIntegerObject()
  {
    using var stream = new MemoryStream(
      "-123"u8.ToArray());

    var result = await _objectReader.Read(stream, null!);

    Assert.Multiple(() =>
    {
      Assert.That(result, Is.TypeOf<IntegerObject>());
      Assert.That(((IntegerObject)result).Value, Is.EqualTo(-123));
    });
  }

  [Test]
  public async Task Read_ShouldReturnFloatObject()
  {
    using var stream = new MemoryStream(
      "123.234"u8.ToArray());

    var result = await _objectReader.Read(stream, null!);

    Assert.Multiple(() =>
    {
      Assert.That(result, Is.TypeOf<FloatObject>());
      Assert.That(((FloatObject)result).Value, Is.EqualTo(123.234).Within(0.001));
    });
  }

  [Test]
  public async Task Read_Negative_ShouldReturnFloatObject()
  {
    using var stream = new MemoryStream(
      "-123.234"u8.ToArray());

    var result = await _objectReader.Read(stream, null!);

    Assert.Multiple(() =>
    {
      Assert.That(result, Is.TypeOf<FloatObject>());
      Assert.That(((FloatObject)result).Value, Is.EqualTo(-123.234).Within(0.001));
    });
  }

  [Test]
  public void Read_TwoDecimalPlaces_Throws()
  {
    using var stream = new MemoryStream(
      "12.3.234"u8.ToArray());

    var exception = Assert.ThrowsAsync<PdfInvalidNumberParsingException>(() => _objectReader.Read(stream, null!));

    Assert.That(exception.Message, Is.EqualTo("Number contains multiple decimal points."));
  }
}