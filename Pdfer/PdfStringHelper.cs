using System;
using System.Globalization;
using System.Linq;

namespace Pdfer;

public static class PdfStringHelper
{
  // TODO (lena.tauchner): Encode special characters
  public static string AsString(string input) =>
    $"({input})";

  public static string AsHexString(string input) =>
    $"<{string.Join("", input.Select(c => $"{Convert.ToByte(c):X2}"))}>";

  public static string AsHexString(byte[] input) =>
    $"<{string.Join("", input.Select(c => $"{c:X2}"))}>";

  public static byte[] FromHexString(string input) =>
    DecodeHexString(input[1..^1]);

  private static byte[] DecodeHexString(string input)
  {
    if (input.Length % 2 != 0)
      throw new ArgumentException("Hex string must have an even length.", nameof(input));

    var bytes = new byte[input.Length / 2];
    for (var i = 0; i < bytes.Length; i++)
    {
      var hexPair = input.Substring(i * 2, 2);
      bytes[i] = byte.Parse(hexPair, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
    }

    return bytes;
  }
}