using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GsaGH.Parameters.Results {
  public static class CacheUtility {
    public static ConcurrentBag<int> GetMissingKeys<T>(
      this IDictionary<int, T> existing, ICollection<int> newKeys) {
      var missingIds = new ConcurrentBag<int>();
      Parallel.ForEach(newKeys, key => {
        if (!existing.ContainsKey(key)) {
          missingIds.Add(key);
        }
      });

      return missingIds;
    }

    public static ConcurrentDictionary<int, T> GetSubset<T>(this IDictionary<int, T> dictionary, 
      ICollection<int> keys) {
      var subset = new ConcurrentDictionary<int, T>();
      Parallel.ForEach(keys, key => subset.TryAdd(key, dictionary[key]));
      return subset;
    }
  }
}
