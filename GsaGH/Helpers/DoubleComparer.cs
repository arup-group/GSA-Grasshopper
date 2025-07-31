using System;
using System.Collections.Generic;

namespace GsaGH.Helpers {
  public class DoubleComparer : IEqualityComparer<double> {
    private readonly double _epsilon;
    private readonly bool _margin;

    public DoubleComparer(double epsilon = 0.01, bool useEpsilonAsMargin = false) {
      _epsilon = epsilon;
      _margin = useEpsilonAsMargin;
    }

    /// <summary>
    ///   Determines whether two double-precision floating-point numbers are equal
    ///   using relative error and epsilon.
    /// </summary>
    public bool Equals(double x, double y) {
      if (x.Equals(y)) {
        return true;
      }

      const double zeroTolerance = 1e-15;
      if (Math.Abs(x) < zeroTolerance && Math.Abs(y) < zeroTolerance) {
        return true;
      }

      if (Math.Abs(x) < zeroTolerance || Math.Abs(y) < zeroTolerance) {
        return Math.Abs(x - y) < _epsilon;
      }

      double relativeError = Math.Abs((x - y) / ((Math.Abs(x) + Math.Abs(y)) / 2));
      return relativeError < _epsilon;
    }

    /// <summary>
    ///   Determines whether two doubles are equal up to specified precision level,
    ///   with optional margin (epsilon as absolute margin).
    /// </summary>
    public bool IsEqualsAtPrecisionLevel(double x, double y, int precisionLevel) {
      x = Math.Round(x, precisionLevel, MidpointRounding.AwayFromZero);
      y = Math.Round(y, precisionLevel, MidpointRounding.AwayFromZero);

      return _margin ? Math.Abs(x - y) < _epsilon : x.Equals(y);
    }

    public int GetHashCode(double value) {
      // Group values into buckets of size `_epsilon`
      double normalized = Math.Round(value / _epsilon) * _epsilon;
      return normalized.GetHashCode();
    }
  }
}
