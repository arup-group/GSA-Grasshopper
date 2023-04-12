using System;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Helpers.GsaApi {
  internal partial class ResultHelper {
    internal static double RoundToSignificantDigits(double d, int digits) {
      if (d == 0.0) {
        return 0.0;
      }

      double leftSideNumbers = Math.Floor(Math.Log10(Math.Abs(d))) + 1;
      double scale = Math.Pow(10, leftSideNumbers);
      double result = scale * Math.Round(d / scale, digits, MidpointRounding.AwayFromZero);

      if ((int)leftSideNumbers >= digits) {
        return Math.Round(result, 0, MidpointRounding.AwayFromZero);
      }

      return Math.Abs(digits - (int)leftSideNumbers) > 15
        ? 0.0
        : Math.Round(result, digits - (int)leftSideNumbers, MidpointRounding.AwayFromZero);
    }

    internal static List<double> SmartRounder(double max, double min) {
      var roundedvals = new List<double>();
      if (max == 0 & min == 0) {
        roundedvals.Add(max);
        roundedvals.Add(min);
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
        while (valString[digits] == '0')
          digits++;
        factor = Math.Pow(10, digits + 1);
        max *= factor;
        min *= factor;
        max = (signMax > 0) ? Math.Ceiling(max) : Math.Floor(max);
        min = (signMin > 0) ? Math.Floor(min) : Math.Ceiling(min);
        max /= factor;
        min /= factor;
        numberOfDigitsOut = digits + significantNumbers;
      }
      else {
        string valString = val.ToString();
        int digits = valString.Split('.')[0].Count();
        int power = 10;
        if (val < 500)
          power = 5;
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
  }
}
