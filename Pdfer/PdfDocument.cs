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
}