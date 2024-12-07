using System;

namespace Pdfer.Objects;

public class PdfParsingException(PdfParsingExceptionType type, string message, Exception? innerException = null) : Exception(message, innerException)
{
  public PdfParsingExceptionType Type { get; } = type;
}