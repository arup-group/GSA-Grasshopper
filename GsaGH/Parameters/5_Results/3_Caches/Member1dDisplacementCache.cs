using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Member1dDisplacementCache : IEntity1dResultCache<IEntity1dDisplacement, IDisplacement,
    ResultVector6<Entity1dExtremaKey>> {

    internal Member1dDisplacementCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Member1dDisplacementCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IApiResult ApiResult { get; set; }

    public ConcurrentDictionary<int, Collection<IEntity1dDisplacement>> Cache { get; }
      = new ConcurrentDictionary<int, Collection<IEntity1dDisplacement>>();

    public IEntity1dResultSubset<IEntity1dDisplacement, IDisplacement, ResultVector6<Entity1dExtremaKey>>
      ResultSubset(ICollection<int> memberIds, int positionCount) {
      var positions = Enumerable.Range(0, positionCount)
       .Select(i => (double)i / (positionCount - 1)).ToList();
      return ResultSubset(memberIds, new ReadOnlyCollection<double>(positions));
    }

    public IEntity1dResultSubset<IEntity1dDisplacement, IDisplacement, ResultVector6<Entity1dExtremaKey>>
      ResultSubset(ICollection<int> memberIds, ReadOnlyCollection<double> positions) {
      ConcurrentBag<int> missingIds
        = Cache.GetMissingKeysAndPositions<IEntity1dDisplacement, IDisplacement>(memberIds, positions);
      if (missingIds.Count > 0) {
        string memberList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Double6>> aCaseResults
              = analysisCase.Member1dDisplacement(memberList, positions);
            Parallel.ForEach(aCaseResults.Keys,
              memberId => Cache.AddOrUpdate(memberId,
                Entity1dResultsFactory.CreateBeamDisplacements(aCaseResults[memberId], positions),
                (key, oldValue)
                  => oldValue.AddMissingPositions(aCaseResults[memberId], positions)));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<Double6>>> cCaseResults
              = combinationCase.Member1dDisplacement(memberList, positions);
            Parallel.ForEach(cCaseResults.Keys,
              memberId => Cache.AddOrUpdate(memberId,
                Entity1dResultsFactory.CreateBeamDisplacements(cCaseResults[memberId], positions),
                (key, oldValue)
                  => oldValue.AddMissingPositions(cCaseResults[memberId], positions)));
            break;
        }
      }

      return new Entity1dDisplacements(Cache.GetSubset(memberIds));
    }
  }
}