using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public interface IEntity0dResultCache<T1, T2> where T1 : IResultItem {
    IApiResult ApiResult { get; }
    IDictionary<int, IList<T1>> Cache { get; }
    IEntity0dResultSubset<T1, T2> ResultSubset(ICollection<int> list);
    void SetStandardAxis(int axisId);
  }
}
