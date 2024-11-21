namespace Pdfer;

public record Header(
  PdfVersion PdfVersion,
  bool ContainsBinaryDataHeader);