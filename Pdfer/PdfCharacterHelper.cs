using System;
using System.Linq;

namespace Pdfer;

public static class PdfCharacterHelper
{
  public static readonly char[] DelimiterCharacters = ['(', ')', '<', '>', '[', ']', '{', '}', '/', '%'];

  public static bool IsDelimitingCharacter(char character) =>
    char.IsWhiteSpace(character) || DelimiterCharacters.Contains(character);
}