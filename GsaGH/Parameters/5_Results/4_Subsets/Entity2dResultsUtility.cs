using OasysUnits;
using OasysUnits.Units;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GsaGH.Parameters.Results {
  internal static partial class ResultsUtility {
    internal static ConcurrentDictionary<int, IList<IQuantity>> GetResultComponent<T>(
      IDictionary<int, IList<IMeshQuantity<T>>> subset, Func<T, IQuantity> selector, int permutation = 0)
      where T : IResultItem {
      var vals = new ConcurrentDictionary<int, IList<IQuantity>>();
      Parallel.ForEach(subset, kvp =>
        vals.TryAdd(kvp.Key, kvp.Value[permutation].Results().Select(selector).ToList()));
      return vals;
    }

    internal static ConcurrentDictionary<int, (IList<double> x, IList<double> y, IList<double> z)> GetResultResultanTranslation(
      IDictionary<int, IList<IMeshQuantity<ITranslation>>> subset, LengthUnit unit, int permutation = 0) {
      var vals = new ConcurrentDictionary<int, (IList<double> x, IList<double> y, IList<double> z)>();
      Parallel.ForEach(subset, kvp =>
        vals.TryAdd(kvp.Key, (
          kvp.Value[permutation].Results().Select(r => r.X.As(unit)).ToList(),
          kvp.Value[permutation].Results().Select(r => r.Y.As(unit)).ToList(),
          kvp.Value[permutation].Results().Select(r => r.Z.As(unit)).ToList()
        )));
      return vals;
    }

    internal static ConcurrentDictionary<int, (IList<double> x, IList<double> y, IList<double> z)> GetResultResultanTranslation(
      IDictionary<int, IList<IMeshQuantity<IDisplacement>>> subset, LengthUnit unit, int permutation = 0) {
      var vals = new ConcurrentDictionary<int, (IList<double> x, IList<double> y, IList<double> z)>();
      Parallel.ForEach(subset, kvp =>
        vals.TryAdd(kvp.Key, (
          kvp.Value[permutation].Results().Select(r => r.X.As(unit)).ToList(),
          kvp.Value[permutation].Results().Select(r => r.Y.As(unit)).ToList(),
          kvp.Value[permutation].Results().Select(r => r.Z.As(unit)).ToList()
        )));
      return vals;
    }
  }
}
