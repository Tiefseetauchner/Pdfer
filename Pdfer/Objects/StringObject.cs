namespace Pdfer.Objects;

public class StringObject(string value, byte[] rawValue, ObjectIdentifier objectIdentifier) : DocumentObject(rawValue, objectIdentifier)
{
  public string Value => value;
}