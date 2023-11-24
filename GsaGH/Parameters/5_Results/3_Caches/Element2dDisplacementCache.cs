using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Eto.Forms;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element2dDisplacementCache
    : IEntity2dResultCache<IEntity2dQuantity<IDisplacement>, IDisplacement, ResultVector6<Entity2dExtremaKey>> {
    public IApiResult ApiResult { get; set; }

    public ConcurrentDictionary<int, Collection<IEntity2dQuantity<IDisplacement>>> Cache { get; }
      = new ConcurrentDictionary<int, Collection<IEntity2dQuantity<IDisplacement>>>();

    internal Element2dDisplacementCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element2dDisplacementCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IEntity2dResultSubset<IEntity2dQuantity<IDisplacement>, IDisplacement, ResultVector6<Entity2dExtremaKey>>
      ResultSubset(ICollection<int> elementIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(elementIds);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Double6>> aCaseResults
              = analysisCase.Element2dDisplacement(elementList, 0);
            Parallel.ForEach(aCaseResults.Keys, elementId => Cache.TryAdd(
              elementId, Entity2dResultsFactory.CreateDisplacements(aCaseResults[elementId])));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<Double6>>> cCaseResults
              = combinationCase.Element2dDisplacement(elementList, 0);
            Parallel.ForEach(cCaseResults.Keys, elementId => Cache.TryAdd(
              elementId, Entity2dResultsFactory.CreateDisplacements(cCaseResults[elementId])));
            break;
        }
      }

      return new Entity2dDisplacements(Cache.GetSubset(elementIds));
    }
  }
}
