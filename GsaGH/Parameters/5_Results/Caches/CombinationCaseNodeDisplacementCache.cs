using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class CombinationCaseNodeDisplacementCache : INodeResultCache<IDisplacement> {
    public IApiResult ApiResult { get; set; }

    public ConcurrentDictionary<int, ICollection<IDisplacement>> Cache { get; set; }
      = new ConcurrentDictionary<int, ICollection<IDisplacement>>();

    internal CombinationCaseNodeDisplacementCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IResultSubset<IDisplacement> ResultSubset(ICollection<int> nodeIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(nodeIds);
      if (missingIds.Count > 0) {
        string nodelist = string.Join(" ", missingIds);
        ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> apiCombinationCaseResults =
          ((CombinationCaseResult)ApiResult.Result).NodeResults(nodelist);
        Parallel.ForEach(missingIds, nodeId => {
          var permutationResults = new Collection<IDisplacement>();
          foreach (NodeResult permutationResult in apiCombinationCaseResults[nodeId]) {
            permutationResults.Add(new GsaDisplacementQuantity(permutationResult.Displacement));
          }

          Cache.TryAdd(nodeId, permutationResults);
        });
      }

      return new GsaNodeDisplacements(Cache.GetSubset(nodeIds));
    }
  }
}