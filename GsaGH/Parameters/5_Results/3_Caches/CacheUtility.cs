using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GsaGH.Parameters.Results {
  internal static class CacheUtility {
    internal static ConcurrentBag<int> GetMissingKeys<T>(
      this IDictionary<int, T> existing, ICollection<int> newKeys) {
      var missingIds = new ConcurrentBag<int>();
      Parallel.ForEach(newKeys, key => {
        if (!existing.ContainsKey(key)) {
          missingIds.Add(key);
        }
      });

      return missingIds;
    }

    internal static ConcurrentBag<int> GetMissingKeysAndPositions<T1, T2>(
      this IDictionary<int, IList<T1>> existing, ICollection<int> newKeys, ReadOnlyCollection<double> positions)
      where T1 : IEntity1dQuantity<T2> where T2 : IResultItem {
      var missingIds = new ConcurrentBag<int>();
      Parallel.ForEach(newKeys, key => {
        if (!existing.ContainsKey(key)) {
          missingIds.Add(key);
        } else {
          foreach (double position in positions) {
            if (!existing[key][0].Results.ContainsKey(position)) {
              missingIds.Add(key);
              return;
            }
          }
        }
      });

      return missingIds;
    }

    internal static IDictionary<int, T> GetSubset<T>(this IDictionary<int, T> dictionary,
      ICollection<int> keys) {
      var subset = new ConcurrentDictionary<int, T>();
      Parallel.ForEach(keys, key => {
        if (dictionary.ContainsKey(key)) {
          subset.TryAdd(key, dictionary[key]);
        }
      });
      return subset;
    }

    internal static IDictionary<int, IList<T1>> GetSubset<T1, T2>(
      this IDictionary<int, IList<T1>> dictionary, ICollection<int> keys, ICollection<double> positions) 
      where T1 : IEntity1dQuantity<T2> where T2 : IResultItem {
      var subset = new ConcurrentDictionary<int, IList<T1>>();
      
      IList<double> differences =
        dictionary.Values.FirstOrDefault().FirstOrDefault().Results.Keys.Except(positions).ToList();
      Parallel.ForEach(keys, key => {
        if (dictionary.ContainsKey(key)) {
          foreach (T1 item in dictionary[key]) {
            foreach (double position in differences) {
              item.Results.Remove(position);
            }
          }
          subset.TryAdd(key, dictionary[key]);
        }
      });
      return subset;
    }
  }
}
