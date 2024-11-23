using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer;

public class PdfDictionaryHelper(IStreamHelper streamHelper) : IPdfDictionaryHelper
{
  public async Task<(Dictionary<string, string> dictionary, byte[] bytes)> ReadDictionary(Stream stream)
  {
    var state = new PdfDictionaryReaderState();

    HandleFirstCharacter(stream, state);

    while (state.OpeningBracketStack.Count != 0)
    {
      if (await stream.ReadAsync(state.Buffer) == 0)
        throw new IOException("Unexpected end of stream");

      var character = (char)state.Buffer[0];

      state.RawBytes.Write(state.Buffer);

      if (HandleEscapeCharacter(state, character))
        continue;

      if (state.OpeningBracketStack.Count == 2)
      {
        HandleCharacter(character, state);
      }

      HandleBrackets(character, state);

      state.BufferStringBuilder.Append(character);
    }

    if (state.Key != null)
      AddDictionaryEntry(state.Dictionary, state.Key, state.BufferStringBuilder.ToString()[..^2]);

    return (state.Dictionary, state.RawBytes.ToArray());
  }

  private static void HandleBrackets(char character, PdfDictionaryReaderState state)
  {
    switch (character)
    {
      case '<' when state.OpeningBracketStack.Peek() != '(':
      case '[' when state.OpeningBracketStack.Peek() != '(':
      case '(':
        state.OpeningBracketStack.Push(character);
        break;
      case '>' when state.OpeningBracketStack.Peek() == '<':
      case ']' when state.OpeningBracketStack.Peek() == '[':
      case ')' when state.OpeningBracketStack.Peek() == '(':
        state.OpeningBracketStack.Pop();
        break;
    }
  }

  private static void HandleCharacter(char character, PdfDictionaryReaderState state)
  {
    switch (character)
    {
      case '/' when !state.KeyReading && (!string.IsNullOrWhiteSpace(state.BufferStringBuilder.ToString()) || state.Key == null):
        state.KeyReading = true;

        if (state.Key != null)
          AddDictionaryEntry(state.Dictionary, state.Key, state.BufferStringBuilder.ToString());

        state.BufferStringBuilder.Clear();
        break;
      case '/' when state.KeyReading:
      case ' ' when state.KeyReading:
      case '\n' when state.KeyReading:
      case '\r' when state.KeyReading:
      case '[' when state.KeyReading:
      case '(' when state.KeyReading:
      case '<' when state.KeyReading:
      case '>' when state.KeyReading:
        state.KeyReading = false;
        state.Key = state.BufferStringBuilder.ToString().Trim();
        state.BufferStringBuilder.Clear();
        break;
    }
  }

  private static bool HandleEscapeCharacter(PdfDictionaryReaderState state, char character)
  {
    if (state.Escaped)
    {
      state.BufferStringBuilder.Append(character);
      state.Escaped = false;
      return true;
    }

    if (character == '\\')
      state.Escaped = true;
    return false;
  }

  private void HandleFirstCharacter(Stream stream, PdfDictionaryReaderState state)
  {
    var firstChar = streamHelper.ReadChar(stream);
    state.BufferStringBuilder.Append(firstChar);
    state.OpeningBracketStack.Push(firstChar);
    state.RawBytes.WriteByte((byte)firstChar);
  }

  private static void AddDictionaryEntry(Dictionary<string, string> dictionary, string arrayKey, string bufferString)
  {
    if (dictionary.ContainsKey(arrayKey))
      throw new InvalidOperationException($"Duplicate key '{arrayKey}' in dictionary");
    if (bufferString.Trim().Length == 0)
      throw new InvalidOperationException($"Empty value for key '{arrayKey}' in dictionary");

    dictionary[arrayKey] = bufferString.Trim();
  }

  public async Task<byte[]> GetDictionaryBytes(Dictionary<string, string> dictionary)
  {
    using var memoryStream = new MemoryStream();
    await WriteDictionary(memoryStream, dictionary);
    return memoryStream.ToArray();
  }

  public async Task WriteDictionary(Stream stream, Dictionary<string, string> dictionary)
  {
    await stream.WriteAsync("<<"u8.ToArray());

    foreach (var (key, value) in dictionary)
    {
      await stream.WriteAsync("\n"u8.ToArray());
      await stream.WriteAsync(Encoding.UTF8.GetBytes(key));
      await stream.WriteAsync(" "u8.ToArray());
      await stream.WriteAsync(Encoding.UTF8.GetBytes(value));
    }

    await stream.WriteAsync(">>"u8.ToArray());
  }
}