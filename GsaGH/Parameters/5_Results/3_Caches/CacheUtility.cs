using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using GsaGH.Helpers;

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

    internal static ConcurrentBag<int> GetMissingKeysAndPositions<T>(
      this IDictionary<int, IList<IEntity1dQuantity<T>>> existing, ICollection<int> newKeys, ReadOnlyCollection<double> positions)
      where T : IResultItem {
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

    internal static IDictionary<int, IList<IEntity1dQuantity<T>>> GetSubset<T>(
      this IDictionary<int, IList<IEntity1dQuantity<T>>> dictionary, ICollection<int> keys, ICollection<double> positions)
      where T : IResultItem {
      if (dictionary.IsNullOrEmpty() || dictionary.Values.IsNullOrEmpty()) {
        return dictionary;
      }

      IList<double> differences =
        dictionary.Values.FirstOrDefault().FirstOrDefault().Results.Keys.Except(positions).ToList();
      if (differences.IsNullOrEmpty()) {
        return GetSubset(dictionary, keys);
      }

      var subset = new ConcurrentDictionary<int, IList<IEntity1dQuantity<T>>>();
      Parallel.ForEach(keys, key => {
        if (dictionary.ContainsKey(key)) {
          var results = dictionary[key].ToList();
          for (int i = 0; i < results.Count; i++) {
            foreach (double position in differences) {
              results[i] = results[i].TakePositions(positions);
            }
          }
          subset.TryAdd(key, results);
        }
      });
      return subset;
    }
  }
}
