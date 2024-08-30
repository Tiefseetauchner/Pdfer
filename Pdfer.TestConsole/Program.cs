using System;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer.TestConsole;

class Program
{
  static async Task Main(string[] args)
  {
    Console.WriteLine("Hello Pdfer!");

    var pdfDocument = await PdfDocumentFactory.Instance.Parse(File.OpenRead(args[0]));

    Console.WriteLine(pdfDocument.PdfVersion);

    Console.WriteLine("Press any key to exit...");
    Console.ReadLine();
  }
}