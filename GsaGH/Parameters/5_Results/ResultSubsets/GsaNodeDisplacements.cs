using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class GsaNodeDisplacements : IResultSubset<IDisplacement> {
    public IDisplacement Max { get; private set; }
    public IDisplacement Min { get; private set; }
    public List<int> Ids => Results.Keys.OrderBy(x => x).ToList();

    /// <summary>
    ///   Combination Case Node Displacement Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = nodeId
    ///   value = Collection of permutations(permutationsResults) ei Collection will have 1 item in case of AnalysisCase
    /// </summary>
    public ConcurrentDictionary<int, Collection<IDisplacement>> Results { get; }
      = new ConcurrentDictionary<int, Collection<IDisplacement>>();

    internal GsaNodeDisplacements(ReadOnlyDictionary<int, NodeResult> apiAnalysisCaseResults) {
      Parallel.ForEach(apiAnalysisCaseResults.Keys, nodeId => {
        var res = new GsaDisplacementQuantity(apiAnalysisCaseResults[nodeId].Displacement);
        Results.TryAdd(nodeId, new Collection<IDisplacement>() { res });
      });
      
      Max = Results.Values.GetMax();
      Min = Results.Values.GetMin();
    }

    internal GsaNodeDisplacements(ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> apiCombinationCaseResults) {
      Parallel.ForEach(apiCombinationCaseResults.Keys, nodeId => {
        var permutationResults = new Collection<IDisplacement>();
        foreach (NodeResult permutationResult in apiCombinationCaseResults[nodeId]) {
          permutationResults.Add(new GsaDisplacementQuantity(permutationResult.Displacement));
        }

        Results.TryAdd(nodeId, permutationResults);
      });
      
      Max = Results.Values.GetMax();
      Min = Results.Values.GetMin();
    }
  }
}