using GsaAPI;
using GsaGH.Parameters.Results;
using OasysUnits;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GsaGH.Parameters.Results {
  // For now, to be refactored
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
      UpdateMaxMin();
    }

    internal GsaNodeDisplacements(ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> apiCombinationCaseResults) {
      Parallel.ForEach(apiCombinationCaseResults.Keys, nodeId => {
        var permutationResults = new Collection<IDisplacement>();
        foreach (NodeResult permutationResult in apiCombinationCaseResults[nodeId]) {
          permutationResults.Add(new GsaDisplacementQuantity(permutationResult.Displacement));
        }

        Results.TryAdd(nodeId, permutationResults);
      });
      UpdateMaxMin();
    }

    private void UpdateMaxMin() {
      double maxX = Results.AsParallel().Select(list => list.Value.Select(res => res.X.Value).Max()).Max();
      double maxY = Results.AsParallel().Select(list => list.Value.Select(res => res.Y.Value).Max()).Max();
      double maxZ = Results.AsParallel().Select(list => list.Value.Select(res => res.Z.Value).Max()).Max();
      double maxXx = Results.AsParallel().Select(list => list.Value.Select(res => res.Xx.Value).Max()).Max();
      double maxYy = Results.AsParallel().Select(list => list.Value.Select(res => res.Yy.Value).Max()).Max();
      double maxZz = Results.AsParallel().Select(list => list.Value.Select(res => res.Zz.Value).Max()).Max();
      Max = new GsaDisplacementQuantity(new Double6(maxX, maxY, maxZ, maxXx, maxYy, maxZz));

      double minX = Results.AsParallel().Select(list => list.Value.Select(res => res.X.Value).Min()).Min();
      double minY = Results.AsParallel().Select(list => list.Value.Select(res => res.Y.Value).Min()).Min();
      double minZ = Results.AsParallel().Select(list => list.Value.Select(res => res.Z.Value).Min()).Min();
      double minXx = Results.AsParallel().Select(list => list.Value.Select(res => res.Xx.Value).Min()).Min();
      double minYy = Results.AsParallel().Select(list => list.Value.Select(res => res.Yy.Value).Min()).Min();
      double minZz = Results.AsParallel().Select(list => list.Value.Select(res => res.Zz.Value).Min()).Min();
      Min = new GsaDisplacementQuantity(new Double6(minX, minY, minZ, minXx, minYy, minZz));
    }
  }
}