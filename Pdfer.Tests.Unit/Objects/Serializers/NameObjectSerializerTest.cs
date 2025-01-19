using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Pdfer.Common.Tests;
using Pdfer.Objects.Serializers;

namespace Pdfer.Tests.Unit.Objects.Serializers;

public class NameObjectSerializerTest
{
  private NameObjectSerializer _serializer;

  [SetUp]
  public void SetUp()
  {
    _serializer = new NameObjectSerializer();
  }

  [Test]
  public async Task Serialize()
  {
    using var memoryStream = new MemoryStream();
    var nameObject = ObjectBuilder.NameObject("NameObject");

    await _serializer.Serialize(memoryStream, nameObject);
    var result = Encoding.Default.GetString(memoryStream.ToArray());

    Assert.That(result, Is.EqualTo("/NameObject"));
  }

  [Test]
  public async Task Serialize_SpecialCharacters()
  {
    using var memoryStream = new MemoryStream();
    var nameObject = ObjectBuilder.NameObject("Name***Object_With-Special;Characters");

    await _serializer.Serialize(memoryStream, nameObject);
    var result = Encoding.Default.GetString(memoryStream.ToArray());

    Assert.That(result, Is.EqualTo("/Name***Object_With-Special;Characters"));
  }

  [Test]
  public async Task Serialize_UnicodeCharacters()
  {
    using var memoryStream = new MemoryStream();
    var nameObject = ObjectBuilder.NameObject("NämÖbθεkt");

    await _serializer.Serialize(memoryStream, nameObject);
    var result = Encoding.Default.GetString(memoryStream.ToArray());

    Assert.That(result, Is.EqualTo("/NämÖbθεkt"));
  }
}