using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Eto.Forms;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Member1dInternalForceCache
    : IElement1dResultCache<IElement1dInternalForce, IInternalForce, ResultVector6<Element1dExtremaKey>> {
    public IApiResult ApiResult { get; set; }

    public ConcurrentDictionary<int, Collection<IElement1dInternalForce>> Cache { get; }
      = new ConcurrentDictionary<int, Collection<IElement1dInternalForce>>();

    internal Member1dInternalForceCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Member1dInternalForceCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IElement1dResultSubset<IElement1dInternalForce, IInternalForce, ResultVector6<Element1dExtremaKey>>
      ResultSubset(ICollection<int> elementIds, int positionCount) {
      var positions = Enumerable.Range(0, positionCount).Select(
        i => (double)i / (positionCount - 1)).ToList();
      return ResultSubset(elementIds, new ReadOnlyCollection<double>(positions));
    }

    public IElement1dResultSubset<IElement1dInternalForce, IInternalForce, ResultVector6<Element1dExtremaKey>>
      ResultSubset(ICollection<int> elementIds, ReadOnlyCollection<double> positions) {
      ConcurrentBag<int> missingIds
        = Cache.GetMissingKeysAndPositions<IElement1dInternalForce, IInternalForce>(elementIds, positions);
      if (missingIds.Count > 0) {
        string memberList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Double6>> aCaseResults
              = analysisCase.Member1dForce(memberList, positions);
            Parallel.ForEach(aCaseResults.Keys, memberId => Cache.AddOrUpdate(
              memberId, Element1dResultsFactory.CreateBeamForces(aCaseResults[memberId], positions),
              (key, oldValue) => oldValue.AddMissingPositions(aCaseResults[memberId], positions)));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<Double6>>> cCaseResults
              = combinationCase.Member1dForce(memberList, positions);
            Parallel.ForEach(cCaseResults.Keys, memberId => Cache.AddOrUpdate(
              memberId, Element1dResultsFactory.CreateBeamForces(cCaseResults[memberId], positions),
              (key, oldValue) => oldValue.AddMissingPositions(cCaseResults[memberId], positions)));
            break;
        }
      }

      return new Element1dInternalForces(Cache.GetSubset(elementIds));
    }
  }
}
