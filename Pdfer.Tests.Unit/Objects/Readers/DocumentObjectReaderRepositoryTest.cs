using System;
using Moq;
using NUnit.Framework;
using Pdfer.Objects;
using Pdfer.Objects.Readers;

namespace Pdfer.Tests.Unit.Objects.Readers;

public class DocumentObjectReaderRepositoryTest
{
  private DocumentObjectReaderRepository _repository;

  [SetUp]
  public void Setup()
  {
    _repository = new DocumentObjectReaderRepository();
  }

  [Test]
  public void GetReader_ValidType_ReturnsReader()
  {
    var reader = new Mock<IDocumentObjectReader<IntegerObject>>();
    _repository.AddReader(reader.Object);

    var result = _repository.GetReader<IntegerObject>();

    Assert.That(result, Is.SameAs(reader.Object));
  }

  [Test]
  public void GetReader_MultipleReaders_ReturnsReader()
  {
    var integerReader = new Mock<IDocumentObjectReader<IntegerObject>>();
    var nameReader = new Mock<IDocumentObjectReader<NameObject>>();
    _repository.AddReader(integerReader.Object);
    _repository.AddReader(nameReader.Object);

    var integerResult = _repository.GetReader<IntegerObject>();
    var nameResult = _repository.GetReader<NameObject>();

    Assert.Multiple(() =>
    {
      Assert.That(integerResult, Is.SameAs(integerReader.Object));
      Assert.That(nameResult, Is.SameAs(nameReader.Object));
    });
  }

  [Test]
  public void GetReader_NotRegistered_Throws()
  {
    var exception = Assert.Throws<ArgumentException>(() => _repository.GetReader<IntegerObject>());

    Assert.That(exception.Message, Is.EqualTo($"No reader found for type {typeof(IntegerObject)}"));
  }
}