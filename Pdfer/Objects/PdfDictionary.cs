using System.Collections.Generic;
using System.Linq;

namespace Pdfer.Objects;

public class PdfDictionary : Dictionary<NameObject, DocumentObject>
{
  public DocumentObject this[string name]
  {
    get => this.Single(x => x.Key.Value == name).Value;
    set => this[new NameObject(name)] = value;
  }

  public bool TryGetValue(string name, out DocumentObject? value) => TryGetValue(new NameObject(name), out value);
}