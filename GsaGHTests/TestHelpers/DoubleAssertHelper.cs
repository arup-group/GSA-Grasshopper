using GsaGH.Helpers;

using Xunit;

namespace GsaGHTests.TestHelpers {
  /// <summary>
  ///   Provides helper methods to assert equality of double values
  ///   with optional precision level, useful in unit tests.
  /// </summary>
  public static class DoubleAssertHelper {
    private static readonly DoubleComparer _doubleComparer = DoubleComparer.Default;

    /// <summary>
    ///   Asserts that two double values are equal using the default precision level.
    ///   Throws an assertion failure if they are not equal.
    /// </summary>
    /// <param name="expected">The expected double value.</param>
    /// <param name="actual">The actual double value.</param>
    public static void Equals(double expected, double actual) {
      Assert.True(_doubleComparer.Equals(expected, actual),
        $"Values are not the same. Expected: {expected}, actual: {actual}");
    }


  }
}
