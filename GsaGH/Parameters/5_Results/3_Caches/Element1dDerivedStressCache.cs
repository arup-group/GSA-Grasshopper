using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Eto.Forms;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element1dDerivedStressCache
    : IEntity1dResultCache<IEntity1dDerivedStress, IStress1dDerived, ResultDerivedStress1d<Entity1dExtremaKey>> {
    public IApiResult ApiResult { get; set; }

    public IDictionary<int, IList<IEntity1dDerivedStress>> Cache { get; }
      = new ConcurrentDictionary<int, IList<IEntity1dDerivedStress>>();

    internal Element1dDerivedStressCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element1dDerivedStressCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IEntity1dResultSubset<IEntity1dDerivedStress, IStress1dDerived, ResultDerivedStress1d<Entity1dExtremaKey>>
      ResultSubset(ICollection<int> elementIds, int positionCount) {
      var positions = Enumerable.Range(0, positionCount).Select(
        i => (double)i / (positionCount - 1)).ToList();
      return ResultSubset(elementIds, new ReadOnlyCollection<double>(positions));
    }

    public IEntity1dResultSubset<IEntity1dDerivedStress, IStress1dDerived, ResultDerivedStress1d<Entity1dExtremaKey>>
      ResultSubset(ICollection<int> elementIds, ReadOnlyCollection<double> positions) {
      ConcurrentBag<int> missingIds
        = Cache.GetMissingKeysAndPositions<IEntity1dDerivedStress, IStress1dDerived>(elementIds, positions);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<DerivedStressResult1d>> aCaseResults
              = analysisCase.Element1dDerivedStress(elementList, positions);
            Parallel.ForEach(aCaseResults.Keys, elementId => 
             ((ConcurrentDictionary<int, IList<IEntity1dDerivedStress>>)Cache).AddOrUpdate(
              elementId, Entity1dResultsFactory.CreateDerivedStresses(aCaseResults[elementId], positions),
              (key, oldValue) => oldValue.AddMissingPositions(aCaseResults[elementId], positions)));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<DerivedStressResult1d>>> cCaseResults
              = combinationCase.Element1dDerivedStress(elementList, positions);
            Parallel.ForEach(cCaseResults.Keys, elementId => 
             ((ConcurrentDictionary<int, IList<IEntity1dDerivedStress>>)Cache).AddOrUpdate(
              elementId, Entity1dResultsFactory.CreateDerivedStresses(cCaseResults[elementId], positions),
              (key, oldValue) => oldValue.AddMissingPositions(cCaseResults[elementId], positions)));
            break;
        }
      }

      return new Entity1dDerivedStresses(Cache.GetSubset(elementIds));
    }
  }
}
