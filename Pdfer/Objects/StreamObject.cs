using System.Collections.Generic;

namespace Pdfer.Objects;

public class StreamObject(
  byte[] value,
  byte[] rawValue,
  ObjectIdentifier objectIdentifier,
  Dictionary<string, string> dictionary) : DocumentObject(rawValue, objectIdentifier)
{
  public Dictionary<string, string> Dictionary => dictionary;
  public byte[] Value => value;
}