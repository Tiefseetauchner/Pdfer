using NUnit.Framework;
using System;

namespace Pdfer.Tests.Unit.Objects;

public static class TypeAssert
{
  public static void VerifyInstanceOf<T>(object documentObject, Action<T> action)
  {
    Assert.That(documentObject, Is.TypeOf<T>());

    action((T)documentObject);
  }
}