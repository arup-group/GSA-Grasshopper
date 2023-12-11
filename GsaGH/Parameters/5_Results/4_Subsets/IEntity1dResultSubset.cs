using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public interface IEntity1dResultSubset<T1, T2, T3>
    where T1 : IEntity1dQuantity<T2>
    where T2 : IResultItem {
    T3 Max { get; }
    T3 Min { get; }
    IList<int> Ids { get; }
    /// <summary>
    /// <para> Key = Node Id
    /// </para>
    /// Value = Collection of results, one for each permutation. Collection will have 1 item in case of AnalysisCase
    /// </summary>
    IDictionary<int, IList<T1>> Subset { get; }
    T2 GetExtrema(IEntity1dExtremaKey key);
  }
}
