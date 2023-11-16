using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public interface IResultCache1d<T1, T2, T3> where T1 : IQuantity1d<T2> where T2 : IResultItem {
    IApiResult ApiResult { get; }
    ConcurrentDictionary<int, Collection<T1>> Cache { get; }
    IResultSubset1d<T1, T2, T3> ResultSubset(ICollection<int> elementIds, int positionCount);

    IResultSubset1d<T1, T2, T3> ResultSubset(
      ICollection<int> elementIds, ReadOnlyCollection<double> positions);
  }
}
