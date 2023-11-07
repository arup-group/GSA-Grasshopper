using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class
    NodeReactionForceCache : INodeResultCache<IInternalForce, ResultVector6<NodeExtremaKey>> {
    public IApiResult ApiResult { get; set; }
    public ConcurrentDictionary<int, Collection<IInternalForce>> Cache { get; }
      = new ConcurrentDictionary<int, Collection<IInternalForce>>();

    internal ReadOnlyDictionary<int, Node> Nodes { get; private set; }

    internal NodeReactionForceCache(AnalysisCaseResult result, Model model) {
      ApiResult = new ApiResult(result);
      Nodes = model.Nodes();
    }

    internal NodeReactionForceCache(CombinationCaseResult result, Model model) {
      ApiResult = new ApiResult(result);
      Nodes = model.Nodes();
    }

    public INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> ResultSubset(
      ICollection<int> nodeIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(nodeIds);

      if (missingIds.Count > 0) {
        string nodelist = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, NodeResult> aCaseResults = analysisCase.NodeResults(nodelist);
            Parallel.ForEach(aCaseResults.Keys, nodeId => {
              if (!IsRestrained(nodeId) && !HasValues(aCaseResults[nodeId].Reaction)) {
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
              if (!IsRestrained(nodeId)) {
                bool hasValues = false;
                foreach (NodeResult value in cCaseResults[nodeId]) {
                  if (HasValues(value.Reaction)) {
                    hasValues = true;
                  }
                }

                if (hasValues) {
                  return;
                }
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

    private bool IsRestrained(int nodeId) {
      NodalRestraint rest = Nodes[nodeId].Restraint;
      return rest.X || rest.Y || rest.Z || rest.XX || rest.YY || rest.ZZ;
    }

    private bool HasValues(Double6 values) {
      return values.X != 0 | values.Y != 0 | values.Z != 0
              | values.XX != 0 | values.YY != 0 | values.ZZ != 0;
    }
  }
}
