using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public interface INodeResultCache<T1, T2> where T1 : IResultItem {
    IApiResult ApiResult { get; }
    ConcurrentDictionary<int, Collection<T1>> Cache { get; }
    INodeResultSubset<T1, T2> ResultSubset(ICollection<int> list);
  }
}