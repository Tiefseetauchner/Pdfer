using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Pdfer.Tests.Unit;

public class PdfDocumentFactoryTests
{
  private PdfDocumentFactory _pdfDocumentFactory;

  [SetUp]
  public void Setup()
  {
    _pdfDocumentFactory = new PdfDocumentFactory();
  }

  [Test]
  public async Task Parse_FromBytes()
  {
    var pdfDocumentBytes = File.ReadAllBytes("../../../TestData/Test.pdf");

    var pdfDocument = await _pdfDocumentFactory.Parse(pdfDocumentBytes);

    Assert.That(pdfDocument.PdfVersion, Is.EqualTo(PdfVersion.Pdf17));
    // TODO (tiefseetauchner): Assert other properties once implemented
  }

  [Test]
  public void Parse_NoHeader_ThrowsException()
  {
    var pdfDocumentBytes = Encoding.UTF8.GetBytes("Hello World!");

    var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await _pdfDocumentFactory.Parse(pdfDocumentBytes));

    Assert.That(exception.Message, Is.EqualTo("Invalid header"));
  }

  [Test]
  public void Parse_NoEOF_ThrowsException()
  {
    var pdfDocumentBytes = File.ReadAllBytes("../../../TestData/Test_NoEOF.pdf");

    var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await _pdfDocumentFactory.Parse(pdfDocumentBytes));

    Assert.That(exception.Message, Is.EqualTo("No EOF found"));
  }
}