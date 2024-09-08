using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Pdfer;

public class PdfDocumentWriter : IPdfDocumentWriter
{
  public void Write(Stream stream, PdfDocument pdfDocument)
  {
    if (!stream.CanWrite)
      throw new ArgumentException("Stream is not writable", nameof(stream));

    WriteHeader(stream, pdfDocument.Header);
    var xRefTable = WriteBody(stream, pdfDocument.Body);
    var xRefTableOffset = WriteXrefTable(stream, xRefTable);
    WriteTrailer(stream, pdfDocument.Trailer, xRefTableOffset);
  }

  private void WriteTrailer(Stream stream, Trailer pdfDocumentTrailer, long xRefTableOffset)
  {
    stream.Write("trailer\n"u8);
    stream.Write("<<"u8);
    foreach (var (key, value) in pdfDocumentTrailer.TrailerDictionary)
    {
      stream.Write("\n/"u8);
      stream.Write(Encoding.ASCII.GetBytes(key));
      stream.Write(" "u8);
      stream.Write(Encoding.ASCII.GetBytes(value));
    }

    stream.Write(">>"u8);

    stream.Write("\n"u8);
    stream.Write("startxref\n"u8);
    stream.Write(Encoding.ASCII.GetBytes(xRefTableOffset.ToString()));
    stream.Write("\n"u8);
    stream.Write("%%EOF\n"u8);
  }

  private long WriteXrefTable(Stream stream, XRefTable xRefTable)
  {
    var xRefTableOffset = stream.Position;
    stream.Write("xref\n"u8);
    stream.Write("0 "u8);
    stream.Write(Encoding.ASCII.GetBytes(xRefTable.Count.ToString()));
    stream.Write("\n"u8);

    var previousObjectNumber = -1;
    foreach (var (identifier, xRefEntry) in xRefTable.OrderBy(_ => _.Key.ObjectNumber))
    {
      if (identifier.ObjectNumber != previousObjectNumber + 1)
        throw new InvalidOperationException($"Object numbers are not consecutive: {previousObjectNumber} -> {identifier.ObjectNumber} (not yet supported)");

      var flagCharacter = xRefEntry.Flag == XRefEntryType.Free ? 'f' : 'n';

      stream.Write(Encoding.ASCII.GetBytes(
        $"{xRefEntry.Position:0000000000} {identifier.Generation:00000} {flagCharacter} \n"));

      previousObjectNumber++;
    }

    return xRefTableOffset;
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
}

public interface IPdfDocumentWriter
{
  void Write(Stream stream, PdfDocument pdfDocument);
}