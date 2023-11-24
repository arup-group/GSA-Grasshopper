using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public interface IEntity2dLayeredResultCache<T1, T2, T3>
    where T1 : IEntity2dQuantity<T2> where T2 : IResultItem  {
    IApiResult ApiResult { get; }
    ConcurrentDictionary<int, Collection<T1>> CacheBottomLayer { get; }
    ConcurrentDictionary<int, Collection<T1>> CacheMiddleLayer { get; }
    ConcurrentDictionary<int, Collection<T1>> CacheTopLayer { get; }
    IEntity2dResultSubset<T1, T2, T3> ResultSubset(ICollection<int> elementIds, Layer2d layer);
  }
}
