using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element1dStressCache
    : IEntity1dResultCache<IStress1d, ResultStress1d<Entity1dExtremaKey>> {
    public IApiResult ApiResult { get; set; }
    public IDictionary<int, IList<IEntity1dQuantity<IStress1d>>> Cache { get; }
      = new ConcurrentDictionary<int, IList<IEntity1dQuantity<IStress1d>>>();
    private int _axisId = -10;

    internal Element1dStressCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element1dStressCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IEntity1dResultSubset<IStress1d, ResultStress1d<Entity1dExtremaKey>>
      ResultSubset(ICollection<int> elementIds, int positionCount) {
      var positions = Enumerable.Range(0, positionCount).Select(
        i => (double)i / (positionCount - 1)).ToList();
      return ResultSubset(elementIds, new ReadOnlyCollection<double>(positions));
    }

    public IEntity1dResultSubset<IStress1d, ResultStress1d<Entity1dExtremaKey>>
      ResultSubset(ICollection<int> elementIds, ReadOnlyCollection<double> positions) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeysAndPositions(elementIds, positions);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        var concurrent = Cache as ConcurrentDictionary<int, IList<IEntity1dQuantity<IStress1d>>>;
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<StressResult1d>> aCaseResults
              = analysisCase.Element1dStress(elementList, positions, _axisId);
            Parallel.ForEach(aCaseResults.Keys, elementId =>
             concurrent.AddOrUpdate(
              elementId,
              // Add
              Entity1dResultsFactory.CreateResults(
                aCaseResults[elementId], positions, (a, b) => new Entity1dStress(a, b)),
              // Update
              (key, oldValue) => oldValue.AddMissingPositions(
                aCaseResults[elementId], positions, (a) => new Stress1d(a))));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<StressResult1d>>> cCaseResults
              = combinationCase.Element1dStress(elementList, positions, _axisId);
            Parallel.ForEach(cCaseResults.Keys, elementId =>
            concurrent.AddOrUpdate(
              elementId,
              // Add
              Entity1dResultsFactory.CreateResults(
                cCaseResults[elementId], positions, (a, b) => new Entity1dStress(a, b)),
              // Update
              (key, oldValue) => oldValue.AddMissingPositions(
                cCaseResults[elementId], positions, (a) => new Stress1d(a))));
            break;
        }
      }

      return new Entity1dStresses(Cache.GetSubset(elementIds, positions));
    }

    public void SetStandardAxis(int axisId) {
      if (axisId != _axisId) {
        Cache.Clear();
      }

      _axisId = axisId;
    }
  }
}
