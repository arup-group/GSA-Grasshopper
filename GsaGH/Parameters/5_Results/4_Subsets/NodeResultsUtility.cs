using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using GsaAPI;

using GsaGH.Helpers;

using OasysUnits;
using OasysUnits.Units;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters.Results {
  internal static partial class ResultsUtility {
    internal static (ConcurrentDictionary<int, IList<IQuantity>> values, ICollection<int> nodeIds)
      MapNodeResultToElements<T1, T2>(
      ReadOnlyDictionary<int, Element> elements, IEntity0dResultCache<T1, T2> nodeResultsCache,
      Func<T1, IQuantity> selector, EnvelopeMethod envelopeType) where T1 : IResultItem {
      var vals = new ConcurrentDictionary<int, IList<IQuantity>>();
      var topology = new ConcurrentBag<int>();
      Parallel.ForEach(elements, kvp => {
        var list = new List<IQuantity>();
        IDictionary<int, IList<T1>> subset = nodeResultsCache.ResultSubset(kvp.Value.Topology).Subset;
        if (subset.Count == 0) {
          return;
        }

        foreach (int nodeId in kvp.Value.Topology) {
          list.Add(subset[nodeId].Select(selector).Envelope(envelopeType));
          topology.Add(nodeId);
        }

        if (kvp.Value.Topology.Count > 2) {
          list.Add(Quantity.From(list.Average(x => x.As(list[0].Unit)), list[0].Unit));
        }

        vals.TryAdd(kvp.Key, list);
      });

      return (vals, topology.Distinct().ToList());
    }

    internal static ConcurrentDictionary<int, IQuantity> GetResultComponent<T>(
      IDictionary<int, IList<T>> subset, Func<T, IQuantity> selector, List<int> permutations,
      EnvelopeMethod envelopeType)
      where T : IResultItem {
      var vals = new ConcurrentDictionary<int, IQuantity>();
      if (permutations.IsNullOrEmpty()) {
        permutations = new List<int>() {
          1
        };
      }

      if (permutations.Count == 1) {
        Parallel.ForEach(subset, kvp =>
          vals.TryAdd(kvp.Key, kvp.Value.Select(selector).ElementAt(permutations[0] - 1)));
        return vals;
      }

      Parallel.ForEach(subset, kvp => {
        try {
          IList<IQuantity> values = permutations.Select(index =>
            kvp.Value.Select(selector).ToList()[index - 1]).ToList();
          vals.TryAdd(kvp.Key, values.Envelope(envelopeType));
        } catch (NullReferenceException) {
          throw new ArgumentException("Result does not contain values for selected component/direction");
        }
      });
      return vals;
    }

    internal static IQuantity Envelope<T>(this IEnumerable<T> subset, EnvelopeMethod envelopeType)
      where T : IQuantity {
      IQuantity val = subset.FirstOrDefault();
      Enum unit = val.Unit;
      switch (envelopeType) {
        case EnvelopeMethod.Maximum:
          for (int i = 1; i < subset.Count(); i++) {
            if (subset.ElementAt(i).As(unit) > val.As(unit)) {
              val = subset.ElementAt(i);
            }
          }
          break;

        case EnvelopeMethod.Minimum:
          for (int i = 1; i < subset.Count(); i++) {
            if (subset.ElementAt(i).As(unit) < val.As(unit)) {
              val = subset.ElementAt(i);
            }
          }

          break;

        case EnvelopeMethod.Absolute:
          val = val.Abs();
          for (int i = 1; i < subset.Count(); i++) {
            if (Math.Abs(subset.ElementAt(i).As(unit)) > val.As(unit)) {
              val = subset.ElementAt(i).Abs();
            }
          }

          break;

        case EnvelopeMethod.SignedAbsolute:
          for (int i = 1; i < subset.Count(); i++) {
            if (Math.Abs(subset.ElementAt(i).As(unit)) > Math.Abs(val.As(unit))) {
              val = subset.ElementAt(i);
            }
          }

          break;
      }

      return val;
    }

    internal static Ratio Envelope(this IEnumerable<Ratio?> subset, EnvelopeMethod envelopeType) {
      var val = new Ratio(0, RatioUnit.DecimalFraction);
      for (int i = 0; i < subset.Count(); i++) {
        if (subset.ElementAt(i).HasValue) {
          val = (Ratio)subset.ElementAt(i);
          break;
        }
      }

      RatioUnit unit = RatioUnit.DecimalFraction;
      switch (envelopeType) {
        case EnvelopeMethod.Maximum:
          for (int i = 1; i < subset.Count(); i++) {
            if (!subset.ElementAt(i).HasValue) {
              continue;
            }
            if (((Ratio)subset.ElementAt(i)).As(unit) > val.As(unit)) {
              val = (Ratio)subset.ElementAt(i);
            }
          }
          break;

        case EnvelopeMethod.Minimum:
          for (int i = 1; i < subset.Count(); i++) {
            if (!subset.ElementAt(i).HasValue) {
              continue;
            }
            if (((Ratio)subset.ElementAt(i)).As(unit) < val.As(unit)) {
              val = (Ratio)subset.ElementAt(i);
            }
          }

          break;

        case EnvelopeMethod.Absolute:
          val = val.Abs();
          for (int i = 1; i < subset.Count(); i++) {
            if (!subset.ElementAt(i).HasValue) {
              continue;
            }
            if (Math.Abs(((Ratio)subset.ElementAt(i)).As(unit)) > val.As(unit)) {
              val = ((Ratio)subset.ElementAt(i)).Abs();
            }
          }

          break;

        case EnvelopeMethod.SignedAbsolute:
          for (int i = 1; i < subset.Count(); i++) {
            if (!subset.ElementAt(i).HasValue) {
              continue;
            }
            if (Math.Abs(((Ratio)subset.ElementAt(i)).As(unit)) > Math.Abs(val.As(unit))) {
              val = (Ratio)subset.ElementAt(i);
            }
          }

          break;
      }

      return val;
    }

    internal static ConcurrentDictionary<int, (double x, double y, double z)> GetResultResultantTranslation(
      IDictionary<int, IList<IDisplacement>> subset, LengthUnit unit, List<int> permutations, EnvelopeMethod envelopeType) {
      var vals = new ConcurrentDictionary<int, (double x, double y, double z)>();
      if (permutations.IsNullOrEmpty()) {
        permutations = new List<int>() {
          1
        };
      }

      Parallel.ForEach(subset, kvp => {
        var res = kvp.Value.Select(r => r.Xyz.As(unit)).ToList();
        (double x, double y, double z) = GetXyz(kvp.Value, unit, permutations[0] - 1);
        double xyz = res[permutations[0] - 1];
        foreach (int permutation in permutations.Skip(1)) {
          switch (envelopeType) {
            case EnvelopeMethod.Maximum:
            case EnvelopeMethod.SignedAbsolute:
            case EnvelopeMethod.Absolute:
              if (res[permutation - 1] > xyz) {
                (x, y, z) = GetXyz(kvp.Value, unit, permutation - 1);
                xyz = res[permutation - 1];
              } else if (res[permutation - 1] == xyz) {
                (double xt, double yt, double zt) = GetXyz(kvp.Value, unit, permutation - 1);
                if (xt + yt + zt > x + y + z) {
                  (x, y, z) = (xt, yt, zt);
                  xyz = res[permutation - 1];
                }
              }

              break;

            case EnvelopeMethod.Minimum:
              if (res[permutation - 1] < xyz) {
                (x, y, z) = GetXyz(kvp.Value, unit, permutation - 1);
                xyz = res[permutation - 1];
              } else if (res[permutation - 1] == xyz) {
                (double xt, double yt, double zt) = GetXyz(kvp.Value, unit, permutation - 1);
                if (xt + yt + zt < x + y + z) {
                  (x, y, z) = (xt, yt, zt);
                  xyz = res[permutation - 1];
                }
              }

              break;
          }
        }
        vals.TryAdd(kvp.Key, (x, y, z));
      });
      return vals;
    }

    private static (double x, double y, double z) GetXyz<T>(IList<T> values, LengthUnit unit, int id)
      where T : IResultVector3InAxis<Length> {
      return (values.Select(r => r.X.As(unit)).ElementAt(id),
              values.Select(r => r.Y.As(unit)).ElementAt(id),
              values.Select(r => r.Z.As(unit)).ElementAt(id));
    }
  }
}
