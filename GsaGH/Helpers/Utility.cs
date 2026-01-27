
using System;

namespace GsaGH.Helpers {
  public static class Utility {
    public static bool IsInRange(double value, double min, double max) {
      float epsilon = 10e-12f;
      return value <= max + epsilon && value >= min - epsilon;
    }

    public static bool IsApproxEqual(double value1, double value2) {
      float epsilon = 10e-12f;
      return Math.Abs(value1 - value2) < epsilon;
    }
  }
}
