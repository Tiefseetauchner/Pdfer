namespace Pdfer.Objects;

public class StringObject(string value) : DocumentObject
{
  public string Value => value;

  public override object RawValue => value;
}