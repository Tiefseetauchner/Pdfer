using NUnit.Framework;

namespace Pdfer.Tests.Unit;

public class PdfDocumentWriterFactoryTest
{
  [Test]
  public void Create() =>
    Assert.That(PdfDocumentWriterFactory.Create(), Is.TypeOf<PdfDocumentWriter>());
}