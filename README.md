# Pdfer
A basic C# library meant to make accessing and manipulating PDFs complicated but extremely powerful.

## Usage
Pdfer uses Streams to read and write PDFs. It's best to open a Stream with your PDF content, like a FileStream, to reduce memory usage while 
loading and parsing the PDF.

For a basic example, see the TestConsole Project.

What, you really want more detail? Fine.

### Parsing

To parse a PDF, you can create a `PdfDocumentParser` with the `PdfDocumentParserFactory`. If you want to adjust behaviour of the parser, you can 
of course make your own and overwrite the behaviour of the various helper classes, but I recommend not doing that unless extremely necessary. If
you need another feature, just make a PR and make the world better for everyone.

```csharp
using var stream = File.OpenRead("test.pdf");
var parser = PdfDocumentParserFactory.Create();
var document = parser.Parse(stream);
```

You can also load the PDF into memory and parse a byte array:

```csharp
byte[] pdf = File.ReadAllBytes("test.pdf");
var parser = PdfDocumentParserFactory.Create();
var document = parser.Parse(pdf);
```

### Manipulating

Currently, manipulation of PDFs is very limited. You can access and edit the objects on the parsed level, like changing the Creator in 
the dictionary:

```csharp
var infoReference = ObjectIdentifier.ParseReference(pdfDocument.Trailer.TrailerDictionary["/Info"]);
var infoDictionary = pdfDocument.Body[infoReference] as DictionaryObject ?? throw new InvalidOperationException("Info dictionary not found");
infoDictionary.Value["/Producer"] = new PdfStringHelper().GetHexString("My PDFer");
```

What you currently can't do is changing the raw data, even though there's a `RawValue` on `DocumentObject`, this is currently ignored.
This might change at some point, if you need it you can make an issue. It's just not a priority for me right now.

### Writing

Writing is done with the `PdfDocumentWriter` and you guessed it, you can make one with the `PdfDocumentWriterFactory`.

```csharp
var writer = PdfDocumentWriterFactory.Create();
var stream = File.OpenWrite("test.pdf");
writer.Write(stream, document);
```

## Known Issues

- The parser doesn't currently support multilayer PDFs.
- Things like signed PDFs with multiple trailers don't work.
- Writing PDFs currently only really works for extraordinarily simple PDFs.

## Why
I and someone I know want a library that allows us to easily manipulate PDFs on a object level, so I decided to parse PDFs.

## Help
Make an issue and pray I have the time to help

## I want to give help
Yes!

[!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/tiefseetauchner)
