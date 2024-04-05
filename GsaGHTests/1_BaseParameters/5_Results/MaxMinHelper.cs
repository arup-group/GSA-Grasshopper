using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsaGHTests.Parameters {
  public class MaxMinHelper {
    public static double? Max(List<double?> values) {
      double? max = null;

      foreach (double? value in values) {
        if (value == null) {
          continue;
        }

        if (max == null) {
          max = value;
        }

        if (value > max) {
          max = value;
        }
      }

      return max;
    }

    public static double? Min(List<double?> values) {
      double? min = null;

      foreach (double? value in values) {
        if (value == null) {
          continue;
        }

        if (min == null) {
          min = value;
        }

        if (value < min) {
          min = value;
        }
      }

      return min;
    }

  }
}
