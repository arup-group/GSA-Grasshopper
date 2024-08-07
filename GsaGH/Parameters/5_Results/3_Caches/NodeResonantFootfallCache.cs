using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class NodeResonantFootfallCache : IEntity0dResultCache<IFootfall, ResultFootfall<Entity0dExtremaKey>> {
    public IApiResult ApiResult { get; set; }
    public IDictionary<int, IList<IFootfall>> Cache { get; }
      = new ConcurrentDictionary<int, IList<IFootfall>>();

    internal NodeResonantFootfallCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IEntity0dResultSubset<IFootfall, ResultFootfall<Entity0dExtremaKey>> ResultSubset(ICollection<int> nodeIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(nodeIds);
      if (missingIds.Count > 0) {
        string nodelist = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, NodeFootfallResult> aCaseResults = analysisCase.NodeResonantFootfall(nodelist);
            Parallel.ForEach(aCaseResults, resultKvp => {
              if (IsInvalid(resultKvp)) {
                return;
              }

              var res = new Footfall(resultKvp.Value);
              ((ConcurrentDictionary<int, IList<IFootfall>>)Cache).TryAdd(
                resultKvp.Key, new Collection<IFootfall>() { res });
            });
            break;
        }
      }

      return new NodeFootfalls(Cache.GetSubset(nodeIds));
    }

    private bool IsInvalid(KeyValuePair<int, NodeFootfallResult> kvp) {
      return double.IsNaN(kvp.Value.MaximumResponseFactor);
    }

    public void SetStandardAxis(int axisId) {
      throw new NotImplementedException("Footfall is independent from chosen axis");
    }
  }
}
