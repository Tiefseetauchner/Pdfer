namespace Pdfer.Objects;

public class ArrayObject(DocumentObject[] value) : DocumentObject
{
  public DocumentObject[] Value => value;
}