namespace Pdfer.Objects;

public class StreamObject(
  byte[] value,
  DictionaryObject dictionary) : DocumentObject
{
  public DictionaryObject Dictionary => dictionary;
  public byte[] Value => value;
}