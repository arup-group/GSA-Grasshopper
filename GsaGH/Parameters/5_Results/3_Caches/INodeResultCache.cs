using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public interface INodeResultCache<T1, T2> where T1 : IResultItem {
    IApiResult ApiResult { get; }
    IDictionary<int, IList<T1>> Cache { get; }
    INodeResultSubset<T1, T2> ResultSubset(ICollection<int> list);
  }
}