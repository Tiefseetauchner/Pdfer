using System.Linq;

namespace Pdfer.Objects;

public class ArrayObject(DocumentObject[] value) : DocumentObject
{
  public DocumentObject[] Value => value;

  public override string ToString() => $"[{string.Join(" ", value.Select(x => x.ToString()))}]";
}