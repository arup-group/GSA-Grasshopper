using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Eto.Forms;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element1dInternalForceCache
    : IEntity1dResultCache<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>> {
    public IApiResult ApiResult { get; set; }

    public ConcurrentDictionary<int, Collection<IEntity1dInternalForce>> Cache { get; }
      = new ConcurrentDictionary<int, Collection<IEntity1dInternalForce>>();

    internal Element1dInternalForceCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element1dInternalForceCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>>
      ResultSubset(ICollection<int> elementIds, int positionCount) {
      var positions = Enumerable.Range(0, positionCount).Select(
        i => (double)i / (positionCount - 1)).ToList();
      return ResultSubset(elementIds, new ReadOnlyCollection<double>(positions));
    }

    public IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>>
      ResultSubset(ICollection<int> elementIds, ReadOnlyCollection<double> positions) {
      ConcurrentBag<int> missingIds
        = Cache.GetMissingKeysAndPositions<IEntity1dInternalForce, IInternalForce>(elementIds, positions);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Double6>> aCaseResults
              = analysisCase.Element1dForce(elementList, positions);
            Parallel.ForEach(aCaseResults.Keys, elementId => Cache.AddOrUpdate(
              elementId, Entity1dResultsFactory.CreateEntity1dForces(aCaseResults[elementId], positions),
              (key, oldValue) => oldValue.AddMissingPositions(aCaseResults[elementId], positions)));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<Double6>>> cCaseResults
              = combinationCase.Element1dForce(elementList, positions);
            Parallel.ForEach(cCaseResults.Keys, elementId => Cache.AddOrUpdate(
              elementId, Entity1dResultsFactory.CreateEntity1dForces(cCaseResults[elementId], positions),
              (key, oldValue) => oldValue.AddMissingPositions(cCaseResults[elementId], positions)));
            break;
        }
      }

      return new Entity1dInternalForces(Cache.GetSubset(elementIds));
    }
  }
}
