using System;
using System.Text;

namespace Pdfer;

public record ObjectIdentifier(
  int ObjectNumber,
  int Generation)
{
  public static ObjectIdentifier ParseReference(string objectReference)
  {
    if (TryParseReference(objectReference, out var parsedReference))
      return parsedReference;

    throw new ArgumentException($"'{objectReference}' is not a valid object reference.", nameof(objectReference));
  }

  public static bool TryParseReference(string objectReference, out ObjectIdentifier parsedReference)
  {
    parsedReference = null!;

    var objectReferenceParts = objectReference.Trim().Split(' ');

    if (objectReferenceParts.Length != 3)
      return false;

    if (!int.TryParse(objectReferenceParts[0], out var number) || !int.TryParse(objectReferenceParts[1], out var generation) || objectReferenceParts[2] != "R")
      return false;

    parsedReference = new ObjectIdentifier(number, generation);
    return true;
  }

  public static ObjectIdentifier ParseIdentifier(string objectIdentifier)
  {
    if (TryParseIdentifier(objectIdentifier, out var parsedIdentifier))
      return parsedIdentifier;

    throw new ArgumentException($"'{objectIdentifier}' is not a valid object identifier.", nameof(objectIdentifier));
  }

  public static bool TryParseIdentifier(string objectReference, out ObjectIdentifier parsedReference)
  {
    parsedReference = null!;

    var objectReferenceParts = objectReference.Trim().Split(' ');

    if (objectReferenceParts.Length != 3)
      return false;

    if (!int.TryParse(objectReferenceParts[0], out var number) || !int.TryParse(objectReferenceParts[1], out var generation) || objectReferenceParts[2] != "obj")
      return false;

    parsedReference = new ObjectIdentifier(number, generation);
    return true;
  }

  public byte[] GetHeaderBytes() =>
    Encoding.UTF8.GetBytes(GetHeaderString());

  public string GetHeaderString() =>
    $"{ObjectNumber} {Generation} obj\n";
}