using System;
using System.IO;
using System.Threading.Tasks;
using Pdfer.Objects;

namespace Pdfer;

public class PdfObjectReader(
  IDocumentObjectReader<DictionaryObject> dictionaryObjectReader,
  IDocumentObjectReader<StringObject> stringObjectReader) : IPdfObjectReader
{
  private static readonly char[] DictionaryStart = ['<', '<'];

  // TODO (lena): Deal with NameObjects
  // TODO (lena): Deal with BooleanObjects
  // TODO (lena): Deal with NullObjects
  public async Task<DocumentObject> Read(StreamReader streamReader, long xrefOffset)
  {
    streamReader.BaseStream.Position = xrefOffset;
    streamReader.DiscardBufferedData();
    var objectIdentifier = await streamReader.ReadLineAsync();

    var objectStartBuffer = new char[2];
    var objectStart = streamReader.Read(objectStartBuffer);
    streamReader.BaseStream.Position -= objectStart;

    if (objectStart == 2 &&
        objectStartBuffer[0] == '<' &&
        objectStartBuffer != DictionaryStart ||
        objectStartBuffer[0] == '(')
      return await stringObjectReader.Read(streamReader.BaseStream);

    if (objectStartBuffer == DictionaryStart)
    {
      var dictionary = await dictionaryObjectReader.Read(streamReader.BaseStream);
      var nextLine = await streamReader.ReadLineAsync();
      // if (nextLine != "stream")
      //   return streamObjectReader.Read(streamReader.BaseStream); 

      return dictionary;
    }

    // check if next line equals "stream"
    //   then read object key "Length"
    //     then check whether reference and retrieve object
    //   then read that amount of bytes.
    // Find next endobj
    // 
    throw new NotImplementedException();
  }
}