using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class NodeDisplacementCache : INodeResultCache<IDisplacement> {
    public IApiResult ApiResult { get; set; }

    public ConcurrentDictionary<int, Collection<IDisplacement>> Cache { get; }
      = new ConcurrentDictionary<int, Collection<IDisplacement>>();

    internal NodeDisplacementCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal NodeDisplacementCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IResultSubset<IDisplacement> ResultSubset(ICollection<int> nodeIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(nodeIds);
      if (missingIds.Count > 0) {
        string nodelist = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, NodeResult> aCaseResults = analysisCase.NodeResults(nodelist);
            Parallel.ForEach(missingIds, nodeId => {
              var res = new GsaDisplacementQuantity(aCaseResults[nodeId].Displacement);
              Cache.TryAdd(nodeId, new Collection<IDisplacement>() { res });
            });
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> cCaseResults = combinationCase.NodeResults(nodelist);
            Parallel.ForEach(missingIds, nodeId => {
              var permutationResults = new Collection<IDisplacement>();
              foreach (NodeResult permutationResult in cCaseResults[nodeId]) {
                permutationResults.Add(new GsaDisplacementQuantity(permutationResult.Displacement));
              }

              Cache.TryAdd(nodeId, permutationResults);
            });
            break;
        }
      }

      return new GsaNodeDisplacements(Cache.GetSubset(nodeIds));
    }
  }
}