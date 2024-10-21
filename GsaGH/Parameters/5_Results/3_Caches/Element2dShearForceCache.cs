using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element2dShearForceCache
    : IMeshResultCache<IMeshQuantity<IShear2d>, IShear2d, ResultVector2<Entity2dExtremaKey>> {
    public IApiResult ApiResult { get; set; }
    public IDictionary<int, IList<IMeshQuantity<IShear2d>>> Cache { get; }
      = new ConcurrentDictionary<int, IList<IMeshQuantity<IShear2d>>>();
    private int _axisId = -10;

    internal Element2dShearForceCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element2dShearForceCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IMeshResultSubset<IMeshQuantity<IShear2d>, IShear2d, ResultVector2<Entity2dExtremaKey>>
      ResultSubset(ICollection<int> elementIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(elementIds);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Vector2>> aCaseResults
              = analysisCase.Element2dShear(elementList, _axisId);
            Parallel.ForEach(aCaseResults.Keys, elementId =>
             ((ConcurrentDictionary<int, IList<IMeshQuantity<IShear2d>>>)Cache).TryAdd(
              elementId, Entity2dResultsFactory.CreateShearForce(aCaseResults[elementId])));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<Vector2>>> cCaseResults
              = combinationCase.Element2dShear(elementList, _axisId);
            Parallel.ForEach(cCaseResults.Keys, elementId =>
             ((ConcurrentDictionary<int, IList<IMeshQuantity<IShear2d>>>)Cache).TryAdd(
              elementId, Entity2dResultsFactory.CreateShearForce(cCaseResults[elementId])));
            break;
        }
      }

      return new Entity2dShearForce(Cache.GetSubset(elementIds));
    }

    public void SetStandardAxis(int axisId) {
      if (axisId != _axisId) {
        Cache.Clear();
      }

      _axisId = axisId;
    }
  }
}
