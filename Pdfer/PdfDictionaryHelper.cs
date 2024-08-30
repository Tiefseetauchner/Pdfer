using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer;

public class PdfDictionaryHelper(StreamHelper streamHelper) : IPdfDictionaryHelper
{
  public async Task<Dictionary<string, string>> ReadDictionary(byte[] bytes)
  {
    using var stream = new MemoryStream(bytes);

    var dictionary = new Dictionary<string, string>();

    while (stream.Position < stream.Length && await streamHelper.ReadStreamTo("/", stream) is { } dictEntry)
    {
      var dictEntryString = Encoding.UTF8.GetString(dictEntry);
      var dictEntrySplit = dictEntryString.Split(' ', 2);

      // NOTE (lena): Due to some fun producers of PDFs, we might get a non-standard conforming PDF.
      //              This might mean there's a DocChecksum element with the value starting with a /.
      //              We could handle that. Ooooor we don't. My choice here is to ignore it. :)
      if (dictEntrySplit.Length < 2 || string.IsNullOrWhiteSpace(dictEntrySplit[1]))
        continue;

      dictionary.Add(dictEntryString.Split(' ')[0].Trim(), dictEntrySplit[1].Trim());
    }

    return dictionary;
  }
}