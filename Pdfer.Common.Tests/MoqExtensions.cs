using Moq;

namespace Pdfer.Common.Tests
{
  public static class MoqExtensions
  {
    public static T MatchesEqual<T>(this T parameter) where T : class =>
      It.Is<T>((_) => _.Equals(parameter));

    public static T MatchesSame<T>(this T parameter) where T : class =>
      It.Is<T>((_) => ReferenceEquals(_, parameter));
  }
}