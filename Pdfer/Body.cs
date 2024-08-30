using System.Collections.Generic;

namespace Pdfer;

public record Body(
  Dictionary<ObjectIdentifier, DocumentObject> Objects)
{
  public DocumentObject this[ObjectIdentifier objectIdentifier] => Objects[objectIdentifier];
};