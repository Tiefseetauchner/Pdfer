using System;
using System.IO;

namespace Pdfer;

public class PdfDocumentWriter : IPdfDocumentWriter
{
  public void Write(Stream stream, PdfDocument pdfDocument)
  {
    if (!stream.CanWrite)
      throw new ArgumentException("Stream is not writable", nameof(stream));

    WriteHeader(stream, pdfDocument.Header);
    WriteBody(stream, pdfDocument.Body);
  }

  private void WriteBody(Stream stream, Body pdfDocumentBody)
  {
    foreach (var (key, value) in pdfDocumentBody.Objects)
    {
      stream.Write(value.RawValue);
      stream.Write("\n\n"u8);
    }
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
}

public interface IPdfDocumentWriter
{
  void Write(Stream stream, PdfDocument pdfDocument);
}