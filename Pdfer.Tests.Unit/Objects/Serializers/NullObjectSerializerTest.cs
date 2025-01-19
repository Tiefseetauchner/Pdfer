using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Pdfer.Common.Tests;
using Pdfer.Objects.Serializers;

namespace Pdfer.Tests.Unit.Objects.Serializers;

public class NullObjectSerializerTest
{
  private NullObjectSerializer _serializer;

  [SetUp]
  public void SetUp()
  {
    _serializer = new NullObjectSerializer();
  }

  [Test]
  public async Task Serialize()
  {
    using var memoryStream = new MemoryStream();
    var dictionaryObject = ObjectBuilder.NullObject();

    await _serializer.Serialize(memoryStream, dictionaryObject);
    var result = Encoding.Default.GetString(memoryStream.ToArray());

    Assert.That(result, Is.EqualTo("null"));
  }
}