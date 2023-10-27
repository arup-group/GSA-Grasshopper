using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public interface IBeamResultCache<T1, T2> where T1 : IResultItem  {
    IApiResult ApiResult { get; }
    ConcurrentDictionary<int, Collection<T1>> Cache { get; }
    IBeamResultSubset<T1, T2> ResultSubset(ICollection<int> elementIds, int positionCount);
    IBeamResultSubset<T1, T2> ResultSubset(ICollection<int> elementIds, ReadOnlyCollection<double> positions);
  }
}
