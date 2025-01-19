using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Pdfer.Common.Tests;
using Pdfer.Objects.Serializers;

namespace Pdfer.Tests.Unit.Objects.Serializers;

public class BooleanObjectSerializerTest
{
  private BooleanObjectSerializer _serializer;

  [SetUp]
  public void SetUp()
  {
    _serializer = new BooleanObjectSerializer();
  }

  [Test]
  public async Task Serialize_True()
  {
    using var memoryStream = new MemoryStream();
    var booleanObject = ObjectBuilder.BooleanObject(true);

    await _serializer.Serialize(memoryStream, booleanObject);
    var result = Encoding.Default.GetString(memoryStream.ToArray());

    Assert.That(result, Is.EqualTo("true"));
  }

  [Test]
  public async Task Serialize_False()
  {
    using var memoryStream = new MemoryStream();
    var booleanObject = ObjectBuilder.BooleanObject(false);

    await _serializer.Serialize(memoryStream, booleanObject);
    var result = Encoding.Default.GetString(memoryStream.ToArray());

    Assert.That(result, Is.EqualTo("false"));
  }
}