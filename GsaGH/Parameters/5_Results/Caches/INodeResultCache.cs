using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public interface INodeResultCache<T> where T : IResultQuantitySet {
    public IApiResult ApiResult { get; set; }
    public IDictionary<string, IResultSubset<T>> Cache { get;}
    public IResultSubset<T> ResultSubset(string list);
  }
}