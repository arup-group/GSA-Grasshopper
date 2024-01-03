using GsaAPI;
using OasysUnits;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GsaGH.Parameters.Results {
  internal static partial class ResultsUtility {
    internal static (ConcurrentDictionary<int, IList<IQuantity>> values, ICollection<int> nodeIds)
      MapNodeResultToElements<T1, T2>(
      ReadOnlyDictionary<int, Element> elements, IEntity0dResultCache<T1, T2> nodeResultsCache, 
      Func<T1, IQuantity> selector, int permutation = 0) where T1 : IResultItem {
      var vals = new ConcurrentDictionary<int, IList<IQuantity>>();
      var topology = new ConcurrentBag<int>();
      Parallel.ForEach(elements, kvp => {
        var list = new List<IQuantity>();
        IDictionary<int, IList<T1>> subset = nodeResultsCache.ResultSubset(kvp.Value.Topology).Subset;
        if (subset.Count == 0) {
          return;
        }

        foreach (int nodeId in kvp.Value.Topology) {
          list.Add(subset[nodeId].Select(selector).ElementAt(permutation));
          topology.Add(nodeId);
        }

        if (kvp.Value.Topology.Count > 2) {
          list.Add(list.Average(list[0].Unit));
        }

        vals.TryAdd(kvp.Key, list);
      });

      return (vals, topology.Distinct().ToList());
    }
  }
}
