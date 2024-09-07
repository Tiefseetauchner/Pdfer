using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Pdfer.Objects;

namespace Pdfer.Tests.Unit;

public class DictionaryObjectReaderTest
{
  [Test]
  public async Task Read_ShouldReturnDictionary()
  {
    using var stream = new MemoryStream("""
                                        1 0 obj
                                        <</TestKey TestValue
                                        /TestKey2 TestValue2
                                        >>
                                        """u8.ToArray());

    var result = await new DictionaryObjectReader(new StreamHelper(), new PdfDictionaryHelper(new StreamHelper())).Read(stream, null!);

    Assert.That(result.Value, Is.EqualTo(new Dictionary<string, string>()
    {
      { "TestKey", "TestValue" },
      { "TestKey2", "TestValue2" }
    }));
  }
}