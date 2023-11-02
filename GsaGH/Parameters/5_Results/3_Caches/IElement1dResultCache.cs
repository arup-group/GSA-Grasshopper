using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public interface IElement1dResultCache<T1, T2, T3>
    where T1 : IElement1dQuantity<T2> where T2 : IResultItem  {
    IApiResult ApiResult { get; }
    ConcurrentDictionary<int, Collection<T1>> Cache { get; }
    IElement1dResultSubset<T1, T2, T3> ResultSubset(ICollection<int> elementIds, int positionCount);
    IElement1dResultSubset<T1, T2, T3> ResultSubset(ICollection<int> elementIds, ReadOnlyCollection<double> positions);
  }
}
