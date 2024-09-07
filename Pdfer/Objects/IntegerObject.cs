namespace Pdfer.Objects;

public class IntegerObject(int value) : DocumentObject
{
  public int Value => value;
  public override object RawValue => value;
}