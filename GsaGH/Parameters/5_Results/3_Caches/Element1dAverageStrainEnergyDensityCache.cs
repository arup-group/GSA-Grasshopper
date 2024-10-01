using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element1dAverageStrainEnergyDensityCache : IEntity0dResultCache<IEnergyDensity, Entity0dExtremaKey> {
    public IApiResult ApiResult { get; set; }

    public IDictionary<int, IList<IEnergyDensity>> Cache { get; }
      = new ConcurrentDictionary<int, IList<IEnergyDensity>>();

    internal Element1dAverageStrainEnergyDensityCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element1dAverageStrainEnergyDensityCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IEntity0dResultSubset<IEnergyDensity, Entity0dExtremaKey> ResultSubset(ICollection<int> elementIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(elementIds);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, double> aCaseResults = analysisCase.Element1dAverageStrainEnergyDensity(elementList);
            Parallel.ForEach(aCaseResults.Keys, elementId => {
              var res = new StrainEnergyDensity(aCaseResults[elementId]);
              ((ConcurrentDictionary<int, IList<IEnergyDensity>>)Cache).TryAdd(
                elementId, new Collection<IEnergyDensity>() { res });
            });
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<double>> cCaseResults = combinationCase.Element1dAverageStrainEnergyDensity(elementList);
            Parallel.ForEach(cCaseResults.Keys, elementId => {
              var permutationResults = new Collection<IEnergyDensity>();
              foreach (double permutationResult in cCaseResults[elementId]) {
                permutationResults.Add(new StrainEnergyDensity(permutationResult));
              }

              ((ConcurrentDictionary<int, IList<IEnergyDensity>>)Cache).TryAdd(
                elementId, permutationResults);
            });
            break;
        }
      }

      return new Entity1dAverageStrainEnergyDensity(Cache.GetSubset(elementIds));
    }

    public void SetStandardAxis(int axisId) {
      throw new NotImplementedException("Strain energy density is independent from chosen axis");
    }
  }
}
