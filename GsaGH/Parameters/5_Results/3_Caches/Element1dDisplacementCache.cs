using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Eto.Forms;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element1dDisplacementCache
    : IEntity1dResultCache<IEntity1dDisplacement, IDisplacement, ResultVector6<Entity1dExtremaKey>> {
    public IApiResult ApiResult { get; set; }

    public ConcurrentDictionary<int, Collection<IEntity1dDisplacement>> Cache { get; }
      = new ConcurrentDictionary<int, Collection<IEntity1dDisplacement>>();

    internal Element1dDisplacementCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element1dDisplacementCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IEntity1dResultSubset<IEntity1dDisplacement, IDisplacement, ResultVector6<Entity1dExtremaKey>>
      ResultSubset(ICollection<int> elementIds, int positionCount) {
      var positions = Enumerable.Range(0, positionCount).Select(
        i => (double)i / (positionCount - 1)).ToList();
      return ResultSubset(elementIds, new ReadOnlyCollection<double>(positions));
    }

    public IEntity1dResultSubset<IEntity1dDisplacement, IDisplacement, ResultVector6<Entity1dExtremaKey>>
      ResultSubset(ICollection<int> elementIds, ReadOnlyCollection<double> positions) {
      ConcurrentBag<int> missingIds
        = Cache.GetMissingKeysAndPositions<IEntity1dDisplacement, IDisplacement>(elementIds, positions);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Double6>> aCaseResults
              = analysisCase.Element1dDisplacement(elementList, positions);
            Parallel.ForEach(aCaseResults.Keys, elementId => Cache.AddOrUpdate(
              elementId, Entity1dResultsFactory.CreateEntity1dDisplacements(aCaseResults[elementId], positions),
              (key, oldValue) => oldValue.AddMissingPositions(aCaseResults[elementId], positions)));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<Double6>>> cCaseResults
              = combinationCase.Element1dDisplacement(elementList, positions);
            Parallel.ForEach(cCaseResults.Keys, elementId => Cache.AddOrUpdate(
              elementId, Entity1dResultsFactory.CreateEntity1dDisplacements(cCaseResults[elementId], positions),
              (key, oldValue) => oldValue.AddMissingPositions(cCaseResults[elementId], positions)));
            break;
        }
      }

      return new Entity1dDisplacements(Cache.GetSubset(elementIds));
    }
  }
}
