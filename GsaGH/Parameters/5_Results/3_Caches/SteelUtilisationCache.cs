using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class SteelUtilisationCache : IEntity0dResultCache<ISteelUtilisation, SteelUtilisationExtremaKeys> {
    public IApiResult ApiResult { get; set; }

    public IDictionary<int, IList<ISteelUtilisation>> Cache { get; }
      = new ConcurrentDictionary<int, IList<ISteelUtilisation>>();

    internal SteelUtilisationCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal SteelUtilisationCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IEntity0dResultSubset<ISteelUtilisation, SteelUtilisationExtremaKeys> ResultSubset(ICollection<int> memberIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(memberIds);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, SteelDesignResult> aCaseResults = analysisCase.Member1dSteelDesignResult(elementList);
            Parallel.ForEach(aCaseResults.Keys, elementId => {
              var res = new SteelUtilisation(aCaseResults[elementId].Utilisation);
              ((ConcurrentDictionary<int, IList<ISteelUtilisation>>)Cache).TryAdd(
                elementId, new Collection<ISteelUtilisation>() { res });
            });
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<SteelDesignResult>> cCaseResults = combinationCase.Member1dSteelDesignResult(elementList);
            Parallel.ForEach(cCaseResults.Keys, elementId => {
              var permutationResults = new Collection<ISteelUtilisation>();
              foreach (SteelDesignResult permutationResult in cCaseResults[elementId]) {
                permutationResults.Add(new SteelUtilisation(permutationResult.Utilisation));
              }

              ((ConcurrentDictionary<int, IList<ISteelUtilisation>>)Cache).TryAdd(
                elementId, permutationResults);
            });
            break;
        }
      }

      return new SteelUtilisations(Cache.GetSubset(memberIds));
    }

    public void SetStandardAxis(int axisId) {
      throw new NotImplementedException("Steel utilisation is independent from chosen axis");
    }
  }
}
