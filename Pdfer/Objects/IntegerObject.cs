namespace Pdfer.Objects;

public class IntegerObject(long value, byte[] rawValue) : NumberObject(rawValue)
{
  public long Value => value;
}