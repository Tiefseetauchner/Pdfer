using System;

namespace Pdfer.Objects;

public abstract class PdfParsingException(string message) : Exception(message);

public class PdfInvalidIndirectObjectReferenceParsingException(string message) : PdfParsingException(message);

public class PdfInvalidBooleanValueParsingException(string message) : PdfParsingException(message);

public class PdfInvalidHexStringValueParsingException(string message) : PdfParsingException(message);

public class PdfInvalidLiteralStringValueParsingException(string message) : PdfParsingException(message);

public class PdfInvalidNullObjectValueParsingException(string message) : PdfParsingException(message);

public class PdfInvalidStreamEndParsingException(string message) : PdfParsingException(message);