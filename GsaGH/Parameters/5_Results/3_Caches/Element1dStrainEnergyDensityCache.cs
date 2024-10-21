using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element1dStrainEnergyDensityCache
    : IEntity1dResultCache<IEnergyDensity, Entity1dExtremaKey> {
    public IApiResult ApiResult { get; set; }

    public IDictionary<int, IList<IEntity1dQuantity<IEnergyDensity>>> Cache { get; }
      = new ConcurrentDictionary<int, IList<IEntity1dQuantity<IEnergyDensity>>>();

    internal Element1dStrainEnergyDensityCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element1dStrainEnergyDensityCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IEntity1dResultSubset<IEnergyDensity, Entity1dExtremaKey>
      ResultSubset(ICollection<int> elementIds, int positionCount) {
      var positions = Enumerable.Range(0, positionCount).Select(
        i => (double)i / (positionCount - 1)).ToList();
      return ResultSubset(elementIds, new ReadOnlyCollection<double>(positions));
    }

    public IEntity1dResultSubset<IEnergyDensity, Entity1dExtremaKey>
      ResultSubset(ICollection<int> elementIds, ReadOnlyCollection<double> positions) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeysAndPositions(elementIds, positions);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        var concurrent = Cache as ConcurrentDictionary<int, IList<IEntity1dQuantity<IEnergyDensity>>>;
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<double>> aCaseResults
              = analysisCase.Element1dStrainEnergyDensity(elementList, positions);
            Parallel.ForEach(aCaseResults.Keys, elementId =>
             concurrent.AddOrUpdate(
              elementId,
              // Add
              Entity1dResultsFactory.CreateResults(
                aCaseResults[elementId], positions, (a, b) => new Entity1dStrainEnergyDensity(a, b)),
              // Update
              (key, oldValue) => oldValue.AddMissingPositions(
                aCaseResults[elementId], positions, (a) => new StrainEnergyDensity(a))));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<double>>> cCaseResults
              = combinationCase.Element1dStrainEnergyDensity(elementList, positions);
            Parallel.ForEach(cCaseResults.Keys, elementId =>
             concurrent.AddOrUpdate(
              elementId,
              // Add
              Entity1dResultsFactory.CreateResults(
                cCaseResults[elementId], positions, (a, b) => new Entity1dStrainEnergyDensity(a, b)),
              // Update
              (key, oldValue) => oldValue.AddMissingPositions(
                cCaseResults[elementId], positions, (a) => new StrainEnergyDensity(a))));
            break;
        }
      }

      return new Entity1dStrainEnergyDensities(Cache.GetSubset(elementIds, positions));
    }

    public void SetStandardAxis(int axisId) {
      throw new NotImplementedException("Strain energy density is independent from chosen axis");
    }
  }
}
