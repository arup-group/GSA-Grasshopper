using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class GsaNodeDisplacements : IResultSubset<IDisplacement> {
    public IDisplacement Max { get; private set; }
    public IDisplacement Min { get; private set; }
    public List<int> Ids => Results.Keys.OrderBy(x => x).ToList();

    /// <summary>
    ///   key = nodeId
    ///   value = Collection of permutations(permutationsResults) ei Collection will have 1 item in case of AnalysisCase
    /// </summary>
    public ConcurrentDictionary<int, Collection<IDisplacement>> Results { get; }
      = new ConcurrentDictionary<int, Collection<IDisplacement>>();

    internal GsaNodeDisplacements(ConcurrentDictionary<int, Collection<IDisplacement>> results) {
      Results = results;
      Max = Results.Values.GetMax();
      Min = Results.Values.GetMin();
    }
  }
}