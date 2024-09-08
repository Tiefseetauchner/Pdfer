namespace Pdfer.Objects;

public class IntegerObject(long value, byte[] rawValue, ObjectIdentifier objectIdentifier) : NumberObject(rawValue, objectIdentifier)
{
  public long Value => value;
}