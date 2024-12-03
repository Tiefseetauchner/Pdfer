namespace Pdfer.Objects;

// TODO (lena.tauchner): Differentiate between Hex and literal string
public class StringObject(string value) : DocumentObject
{
  public string Value => value;
}