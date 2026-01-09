using GsaGH.Helpers;

using Xunit;

namespace GsaGHTests.Helpers {
  public class DoubleComparerTests {
    [Theory]
    [InlineData(1.0000001, 1.0000002, 0.0001, true)]
    [InlineData(1.0, 1.1, 0.02, false)]
    [InlineData(0, 0, 0.01, true)]
    [InlineData(0, 0.005, 0.6, true)]
    [InlineData(0, 0.02, 0.01, false)]
    public void Equals_RelativeErrorBehavior(double a, double b, double epsilon, bool expected) {
      //relative difference is calculated as (x - y) / ((x + y) / 2)
      var comparer = new DoubleComparer(epsilon);
      Assert.Equal(expected, comparer.Equals(a, b));
    }

    [Fact]
    public void ValuesDifferByVeryLittleAndEpsilonIsZero() {
      Assert.NotEqual(10.0, 10.000001, new DoubleComparer(0, true));
    }

    [Fact]
    public void ValuesDifferLessThanEpsilonShouldBeConsideredEqualNoMargin() {
      Assert.Equal(10.0, 10.01, DoubleComparer.Default);
    }

    [Fact]
    public void ValuesDifferLessThanEpsilonShouldBeConsideredEqualWithMargin() {
      Assert.Equal(10.0, 10.001, new DoubleComparer(0.1, true));
    }

    [Fact]
    public void ValuesLargerThanEpsilonShouldBeConsideredEqual() {
      double epsilon = 1f;
      Assert.NotEqual(10.0, 10.0 + (epsilon * 2), new DoubleComparer(epsilon, true));
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
