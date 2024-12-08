namespace Pdfer.Objects;

public class BooleanObject(bool value) : DocumentObject
{
  public bool Value = value;

  public override string ToString() => Value ? "true" : "false";
}