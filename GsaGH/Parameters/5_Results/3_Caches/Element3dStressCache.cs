using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element3dStressCache
    : IMeshResultCache<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> {
    public IApiResult ApiResult { get; set; }
    public IDictionary<int, IList<IMeshQuantity<IStress>>> Cache { get; }
      = new ConcurrentDictionary<int, IList<IMeshQuantity<IStress>>>();
    private int _axisId = -10;

    internal Element3dStressCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element3dStressCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>>
      ResultSubset(ICollection<int> elementIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(elementIds);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Tensor3>> aCaseResults
              = analysisCase.Element3dStress(elementList, _axisId);
            Parallel.ForEach(aCaseResults.Keys, elementId =>
             ((ConcurrentDictionary<int, IList<IMeshQuantity<IStress>>>)Cache).TryAdd(
              elementId, Entity3dResultsFactory.CreateStresses(aCaseResults[elementId])));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<Tensor3>>> cCaseResults
              = combinationCase.Element3dStress(elementList, _axisId);
            Parallel.ForEach(cCaseResults.Keys, elementId =>
             ((ConcurrentDictionary<int, IList<IMeshQuantity<IStress>>>)Cache).TryAdd(
              elementId, Entity3dResultsFactory.CreateStresses(cCaseResults[elementId])));
            break;
        }
      }

      return new MeshStresses(Cache.GetSubset(elementIds));
    }

    public void SetStandardAxis(int axisId) {
      if (axisId != _axisId) {
        Cache.Clear();
      }

      _axisId = axisId;
    }
  }
}
