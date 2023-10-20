using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class CombinationCaseNodeDisplacementCache : INodeResultCache<IDisplacement> {
    public IApiResult ApiResult { get; set; }

    // API Node results (will not be needed after GSA-7517)
    internal Dictionary<string, ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>>>
      CombinationCaseNodeResults { get; set; }
      = new Dictionary<string, ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>>>();

    public IDictionary<string, IResultSubset<IDisplacement>> Cache { get; set; }
      = new Dictionary<string, IResultSubset<IDisplacement>>();

    public CombinationCaseNodeDisplacementCache(CombinationCaseResult result) {
      ApiResult = new CombinationCaseApiResult(result);
    }

    public IResultSubset<IDisplacement> ResultSubset(string nodelist) {
      if (nodelist.ToLower() == "all" || nodelist == string.Empty) {
        nodelist = "All";
      }

      if (!Cache.ContainsKey(nodelist)) {
        if (!CombinationCaseNodeResults.ContainsKey(nodelist)) {
          CombinationCaseNodeResults.Add(nodelist, ((CombinationCaseResult)ApiResult.Result).NodeResults(nodelist));
        }

        Cache.Add(nodelist, new GsaNodeDisplacements(CombinationCaseNodeResults[nodelist]));
      }

      return Cache[nodelist];
    }
  }
}