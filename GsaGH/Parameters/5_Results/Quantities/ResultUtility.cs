using GsaGH.Helpers;
using OasysUnits;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters.Results {
  internal static class ResultUtility {
    internal static T PythagoreanQuadruple<T>(T x, T y, T z, Enum unit) where T : IQuantity {
      double a = x.As(unit);
      double b = y.As(unit);
      double c = z.As(unit);
      double pyth = Math.Sqrt((a * a) + (b * b) + (c * c));
      return (T)Quantity.From(pyth, unit);
    }

    internal static T Max<T>(this ICollection<T> collection, Enum unit) where T : IQuantity {
      if (collection.IsNullOrEmpty()) {
        return (T)Quantity.From(0, unit);
      }

      return (T)Quantity.From(collection.Select(x => x.As(unit)).Max(), unit);
    }

    internal static T Min<T>(this ICollection<T> collection, Enum unit) where T : IQuantity {
      if (collection.IsNullOrEmpty()) {
        return (T)Quantity.From(0, unit);
      }

      return (T)Quantity.From(collection.Select(x => x.As(unit)).Min(), unit);
    }
  }
}
