using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public interface IEntity0dResultSubset<T1, T2> where T1 : IResultItem {
    T2 Max { get; }
    T2 Min { get; }
    IList<int> Ids { get; }
    /// <summary>
    /// <para> Key = Node Id
    /// </para>
    /// Value = Collection of results, one for each permutation. Collection will have 1 item in case of AnalysisCase
    /// </summary>
    IDictionary<int, IList<T1>> Subset { get; }
    T1 GetExtrema(IExtremaKey key);
  }
}
