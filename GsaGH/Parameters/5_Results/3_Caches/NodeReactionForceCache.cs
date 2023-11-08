using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class
    NodeReactionForceCache : INodeResultCache<IInternalForce, ResultVector6<NodeExtremaKey>> {
    internal ConcurrentBag<int> SupportNodeIds { get; private set; }
    public IApiResult ApiResult { get; set; }
    public ConcurrentDictionary<int, Collection<IInternalForce>> Cache { get; }
      = new ConcurrentDictionary<int, Collection<IInternalForce>>();

    internal ReadOnlyDictionary<int, Node> Nodes { get; private set; }

    internal NodeReactionForceCache(AnalysisCaseResult result, Model model) {
      ApiResult = new ApiResult(result);
      SetSupportNodeIds(model);
    }

    internal NodeReactionForceCache(CombinationCaseResult result, Model model) {
      ApiResult = new ApiResult(result);
      SetSupportNodeIds(model);
    }

    public INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> ResultSubset(
      ICollection<int> nodeIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(nodeIds);

      if (missingIds.Count > 0) {
        string nodelist = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, NodeResult> aCaseResults = analysisCase.NodeResults(nodelist);
            Parallel.ForEach(aCaseResults, resultKvp => {
              if (!IsSupport(resultKvp)) {
                return;
              }

              var res = new ReactionForce(resultKvp.Value.Reaction);
              Cache.TryAdd(resultKvp.Key, new Collection<IInternalForce>() {
                res,
              });
            });
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> cCaseResults
              = combinationCase.NodeResults(nodelist);
            Parallel.ForEach(cCaseResults, resultKvp => {
              if (!IsSupport(resultKvp)) {
                return;
              }

              var permutationResults = new Collection<IInternalForce>();
              foreach (NodeResult permutationResult in resultKvp.Value) {
                permutationResults.Add(new ReactionForce(permutationResult.Reaction));
              }

              Cache.TryAdd(resultKvp.Key, permutationResults);
            });
            break;
        }
      }

      return new NodeForceSubset(Cache.GetSubset(nodeIds));
    }

    private bool IsSupport(KeyValuePair<int, NodeResult> kvp) {
      return SupportNodeIds.Contains(kvp.Key) || HasValues(kvp.Value.Reaction);
    }

    private bool IsSupport(KeyValuePair<int, ReadOnlyCollection<NodeResult>> kvp) {
      if (SupportNodeIds.Contains(kvp.Key)) {
        return true;
      }
      
      foreach (NodeResult res in kvp.Value) {
        if (HasValues(res.Reaction)) {
          return true;
        }
      }
      
      return false;
    }

    private bool IsRestrained(NodalRestraint rest) {
      return rest.X || rest.Y || rest.Z || rest.XX || rest.YY || rest.ZZ;
    }

    private bool HasValues(Double6 values) {
      return HasValue(values.X) || HasValue(values.Y) || HasValue(values.Z)
        || HasValue(values.XX) || HasValue(values.YY) || HasValue(values.ZZ);
    }

    private bool HasValue(double value) {
      return !double.IsNaN(value) && value != 0;
    }

    private void SetSupportNodeIds(Model model) {
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
  }
}
