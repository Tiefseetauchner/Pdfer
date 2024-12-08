namespace Pdfer.Objects;

public class NameObject(string value) : DocumentObject()
{
  public string Value => value;

  public override string ToString() => $"/{Value}";
}