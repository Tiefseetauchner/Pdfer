using System.Collections.Generic;

namespace Pdfer;

public record Trailer(
  Dictionary<string, string> TrailerDictionary,
  long XRefByteOffset);