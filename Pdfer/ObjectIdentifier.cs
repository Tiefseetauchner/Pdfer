namespace Pdfer;

public record ObjectIdentifier(
  int ObjectNumber,
  int Generation)
{
  public static ObjectIdentifier ParseReference(string objectIdentifier)
  {
    var objectIdentifierParts = objectIdentifier.Split(' ');
    return new ObjectIdentifier(int.Parse(objectIdentifierParts[0]), int.Parse(objectIdentifierParts[1]));
  }
}