using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class NodalForcesAndMomentsCache {
    public IApiResult ApiResult { get; set; }

    public IDictionary<int, IDictionary<int, IList<IInternalForce>>> Cache { get; }
      = new ConcurrentDictionary<int, IDictionary<int, IList<IInternalForce>>>();

    internal NodalForcesAndMomentsCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal NodalForcesAndMomentsCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public NodalForcesAndMomentsSubset ResultSubset(ICollection<int> nodeIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(nodeIds);

      if (missingIds.Count > 0) {
        string nodelist = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, Double6> aCaseResults = analysisCase.NodeReactionForce(nodelist);
            Parallel.ForEach(aCaseResults, resultKvp => {
              var res = new ReactionForce(resultKvp.Value);
              ((ConcurrentDictionary<int, IList<IReactionForce>>)Cache).TryAdd(
                resultKvp.Key, new Collection<IReactionForce>() {
                res,
              });
            });
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Double6>> cCaseResults
              = combinationCase.NodeReactionForce(nodelist);
            Parallel.ForEach(cCaseResults, resultKvp => {
              var permutationResults = new Collection<IReactionForce>();
              foreach (Double6 permutation in resultKvp.Value) {
                permutationResults.Add(new ReactionForce(permutation));
              }

              ((ConcurrentDictionary<int, IList<IReactionForce>>)Cache).TryAdd(
                resultKvp.Key, permutationResults);
            });
            break;
        }
      }

      return new NodalForcesAndMomentsSubset(Cache.GetSubset(nodeIds));
    }
  }
}
