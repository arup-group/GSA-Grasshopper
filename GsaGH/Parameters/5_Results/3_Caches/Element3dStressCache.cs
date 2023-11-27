using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element3dStressCache
    : IEntity2dResultCache<IEntity2dQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> {
    public IApiResult ApiResult { get; set; }

    public ConcurrentDictionary<int, Collection<IEntity2dQuantity<IStress>>> Cache { get; }
      = new ConcurrentDictionary<int, Collection<IEntity2dQuantity<IStress>>>();

    internal Element3dStressCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element3dStressCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IEntity2dResultSubset<IEntity2dQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>>
      ResultSubset(ICollection<int> elementIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(elementIds);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Tensor3>> aCaseResults
              = analysisCase.Element3dStress(elementList);
            Parallel.ForEach(aCaseResults.Keys, elementId => Cache.TryAdd(
              elementId, Entity3dResultsFactory.CreateStresses(aCaseResults[elementId])));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<Tensor3>>> cCaseResults
              = combinationCase.Element3dStress(elementList);
            Parallel.ForEach(cCaseResults.Keys, elementId => Cache.TryAdd(
              elementId, Entity3dResultsFactory.CreateStresses(cCaseResults[elementId])));
            break;
        }
      }

      return new Entity2dStresses(Cache.GetSubset(elementIds));
    }
  }
}
