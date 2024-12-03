using System.Collections.Generic;
using Pdfer.Objects;

namespace Pdfer;

public record Trailer(
  Dictionary<string, DocumentObject> TrailerDictionary,
  long XRefByteOffset);