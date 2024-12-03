namespace Pdfer.Objects;

public class IndirectObject(DocumentObject value, ObjectIdentifier objectIdentifier) : DocumentObject
{
  public ObjectIdentifier ObjectIdentifier => objectIdentifier;
  public DocumentObject Value => value;
}