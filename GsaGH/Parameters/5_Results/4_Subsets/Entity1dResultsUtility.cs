using OasysUnits;
using OasysUnits.Units;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GsaGH.Parameters.Results {
  internal static partial class ResultsUtility {
    internal static ConcurrentDictionary<int, IList<IQuantity>> GetResultComponent<T1, T2>(
      IDictionary<int, IList<T1>> subset, Func<T2, IQuantity> selector, int permutation = 0)
      where T1 : IEntity1dQuantity<T2> where T2 : IResultItem {
      var vals = new ConcurrentDictionary<int, IList<IQuantity>>();
      Parallel.ForEach(subset, kvp =>
        vals.TryAdd(kvp.Key, kvp.Value[permutation].Results.Values.Select(selector).ToList()));
      return vals;
    }

    internal static ConcurrentDictionary<int, (IList<double> x, IList<double> y, IList<double> z)> GetResultResultanTranslation(
      IDictionary<int, IList<IEntity1dDisplacement>> subset, LengthUnit unit, int permutation = 0) {
      var vals = new ConcurrentDictionary<int, (IList<double> x, IList<double> y, IList<double> z)>();
      Parallel.ForEach(subset, kvp =>
        vals.TryAdd(kvp.Key, (
          kvp.Value[permutation].Results.Values.Select(r => r.X.As(unit)).ToList(),
          kvp.Value[permutation].Results.Values.Select(r => r.Y.As(unit)).ToList(),
          kvp.Value[permutation].Results.Values.Select(r => r.Z.As(unit)).ToList()
        )));
      return vals;
    }
  }
}
