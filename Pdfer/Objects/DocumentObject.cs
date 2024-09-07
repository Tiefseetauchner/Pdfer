namespace Pdfer.Objects;

public abstract class DocumentObject(byte[] rawValue)
{
  public byte[] RawValue { get; set; } = rawValue;
}