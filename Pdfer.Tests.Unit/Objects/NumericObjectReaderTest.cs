using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Pdfer.Objects;

namespace Pdfer.Tests.Unit.Objects;

public class NumericObjectReaderTest
{
  private NumericObjectReader _objectReader;

  [SetUp]
  public void SetUp()
  {
    _objectReader = new NumericObjectReader(new StreamHelper());
  }

  [Test]
  public async Task Read_ShouldReturnIntegerObject()
  {
    using var stream = new MemoryStream(
      """
        123
        endobj
        """u8.ToArray());

    var result = await _objectReader.Read(stream, null!, new ObjectIdentifier(1, 0));

    Assert.Multiple(() =>
    {
      Assert.That(result, Is.TypeOf<IntegerObject>());
      Assert.That(((IntegerObject)result).Value, Is.EqualTo(123));
    });
  }

  [Test]
  public async Task Read_ShouldReturnFloatObject()
  {
    using var stream = new MemoryStream(
      """
        123.234
        endobj
        """u8.ToArray());

    var result = await _objectReader.Read(stream, null!, new ObjectIdentifier(1, 0));

    Assert.Multiple(() =>
    {
      Assert.That(result, Is.TypeOf<FloatObject>());
      Assert.That(((FloatObject)result).Value, Is.EqualTo(123.234).Within(0.001));
    });
  }
}