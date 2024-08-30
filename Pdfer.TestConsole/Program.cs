using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pdfer.TestConsole;

class Program
{
  static async Task Main(string[] args)
  {
    Console.WriteLine("Hello Pdfer!");

    var pdfDocument = await PdfDocumentFactory.Instance.Parse(File.OpenRead(args[0]));

    Console.WriteLine($"PdfVersion:       {pdfDocument.PdfVersion}");
    pdfDocument.Trailer.TrailerDictionary.ToList().ForEach(entry => Console.WriteLine($"trailer entry: {entry.Key} {entry.Value}"));
    Console.WriteLine($"startxref offset: {pdfDocument.Trailer.XRefByteOffset}");
    pdfDocument.XRefTable.ToList().ForEach(entry => Console.WriteLine($"xref entry: {entry.Key} {entry.Value}"));


    Console.Write("Press any key to exit...");
    Console.ReadKey();
    Console.WriteLine();
  }
}