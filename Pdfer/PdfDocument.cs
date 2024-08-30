using System;
using System.IO;

namespace Pdfer;

public class PdfDocument(
  Header header,
  Body body,
  XRefTable xRefTable,
  Trailer trailer)
{
  public PdfVersion PdfVersion => Header.Version;
  public Header Header => header;
  public Body Body => body;
  public XRefTable XRefTable => xRefTable;
  public Trailer Trailer => trailer;

  public void ToStream(Stream stream)
  {
    stream.WriteByte(100);
    Console.WriteLine(Header);
    Console.WriteLine(Body);
    Console.WriteLine(XRefTable);
    Console.WriteLine(Trailer);
  }
}