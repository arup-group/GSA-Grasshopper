using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public interface INodeResultCache<T> where T : IResultItem {
    IApiResult ApiResult { get; }
    ConcurrentDictionary<int, Collection<T>> Cache { get; }
    INodeResultSubset<T> ResultSubset(ICollection<int> list);
  }
}