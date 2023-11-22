using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class
    NodeSpringForceCache : INodeResultCache<IInternalForce, ResultVector6<NodeExtremaKey>> {

    internal NodeSpringForceCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal NodeSpringForceCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IApiResult ApiResult { get; set; }
    public ConcurrentDictionary<int, Collection<IInternalForce>> Cache { get; }
      = new ConcurrentDictionary<int, Collection<IInternalForce>>();

    public INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> ResultSubset(
      ICollection<int> nodeIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(nodeIds);

      if (missingIds.Count > 0) {
        string nodelist = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, NodeResult> aCaseResults = analysisCase.NodeResults(nodelist);
            Parallel.ForEach(aCaseResults, resultKvp => {
              if (!HasValues(resultKvp)) {
                return;
              }

              var res = new ReactionForce(resultKvp.Value.SpringForce);
              Cache.TryAdd(resultKvp.Key, new Collection<IInternalForce>() {
                res,
              });
            });
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> cCaseResults
              = combinationCase.NodeResults(nodelist);
            Parallel.ForEach(cCaseResults, resultKvp => {
              if (!HasValues(resultKvp)) {
                return;
              }

              var permutationResults = new Collection<IInternalForce>();
              foreach (NodeResult permutationResult in resultKvp.Value) {
                permutationResults.Add(new ReactionForce(permutationResult.SpringForce));
              }

              Cache.TryAdd(resultKvp.Key, permutationResults);
            });
            break;
            
        }
      }

      return new NodeForceSubset(Cache.GetSubset(nodeIds));
    }

    private bool HasValues(KeyValuePair<int, NodeResult> kvp) {
      return HasValues(kvp.Value.SpringForce);
    }

    private bool HasValues(KeyValuePair<int, ReadOnlyCollection<NodeResult>> kvp) {
      foreach (NodeResult res in kvp.Value) {
        if (HasValues(res.SpringForce)) {
          return true;
        }
      }

      return false;
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
