using System;

namespace Pdfer.Objects;

public abstract class PdfParsingException(string message) : Exception(message);

public class PdfInvalidIndirectObjectReferenceParsingException(string message) : PdfParsingException(message);

public class PdfInvalidBooleanValueParsingException(string message) : PdfParsingException(message);