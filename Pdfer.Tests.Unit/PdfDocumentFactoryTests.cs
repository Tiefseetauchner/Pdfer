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
    var pdfDocumentBytes = await File.ReadAllBytesAsync("../../../TestData/Test.pdf");

    var pdfDocument = await _pdfDocumentFactory.Parse(pdfDocumentBytes);
    Assert.Multiple(() =>
    {
      Assert.That(pdfDocument.PdfVersion, Is.EqualTo(PdfVersion.Pdf17));
      Assert.That(pdfDocument.Header.ContainsBinaryDataHeader, Is.True);
      Assert.That(pdfDocument.Trailer.XRefByteOffset, Is.EqualTo(7530));
      Assert.That(pdfDocument.XRefTable, Has.Count.EqualTo(18));
      Assert.That(pdfDocument.XRefTable[new ObjectIdentifier(0, 65_535)], Is.EqualTo(new XRefEntry(0, XRefEntryType.Free)));
    });
  }

  [Test]
  public async Task Parse_FromStream()
  {
    await using var pdfDocumentBytes = File.OpenRead("../../../TestData/Test.pdf");

    var pdfDocument = await _pdfDocumentFactory.Parse(pdfDocumentBytes);
    Assert.Multiple(() =>
    {
      Assert.That(pdfDocument.PdfVersion, Is.EqualTo(PdfVersion.Pdf17));
      Assert.That(pdfDocument.Header.ContainsBinaryDataHeader, Is.True);
      Assert.That(pdfDocument.Trailer.XRefByteOffset, Is.EqualTo(7530));
      Assert.That(pdfDocument.XRefTable, Has.Count.EqualTo(18));
      Assert.That(pdfDocument.XRefTable[new ObjectIdentifier(0, 65_535)], Is.EqualTo(new XRefEntry(0, XRefEntryType.Free)));
    });
  }

  [Test]
  public void Parse_NoHeader_ThrowsException()
  {
    var pdfDocumentBytes = Encoding.UTF8.GetBytes("Hello World!");

    var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await _pdfDocumentFactory.Parse(pdfDocumentBytes));

    Assert.That(exception.Message, Is.EqualTo("Invalid header"));
  }

  [Test]
  public void Parse_EmptyDocument_ThrowsException()
  {
    byte[] pdfDocumentBytes = [];

    var exception = Assert.ThrowsAsync<IOException>(async () => await _pdfDocumentFactory.Parse(pdfDocumentBytes));

    Assert.That(exception.Message, Is.EqualTo("Unexpected end of stream"));
  }

  [Test]
  [TestCase("1.8")]
  [TestCase("2.0")]
  [TestCase("huh")]
  [TestCase("1-1")]
  public void Parse_InvalidVersion_ThrowsException(string version)
  {
    byte[] pdfDocumentBytes = Encoding.UTF8.GetBytes($"%PDF-{version}");

    var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await _pdfDocumentFactory.Parse(pdfDocumentBytes));

    Assert.That(exception.Message, Is.EqualTo($"Invalid version number '{version}'"));
  }

  [Test]
  public void Parse_NoEOF_ThrowsException()
  {
    var pdfDocumentBytes = File.ReadAllBytes("../../../TestData/Test_NoEOF.pdf");

    var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await _pdfDocumentFactory.Parse(pdfDocumentBytes));

    Assert.That(exception.Message, Is.EqualTo("No EOF found"));
  }

  [Test]
  public void Parse_NoStartxref_ThrowsException()
  {
    var pdfDocumentBytes = File.ReadAllBytes("../../../TestData/Test_NoStartxref.pdf");

    var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await _pdfDocumentFactory.Parse(pdfDocumentBytes));

    Assert.That(exception.Message, Is.EqualTo("No 'startxref' keyword found"));
  }

  [Test]
  public void Parse_StartxrefNotANumber_ThrowsException()
  {
    var pdfDocumentBytes = File.ReadAllBytes("../../../TestData/Test_CorruptStartxref.pdf");

    var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await _pdfDocumentFactory.Parse(pdfDocumentBytes));

    Assert.That(exception.Message, Is.EqualTo("xref offset is not a number"));
  }
}