using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public interface INodeResultSubset<T> where T : IResultQuantitySet {
    public IResultExtrema Max { get; }
    public IResultExtrema Min { get; }
    public IList<int> Ids { get; }
    /// <summary>
    /// <para> Key = Node Id
    /// </para>
    /// Value = Collection of results, one for each permutation. Collection will have 1 item in case of AnalysisCase
    /// </summary>
    public ConcurrentDictionary<int, Collection<T>> Subset { get; }
  }
}
