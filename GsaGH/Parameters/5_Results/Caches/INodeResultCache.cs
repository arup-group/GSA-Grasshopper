using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public interface INodeResultCache<T> where T : IResultQuantitySet {
    public IApiResult ApiResult { get; set; }
    public ConcurrentDictionary<int, Collection<T>> Cache { get; }
    public IResultSubset<T> ResultSubset(ICollection<int> list);
  }
}