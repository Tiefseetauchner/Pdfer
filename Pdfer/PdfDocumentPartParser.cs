using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Pdfer.Objects;

namespace Pdfer;

public class PdfDocumentPartParser(
  StreamHelper streamHelper,
  PdfDictionaryHelper pdfDictionaryHelper,
  PdfObjectReader pdfObjectReader)
{
  public async Task<List<PdfDocumentPart>> Parse(Stream stream)
  {
    using var streamReader = new StreamReader(stream, leaveOpen: true);

    var documentParts = new List<PdfDocumentPart>();
    var hasNextPart = true;
    var currentXrefOffset = await GetInitialXrefOffset(stream);

    while (hasNextPart)
    {
      var xrefTable = await GetXrefTable(stream, streamReader, currentXrefOffset);
      var trailerDictionary = await GetTrailerDictionary(stream);

      var trailer = new Trailer(trailerDictionary, currentXrefOffset);

      if (trailerDictionary.TryGetValue("/Prev", out var prevXRefOffset))
      {
        hasNextPart = prevXRefOffset != "0";
        currentXrefOffset = long.Parse(prevXRefOffset);
      }
      else
      {
        hasNextPart = false;
      }

      var body = await GetBody(stream, xrefTable, trailer);

      documentParts = documentParts.Prepend(new PdfDocumentPart(body, xrefTable, trailer)).ToList();
    }

    return documentParts;
  }

  private async Task<long> GetInitialXrefOffset(Stream stream)
  {
    await using var reverseStream = new ReverseStream(stream);
    reverseStream.Position = 0;
    using var reverseStreamReader = new StreamReader(reverseStream);

    await VerifyEofExists(reverseStreamReader);

    var xRefOffset = await GetXrefOffset(reverseStreamReader);

    await VerifyStartxrefExists(reverseStreamReader);

    return xRefOffset;
  }

  private async Task VerifyEofExists(StreamReader streamReader)
  {
    var eofFound = false;
    while (!eofFound)
    {
      var line = await streamHelper.ReadReverseLine(streamReader);

      if (string.IsNullOrWhiteSpace(line))
        continue;

      if (line != "%%EOF")
        throw new InvalidOperationException("No EOF found");

      eofFound = true;
    }
  }

  private async Task<long> GetXrefOffset(StreamReader streamReader)
  {
    var xrefOffsetString = await streamHelper.ReadReverseLine(streamReader);
    var xrefOffsetParsed = long.TryParse(xrefOffsetString, out var xrefOffset);
    if (!xrefOffsetParsed)
      throw new InvalidOperationException("xref offset is not a number");
    return xrefOffset;
  }

  private async Task VerifyStartxrefExists(StreamReader streamReader)
  {
    var startXref = await streamHelper.ReadReverseLine(streamReader);

    if (startXref != "startxref")
      throw new InvalidOperationException("No 'startxref' keyword found");
  }

  private async Task<XRefTable> GetXrefTable(Stream baseStream, StreamReader streamReader, long xrefOffset)
  {
    baseStream.Seek(xrefOffset, SeekOrigin.Begin);
    streamReader.DiscardBufferedData();

    var xRefTable = new XRefTable();
    var line = await streamReader.ReadLineAsync();

    if (line != "xref")
      throw new InvalidOperationException("No 'xref' keyword found");

    while ((line = await streamReader.ReadLineAsync()) != null && line != "trailer")
    {
      var (objectNumber, objectsInSection) = ParseSubsectionHeader(line);

      await AddXRefEntriesInSubsection(objectsInSection, streamReader, objectNumber, xRefTable);
    }

    baseStream.Seek(xrefOffset, SeekOrigin.Begin);

    return xRefTable;
  }

  private (int ObjectNumber, int RemainingInSection) ParseSubsectionHeader(string line)
  {
    var subsectionHeaderParts = line.Split(' ');
    return (int.Parse(subsectionHeaderParts[0]), int.Parse(subsectionHeaderParts[1]));
  }

  private async Task AddXRefEntriesInSubsection(int objectsInSection, StreamReader streamReader, int objectNumber, XRefTable xRefTable)
  {
    for (var i = 0; i < objectsInSection; i++)
    {
      var line = await streamReader.ReadLineAsync();

      if (string.IsNullOrWhiteSpace(line))
        throw new IOException("Unexpected end of stream");

      var (objectIdentifier, xRefEntry) = GetXrefEntry(line, objectNumber);
      xRefTable.Add(objectIdentifier, xRefEntry);
      objectNumber++;
    }
  }

  private (ObjectIdentifier, XRefEntry) GetXrefEntry(string line, int objectNumber)
  {
    var objectEntryParts = line.Split(' ');
    var offset = long.Parse(objectEntryParts[0]);
    var generationNumber = int.Parse(objectEntryParts[1]);
    var type = objectEntryParts[2] switch
    {
      "f" => XRefEntryType.Free,
      "n" => XRefEntryType.Used,
      _ => throw new InvalidOperationException($"Invalid xref entry type '{objectEntryParts[2]}'")
    };
    return (new ObjectIdentifier(objectNumber, generationNumber), new XRefEntry(offset, type));
  }

  private async Task<Dictionary<string, string>> GetTrailerDictionary(Stream stream)
  {
    await streamHelper.ReadStreamTo("trailer", stream);
    await streamHelper.ReadStreamTo("\n", stream);

    return (await pdfDictionaryHelper.ReadDictionary(stream)).dictionary;
  }

  private async Task<Body> GetBody(Stream stream, XRefTable xRefTable, Trailer trailer)
  {
    var objectRepository = new ObjectRepository(pdfObjectReader, xRefTable);

    var usedXrefEntries = xRefTable
      .Where(entry => entry.Value.Flag == XRefEntryType.Used);

    foreach (var xRefEntry in usedXrefEntries)
    {
      await objectRepository.RetrieveObject<DocumentObject>(xRefEntry.Key, stream);
    }

    return new Body(objectRepository.Objects);
  }
}