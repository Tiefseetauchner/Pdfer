using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer;

public class PdfDocumentWriter(IPdfDictionaryHelper pdfDictionaryHelper) : IPdfDocumentWriter
{
  public async Task Write(Stream stream, PdfDocument pdfDocument)
  {
    if (!stream.CanWrite)
      throw new ArgumentException("Stream is not writable", nameof(stream));

    WriteHeader(stream, pdfDocument.Header);
    var xRefTable = WriteBody(stream, pdfDocument.Body);
    var xRefTableOffset = WriteXrefTable(stream, xRefTable);
    await WriteTrailer(stream, pdfDocument.Trailer, xRefTableOffset);
  }

  private void WriteHeader(Stream stream, Header pdfDocumentHeader)
  {
    stream.Write("%PDF-"u8);

    stream.Write(pdfDocumentHeader.Version switch
    {
      PdfVersion.Pdf10 => "1.0"u8,
      PdfVersion.Pdf11 => "1.1"u8,
      PdfVersion.Pdf12 => "1.2"u8,
      PdfVersion.Pdf13 => "1.3"u8,
      PdfVersion.Pdf14 => "1.4"u8,
      PdfVersion.Pdf15 => "1.5"u8,
      PdfVersion.Pdf16 => "1.6"u8,
      PdfVersion.Pdf17 => "1.7"u8,
      _ => throw new InvalidOperationException($"Invalid version number '{pdfDocumentHeader.Version}'")
    });

    stream.Write("\n"u8);

    if (pdfDocumentHeader.ContainsBinaryDataHeader)
      stream.Write("%äöüß"u8);

    stream.Write("\n"u8);
  }

  // TODO (lena): Make an object writer for each object type to write them from the value instead of the raw data
  private XRefTable WriteBody(Stream stream, Body pdfDocumentBody)
  {
    var xRefTable = new XRefTable
    {
      { new ObjectIdentifier(0, 65535), new XRefEntry(0, XRefEntryType.Free) }
    };

    foreach (var (key, value) in pdfDocumentBody.Objects)
    {
      var position = stream.Position;
      xRefTable.Add(key, new XRefEntry(position, XRefEntryType.Used));
      stream.Write(value.RawValue);
      stream.Write("\n\n"u8);
    }

    return xRefTable;
  }

  private long WriteXrefTable(Stream stream, XRefTable xRefTable)
  {
    var xRefTableOffset = stream.Position;
    stream.Write("xref\n"u8);

    var xRefTableSection = new List<string>();

    var firstObjectNumberInSection = 0;
    int? previousObjectNumber = null;

    foreach (var (identifier, xRefEntry) in xRefTable.OrderBy(_ => _.Key.ObjectNumber))
    {
      if (previousObjectNumber == null || identifier.ObjectNumber != previousObjectNumber + 1)
      {
        WriteXrefTableSection(stream, xRefTableSection, firstObjectNumberInSection);

        xRefTableSection.Clear();
        firstObjectNumberInSection = identifier.ObjectNumber;
      }


      var flagCharacter = xRefEntry.Flag == XRefEntryType.Free ? 'f' : 'n';

      xRefTableSection.Add($"{xRefEntry.Position:0000000000} {identifier.Generation:00000} {flagCharacter} \n");

      previousObjectNumber = identifier.ObjectNumber;
    }

    WriteXrefTableSection(stream, xRefTableSection, firstObjectNumberInSection);

    return xRefTableOffset;
  }

  private void WriteXrefTableSection(Stream stream, List<string> xRefTableSection, int firstObjectNumberInSection)
  {
    if (xRefTableSection.Count == 0)
      return;

    stream.Write(Encoding.ASCII.GetBytes($"{firstObjectNumberInSection} {xRefTableSection.Count.ToString()}\n"));

    var xRefTableEntries = string.Concat(xRefTableSection);
    stream.Write(Encoding.ASCII.GetBytes(xRefTableEntries));
  }

  // TODO (lena): Change Size in Trailer
  private async Task WriteTrailer(Stream stream, Trailer pdfDocumentTrailer, long xRefTableOffset)
  {
    await stream.WriteAsync("trailer\n"u8.ToArray());

    pdfDictionaryHelper.WriteDictionary(stream, pdfDocumentTrailer.TrailerDictionary);

    await stream.WriteAsync("\n"u8.ToArray());
    await stream.WriteAsync("startxref\n"u8.ToArray());
    await stream.WriteAsync(Encoding.ASCII.GetBytes(xRefTableOffset.ToString()));
    await stream.WriteAsync("\n"u8.ToArray());
    await stream.WriteAsync("%%EOF\n"u8.ToArray());
  }
}

public interface IPdfDocumentWriter
{
  Task Write(Stream stream, PdfDocument pdfDocument);
}