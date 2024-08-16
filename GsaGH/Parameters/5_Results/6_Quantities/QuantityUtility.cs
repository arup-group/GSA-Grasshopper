using System;
using System.Collections.Generic;

using OasysUnits;

namespace GsaGH.Parameters.Results {
  public static class QuantityUtility {
    internal static T PythagoreanQuadruple<T>(T x, T y, T z) where T : IQuantity {
      double a = x.Value;
      double b = y.Value;
      double c = z.Value;
      double pythagoras = Pythagoras(new List<double> { a, b, c });
      return (T)Quantity.From(pythagoras, x.Unit);
    }

    internal static T PythagoreanTriple<T>(T x, T y) where T : IQuantity {
      double a = x.Value;
      double b = y.Value;
      double pythagoras = Pythagoras(new List<double> { a, b });
      return (T)Quantity.From(pythagoras, x.Unit);
    }

    private static double Pythagoras(List<double> doubles) {
      double sum = 0;
      foreach (double val in doubles) {
        sum += val * val;
      }

      return Math.Sqrt(sum);
    }
  }
}
