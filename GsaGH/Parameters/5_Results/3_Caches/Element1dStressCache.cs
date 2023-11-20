using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Eto.Forms;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element1dStressCache
    : IEntity1dResultCache<IEntity1dStress, IStress1d, ResultStress1d<Entity1dExtremaKey>> {
    public IApiResult ApiResult { get; set; }

    public ConcurrentDictionary<int, Collection<IEntity1dStress>> Cache { get; }
      = new ConcurrentDictionary<int, Collection<IEntity1dStress>>();

    internal Element1dStressCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element1dStressCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IEntity1dResultSubset<IEntity1dStress, IStress1d, ResultStress1d<Entity1dExtremaKey>>
      ResultSubset(ICollection<int> elementIds, int positionCount) {
      var positions = Enumerable.Range(0, positionCount).Select(
        i => (double)i / (positionCount - 1)).ToList();
      return ResultSubset(elementIds, new ReadOnlyCollection<double>(positions));
    }

    public IEntity1dResultSubset<IEntity1dStress, IStress1d, ResultStress1d<Entity1dExtremaKey>>
      ResultSubset(ICollection<int> elementIds, ReadOnlyCollection<double> positions) {
      ConcurrentBag<int> missingIds
        = Cache.GetMissingKeysAndPositions<IEntity1dStress, IStress1d>(elementIds, positions);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<StressResult1d>> aCaseResults
              = analysisCase.Element1dStress(elementList, positions);
            Parallel.ForEach(aCaseResults.Keys, elementId => Cache.AddOrUpdate(
              elementId, Entity1dResultsFactory.CreateBeamStresses(aCaseResults[elementId], positions),
              (key, oldValue) => oldValue.AddMissingPositions(aCaseResults[elementId], positions)));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<StressResult1d>>> cCaseResults
              = combinationCase.Element1dStress(elementList, positions);
            Parallel.ForEach(cCaseResults.Keys, elementId => Cache.AddOrUpdate(
              elementId, Entity1dResultsFactory.CreateBeamStresses(cCaseResults[elementId], positions),
              (key, oldValue) => oldValue.AddMissingPositions(cCaseResults[elementId], positions)));
            break;
        }
      }

      return new Element1dStresses(Cache.GetSubset(elementIds));
    }
  }
}
