using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element1dDisplacementCache : IElement1dResultCache<IDisplacement1D, IDisplacement,
    ResultVector6<ExtremaKey1D>> {

    internal Element1dDisplacementCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element1dDisplacementCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IApiResult ApiResult { get; set; }

    public ConcurrentDictionary<int, Collection<IDisplacement1D>> Cache { get; }
      = new ConcurrentDictionary<int, Collection<IDisplacement1D>>();

    public IElement1dResultSubset<IDisplacement1D, IDisplacement, ResultVector6<ExtremaKey1D>>
      ResultSubset(ICollection<int> elementIds, int positionCount) {
      var positions = Enumerable.Range(0, positionCount)
       .Select(i => (double)i / (positionCount - 1)).ToList();
      return ResultSubset(elementIds, new ReadOnlyCollection<double>(positions));
    }

    public IElement1dResultSubset<IDisplacement1D, IDisplacement, ResultVector6<ExtremaKey1D>>
      ResultSubset(ICollection<int> elementIds, ReadOnlyCollection<double> positions) {
      ConcurrentBag<int> missingIds
        = Cache.GetMissingKeysAndPositions<IDisplacement1D, IDisplacement>(elementIds, positions);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Double6>> aCaseResults
              = analysisCase.Element1dDisplacement(elementList, positions);
            Parallel.ForEach(aCaseResults.Keys,
              elementId => Cache.AddOrUpdate(elementId,
                Element1dResultsFactory.CreateBeamDisplacements(aCaseResults[elementId], positions),
                (key, oldValue)
                  => oldValue.AddMissingPositions(aCaseResults[elementId], positions)));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<Double6>>> cCaseResults
              = combinationCase.Element1dDisplacement(elementList, positions);
            Parallel.ForEach(cCaseResults.Keys,
              elementId => Cache.AddOrUpdate(elementId,
                Element1dResultsFactory.CreateBeamDisplacements(cCaseResults[elementId], positions),
                (key, oldValue)
                  => oldValue.AddMissingPositions(cCaseResults[elementId], positions)));
            break;
        }
      }

      return new Displacements1D(Cache.GetSubset(elementIds));
    }
  }
}
