using System;
using System.IO;

namespace Pdfer;

public class PdfDocument(
  Header header,
  Body body,
  XRefTable xrefTable,
  Trailer trailer)
{
  public PdfVersion PdfVersion { get; set; } = header.Version;

  public void ToStream(Stream stream)
  {
    stream.WriteByte(100);
    Console.WriteLine(header);
    Console.WriteLine(body);
    Console.WriteLine(xrefTable);
    Console.WriteLine(trailer);
  }
}