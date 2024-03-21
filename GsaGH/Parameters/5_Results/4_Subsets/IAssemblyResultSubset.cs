using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public interface IAssemblyResultSubset<T, U>
    where T : IResultItem {
    U Max { get; }
    U Min { get; }
    IList<int> Ids { get; }
    /// <summary>
    /// <para> Key = Entity Id
    /// </para>
    /// Value = Collection of results, one for each permutation. Collection will have 1 item in case of AnalysisCase
    /// </summary>
    IDictionary<int, IList<IAssemblyQuantity<T>>> Subset { get; }
    T GetExtrema(IEntity1dExtremaKey key);
  }
}
