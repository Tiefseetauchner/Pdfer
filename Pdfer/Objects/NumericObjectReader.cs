using System;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class NumericObjectReader : IDocumentObjectReader<NumericObject>
{
  async Task<DocumentObject> IDocumentObjectReader.Read(Stream stream, ObjectRepository objectRepository) =>
    await Read(stream, objectRepository);

  public async Task<NumericObject> Read(Stream stream, ObjectRepository objectRepository)
  {
    var buffer = new byte[1];
    var number = .0f;
    var isDecimal = false;
    var decimalDivider = 1;
    var isNegative = false;

    while (await stream.ReadAsync(buffer) > 0 && (char.IsNumber((char)buffer[0]) || buffer[0] == '.' || buffer[0] == '-'))
    {
      if (buffer[0] == '-')
      {
        isNegative = true;
        continue;
      }

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

    number = isNegative ? -number : number;

    stream.Position -= 1;

    if (decimalDivider == 1)
      return new IntegerObject((int)number);

    return new FloatObject(number);
  }

  private static InvalidOperationException CreateInvalidFormattingException() =>
    new("Number Object is not a valid number.");
}