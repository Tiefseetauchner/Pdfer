using System;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class NumericObjectReader : IDocumentObjectReader<NumericObject>
{
  async Task<DocumentObject> IDocumentObjectReader.Read(Stream stream) =>
    await Read(stream);

  public async Task<NumericObject> Read(Stream stream)
  {
    var buffer = new byte[1];
    var number = .0f;
    var isDecimal = false;
    var decimalDivider = 1;

    while (await stream.ReadAsync(buffer) > 0 && char.IsNumber((char)buffer[0]))
    {
      if (buffer[0] == '.')
      {
        if (isDecimal)
          throw CreateInvalidFormattingException();

        isDecimal = true;

        continue;
      }

      if (isDecimal)
        decimalDivider *= 10;

      number *= 10;
      number += (int)char.GetNumericValue((char)buffer[0]);
    }

    number /= decimalDivider;

    if (decimalDivider == 1)
      return new IntegerObject((int)number);

    return new FloatObject(number);
  }

  private static InvalidOperationException CreateInvalidFormattingException() =>
    new("Number Object is not a valid number.");
}