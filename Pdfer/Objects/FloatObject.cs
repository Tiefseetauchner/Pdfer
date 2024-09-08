namespace Pdfer.Objects;

public class FloatObject(float value, byte[] rawValue, ObjectIdentifier objectIdentifier) : NumberObject(rawValue, objectIdentifier)
{
  public float Value => value;
}