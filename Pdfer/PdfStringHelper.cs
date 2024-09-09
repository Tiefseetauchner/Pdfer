using System;
using System.Linq;

namespace Pdfer;

public class PdfStringHelper
{
  public string GetString(string input) => 
    $"({input})";
  
  public string GetHexString(string input) =>
    $"<{string.Join("", input.Select(c => $"{Convert.ToByte(c):X2}"))}>";
}