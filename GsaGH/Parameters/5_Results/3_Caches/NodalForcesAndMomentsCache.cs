using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class NodalForcesAndMomentsCache {
    public IApiResult ApiResult { get; set; }
    private int _axisId = -10;

    public IDictionary<int, Collection<IDictionary<int, IReactionForce>>> Cache { get; }
      = new ConcurrentDictionary<int, Collection<IDictionary<int, IReactionForce>>>();

    internal NodalForcesAndMomentsCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal NodalForcesAndMomentsCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public NodalForcesAndMoments ResultSubset(ICollection<int> nodeIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(nodeIds);
      if (missingIds.Count > 0) {
        string nodelist = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyDictionary<int, Double6>> aCaseResults = analysisCase.NodalForcesAndMoments(nodelist, _axisId);
            Parallel.ForEach(aCaseResults, resultKvp => {
              var res = new ConcurrentDictionary<int, IReactionForce>();
              foreach (KeyValuePair<int, Double6> force in resultKvp.Value) {
                res.TryAdd(force.Key, new ReactionForce(force.Value));
              }

              Cache.Add(resultKvp.Key, new Collection<IDictionary<int, IReactionForce>>() { res });
            });
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyDictionary<int, Double6>>> cCaseResults = combinationCase.NodalForcesAndMoments(nodelist, _axisId);
            Parallel.ForEach(cCaseResults, resultKvp => {
              var permutationResults = new Collection<IDictionary<int, IReactionForce>>();
              foreach (ReadOnlyDictionary<int, Double6> permutation in resultKvp.Value) {
                var res = new ConcurrentDictionary<int, IReactionForce>();
                foreach (KeyValuePair<int, Double6> force in permutation) {
                  res.TryAdd(force.Key, new ReactionForce(force.Value));
                }

                permutationResults.Add(res);
              }

              Cache.Add(resultKvp.Key, permutationResults);
            });
            break;
        }
      }
      return new NodalForcesAndMoments(Cache.GetSubset(nodeIds));
    }

    public void SetStandardAxis(int axisId) {
      if (axisId != _axisId) {
        Cache.Clear();
      }

      _axisId = axisId;
    }
  }
}
