using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element2dStressCache
    : IEntity2dLayeredResultCache<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> {
    public IApiResult ApiResult { get; set; }
    public IDictionary<int, IList<IMeshQuantity<IStress>>> CacheBottomLayer { get; }
      = new ConcurrentDictionary<int, IList<IMeshQuantity<IStress>>>();
    public IDictionary<int, IList<IMeshQuantity<IStress>>> CacheMiddleLayer { get; }
      = new ConcurrentDictionary<int, IList<IMeshQuantity<IStress>>>();
    public IDictionary<int, IList<IMeshQuantity<IStress>>> CacheTopLayer { get; }
      = new ConcurrentDictionary<int, IList<IMeshQuantity<IStress>>>();
    private int _axisId = -10;

    internal Element2dStressCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element2dStressCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>>
      ResultSubset(ICollection<int> elementIds, Layer2d layer) {
      ConcurrentDictionary<int, IList<IMeshQuantity<IStress>>> cache = null;
      GsaAPI.Layer2d fLayer = GsaAPI.Layer2d.Middle;
      switch (layer) {
        case Layer2d.Top:
          cache = (ConcurrentDictionary<int, IList<IMeshQuantity<IStress>>>)CacheTopLayer;
          fLayer = GsaAPI.Layer2d.Top;
          break;

        case Layer2d.Middle:
          cache = (ConcurrentDictionary<int, IList<IMeshQuantity<IStress>>>)CacheMiddleLayer;
          fLayer = GsaAPI.Layer2d.Middle;
          break;

        case Layer2d.Bottom:
          cache = (ConcurrentDictionary<int, IList<IMeshQuantity<IStress>>>)CacheBottomLayer;
          fLayer = GsaAPI.Layer2d.Bottom;
          break;
      }

      ConcurrentBag<int> missingIds = cache.GetMissingKeys(elementIds);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Tensor3>> aCaseResults
              = analysisCase.Element2dStress(elementList, fLayer, _axisId);
            Parallel.ForEach(aCaseResults.Keys, elementId => cache.TryAdd(
              elementId, Entity2dResultsFactory.CreateStresses(aCaseResults[elementId])));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<Tensor3>>> cCaseResults
              = combinationCase.Element2dStress(elementList, fLayer, _axisId);
            Parallel.ForEach(cCaseResults.Keys, elementId => cache.TryAdd(
              elementId, Entity2dResultsFactory.CreateStresses(cCaseResults[elementId])));
            break;
        }
      }

      return new MeshStresses(cache.GetSubset(elementIds));
    }

    public void SetStandardAxis(int axisId) {
      if (axisId != _axisId) {
        CacheTopLayer.Clear();
        CacheMiddleLayer.Clear();
        CacheBottomLayer.Clear();
      }

      _axisId = axisId;
    }
  }
}
