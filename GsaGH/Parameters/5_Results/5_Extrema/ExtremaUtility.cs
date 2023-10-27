using OasysUnits;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public static class ExtremaUtility {
    internal static (int Max, int Min) PermExtrema<T>(Collection<T> collection, Func<T, IQuantity> sortFunction)
    {
      IOrderedEnumerable<T> sorted = collection.OrderBy(sortFunction);
      int max = collection.IndexOf(sorted.Last());
      int min = collection.IndexOf(sorted.First());
      return (max, min);
    }
  }
}
