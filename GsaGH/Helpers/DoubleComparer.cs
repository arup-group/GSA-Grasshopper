using System;
using System.Collections.Generic;

namespace GsaGH.Helpers {
  public class DoubleComparer : IEqualityComparer<double>, IEqualityComparer<double?> {
    public static readonly DoubleComparer Default = new DoubleComparer();

    private readonly double _epsilon;
    private readonly bool _margin;
    private readonly int _roundOff = 6;
    public DoubleComparer(double epsilon = 0.01, bool useEpsilonAsMargin = false) {
      _epsilon = epsilon;
      _margin = useEpsilonAsMargin;
    }

    public bool Equals(double? x, double? y) {
      if (!x.HasValue && !y.HasValue) {
        return true;
      }
      if (!x.HasValue || !y.HasValue) {
        return false;
      }
      return Equals(x.Value, y.Value);
    }

    public bool Equals(double x, double y) {
      x = Math.Round(x, _roundOff);
      y = Math.Round(y, _roundOff);

      if (x.Equals(y)) {
        return true;
      }

      if (_margin) {
        if (Math.Abs(x - y) < _epsilon) {
          return true;
        }
      } else {
        double error = Math.Abs((x - y) / (x + y) * 0.5);
        return error < _epsilon;
      }

      return false;
    }


    public int GetHashCode(double? value) {
      if (!value.HasValue) {
        return 0;
      }
      return GetHashCode(value.Value);
    }
    public int GetHashCode(double value) {
      // Group values into buckets of size `_epsilon`
      double normalized = Math.Round(value / _epsilon) * _epsilon;

      // Convert to an integer hash
      return normalized.GetHashCode();
    }
  }
}
