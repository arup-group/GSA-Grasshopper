﻿using System.Collections.Concurrent;
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
    public IDictionary<int, IList<IInternalForce>> Cache { get; }
      = new ConcurrentDictionary<int, IList<IInternalForce>>();

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
            ReadOnlyDictionary<int, Double6> aCaseResults = analysisCase.NodeReactionForce(nodelist);
            Parallel.ForEach(aCaseResults, resultKvp => {
              if (!IsSupport(resultKvp)) {
                return;
              }

              var res = new ReactionForce(resultKvp.Value);
              ((ConcurrentDictionary<int, IList<IInternalForce>>)Cache).TryAdd(
                resultKvp.Key, new Collection<IInternalForce>() {
                res,
              });
            });
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Double6>> cCaseResults
              = combinationCase.NodeReactionForce(nodelist);
            Parallel.ForEach(cCaseResults, resultKvp => {
              if (!IsSupport(resultKvp)) {
                return;
              }

              var permutationResults = new Collection<IInternalForce>();
              foreach (Double6 permutation in resultKvp.Value) {
                permutationResults.Add(new ReactionForce(permutation));
              }

              ((ConcurrentDictionary<int, IList<IInternalForce>>)Cache).TryAdd(
                resultKvp.Key, permutationResults);
            });
            break;
        }
      }

      return new NodeForceSubset(Cache.GetSubset(nodeIds));
    }

    private bool IsSupport(KeyValuePair<int, Double6> kvp) {
      return SupportNodeIds.Contains(kvp.Key) || IsNotNaN(kvp.Value);
    }

    private bool IsSupport(KeyValuePair<int, ReadOnlyCollection<Double6>> kvp) {
      return SupportNodeIds.Contains(kvp.Key) || kvp.Value.Any(res => IsNotNaN(res));
    }

    private bool IsRestrained(NodalRestraint rest) {
      return rest.X || rest.Y || rest.Z || rest.XX || rest.YY || rest.ZZ;
    }

    private bool IsNotNaN(Double6 values) {
      return !double.IsNaN(values.X) || !double.IsNaN(values.Y) || !double.IsNaN(values.Z)
        || !double.IsNaN(values.XX) || !double.IsNaN(values.YY) || !double.IsNaN(values.ZZ);
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
