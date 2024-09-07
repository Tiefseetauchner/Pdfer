namespace Pdfer.Objects;

public class StreamObject(byte[] value, byte[] rawValue) : DocumentObject(rawValue)
{
  public byte[] Value => value;
}