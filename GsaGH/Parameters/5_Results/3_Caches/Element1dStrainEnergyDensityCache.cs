using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element1dStrainEnergyDensityCache
    : IEntity1dResultCache<IEntity1dStrainEnergyDensity, IEnergyDensity, Entity1dExtremaKey> {
    public IApiResult ApiResult { get; set; }

    public IDictionary<int, IList<IEntity1dStrainEnergyDensity>> Cache { get; }
      = new ConcurrentDictionary<int, IList<IEntity1dStrainEnergyDensity>>();

    internal Element1dStrainEnergyDensityCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element1dStrainEnergyDensityCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IEntity1dResultSubset<IEntity1dStrainEnergyDensity, IEnergyDensity, Entity1dExtremaKey>
      ResultSubset(ICollection<int> elementIds, int positionCount) {
      var positions = Enumerable.Range(0, positionCount).Select(
        i => (double)i / (positionCount - 1)).ToList();
      return ResultSubset(elementIds, new ReadOnlyCollection<double>(positions));
    }

    public IEntity1dResultSubset<IEntity1dStrainEnergyDensity, IEnergyDensity, Entity1dExtremaKey>
      ResultSubset(ICollection<int> elementIds, ReadOnlyCollection<double> positions) {
      ConcurrentBag<int> missingIds
        = Cache.GetMissingKeysAndPositions<IEntity1dStrainEnergyDensity, IEnergyDensity>(elementIds, positions);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<double>> aCaseResults
              = analysisCase.Element1dStrainEnergyDensity(elementList, positions);
            Parallel.ForEach(aCaseResults.Keys, elementId =>
             ((ConcurrentDictionary<int, IList<IEntity1dStrainEnergyDensity>>)Cache).AddOrUpdate(
              elementId,
              Entity1dResultsFactory.CreateStrainEnergyDensities(aCaseResults[elementId], positions),
              (key, oldValue) => oldValue.AddMissingPositions(aCaseResults[elementId], positions)));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<double>>> cCaseResults
              = combinationCase.Element1dStrainEnergyDensity(elementList, positions);
            Parallel.ForEach(cCaseResults.Keys, elementId =>
             ((ConcurrentDictionary<int, IList<IEntity1dStrainEnergyDensity>>)Cache).AddOrUpdate(
              elementId,
              Entity1dResultsFactory.CreateStrainEnergyDensities(cCaseResults[elementId], positions),
              (key, oldValue) => oldValue.AddMissingPositions(cCaseResults[elementId], positions)));
            break;
        }
      }

      return new Entity1dStrainEnergyDensities(
        Cache.GetSubset<IEntity1dStrainEnergyDensity, IEnergyDensity>(elementIds, positions));
    }
  }
}
