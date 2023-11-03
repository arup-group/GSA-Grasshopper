using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class
    NodeSpringForceCache : INodeResultCache<IInternalForce, ResultVector6<NodeExtremaKey>> {

    internal NodeSpringForceCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal NodeSpringForceCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IApiResult ApiResult { get; set; }
    public ConcurrentDictionary<int, Collection<IInternalForce>> Cache { get; }
      = new ConcurrentDictionary<int, Collection<IInternalForce>>();

    public INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> ResultSubset(
      ICollection<int> nodeIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(nodeIds);

      if (missingIds.Count > 0) {
        string nodelist = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, NodeResult> aCaseResults = analysisCase.NodeResults(nodelist);
            Parallel.ForEach(aCaseResults.Keys, nodeId => {
              var res = new ReactionForce(aCaseResults[nodeId].SpringForce);
              Cache.TryAdd(nodeId, new Collection<IInternalForce>() {
                res,
              });
            });
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> cCaseResults
              = combinationCase.NodeResults(nodelist);
            Parallel.ForEach(cCaseResults.Keys, nodeId => {
              var permutationResults = new Collection<IInternalForce>();
              foreach (NodeResult permutationResult in cCaseResults[nodeId]) {
                permutationResults.Add(new ReactionForce(permutationResult.SpringForce));
              }

              Cache.TryAdd(nodeId, permutationResults);
            });
            break;
        }
      }

      return new NodeForceSubset(Cache.GetSubset(nodeIds));
    }
  }
}
