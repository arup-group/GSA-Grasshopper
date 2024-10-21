using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element2dDisplacementCache
    : IMeshResultCache<IMeshQuantity<IDisplacement>, IDisplacement, ResultVector6<Entity2dExtremaKey>> {
    public IApiResult ApiResult { get; set; }
    public IDictionary<int, IList<IMeshQuantity<IDisplacement>>> Cache { get; }
      = new ConcurrentDictionary<int, IList<IMeshQuantity<IDisplacement>>>();
    private int _axisId = -10;

    internal Element2dDisplacementCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element2dDisplacementCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IMeshResultSubset<IMeshQuantity<IDisplacement>, IDisplacement, ResultVector6<Entity2dExtremaKey>>
      ResultSubset(ICollection<int> elementIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(elementIds);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Double6>> aCaseResults
              = analysisCase.Element2dDisplacement(elementList, _axisId);
            Parallel.ForEach(aCaseResults.Keys, elementId =>
             ((ConcurrentDictionary<int, IList<IMeshQuantity<IDisplacement>>>)Cache).TryAdd(
              elementId, Entity2dResultsFactory.CreateDisplacements(aCaseResults[elementId])));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<Double6>>> cCaseResults
              = combinationCase.Element2dDisplacement(elementList, _axisId);
            Parallel.ForEach(cCaseResults.Keys, elementId =>
             ((ConcurrentDictionary<int, IList<IMeshQuantity<IDisplacement>>>)Cache).TryAdd(
              elementId, Entity2dResultsFactory.CreateDisplacements(cCaseResults[elementId])));
            break;
        }
      }

      return new Entity2dDisplacements(Cache.GetSubset(elementIds));
    }

    public void SetStandardAxis(int axisId) {
      if (axisId != _axisId) {
        Cache.Clear();
      }

      _axisId = axisId;
    }
  }
}
