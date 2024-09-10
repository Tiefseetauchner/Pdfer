namespace Pdfer.Objects;

public class NameObject(string value, byte[] rawValue, ObjectIdentifier objectIdentifier) : DocumentObject(rawValue, objectIdentifier)
{
  public string Value => value;
}