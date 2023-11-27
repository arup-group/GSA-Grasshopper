using OasysUnits;
using OasysUnits.Units;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GsaGH.Parameters.Results {
  public static partial class ResultsUtility {
    public static ConcurrentDictionary<int, List<IQuantity>> GetResultComponent<T>(
      ConcurrentDictionary<int, Collection<IEntity2dQuantity<T>>> subset, Func<T, IQuantity> selector, int permutation = 0)
      where T : IResultItem {
      var vals = new ConcurrentDictionary<int, List<IQuantity>>();
      Parallel.ForEach(subset, kvp =>
        vals.TryAdd(kvp.Key, kvp.Value[permutation].Results().Select(selector).ToList()));
      return vals;
    }

    public static ConcurrentDictionary<int, (List<double> x, List<double> y, List<double> z)> GetResultResultanTranslation(
      ConcurrentDictionary<int, Collection<IEntity2dQuantity<ITranslation>>> subset, LengthUnit unit, int permutation = 0) {
      var vals = new ConcurrentDictionary<int, (List<double> x, List<double> y, List<double> z)>();
      Parallel.ForEach(subset, kvp =>
        vals.TryAdd(kvp.Key, (
          kvp.Value[permutation].Results().Select(r => r.X.As(unit)).ToList(),
          kvp.Value[permutation].Results().Select(r => r.Y.As(unit)).ToList(),
          kvp.Value[permutation].Results().Select(r => r.Z.As(unit)).ToList()
        )));
      return vals;
    }

    public static ConcurrentDictionary<int, (List<double> x, List<double> y, List<double> z)> GetResultResultanTranslation(
      ConcurrentDictionary<int, Collection<IEntity2dQuantity<IDisplacement>>> subset, LengthUnit unit, int permutation = 0) {
      var vals = new ConcurrentDictionary<int, (List<double> x, List<double> y, List<double> z)>();
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
