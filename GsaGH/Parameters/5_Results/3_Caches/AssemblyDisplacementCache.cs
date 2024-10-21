using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class AssemblyDisplacementCache {
    public IApiResult ApiResult { get; set; }

    public ConcurrentDictionary<int, IList<IEntity1dQuantity<IDisplacement>>> Cache { get; }
      = new ConcurrentDictionary<int, IList<IEntity1dQuantity<IDisplacement>>>();

    internal AssemblyDisplacementCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal AssemblyDisplacementCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public AssemblyDisplacements ResultSubset(ICollection<int> entityIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(entityIds);
      if (missingIds.Count > 0) {
        string entityList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<AssemblyResult>> aCaseResults
               = analysisCase.AssemblyDisplacements(entityList);

            Parallel.ForEach(aCaseResults.Keys, entityId => {
              var positions = new Collection<double>();
              foreach (AssemblyResult result in aCaseResults[entityId]) {
                positions.Add(result.Position);
              }

              var results = new AssemblyDisplacement(aCaseResults[entityId]);
              Cache.TryAdd(entityId, new List<IEntity1dQuantity<IDisplacement>>() { results });
            });
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<AssemblyResult>>> cCaseResults
            = combinationCase.AssemblyDisplacements(entityList);

            Parallel.ForEach(cCaseResults.Keys, entityId => {
              var permutationResults = new Collection<IEntity1dQuantity<IDisplacement>>();
              foreach (ReadOnlyCollection<AssemblyResult> permutation in cCaseResults[entityId]) {
                var positions = new Collection<double>();
                foreach (AssemblyResult result in permutation) {
                  positions.Add(result.Position);
                }

                var results = new AssemblyDisplacement(permutation);
                permutationResults.Add(results);
              }

              Cache.TryAdd(entityId, permutationResults);
            });
            break;
        }
      }

      return new AssemblyDisplacements(Cache.GetSubset(entityIds));
    }
  }
}
