using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class
    NodeSpringForceCache : INodeResultCache<IReactionForce, ResultVector6<NodeExtremaKey>> {
    public IApiResult ApiResult { get; set; }
    public IDictionary<int, IList<IReactionForce>> Cache { get; }
      = new ConcurrentDictionary<int, IList<IReactionForce>>();

    internal NodeSpringForceCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal NodeSpringForceCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public INodeResultSubset<IReactionForce, ResultVector6<NodeExtremaKey>> ResultSubset(
      ICollection<int> nodeIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(nodeIds);

      if (missingIds.Count > 0) {
        string nodelist = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, Double6> aCaseResults = analysisCase.NodeSpringForce(nodelist);
            Parallel.ForEach(aCaseResults, resultKvp => {
              if (!HasValues(resultKvp)) {
                return;
              }

              var res = new ReactionForce(resultKvp.Value);
              ((ConcurrentDictionary<int, IList<IReactionForce>>)Cache).TryAdd(
                resultKvp.Key, new Collection<IReactionForce>() {
                res,
              });
            });
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Double6>> cCaseResults
              = combinationCase.NodeSpringForce(nodelist);
            Parallel.ForEach(cCaseResults, resultKvp => {
              if (!HasValues(resultKvp)) {
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

      return new NodeForceSubset(Cache.GetSubset(nodeIds));
    }

    private bool HasValues(KeyValuePair<int, Double6> kvp) {
      return HasValues(kvp.Value);
    }

    private bool HasValues(KeyValuePair<int, ReadOnlyCollection<Double6>> kvp) {
      return kvp.Value.Any(res => HasValues(res));
    }

    private bool HasValues(Double6 values) {
      return HasValue(values.X) || HasValue(values.Y) || HasValue(values.Z)
        || HasValue(values.XX) || HasValue(values.YY) || HasValue(values.ZZ);
    }

    private bool HasValue(double value) {
      return !double.IsNaN(value) && value != 0;
    }
  }
}
