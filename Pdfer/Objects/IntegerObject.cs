namespace Pdfer.Objects;

public class IntegerObject(long value) : NumericObject()
{
  public long Value => value;
  public override string ToString() => $"{Value}";
}