using System.Collections.Generic;
using System.Linq;
using Pdfer.Objects;

namespace Pdfer.Common.Tests;

public static class ObjectBuilder
{
  public static ArrayObject ArrayObject() =>
    new([]);

  public static ArrayObject With(this ArrayObject arrayObject, params DocumentObject[] objects) =>
    new(arrayObject.Value.Concat(objects).ToArray());

  public static BooleanObject BooleanObject(bool value) =>
    new(value);

  public static DictionaryObject DictionaryObject() =>
    new([]);

  public static DictionaryObject With(this DictionaryObject dictionaryObject, params (NameObject nameObject, DocumentObject documentObject)[] objects) =>
    new(
      new PdfDictionary(
        dictionaryObject.Value.Concat(
            objects
              .Select(x => new KeyValuePair<NameObject, DocumentObject>(x.nameObject, x.documentObject)))
          .ToDictionary(x => x.Key, x => x.Value)));

  public static NameObject NameObject(string value) =>
    new(value);

  public static NullObject NullObject() =>
    new();

  public static NumericObject NumericObject(long value) =>
    new IntegerObject(value);

  public static NumericObject NumericObject(int value) =>
    new IntegerObject(value);

  public static NumericObject NumericObject(double value) =>
    new FloatObject(value);

  public static NumericObject NumericObject(float value) =>
    new FloatObject(value);

  public static ReferenceObject ReferenceObject() =>
    new(null, new ObjectIdentifier(0, 0));

  public static ReferenceObject WithValue(this ReferenceObject referenceObject, DocumentObject value) =>
    new(value, referenceObject.ObjectIdentifier);

  public static ReferenceObject WithIdentifier(this ReferenceObject referenceObject, int objectNumber, int generationNumber) =>
    new(referenceObject.Value, new ObjectIdentifier(objectNumber, generationNumber));

  public static StreamObject StreamObject() =>
    new([], null!);

  public static StreamObject WithValue(this StreamObject streamObject, params byte[] bytes) =>
    new(bytes, streamObject.Dictionary);

  public static StreamObject WithDictionary(this StreamObject streamObject, DictionaryObject dictionary) =>
    new(streamObject.Value, dictionary);

  public static StringObject StringObject(string value) =>
    new(value);

  public static StringObject LiteralStringObject(string value) =>
    new($"{PdfStringHelper.AsString(value)}");

  public static StringObject HexStringObject(string value) =>
    new($"<{PdfStringHelper.AsHexString(value)}>");
}