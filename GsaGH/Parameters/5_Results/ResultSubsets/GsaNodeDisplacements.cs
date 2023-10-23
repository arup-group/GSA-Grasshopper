using System.Collections.Concurrent;
using System.Collections.Generic;
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
    public ConcurrentDictionary<int, ICollection<IDisplacement>> Results { get; }
      = new ConcurrentDictionary<int, ICollection<IDisplacement>>();

    internal GsaNodeDisplacements(ConcurrentDictionary<int, ICollection<IDisplacement>> results) {
      Results = results;
      Max = Results.Values.GetMax();
      Min = Results.Values.GetMin();
    }
  }
}