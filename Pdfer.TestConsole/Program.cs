using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Pdfer.Objects;

namespace Pdfer.TestConsole;

class Program
{
  static async Task Main(string[] args)
  {
    Console.WriteLine("Hello Pdfer!");

    var pdfDocument = await new PdfDocumentParserFactory().Create().Parse(File.OpenRead(args[0]));
    
    var infoReference = ObjectIdentifier.ParseReference(pdfDocument.Trailer.TrailerDictionary["/Info"]);
    var infoDictionary = pdfDocument.Body[infoReference] as DictionaryObject ?? throw new InvalidOperationException("Info dictionary not found");
    infoDictionary.Value["/Producer"] = new PdfStringHelper().GetHexString("My PDFer");

    await using var outputStream = File.OpenWrite(args[1]);
    outputStream.SetLength(0);
    await new PdfDocumentWriterFactory().Create().Write(outputStream, pdfDocument);
  }
}