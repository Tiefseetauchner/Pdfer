namespace Pdfer.Objects;

public class FloatObject(float value, byte[] rawValue) : NumberObject(rawValue)
{
  public float Value => value;
}