using System.Collections.Generic;

namespace Pdfer.Objects;

public class DictionaryObject(Dictionary<string, string> value, byte[] rawValue, ObjectIdentifier objectIdentifier) : DocumentObject(rawValue, objectIdentifier)
{
  public Dictionary<string, string> Value => value;
}