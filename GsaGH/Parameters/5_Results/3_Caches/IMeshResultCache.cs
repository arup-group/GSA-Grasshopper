using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public interface IMeshResultCache<T1, T2, T3>
    where T1 : IMeshQuantity<T2> where T2 : IResultItem {
    IApiResult ApiResult { get; }
    IDictionary<int, IList<T1>> Cache { get; }
    IMeshResultSubset<T1, T2, T3> ResultSubset(ICollection<int> elementIds);
    void SetStandardAxis(int axisId);
  }
}
