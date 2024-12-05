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

    var infoDictionary = pdfDocument.DocumentParts[0].Trailer.TrailerDictionary["Info"] switch
    {
      IndirectObject indirectObject => pdfDocument.DocumentParts[0].Body[indirectObject.ObjectIdentifier] as DictionaryObject
                                       ?? throw new InvalidOperationException("Info dictionary not found"),
      DictionaryObject dictionaryObject => dictionaryObject,
      _ => throw new InvalidOperationException("Info dictionary not found")
    };

    infoDictionary.Value["Producer"] = new StringObject(PdfStringHelper.AsHexString("PDFer"));
    infoDictionary.Value["Title"] = new StringObject(PdfStringHelper.AsHexString("My PDFer Specification!!!"));

    await using var outputStream = File.OpenWrite(args[1]);
    outputStream.SetLength(0);
    await PdfDocumentWriterFactory.Create().Write(outputStream, pdfDocument);
  }
}