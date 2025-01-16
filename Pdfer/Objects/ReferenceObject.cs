namespace Pdfer.Objects;

public class ReferenceObject(DocumentObject? value, ObjectIdentifier objectIdentifier) : DocumentObject
{
  public ObjectIdentifier ObjectIdentifier => objectIdentifier;
  public DocumentObject? Value => value;

  public override string ToString() => $"{objectIdentifier} 0 R";
}