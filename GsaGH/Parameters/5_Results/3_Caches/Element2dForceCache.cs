using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Eto.Forms;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element2dForceCache
    : IEntity2dResultCache<IEntity2dQuantity<IForce2d>, IForce2d, ResultTensor2InAxis<Entity2dExtremaKey>> {
    public IApiResult ApiResult { get; set; }

    public ConcurrentDictionary<int, Collection<IEntity2dQuantity<IForce2d>>> Cache { get; }
      = new ConcurrentDictionary<int, Collection<IEntity2dQuantity<IForce2d>>>();

    internal Element2dForceCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element2dForceCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IEntity2dResultSubset<IEntity2dQuantity<IForce2d>, IForce2d, ResultTensor2InAxis<Entity2dExtremaKey>>
      ResultSubset(ICollection<int> elementIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(elementIds);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Tensor2>> aCaseResults
              = analysisCase.Element2dForce(elementList, 0);
            Parallel.ForEach(aCaseResults.Keys, elementId => Cache.TryAdd(
              elementId, Entity2dResultsFactory.CreateForce(aCaseResults[elementId])));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<Tensor2>>> cCaseResults
              = combinationCase.Element2dForce(elementList, 0);
            Parallel.ForEach(cCaseResults.Keys, elementId => Cache.TryAdd(
              elementId, Entity2dResultsFactory.CreateForce(cCaseResults[elementId])));
            break;
        }
      }

      return new Entity2dForce(Cache.GetSubset(elementIds));
    }
  }
}
