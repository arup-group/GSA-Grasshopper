using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class AnalysisCaseNodeDisplacementCache : INodeResultCache<IDisplacement> {
    public IApiResult ApiResult { get; set; }

    // API Node results (will not be needed after GSA-7517)
    internal Dictionary<string, ReadOnlyDictionary<int, NodeResult>> AnalysisCaseNodeResults { get; set; }
      = new Dictionary<string, ReadOnlyDictionary<int, NodeResult>>();

    public IDictionary<string, IResultSubset<IDisplacement>> Cache { get; set; }
      = new Dictionary<string, IResultSubset<IDisplacement>>();

    internal AnalysisCaseNodeDisplacementCache(AnalysisCaseResult result) {
      ApiResult = new AnalysisCaseApiResult(result);
    }

    public IResultSubset<IDisplacement> ResultSubset(string nodelist) {
      if (nodelist.ToLower() == "all" || nodelist == string.Empty) {
        nodelist = "All";
      }

      if (!Cache.ContainsKey(nodelist)) {
        if (!AnalysisCaseNodeResults.ContainsKey(nodelist)) {
          AnalysisCaseNodeResults.Add(nodelist, ((AnalysisCaseResult)ApiResult.Result).NodeResults(nodelist));
        }

        Cache.Add(nodelist, new GsaNodeDisplacements(AnalysisCaseNodeResults[nodelist]));
      }

      return Cache[nodelist];
    }
  }
}