using System.Collections.Generic;

namespace Pdfer.Objects;

public class DictionaryObject(Dictionary<string, string> value, byte[] rawValue) : DocumentObject(rawValue)
{
  public Dictionary<string, string> Value => value;
}