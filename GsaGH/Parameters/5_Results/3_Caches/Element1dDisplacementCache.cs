using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Eto.Forms;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element1dDisplacementCache 
    : IElement1dResultCache<IElement1dDisplacement, IDisplacement, ResultVector6<Element1dExtremaKey>> {
    public IApiResult ApiResult { get; set; }

    public ConcurrentDictionary<int, Collection<IElement1dDisplacement>> Cache { get; }
      = new ConcurrentDictionary<int, Collection<IElement1dDisplacement>>();

    internal Element1dDisplacementCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element1dDisplacementCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IElement1dResultSubset<IElement1dDisplacement, IDisplacement, ResultVector6<Element1dExtremaKey>> 
      ResultSubset(ICollection<int> elementIds, int positionCount) {
      var positions = Enumerable.Range(0, positionCount).Select(
        i => (double)i / (positionCount - 1)).ToList();
      return ResultSubset(elementIds, new ReadOnlyCollection<double>(positions));
    }

    public IElement1dResultSubset<IElement1dDisplacement, IDisplacement, ResultVector6<Element1dExtremaKey>> 
      ResultSubset(ICollection<int> elementIds, ReadOnlyCollection<double> positions) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(elementIds);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Double6>> aCaseResults 
              = analysisCase.Element1dDisplacement(elementList, positions);
            Parallel.ForEach(missingIds, elementId => Cache.AddOrUpdate(
              elementId, Element1dResultsFactory.CreateBeamDisplacements(aCaseResults[elementId], positions),
              (key, oldValue) => oldValue.AddMissingPositions(aCaseResults[elementId], positions)));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<Double6>>> cCaseResults 
              = combinationCase.Element1dDisplacement(elementList, positions);
            Parallel.ForEach(missingIds, elementId => Cache.AddOrUpdate(
              elementId, Element1dResultsFactory.CreateBeamDisplacements(cCaseResults[elementId], positions),
              (key, oldValue) => oldValue.AddMissingPositions(cCaseResults[elementId], positions)));
            break;
        }
      }

      return new Element1dDisplacements(Cache.GetSubset(elementIds));
    }
  }
}
