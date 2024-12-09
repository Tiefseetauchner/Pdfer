using NUnit.Framework;
using Pdfer.Objects;

namespace Pdfer.Tests.Unit;

public class DocumentObjectSerializerRepositoryFactoryTest
{
  [Test]
  public void CreateForAllSerializers() =>
    Assert.That(DocumentObjectSerializerRepositoryFactory.CreateForAllSerializers(), Is.TypeOf<DocumentObjectSerializerRepository>());
}