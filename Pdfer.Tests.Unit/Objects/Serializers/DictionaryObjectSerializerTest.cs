using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Pdfer.Common.Tests;
using Pdfer.Objects.Serializers;

namespace Pdfer.Tests.Unit.Objects.Serializers;

public class DictionaryObjectSerializerTest
{
  private DictionaryObjectSerializer _serializer;

  [SetUp]
  public void SetUp()
  {
    _serializer = new DictionaryObjectSerializer(DocumentObjectSerializerRepositoryFactory.CreateForAllSerializers());
  }

  [Test]
  public async Task Serialize_Empty()
  {
    using var memoryStream = new MemoryStream();
    var dictionaryObject = ObjectBuilder.DictionaryObject();

    await _serializer.Serialize(memoryStream, dictionaryObject);
    var result = Encoding.Default.GetString(memoryStream.ToArray());

    Assert.That(result, Is.EqualTo("<< >>"));
  }

  [Test]
  public async Task Serialize_VariousObjects()
  {
    using var memoryStream = new MemoryStream();
    var dictionaryObject = ObjectBuilder.DictionaryObject().With(
      (ObjectBuilder.NameObject("Object1"), ObjectBuilder.NameObject("Value1")),
      (ObjectBuilder.NameObject("Object2"), ObjectBuilder.LiteralStringObject("Value2")),
      (ObjectBuilder.NameObject("Object3"), ObjectBuilder.NumericObject(3)));

    await _serializer.Serialize(memoryStream, dictionaryObject);
    var result = Encoding.Default.GetString(memoryStream.ToArray());

    Assert.That(result, Is.EqualTo("<< /Object1 /Value1 /Object2 (Value2) /Object3 3 >>"));
  }

  [Test]
  public async Task Serialize_NestedDictionary()
  {
    using var memoryStream = new MemoryStream();
    var dictionaryObject = ObjectBuilder.DictionaryObject().With(
      (ObjectBuilder.NameObject("Object1"), ObjectBuilder.DictionaryObject()),
      (ObjectBuilder.NameObject("Object2"), ObjectBuilder.DictionaryObject().With(
        (ObjectBuilder.NameObject("Nested"), ObjectBuilder.LiteralStringObject("Value")))));

    await _serializer.Serialize(memoryStream, dictionaryObject);
    var result = Encoding.Default.GetString(memoryStream.ToArray());

    Assert.That(result, Is.EqualTo("<< /Object1 << >> /Object2 << /Nested (Value) >> >>"));
  }

  [Test]
  public async Task Serialize_Array()
  {
    using var memoryStream = new MemoryStream();
    var dictionaryObject = ObjectBuilder.DictionaryObject().With(
      (ObjectBuilder.NameObject("Object1"), ObjectBuilder.ArrayObject().With(
        ObjectBuilder.NameObject("Nested"), ObjectBuilder.LiteralStringObject("Value"))));

    await _serializer.Serialize(memoryStream, dictionaryObject);
    var result = Encoding.Default.GetString(memoryStream.ToArray());

    Assert.That(result, Is.EqualTo("<< /Object1 [ /Nested (Value) ] >>"));
  }
}