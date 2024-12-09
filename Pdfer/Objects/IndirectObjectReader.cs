using System;
using System.IO;
using System.Threading.Tasks;

namespace Pdfer.Objects;

public class IndirectObjectReader : IDocumentObjectReader<IndirectObject>
{
  async Task<DocumentObject> IDocumentObjectReader.Read(Stream stream, IObjectRepository objectRepository) =>
    await Read(stream, objectRepository);

  public async Task<IndirectObject> Read(Stream stream, IObjectRepository objectRepository)
  {
    var state = new IndirectObjectReaderState();

    while (await stream.ReadAsync(state.Buffer) > 0)
    {
      var nextChar = (char)state.Buffer[0];

      if (state.ReadingNumber)
      {
        ReadObjectNumber(nextChar, state);
        continue;
      }

      if (state.ReadingGeneration)
      {
        ReadObjectGeneration(nextChar, state);
        continue;
      }

      if (state.ReadingSuffix && nextChar == 'R')
        break;

      throw CreateInvalidReferenceException();
    }

    if (!state.ReadingSuffix || state.Number == null || state.Generation == null)
      throw CreateInvalidReferenceException();

    var objectIdentifier = new ObjectIdentifier(state.Number.Value, state.Generation.Value);
    var previousPosition = stream.Position;
    var objectValue = await objectRepository.RetrieveObject<DocumentObject>(objectIdentifier, stream);
    stream.Position = previousPosition;
    return new IndirectObject(objectValue, objectIdentifier);
  }

  private static void ReadObjectNumber(char nextChar, IndirectObjectReaderState state)
  {
    if (char.IsNumber(nextChar))
    {
      state.Number = (state.Number ?? 0) * 10 + (int)char.GetNumericValue(nextChar);
      return;
    }

    if (nextChar != ' ')
      throw CreateInvalidReferenceException();

    state.ReadingNumber = false;
    state.ReadingGeneration = true;
  }

  private static void ReadObjectGeneration(char nextChar, IndirectObjectReaderState state)
  {
    if (char.IsNumber(nextChar))
    {
      state.Generation = (state.Generation ?? 0) * 10 + (int)char.GetNumericValue(nextChar);
      return;
    }

    if (nextChar != ' ')
      throw CreateInvalidReferenceException();

    state.ReadingGeneration = false;
    state.ReadingSuffix = true;
  }

  private static Exception CreateInvalidReferenceException() =>
    throw new PdfInvalidIndirectObjectReferenceParsingException("Indirect object reference is not a valid reference.");

  private class IndirectObjectReaderState
  {
    public byte[] Buffer { get; } = new byte[1];
    public bool ReadingNumber { get; set; } = true;
    public int? Number { get; set; }
    public bool ReadingGeneration { get; set; }
    public int? Generation { get; set; }
    public bool ReadingSuffix { get; set; }
  }
}