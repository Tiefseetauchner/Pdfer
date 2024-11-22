using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Pdfer.Tests.Unit;

public class PdfDocumentParserTest
{
  private PdfDocumentParser _pdfDocumentParser;

  [SetUp]
  public void Setup()
  {
    var pdfDocumentPartParser = new PdfDocumentPartParserFactory().Create();
    _pdfDocumentParser = new PdfDocumentParser(pdfDocumentPartParser);
  }

  [Test]
  public async Task Parse_FromBytes()
  {
    var pdfDocumentBytes = await File.ReadAllBytesAsync("../../../TestData/Test.pdf");

    var pdfDocument = await _pdfDocumentParser.Parse(pdfDocumentBytes);

    AssertPdfFile(pdfDocument);
  }


  [Test]
  public async Task Parse_FromStream()
  {
    await using var pdfDocumentBytes = File.OpenRead("../../../TestData/Test.pdf");

    var pdfDocument = await _pdfDocumentParser.Parse(pdfDocumentBytes);

    AssertPdfFile(pdfDocument);
  }

  private static void AssertPdfFile(PdfDocument pdfDocument)
  {
    Assert.Multiple(() =>
    {
      Assert.That(pdfDocument.PdfHeader.PdfVersion, Is.EqualTo(PdfVersion.Pdf17));
      Assert.That(pdfDocument.PdfHeader.ContainsBinaryDataHeader, Is.True);

      Assert.That(pdfDocument.DocumentParts[0].Trailer.XRefByteOffset, Is.EqualTo(7530));

      Assert.That(pdfDocument.DocumentParts[0].Trailer.TrailerDictionary, Has.Count.EqualTo(5));
      Assert.That(pdfDocument.DocumentParts[0].Trailer.TrailerDictionary["/Size"], Is.EqualTo("18"));
      Assert.That(pdfDocument.DocumentParts[0].Trailer.TrailerDictionary["/Root"], Is.EqualTo("16 0 R"));
      Assert.That(pdfDocument.DocumentParts[0].Trailer.TrailerDictionary["/ID"], Is.EqualTo("[ <5414416A1ACD8FC419744A93F45175A8>\n<5414416A1ACD8FC419744A93F45175A8> ]"));
      Assert.That(pdfDocument.DocumentParts[0].Trailer.TrailerDictionary["/DocChecksum"], Is.EqualTo("/AC2F9689252F6547045729CC0AC89AAD"));

      Assert.That(pdfDocument.DocumentParts[0].XRefTable, Has.Count.EqualTo(18));
      Assert.That(pdfDocument.DocumentParts[0].XRefTable[new ObjectIdentifier(0, 65_535)], Is.EqualTo(new XRefEntry(0, XRefEntryType.Free)));

      Assert.That(pdfDocument.DocumentParts[0].Body.Objects, Has.Count.EqualTo(17));
    });
  }

  [Test]
  public void Parse_NoHeader_ThrowsException()
  {
    var pdfDocumentBytes = Encoding.UTF8.GetBytes("Hello World!");

    var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await _pdfDocumentParser.Parse(pdfDocumentBytes));

    Assert.That(exception.Message, Is.EqualTo("Invalid header"));
  }

  [Test]
  public void Parse_EmptyDocument_ThrowsException()
  {
    byte[] pdfDocumentBytes = [];

    var exception = Assert.ThrowsAsync<IOException>(async () => await _pdfDocumentParser.Parse(pdfDocumentBytes));

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

    var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await _pdfDocumentParser.Parse(pdfDocumentBytes));

    Assert.That(exception.Message, Is.EqualTo($"Invalid version number '{version}'"));
  }

  [Test]
  public void Parse_NoEOF_ThrowsException()
  {
    var pdfDocumentBytes = File.ReadAllBytes("../../../TestData/Test_NoEOF.pdf");

    var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await _pdfDocumentParser.Parse(pdfDocumentBytes));

    Assert.That(exception.Message, Is.EqualTo("No EOF found"));
  }

  [Test]
  public void Parse_NoStartxref_ThrowsException()
  {
    var pdfDocumentBytes = File.ReadAllBytes("../../../TestData/Test_NoStartxref.pdf");

    var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await _pdfDocumentParser.Parse(pdfDocumentBytes));

    Assert.That(exception.Message, Is.EqualTo("No 'startxref' keyword found"));
  }

  [Test]
  public void Parse_StartxrefNotANumber_ThrowsException()
  {
    var pdfDocumentBytes = File.ReadAllBytes("../../../TestData/Test_CorruptStartxref.pdf");

    var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await _pdfDocumentParser.Parse(pdfDocumentBytes));

    Assert.That(exception.Message, Is.EqualTo("xref offset is not a number"));
  }
}