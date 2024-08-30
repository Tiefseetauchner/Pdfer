using System.Collections.Generic;
using Pdfer.Objects;

namespace Pdfer;

public record Body(
  Dictionary<ObjectIdentifier, DocumentObject> Objects)
{
  public DocumentObject this[ObjectIdentifier objectIdentifier] => Objects[objectIdentifier];
};