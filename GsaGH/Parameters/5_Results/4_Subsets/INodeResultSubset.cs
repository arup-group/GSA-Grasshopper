using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public interface INodeResultSubset<T> where T : IResultItem {
    IResultExtrema Max { get; }
    IResultExtrema Min { get; }
    IList<int> Ids { get; }
    /// <summary>
    /// <para> Key = Node Id
    /// </para>
    /// Value = Collection of results, one for each permutation. Collection will have 1 item in case of AnalysisCase
    /// </summary>
    ConcurrentDictionary<int, Collection<T>> Subset { get; }
  }
}
