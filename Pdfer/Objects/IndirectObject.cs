namespace Pdfer.Objects;

public class IndirectObject(DocumentObject? value, ObjectIdentifier objectIdentifier) : DocumentObject
{
  public ObjectIdentifier ObjectIdentifier => objectIdentifier;
  public DocumentObject? Value => value;

  public override string ToString() => $"{objectIdentifier} 0 R";
}