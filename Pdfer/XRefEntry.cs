namespace Pdfer;

public record XRefEntry(
  long Position,
  long GenerationNumber,
  XRefInUseFlag Flag);