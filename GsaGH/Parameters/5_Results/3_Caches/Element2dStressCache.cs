using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element2dStressCache
    : IEntity2dLayeredResultCache<IEntity2dQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> {
    public IApiResult ApiResult { get; set; }

    public ConcurrentDictionary<int, Collection<IEntity2dQuantity<IStress>>> CacheBottomLayer { get; }
      = new ConcurrentDictionary<int, Collection<IEntity2dQuantity<IStress>>>();
    public ConcurrentDictionary<int, Collection<IEntity2dQuantity<IStress>>> CacheMiddleLayer { get; }
      = new ConcurrentDictionary<int, Collection<IEntity2dQuantity<IStress>>>();
    public ConcurrentDictionary<int, Collection<IEntity2dQuantity<IStress>>> CacheTopLayer { get; }
      = new ConcurrentDictionary<int, Collection<IEntity2dQuantity<IStress>>>();

    internal Element2dStressCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element2dStressCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IEntity2dResultSubset<IEntity2dQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>>
      ResultSubset(ICollection<int> elementIds, Layer2d layer) {
      ConcurrentDictionary<int, Collection<IEntity2dQuantity<IStress>>> cache = null;
      double fLayer = 0;
      switch (layer) {
        case Layer2d.Top:
          cache = CacheTopLayer;
          fLayer = 1;
          break;

        case Layer2d.Middle:
          cache = CacheMiddleLayer;
          fLayer = 0;
          break;

        case Layer2d.Bottom:
          cache = CacheBottomLayer;
          fLayer = -1;
          break;
      }

      ConcurrentBag<int> missingIds = cache.GetMissingKeys(elementIds);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Tensor3>> aCaseResults
              = analysisCase.Element2dStress(elementList, fLayer);
            Parallel.ForEach(aCaseResults.Keys, elementId => cache.TryAdd(
              elementId, Entity2dResultsFactory.CreateStresses(aCaseResults[elementId])));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<Tensor3>>> cCaseResults
              = combinationCase.Element2dStress(elementList, fLayer);
            Parallel.ForEach(cCaseResults.Keys, elementId => cache.TryAdd(
              elementId, Entity2dResultsFactory.CreateStresses(cCaseResults[elementId])));
            break;
        }
      }

      return new Entity2dStresses(cache.GetSubset(elementIds));
    }
  }
}
