namespace Pdfer.Objects;

public class ArrayObject(string[] value, byte[] rawValue, ObjectIdentifier objectIdentifier) : DocumentObject(rawValue, objectIdentifier)
{
  public string[] Value => value;
}