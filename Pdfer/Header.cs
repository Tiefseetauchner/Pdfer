namespace Pdfer;

public record Header(
  PdfVersion Version,
  bool ContainsBinaryDataHeader);