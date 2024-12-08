using System.Linq;

namespace Pdfer.Objects;

public class ArrayObject(DocumentObject[] value) : DocumentObject
{
  public DocumentObject[] Value => value;

  public override string ToString()
  {
    return $"[{string.Join(" ", value.Select(x => x.ToString()))}]";
  }
}