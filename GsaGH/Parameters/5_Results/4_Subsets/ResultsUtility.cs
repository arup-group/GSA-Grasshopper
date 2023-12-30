using GsaAPI;
using OasysUnits;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters.Results {
  internal static partial class ResultsUtility {
    internal static (ConcurrentDictionary<int, IList<IQuantity>> values, ICollection<int> nodeIds)
      MapNodeResultToElements<T1, T2>(
      ReadOnlyDictionary<int, Element> elements, INodeResultCache<T1, T2> nodeResultsCache,
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
          list.Add(list.Average(list[0].Unit));
        }

        vals.TryAdd(kvp.Key, list);
      });

      return (vals, topology.Distinct().ToList());
    }

    internal static ConcurrentDictionary<int, IQuantity> GetResultComponent<T>(
      IDictionary<int, IList<T>> subset, Func<T, IQuantity> selector, EnvelopeMethod envelopeType)
      where T : IResultItem {
      var vals = new ConcurrentDictionary<int, IQuantity>();
      if (subset.Values.FirstOrDefault().Count < 2) {
        Parallel.ForEach(subset, kvp =>
          vals.TryAdd(kvp.Key, kvp.Value.Select(selector).FirstOrDefault()));
        return vals;
      }

      Parallel.ForEach(subset, kvp => {
        IList<IQuantity> values = kvp.Value.Select(selector).ToList();
        vals.TryAdd(kvp.Key, values.Envelope(envelopeType));
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

    internal static ConcurrentDictionary<int, (double x, double y, double z)> GetResultResultantTranslation(
      IDictionary<int, IList<IDisplacement>> subset, LengthUnit unit, EnvelopeMethod envelopeType) {
      var vals = new ConcurrentDictionary<int, (double x, double y, double z)>();
      Parallel.ForEach(subset, kvp => {
        var xyzs = kvp.Value.Select(r => r.Xyz.As(unit)).ToList();
        (double x, double y, double z) = GetXyz(kvp.Value, unit, 0);
        double xyz = xyzs[0];
        for (int permutation = 1; permutation < kvp.Value.Count; permutation++) {
          switch (envelopeType) {
            case EnvelopeMethod.Maximum:
            case EnvelopeMethod.SignedAbsolute:
            case EnvelopeMethod.Absolute:
              if (xyzs[permutation] > xyz) {
                (x, y, z) = GetXyz(kvp.Value, unit, permutation);
                xyz = xyzs[permutation];
              }

              break;

            case EnvelopeMethod.Minimum:
              if (xyzs[permutation] < xyz) {
                (x, y, z) = GetXyz(kvp.Value, unit, permutation);
                xyz = xyzs[permutation];
              }

              break;
          }
        }
        vals.TryAdd(kvp.Key, (x, y, z));
      });
      return vals;
    }
  }
}
