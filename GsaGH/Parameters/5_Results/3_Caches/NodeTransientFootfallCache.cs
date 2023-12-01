using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;
using Newtonsoft.Json.Linq;

namespace GsaGH.Parameters.Results {
  public class NodeTransientFootfallCache : INodeResultCache<IFootfall, ResultFootfall<NodeExtremaKey>> {
    public IApiResult ApiResult { get; set; }
    public IDictionary<int, IList<IFootfall>> Cache { get; }
      = new ConcurrentDictionary<int, IList<IFootfall>>();
    
    internal NodeTransientFootfallCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }
    internal NodeTransientFootfallCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public INodeResultSubset<IFootfall, ResultFootfall<NodeExtremaKey>> ResultSubset(ICollection<int> nodeIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(nodeIds);
      if (missingIds.Count > 0) {
        string nodelist = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, NodeFootfallResult> aCaseResults = analysisCase.NodeTransientFootfall(nodelist);
            Parallel.ForEach(aCaseResults, resultKvp => {
              if (IsInvalid(resultKvp)) {
                return;
              }

              var res = new Footfall(resultKvp.Value);
              ((ConcurrentDictionary<int, IList<IFootfall>>)Cache).TryAdd(
                resultKvp.Key, new List<IFootfall>() { res });
            });
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<NodeFootfallResult>> cCaseResults = combinationCase.NodeTransientFootfall(nodelist);
            Parallel.ForEach(cCaseResults, resultKvp => {
              if (IsInvalid(resultKvp)) {
                return;
              }

              var permutationResults = new List<IFootfall>();
              foreach (NodeFootfallResult permutationResult in resultKvp.Value) {
                permutationResults.Add(new Footfall(permutationResult));
              }

              ((ConcurrentDictionary<int, IList<IFootfall>>)Cache).TryAdd(
                resultKvp.Key, permutationResults);
            });
            break;
        }
      }

      return new NodeFootfalls(Cache.GetSubset(nodeIds));
    }

    private bool IsInvalid(KeyValuePair<int, ReadOnlyCollection<NodeFootfallResult>> kvp) {
      return kvp.Value.Any(res => double.IsNaN(res.MaximumResponseFactor));
    }

    private bool IsInvalid(KeyValuePair<int, NodeFootfallResult> kvp) {
      return double.IsNaN(kvp.Value.MaximumResponseFactor);
    }
  }
}
