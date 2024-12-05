namespace Pdfer.Objects;

public class DictionaryObject(PdfDictionary value) : DocumentObject
{
  public PdfDictionary Value => value;
}