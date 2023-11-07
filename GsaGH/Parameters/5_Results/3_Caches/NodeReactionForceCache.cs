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

    internal NodeReactionForceCache(AnalysisCaseResult result, Model model) {
      ApiResult = new ApiResult(result);
      SetSupportNodeIds(model);
    }

    internal NodeReactionForceCache(CombinationCaseResult result, Model model) {
      ApiResult = new ApiResult(result);
      SetSupportNodeIds(model);
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
            Parallel.ForEach(aCaseResults.Keys, nodeId => {
              if (!SupportNodeIds.Contains(nodeId)) {
                return;
              }

              var res = new ReactionForce(aCaseResults[nodeId].Reaction);
              Cache.TryAdd(nodeId, new Collection<IInternalForce>() {
                res,
              });
            });
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> cCaseResults
              = combinationCase.NodeResults(nodelist);
            Parallel.ForEach(cCaseResults.Keys, nodeId => {
              if (!SupportNodeIds.Contains(nodeId)) {
                return;
              }

              var permutationResults = new Collection<IInternalForce>();
              foreach (NodeResult permutationResult in cCaseResults[nodeId]) {
                permutationResults.Add(new ReactionForce(permutationResult.Reaction));
              }

              Cache.TryAdd(nodeId, permutationResults);
            });
            break;
        }
      }

      return new NodeForceSubset(Cache.GetSubset(nodeIds));
    }

    private void SetSupportNodeIds(Model model) {
      ConcurrentBag<int> supportnodeIDs = null;
      supportnodeIDs = new ConcurrentBag<int>();
      ReadOnlyDictionary<int, Node> nodes = model.Nodes();
      Parallel.ForEach(nodes, node => {
        NodalRestraint rest = node.Value.Restraint;
        if (rest.X || rest.Y || rest.Z || rest.XX || rest.YY || rest.ZZ) {
          supportnodeIDs.Add(node.Key);
        }
      });
      SupportNodeIds = supportnodeIDs;
    }
  }
}
