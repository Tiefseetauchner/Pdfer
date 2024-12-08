namespace Pdfer.Tests.Unit.Objects;

public class StringObjectReaderTest
{
  // private StringObjectReader _objectReader;
  //
  // [SetUp]
  // public void SetUp()
  // {
  //   _objectReader = new StringObjectReader();
  // }
  //
  // [Test]
  // public async Task Read_ShouldReturnStringObject_LiteralString()
  // {
  //   using var stream = new MemoryStream(
  //     """
  //       (testString \( \) \< \> \\ \\\\ Test \\)
  //       endobj
  //       """u8.ToArray());
  //
  //   var result = await _objectReader.Read(stream, null!, new ObjectIdentifier(1, 0));
  //
  //   Assert.That(result.Value, Is.EqualTo(@"(testString \( \) \< \> \\ \\\\ Test \\)"));
  // }
  //
  // [Test]
  // public async Task Read_ShouldReturnStringObject_HexString()
  // {
  //   using var stream = new MemoryStream(
  //     """
  //       <A78937649845ABCDEF21234568757>
  //       endobj
  //       """u8.ToArray());
  //
  //   var result = await _objectReader.Read(stream, null!, new ObjectIdentifier(1, 0));
  //
  //   Assert.That(result.Value, Is.EqualTo(@"<A78937649845ABCDEF21234568757>"));
  // }
}