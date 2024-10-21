using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element2dMomentCache
    : IMeshResultCache<IMeshQuantity<IMoment2d>, IMoment2d, ResultTensor2AroundAxis<Entity2dExtremaKey>> {
    public IApiResult ApiResult { get; set; }
    public IDictionary<int, IList<IMeshQuantity<IMoment2d>>> Cache { get; }
      = new ConcurrentDictionary<int, IList<IMeshQuantity<IMoment2d>>>();
    private int _axisId = -10;

    internal Element2dMomentCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element2dMomentCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IMeshResultSubset<IMeshQuantity<IMoment2d>, IMoment2d, ResultTensor2AroundAxis<Entity2dExtremaKey>>
      ResultSubset(ICollection<int> elementIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(elementIds);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Tensor2>> aCaseResults
              = analysisCase.Element2dMoment(elementList, _axisId);
            Parallel.ForEach(aCaseResults.Keys, elementId =>
             ((ConcurrentDictionary<int, IList<IMeshQuantity<IMoment2d>>>)Cache).TryAdd(
              elementId, Entity2dResultsFactory.CreateMoment(aCaseResults[elementId])));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<Tensor2>>> cCaseResults
              = combinationCase.Element2dMoment(elementList, _axisId);
            Parallel.ForEach(cCaseResults.Keys, elementId =>
             ((ConcurrentDictionary<int, IList<IMeshQuantity<IMoment2d>>>)Cache).TryAdd(
              elementId, Entity2dResultsFactory.CreateMoment(cCaseResults[elementId])));
            break;
        }
      }

      return new Entity2dMoment(Cache.GetSubset(elementIds));
    }

    public void SetStandardAxis(int axisId) {
      if (axisId != _axisId) {
        Cache.Clear();
      }

      _axisId = axisId;
    }
  }
}
