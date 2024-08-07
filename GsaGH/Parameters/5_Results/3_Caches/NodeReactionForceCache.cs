using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class
    NodeReactionForceCache : IEntity0dResultCache<IReactionForce, ResultVector6<Entity0dExtremaKey>> {
    internal ConcurrentBag<int> SupportNodeIds { get; private set; }
    public IApiResult ApiResult { get; set; }
    public IDictionary<int, IList<IReactionForce>> Cache { get; }
      = new ConcurrentDictionary<int, IList<IReactionForce>>();
    internal ReadOnlyDictionary<int, Node> Nodes { get; private set; }
    private int _axisId = -10;

    internal NodeReactionForceCache(AnalysisCaseResult result, Model model) {
      ApiResult = new ApiResult(result);
      SetSupportNodeIds(model);
    }

    internal NodeReactionForceCache(CombinationCaseResult result, Model model) {
      ApiResult = new ApiResult(result);
      SetSupportNodeIds(model);
    }
    private static bool IsNaN(Double6 values) {
      return double.IsNaN(values.X) && double.IsNaN(values.XX);
    }

    private static bool IsRestrained(NodalRestraint rest) {
      return rest.X || rest.Y || rest.Z || rest.XX || rest.YY || rest.ZZ;
    }

    public IEntity0dResultSubset<IReactionForce, ResultVector6<Entity0dExtremaKey>> ResultSubset(
      ICollection<int> nodeIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(nodeIds);

      if (missingIds.Count > 0) {
        string nodelist = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, Double6> aCaseResults = analysisCase.NodeReactionForce(nodelist, _axisId);
            Parallel.ForEach(aCaseResults, resultKvp => {
              if (!SupportNodeIds.Contains(resultKvp.Key) && IsNaN(resultKvp.Value)) {
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
              = combinationCase.NodeReactionForce(nodelist, _axisId);
            Parallel.ForEach(cCaseResults, resultKvp => {
              if (!SupportNodeIds.Contains(resultKvp.Key) && resultKvp.Value.Any(IsNaN)) {
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

    private void SetSupportNodeIds(Model model) {
      if (model == null) {
        return;
      }
      ConcurrentBag<int> supportnodeIDs = null;
      supportnodeIDs = new ConcurrentBag<int>();
      ReadOnlyDictionary<int, Node> nodes = model.Nodes();
      Parallel.ForEach(nodes, node => {
        if (IsRestrained(node.Value.Restraint)) {
          supportnodeIDs.Add(node.Key);
        }
      });

      SupportNodeIds = supportnodeIDs;
    }

    public void SetStandardAxis(int axisId) {
      if (axisId != _axisId) {
        Cache.Clear();
      }

      _axisId = axisId;
    }
  }
}
