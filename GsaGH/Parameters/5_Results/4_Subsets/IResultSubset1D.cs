using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public interface IResultSubset1d<T1, T2, T3> where T1 : IQuantity1d<T2> where T2 : IResultItem {
    T3 Max { get; }
    T3 Min { get; }
    IList<int> Ids { get; }
    /// <summary>
    ///   <para>
    ///     Key = Node Id
    ///   </para>
    ///   Value = Collection of results, one for each permutation. Collection will have 1 item in case of AnalysisCase
    /// </summary>
    ConcurrentDictionary<int, Collection<T1>> Subset { get; }
    T2 GetExtrema(ExtremaKey1d key);
  }
}
