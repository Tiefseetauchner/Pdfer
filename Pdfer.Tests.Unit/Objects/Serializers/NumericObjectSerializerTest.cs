using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Pdfer.Common.Tests;
using Pdfer.Objects.Serializers;

namespace Pdfer.Tests.Unit.Objects.Serializers;

public class NumericObjectSerializerTest
{
  private NumericObjectSerializer _serializer;

  [SetUp]
  public void SetUp()
  {
    _serializer = new NumericObjectSerializer();
  }

  [Test]
  public async Task Serialize_Integer()
  {
    using var memoryStream = new MemoryStream();
    var numericObject = ObjectBuilder.NumericObject(42);

    await _serializer.Serialize(memoryStream, numericObject);
    var result = Encoding.Default.GetString(memoryStream.ToArray());

    Assert.That(result, Is.EqualTo("42"));
  }

  [Test]
  public async Task Serialize_Float()
  {
    using var memoryStream = new MemoryStream();
    var numericObject = ObjectBuilder.NumericObject(42.69);

    await _serializer.Serialize(memoryStream, numericObject);
    var result = Encoding.Default.GetString(memoryStream.ToArray());

    Assert.That(result, Is.EqualTo("42.69"));
  }
}