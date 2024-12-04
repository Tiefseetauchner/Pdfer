using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Pdfer.Objects;

namespace Pdfer;

public class PdfDictionaryHelper(
  IStreamHelper streamHelper,
  IPdfObjectReader pdfObjectReader) : IPdfDictionaryHelper
{
  public async Task<Dictionary<string, DocumentObject>> ReadDictionary(Stream stream)
  {
    var buffer = new byte[2];
    NameObject? key = null;
    DocumentObject? value = null;

    while (await streamHelper.Peak(stream, buffer) != 2 && buffer[0] == '>' && buffer[1] == '>')
    {
      if (key == null)
        key = pdfObjectReader.Read(stream,)
    }

    if (state.Key != null)
      AddDictionaryEntry(state.Dictionary, state.Key, state.BufferStringBuilder.ToString()[..^2]);

    return state.Dictionary;
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

  private static void AddDictionaryEntry(Dictionary<string, DocumentObject> dictionary, string arrayKey, string bufferString)
  {
    if (dictionary.ContainsKey(arrayKey))
      throw new InvalidOperationException($"Duplicate key '{arrayKey}' in dictionary");
    if (bufferString.Trim().Length == 0)
      throw new InvalidOperationException($"Empty value for key '{arrayKey}' in dictionary");

    dictionary[arrayKey] = bufferString.Trim();
  }

  public async Task<byte[]> GetDictionaryBytes(Dictionary<string, DocumentObject> dictionary)
  {
    using var memoryStream = new MemoryStream();
    await WriteDictionary(memoryStream, dictionary);
    return memoryStream.ToArray();
  }

  public async Task WriteDictionary(Stream stream, Dictionary<string, DocumentObject> dictionary)
  {
    await stream.WriteAsync("<<"u8.ToArray());

    foreach (var (key, value) in dictionary)
    {
      await stream.WriteAsync("\n"u8.ToArray());
      await stream.WriteAsync(Encoding.UTF8.GetBytes(key));
      await stream.WriteAsync(" "u8.ToArray());
      // TODO (lena.tauchner): DocumentObjectWriter
      await stream.WriteAsync(Encoding.UTF8.GetBytes(value));
    }

    await stream.WriteAsync(">>"u8.ToArray());
  }
}