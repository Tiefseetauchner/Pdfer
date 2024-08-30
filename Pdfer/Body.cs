using System.Collections.Generic;

namespace Pdfer;

public record Body(
  Dictionary<ObjectIdentifier, DocumentObject> Objects);