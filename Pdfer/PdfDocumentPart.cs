namespace Pdfer;

public class PdfDocumentPart(
  Body body,
  XRefTable xRefTable,
  Trailer trailer)
{
  public Body Body => body;
  public XRefTable XRefTable => xRefTable;
  public Trailer Trailer => trailer;
}