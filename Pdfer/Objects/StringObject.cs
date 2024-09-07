namespace Pdfer.Objects;

public class StringObject(string value, byte[] rawValue) : DocumentObject(rawValue)
{
  public string Value => value;

}