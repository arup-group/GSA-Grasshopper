using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class NodeResonantFootfallCache : INodeResultCache<IFootfall, ResultFootfall<NodeExtremaKey>> {
    public IApiResult ApiResult { get; set; }
    public ConcurrentDictionary<int, Collection<IFootfall>> Cache { get; }
      = new ConcurrentDictionary<int, Collection<IFootfall>>();
    
    internal NodeResonantFootfallCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }
    internal NodeResonantFootfallCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public INodeResultSubset<IFootfall, ResultFootfall<NodeExtremaKey>> ResultSubset(ICollection<int> nodeIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(nodeIds);
      if (missingIds.Count > 0) {
        string nodelist = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, NodeFootfallResult> aCaseResults = analysisCase.NodeTransientFootfall(nodelist);
            Parallel.ForEach(aCaseResults.Keys, nodeId => {
              var res = new Footfall(aCaseResults[nodeId]);
              Cache.TryAdd(nodeId, new Collection<IFootfall>() { res });
            });
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<NodeFootfallResult>> cCaseResults = combinationCase.NodeResonantFootfall(nodelist);
            Parallel.ForEach(cCaseResults.Keys, nodeId => {
              var permutationResults = new Collection<IFootfall>();
              foreach (NodeFootfallResult permutationResult in cCaseResults[nodeId]) {
                permutationResults.Add(new Footfall(permutationResult));
              }

              Cache.TryAdd(nodeId, permutationResults);
            });
            break;
        }
      }

      return new NodeFootfalls(Cache.GetSubset(nodeIds));
    }
  }
}
