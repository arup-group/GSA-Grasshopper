using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Eto.Forms;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Member1dInternalForceCache
    : IEntity1dResultCache<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>> {
    public IApiResult ApiResult { get; set; }

    public ConcurrentDictionary<int, Collection<IEntity1dInternalForce>> Cache { get; }
      = new ConcurrentDictionary<int, Collection<IEntity1dInternalForce>>();

    internal Member1dInternalForceCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Member1dInternalForceCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>>
      ResultSubset(ICollection<int> memberIds, int positionCount) {
      var positions = Enumerable.Range(0, positionCount).Select(
        i => (double)i / (positionCount - 1)).ToList();
      return ResultSubset(memberIds, new ReadOnlyCollection<double>(positions));
    }

    public IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>>
      ResultSubset(ICollection<int> memberIds, ReadOnlyCollection<double> positions) {
      ConcurrentBag<int> missingIds
        = Cache.GetMissingKeysAndPositions<IEntity1dInternalForce, IInternalForce>(memberIds, positions);
      if (missingIds.Count > 0) {
        string memberList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Double6>> aCaseResults
              = analysisCase.Member1dForce(memberList, positions);
            Parallel.ForEach(aCaseResults.Keys, memberId => Cache.AddOrUpdate(
              memberId, Entity1dResultsFactory.CreateEntity1dForces(aCaseResults[memberId], positions),
              (key, oldValue) => oldValue.AddMissingPositions(aCaseResults[memberId], positions)));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<Double6>>> cCaseResults
              = combinationCase.Member1dForce(memberList, positions);
            Parallel.ForEach(cCaseResults.Keys, memberId => Cache.AddOrUpdate(
              memberId, Entity1dResultsFactory.CreateEntity1dForces(cCaseResults[memberId], positions),
              (key, oldValue) => oldValue.AddMissingPositions(cCaseResults[memberId], positions)));
            break;
        }
      }

      return new Entity1dInternalForces(Cache.GetSubset(memberIds));
    }
  }
}
