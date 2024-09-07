namespace Pdfer.Objects;

public class StreamObject(byte[] value) : DocumentObject
{
  public byte[] Value => value;
  public override object RawValue => value;
}