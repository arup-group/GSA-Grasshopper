using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class AssemblyDriftsCache {
    public IApiResult ApiResult { get; set; }

    public ConcurrentDictionary<int, IList<IEntity1dQuantity<Drift>>> Cache { get; }
      = new ConcurrentDictionary<int, IList<IEntity1dQuantity<Drift>>>();

    internal AssemblyDriftsCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal AssemblyDriftsCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public AssemblyDrifts ResultSubset(ICollection<int> entityIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(entityIds);
      if (missingIds.Count > 0) {
        string entityList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<AssemblyDriftResult>> aCaseResults
               = analysisCase.AssemblyDrifts(entityList);

            Parallel.ForEach(aCaseResults.Keys, entityId => {
              var positions = new Collection<double>();
              foreach (AssemblyDriftResult result in aCaseResults[entityId]) {
                positions.Add(result.Position);
              }

              var results = new AssemblyDrift(aCaseResults[entityId]);
              Cache.TryAdd(entityId, new List<IEntity1dQuantity<Drift>>() { results });
            });
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<AssemblyDriftResult>>> cCaseResults
            = combinationCase.AssemblyDrifts(entityList);

            Parallel.ForEach(cCaseResults.Keys, entityId => {
              var permutationResults = new Collection<IEntity1dQuantity<Drift>>();
              foreach (ReadOnlyCollection<AssemblyDriftResult> permutation in cCaseResults[entityId]) {
                var positions = new Collection<double>();
                foreach (AssemblyDriftResult result in permutation) {
                  positions.Add(result.Position);
                }

                var results = new AssemblyDrift(permutation);
                permutationResults.Add(results);
              }

              Cache.TryAdd(entityId, permutationResults);
            });
            break;
        }
      }

      return new AssemblyDrifts(Cache.GetSubset(entityIds));
    }
  }
}
