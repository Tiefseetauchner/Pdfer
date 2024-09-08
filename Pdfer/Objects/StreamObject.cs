namespace Pdfer.Objects;

public class StreamObject(byte[] value, byte[] rawValue, ObjectIdentifier objectIdentifier) : DocumentObject(rawValue, objectIdentifier)
{
  public byte[] Value => value;
}