using System;

namespace Pdfer.TestConsole;

class Program
{
  static void Main(string[] args)
  {
    Console.WriteLine("Hello Pdfer!");
    
    var pdfDocument = PdfDocumentFactory.Instance.Parse(System.IO.File.OpenRead(args[0]));
    
    Console.WriteLine(pdfDocument.PdfVersion);

    Console.WriteLine("Press any key to exit...");
    Console.ReadLine();
  }
}