using System;
using System.IO;
using System.Threading.Tasks;
using Pdfer.Objects;

namespace Pdfer.TestConsole;

class Program
{
  static async Task Main(string[] args)
  {
    Console.WriteLine("Hello Pdfer!");

    var pdfDocument = await new PdfDocumentParser(new PdfDocumentPartParserFactory().Create()).Parse(File.OpenRead(args[0]));

    var infoReference = ObjectIdentifier.ParseReference(pdfDocument.DocumentParts[0].Trailer.TrailerDictionary["/Info"]);
var infoDictionary = pdfDocument.DocumentParts[0]   .Body[infoReference] as DictionaryObject ?? throw new InvalidOperationException("Info dictionary not found");
    infoDictionary.Value["/Producer"] = PdfStringHelper.AsHexString("PDFer");
    infoDictionary.Value["/Title"] = PdfStringHelper.AsHexString("My PDFer Specification!!!");

    await using var outputStream = File.OpenWrite(args[1]);
    outputStream.SetLength(0);
    await new PdfDocumentWriterFactory().Create().Write(outputStream, pdfDocument);
  }
}