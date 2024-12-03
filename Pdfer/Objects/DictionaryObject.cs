using System.Collections.Generic;

namespace Pdfer.Objects;

public class DictionaryObject(Dictionary<string, DocumentObject> value) : DocumentObject
{
  public Dictionary<string, DocumentObject> Value => value;
}