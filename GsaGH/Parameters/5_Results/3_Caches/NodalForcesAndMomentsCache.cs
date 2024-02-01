using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class NodalForcesAndMomentsCache : IEntity0dResultCache<IReactionForce, ResultVector6<Entity0dExtremaKey>> {
    public IApiResult ApiResult { get; set; }

    private readonly Model _model;

    public IDictionary<int, IList<IReactionForce>> Cache { get; }
      = new ConcurrentDictionary<int, IList<IReactionForce>>();

    internal NodalForcesAndMomentsCache(AnalysisCaseResult result, Model model) {
      ApiResult = new ApiResult(result);
      _model = model;
    }

    internal NodalForcesAndMomentsCache(CombinationCaseResult result, Model model) {
      ApiResult = new ApiResult(result);
      _model = model;
    }

    public IEntity0dResultSubset<IReactionForce, ResultVector6<Entity0dExtremaKey>> ResultSubset(ICollection<int> nodeIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(nodeIds);
      if (missingIds.Count > 0) {
        string nodelist = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, Double6> aCaseResults = analysisCase.NodeConstraintForce(nodelist);
            Parallel.ForEach(aCaseResults, resultKvp => {
              if (IsInvalid(resultKvp)) {
                return;
              }

              var res = new ReactionForce(resultKvp.Value);
              ((ConcurrentDictionary<int, IList<IReactionForce>>)Cache).TryAdd(
                resultKvp.Key, new Collection<IReactionForce>() { res });
            });
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Double6>> cCaseResults = combinationCase.NodeConstraintForce(nodelist);
            Parallel.ForEach(cCaseResults, resultKvp => {
              if (IsInvalid(resultKvp)) {
                return;
              }

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

    private bool IsInvalid(KeyValuePair<int, ReadOnlyCollection<Double6>> kvp) {
      return kvp.Value.Any(res => !IsNotNaN(res));
    }

    private bool IsInvalid(KeyValuePair<int, Double6> kvp) {
      return !IsNotNaN(kvp.Value);
    }

    private bool IsNotNaN(Double6 values) {
      return !double.IsNaN(values.X) || !double.IsNaN(values.Y) || !double.IsNaN(values.Z)
        || !double.IsNaN(values.XX) || !double.IsNaN(values.YY) || !double.IsNaN(values.ZZ);
    }
  }
}
