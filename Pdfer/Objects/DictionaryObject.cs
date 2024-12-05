using System.Linq;

namespace Pdfer.Objects;

public class DictionaryObject(PdfDictionary value) : DocumentObject
{
  public PdfDictionary Value => value;

  public override string ToString() => $"<<{string.Join(" ", value.Select(x => $"{x.Key} {x.Value}"))}>>";
}