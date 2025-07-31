using GsaGH.Helpers;

using Xunit;

namespace GsaGHTests.Helpers {
  public class DoubleComparerTests {
    [Theory]
    [InlineData(1.0000001, 1.0000002, 0.0001, true)]
    [InlineData(1.0, 1.1, 0.05, false)]
    [InlineData(0, 0, 0.01, true)]
    [InlineData(0, 0.005, 0.01, true)]
    [InlineData(0, 0.02, 0.01, false)]
    public void Equals_RelativeErrorBehavior(double a, double b, double epsilon, bool expected) {
      var comparer = new DoubleComparer(epsilon);
      Assert.Equal(expected, comparer.Equals(a, b));
    }

    [Theory]
    [InlineData(1.2345, 1.2344, 3, false, false)]
    [InlineData(1.2345, 1.2360, 3, false, false)]
    [InlineData(1.2345, 1.2360, 3, true, true)]
    [InlineData(1.2345, 1.2450, 3, true, false)]
    public void IsEqualsAtPrecisionLevel_BehavesAsExpected(
      double a, double b, int precision, bool useMargin, bool expected) {
      var comparer = new DoubleComparer(0.01, useMargin);
      Assert.Equal(expected, comparer.IsEqualsAtPrecisionLevel(a, b, precision));
    }

    [Theory]
    [InlineData(1.01, 1.02, 0.05, true)]
    [InlineData(1.01, 1.04, 0.05, false)]
    public void GetHashCode_HashEquality_MatchesBucketGrouping(double a, double b, double epsilon, bool expectSameHash) {
      var comparer = new DoubleComparer(epsilon);

      int hashA = comparer.GetHashCode(a);
      int hashB = comparer.GetHashCode(b);

      if (expectSameHash) {
        Assert.Equal(hashA, hashB);
      } else {
        Assert.NotEqual(hashA, hashB);
      }
    }
  }
}
