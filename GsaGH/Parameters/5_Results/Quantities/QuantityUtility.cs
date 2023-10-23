using OasysUnits;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public static class QuantityUtility {
    public static IDisplacement GetMax(this ICollection<ICollection<IDisplacement>> c) {
      return new GsaDisplacementQuantity(
        c.Select(c => c.Select(p => p.X).Max()).Max(),
        c.Select(c => c.Select(p => p.Y).Max()).Max(),
        c.Select(c => c.Select(p => p.Z).Max()).Max(),
        c.Select(c => c.Select(p => p.Xyz).Max()).Max(),
        c.Select(c => c.Select(p => p.Xx).Max()).Max(),
        c.Select(c => c.Select(p => p.Yy).Max()).Max(),
        c.Select(c => c.Select(p => p.Zz).Max()).Max(),
        c.Select(c => c.Select(p => p.Xxyyzz).Max()).Max());
    }

    public static IDisplacement GetMin(this ICollection<ICollection<IDisplacement>> c) {
      return new GsaDisplacementQuantity(
        c.Select(c => c.Select(p => p.X).Min()).Min(),
        c.Select(c => c.Select(p => p.Y).Min()).Min(),
        c.Select(c => c.Select(p => p.Z).Min()).Min(),
        c.Select(c => c.Select(p => p.Xyz).Min()).Min(),
        c.Select(c => c.Select(p => p.Xx).Min()).Min(),
        c.Select(c => c.Select(p => p.Yy).Min()).Min(),
        c.Select(c => c.Select(p => p.Zz).Min()).Min(),
        c.Select(c => c.Select(p => p.Xxyyzz).Min()).Min());
    }

    internal static T PythagoreanQuadruple<T>(T x, T y, T z) where T : IQuantity {
      double a = x.Value;
      double b = y.Value;
      double c = z.Value;
      double pythagoras = Pythagoras(new List<double> { a, b, c });
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
