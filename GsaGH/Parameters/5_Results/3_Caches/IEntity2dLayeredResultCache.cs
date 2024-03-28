using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public interface IEntity2dLayeredResultCache<T1, T2, T3>
    where T1 : IMeshQuantity<T2> where T2 : IResultItem {
    IApiResult ApiResult { get; }
    IDictionary<int, IList<T1>> CacheBottomLayer { get; }
    IDictionary<int, IList<T1>> CacheMiddleLayer { get; }
    IDictionary<int, IList<T1>> CacheTopLayer { get; }
    IMeshResultSubset<T1, T2, T3> ResultSubset(ICollection<int> elementIds, Layer2d layer);
    void SetStandardAxis(int axisId);
  }
}
