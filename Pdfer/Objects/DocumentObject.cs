namespace Pdfer.Objects;

public abstract class DocumentObject(byte[] rawValue, ObjectIdentifier objectIdentifier)
{
  public byte[] RawValue { get; set; } = rawValue;
  public ObjectIdentifier ObjectIdentifier => objectIdentifier;
}