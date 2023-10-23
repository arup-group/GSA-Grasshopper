using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class AnalysisCaseNodeDisplacementCache : INodeResultCache<IDisplacement> {
    public IApiResult ApiResult { get; set; }

    public ConcurrentDictionary<int, ICollection<IDisplacement>> Cache { get; }
      = new ConcurrentDictionary<int, ICollection<IDisplacement>>();

    internal AnalysisCaseNodeDisplacementCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IResultSubset<IDisplacement> ResultSubset(ICollection<int> nodeIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(nodeIds);
      if (missingIds.Count > 0) {
        string nodelist = string.Join(" ", missingIds);
        ReadOnlyDictionary<int, NodeResult> apiAnalysisCaseResults = 
          ((AnalysisCaseResult)ApiResult.Result).NodeResults(nodelist);
        Parallel.ForEach(missingIds, nodeId => {
          var res = new GsaDisplacementQuantity(apiAnalysisCaseResults[nodeId].Displacement);
          Cache.TryAdd(nodeId, new Collection<IDisplacement>() { res });
        });
      }

      return new GsaNodeDisplacements(Cache.GetSubset(nodeIds));
    }
  }
}