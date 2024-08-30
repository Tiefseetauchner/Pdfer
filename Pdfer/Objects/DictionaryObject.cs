using System.Collections.Generic;

namespace Pdfer.Objects;

public class DictionaryObject(Dictionary<string, string> value) : DocumentObject
{
  public Dictionary<string, string> Value => value;

  public override object RawValue => value;
}