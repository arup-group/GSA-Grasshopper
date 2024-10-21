using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class AssemblyDriftIndicesCache {
    public IApiResult ApiResult { get; set; }

    public ConcurrentDictionary<int, IList<IEntity1dQuantity<DriftIndex>>> Cache { get; }
      = new ConcurrentDictionary<int, IList<IEntity1dQuantity<DriftIndex>>>();

    internal AssemblyDriftIndicesCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal AssemblyDriftIndicesCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public AssemblyDriftIndices ResultSubset(ICollection<int> entityIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(entityIds);
      if (missingIds.Count > 0) {
        string entityList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<AssemblyDriftIndexResult>> aCaseResults
               = analysisCase.AssemblyDriftIndices(entityList);

            Parallel.ForEach(aCaseResults.Keys, entityId => {
              var positions = new Collection<double>();
              foreach (AssemblyDriftIndexResult result in aCaseResults[entityId]) {
                positions.Add(result.Position);
              }

              var results = new AssemblyDriftIndex(aCaseResults[entityId]);
              Cache.TryAdd(entityId, new List<IEntity1dQuantity<DriftIndex>>() { results });
            });
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<AssemblyDriftIndexResult>>> cCaseResults
            = combinationCase.AssemblyDriftIndices(entityList);

            Parallel.ForEach(cCaseResults.Keys, entityId => {
              var permutationResults = new Collection<IEntity1dQuantity<DriftIndex>>();
              foreach (ReadOnlyCollection<AssemblyDriftIndexResult> permutation in cCaseResults[entityId]) {
                var positions = new Collection<double>();
                foreach (AssemblyDriftIndexResult result in permutation) {
                  positions.Add(result.Position);
                }

                var results = new AssemblyDriftIndex(permutation);
                permutationResults.Add(results);
              }

              Cache.TryAdd(entityId, permutationResults);
            });
            break;
        }
      }

      return new AssemblyDriftIndices(Cache.GetSubset(entityIds));
    }
  }
}
