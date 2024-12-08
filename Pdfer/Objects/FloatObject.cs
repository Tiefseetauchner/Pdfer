namespace Pdfer.Objects;

public class FloatObject(double value) : NumericObject
{
  public double Value => value;

  public override string ToString() => $"{Value}";
}