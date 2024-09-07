using System;
using System.IO;
using System.Threading.Tasks;
using Pdfer.Objects;

namespace Pdfer;

public class PdfObjectReader(
  IDocumentObjectReader<DictionaryObject> dictionaryObjectReader,
  IDocumentObjectReader<StringObject> stringObjectReader,
  IDocumentObjectReader<StreamObject> streamObjectReader) : IPdfObjectReader
{
  private static readonly char[] DictionaryStart = ['<', '<'];

  // TODO (lena): Deal with NameObjects
  // TODO (lena): Deal with BooleanObjects
  // TODO (lena): Deal with NullObjects
  public async Task<DocumentObject> Read(StreamReader streamReader, long xrefOffset)
  {
    streamReader.DiscardBufferedData();
    streamReader.BaseStream.Seek(xrefOffset, SeekOrigin.Begin);
    var objectIdentifier = await streamReader.ReadLineAsync();

    var objectStartBuffer = new char[2];
    var objectStart = streamReader.Read(objectStartBuffer);
    streamReader.BaseStream.Position -= objectStart;

    if (objectStart != 2)
      throw new IOException("Unexpected end of stream");
    
    if (objectStartBuffer[0] == '<' &&
        objectStartBuffer[0] != '<' ||
        objectStartBuffer[0] == '(')
      return await stringObjectReader.Read(streamReader.BaseStream);

    if (objectStartBuffer[0] == '<' &&
        objectStartBuffer[0] == '<')
    {
      var dictionary = await dictionaryObjectReader.Read(streamReader.BaseStream);
      streamReader.DiscardBufferedData();
      var nextLine = streamReader.ReadLine();
      if (nextLine == "stream")
        return await streamObjectReader.Read(streamReader.BaseStream, long.Parse(dictionary.Value["Size"])); 

      return dictionary;
    }

    // check if next line equals "stream"
    //   then read object key "Length"
    //     then check whether reference and retrieve object
    //   then read that amount of bytes.
    // Find next endobj
    // 
    return null!;
  }
}