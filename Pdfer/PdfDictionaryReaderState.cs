    using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pdfer;

public class PdfDictionaryReaderState
{
  public readonly Dictionary<string, string> Dictionary = new();
  public readonly byte[] Buffer = new byte[1];
  public StringBuilder BufferStringBuilder { get; } = new();
  public MemoryStream RawBytes { get; } = new();
  public Stack<char> OpeningBracketStack { get; } = new();
  public string? Key { get; set; }
  public bool KeyReading { get; set; }
  public bool Escaped { get; set; }
}