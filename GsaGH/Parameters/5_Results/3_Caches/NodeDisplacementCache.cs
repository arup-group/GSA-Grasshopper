using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class NodeDisplacementCache : IEntity0dResultCache<IDisplacement, ResultVector6<Entity0dExtremaKey>> {
    public IApiResult ApiResult { get; set; }
    public IDictionary<int, IList<IDisplacement>> Cache { get; }
      = new ConcurrentDictionary<int, IList<IDisplacement>>();
    private int _axisId = -10;

    internal NodeDisplacementCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal NodeDisplacementCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IEntity0dResultSubset<IDisplacement, ResultVector6<Entity0dExtremaKey>> ResultSubset(ICollection<int> nodeIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(nodeIds);
      if (missingIds.Count > 0) {
        string nodelist = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, Double6> aCaseResults = analysisCase.NodeDisplacement(nodelist, _axisId);
            Parallel.ForEach(aCaseResults, resultKvp => {
              if (IsInvalid(resultKvp)) {
                return;
              }

              var res = new Displacement(resultKvp.Value);
              ((ConcurrentDictionary<int, IList<IDisplacement>>)Cache).TryAdd(
                resultKvp.Key, new Collection<IDisplacement>() { res });
            });
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Double6>> cCaseResults = combinationCase.NodeDisplacement(nodelist, _axisId);
            Parallel.ForEach(cCaseResults, resultKvp => {
              if (IsInvalid(resultKvp)) {
                return;
              }

              var permutationResults = new Collection<IDisplacement>();
              foreach (Double6 permutation in resultKvp.Value) {
                permutationResults.Add(new Displacement(permutation));
              }

              ((ConcurrentDictionary<int, IList<IDisplacement>>)Cache).TryAdd(
                resultKvp.Key, permutationResults);
            });
            break;
        }
      }

      return new NodeDisplacements(Cache.GetSubset(nodeIds));
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

    public void SetStandardAxis(int axisId) {
      if (axisId != _axisId) {
        Cache.Clear();
      }

      _axisId = axisId;
    }
  }
}
