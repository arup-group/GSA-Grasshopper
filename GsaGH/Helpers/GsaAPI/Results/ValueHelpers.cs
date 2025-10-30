using System;
using System.Collections.Generic;
using System.Linq;

using OasysUnits;

namespace GsaGH.Helpers.GsaApi {
  internal static partial class ResultHelper {

    internal static bool IsNaNOrInfinity(double d) {
      return double.IsInfinity(d) || double.IsNaN(d);
    }

    internal static bool IsNaNOrInfinity(Force force) {
      return double.IsInfinity(force.Value) || double.IsNaN(force.Value);
    }

    internal static bool IsNaNOrInfinity(Moment moment) {
      return double.IsInfinity(moment.Value) || double.IsNaN(moment.Value);
    }

    internal static bool IsNaNOrInfinity(Length length) {
      return double.IsInfinity(length.Value) || double.IsNaN(length.Value);
    }

    internal static bool IsNaNOrInfinity(Ratio ratio) {
      return double.IsInfinity(ratio.Value) || double.IsNaN(ratio.Value);
    }

    internal static double RoundToSignificantDigits(double d, int digits) {

      if (IsNaNOrInfinity(d)) {
        return d;
      }

      if (d == 0.0) {
        return 0.0;
      }

      double leftSideNumbers = Math.Floor(Math.Log10(Math.Abs(d))) + 1;
      double scale = Math.Pow(10, leftSideNumbers);
      double result = scale * Math.Round(d / scale, digits, MidpointRounding.AwayFromZero);

      if ((int)leftSideNumbers >= digits) {
        return Math.Round(result, 0, MidpointRounding.AwayFromZero);
      }

      return Math.Abs(digits - (int)leftSideNumbers) > 15 ? 0.0 : Math.Round(result,
        digits - (int)leftSideNumbers, MidpointRounding.AwayFromZero);
    }

    internal static List<double> SmartRounder(double max, double min) {
      var roundedvals = new List<double>();
      if (max == 0 && min == 0) {
        roundedvals.Add(0.000000000001);
        roundedvals.Add(-0.000000000001);
        roundedvals.Add(0);
        return roundedvals;
      }

      int signMax = Math.Sign(max);
      int signMin = Math.Sign(min);
      const int significantNumbers = 2;
      double val = Math.Max(Math.Abs(max), Math.Abs(min));
      max = Math.Abs(max);
      min = Math.Abs(min);

      int numberOfDigitsOut = significantNumbers;
      double factor = 1;
      if (val < 1) {
        string valString = val.ToString().Split('.')[1];
        int digits = 0;
        while (valString[digits] == '0') {
          digits++;
        }

        factor = Math.Pow(10, digits + 1);
        max *= factor;
        min *= factor;
        max = (signMax > 0) ? Math.Ceiling(max) : Math.Floor(max);
        min = (signMin > 0) ? Math.Floor(min) : Math.Ceiling(min);
        max /= factor;
        min /= factor;
        numberOfDigitsOut = digits + significantNumbers;
      } else {
        string valString = val.ToString();
        int digits = valString.Split('.')[0].Count();
        int power = 10;
        if (val < 500) {
          power = 5;
        }

        factor = Math.Pow(power, digits - 1);
        max /= factor;
        min /= factor;
        max = (signMax > 0) ? Math.Ceiling(max) : Math.Floor(max);
        min = (signMin > 0) ? Math.Floor(min) : Math.Ceiling(min);
        max *= factor;
        min *= factor;
        numberOfDigitsOut = significantNumbers;
      }

      roundedvals.Add(max * signMax);
      roundedvals.Add(min * signMin);
      roundedvals.Add(numberOfDigitsOut);

      return roundedvals;
    }

    public static double GetSafeExtrema<T>(T key, Func<T, double> getValue) where T : class {
      if (key == null) {
        return double.NaN;
      }
      try {
        return getValue(key);
      } catch (Exception) {
        return double.NaN;
      }
    }
  }
}
