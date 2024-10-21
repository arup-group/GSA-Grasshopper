using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class SteelDesignEffectiveLengthCache {
    public IApiResult ApiResult { get; set; }

    public IDictionary<int, IList<ISteelDesignEffectiveLength>> Cache { get; }
      = new ConcurrentDictionary<int, IList<ISteelDesignEffectiveLength>>();

    internal SteelDesignEffectiveLengthCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal SteelDesignEffectiveLengthCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public SteelDesignEffectiveLengths ResultSubset(ICollection<int> elementIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(elementIds);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, SteelDesignResult> aCaseResults = analysisCase.Member1dSteelDesignResult(elementList);
            Parallel.ForEach(aCaseResults.Keys, elementId => {
              var res = new SteelDesignEffectiveLength(aCaseResults[elementId].EffectiveLength);
              ((ConcurrentDictionary<int, IList<ISteelDesignEffectiveLength>>)Cache).TryAdd(
                elementId, new Collection<ISteelDesignEffectiveLength>() { res });
            });
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<SteelDesignResult>> cCaseResults = combinationCase.Member1dSteelDesignResult(elementList);
            Parallel.ForEach(cCaseResults.Keys, elementId => {
              var permutationResults = new Collection<ISteelDesignEffectiveLength>();
              foreach (SteelDesignResult permutationResult in cCaseResults[elementId]) {
                permutationResults.Add(new SteelDesignEffectiveLength(permutationResult.EffectiveLength));
              }

              ((ConcurrentDictionary<int, IList<ISteelDesignEffectiveLength>>)Cache).TryAdd(
                elementId, permutationResults);
            });
            break;
        }
      }

      return new SteelDesignEffectiveLengths(Cache.GetSubset(elementIds));
    }
  }
}
