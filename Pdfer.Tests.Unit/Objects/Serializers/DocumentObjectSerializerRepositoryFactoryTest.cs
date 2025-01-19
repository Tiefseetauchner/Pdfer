using NUnit.Framework;
using Pdfer.Common.Tests;
using Pdfer.Objects.Serializers;

namespace Pdfer.Tests.Unit.Objects.Serializers;

public class DocumentObjectSerializerRepositoryFactoryTest
{
  [Test]
  public void CreateForAllSerializers() =>
    TypeAssert.VerifyInstanceOf<DocumentObjectSerializerRepository>(DocumentObjectSerializerRepositoryFactory.CreateForAllSerializers(), repository =>
    {
      TypeAssert.VerifyInstanceOf<ArrayObjectSerializer>(repository.GetSerializer(ObjectBuilder.ArrayObject()));
      TypeAssert.VerifyInstanceOf<BooleanObjectSerializer>(repository.GetSerializer(ObjectBuilder.BooleanObject(true)));
      TypeAssert.VerifyInstanceOf<DictionaryObjectSerializer>(repository.GetSerializer(ObjectBuilder.DictionaryObject()));
      TypeAssert.VerifyInstanceOf<ReferenceObjectSerializer>(repository.GetSerializer(ObjectBuilder.ReferenceObject()));
      TypeAssert.VerifyInstanceOf<NameObjectSerializer>(repository.GetSerializer(ObjectBuilder.NameObject("Test")));
      TypeAssert.VerifyInstanceOf<NullObjectSerializer>(repository.GetSerializer(ObjectBuilder.NullObject()));
      TypeAssert.VerifyInstanceOf<NumericObjectSerializer>(repository.GetSerializer(ObjectBuilder.NumericObject(1)));
      TypeAssert.VerifyInstanceOf<NumericObjectSerializer>(repository.GetSerializer(ObjectBuilder.NumericObject(1.25)));
      TypeAssert.VerifyInstanceOf<StreamObjectSerializer>(repository.GetSerializer(ObjectBuilder.StreamObject()));
      TypeAssert.VerifyInstanceOf<StringObjectSerializer>(repository.GetSerializer(ObjectBuilder.StringObject("Test")));
    });
}