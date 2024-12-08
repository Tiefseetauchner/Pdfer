using Pdfer.Objects;

namespace Pdfer;

public record Trailer(
  PdfDictionary TrailerDictionary,
  long XRefByteOffset);