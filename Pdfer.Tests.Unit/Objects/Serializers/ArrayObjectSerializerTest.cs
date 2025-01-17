using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Pdfer.Common.Tests;
using Pdfer.Objects.Serializers;

namespace Pdfer.Tests.Unit.Objects.Serializers;

public class ArrayObjectSerializerTest
{
  private ArrayObjectSerializer _serializer;

  [SetUp]
  public void SetUp()
  {
    _serializer = new ArrayObjectSerializer(DocumentObjectSerializerRepositoryFactory.CreateForAllSerializers());
  }

  [Test]
  public async Task Serialize_EmptyArray()
  {
    using var memoryStream = new MemoryStream();
    var arrayObject = ObjectBuilder.ArrayObject();

    await _serializer.Serialize(memoryStream, arrayObject);
    var result = Encoding.Default.GetString(memoryStream.ToArray());

    Assert.That(result, Is.EqualTo("[ ]"));
  }

  [Test]
  public async Task Serialize_VariousObjects()
  {
    using var memoryStream = new MemoryStream();
    var arrayObject = ObjectBuilder.ArrayObject().With(
      ObjectBuilder.NameObject("NameObject"),
      ObjectBuilder.LiteralStringObject("String Object"),
      ObjectBuilder.NumericObject(42));

    await _serializer.Serialize(memoryStream, arrayObject);
    var result = Encoding.Default.GetString(memoryStream.ToArray());

    Assert.That(result, Is.EqualTo("[ /NameObject (String Object) 42 ]"));
  }

  [Test]
  public async Task Serialize_NestedArray()
  {
    using var memoryStream = new MemoryStream();
    var arrayObject = ObjectBuilder.ArrayObject().With(
      ObjectBuilder.NameObject("FirstValue"),
      ObjectBuilder.ArrayObject().With(ObjectBuilder.NameObject("NestedValue")),
      ObjectBuilder.NameObject("LastValue"));

    await _serializer.Serialize(memoryStream, arrayObject);
    var result = Encoding.Default.GetString(memoryStream.ToArray());

    Assert.That(result, Is.EqualTo("[ /FirstValue [ /NestedValue ] /LastValue ]"));
  }
}