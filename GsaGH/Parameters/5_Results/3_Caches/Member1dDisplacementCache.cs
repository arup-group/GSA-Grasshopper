using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Member1dDisplacementCache : IResultCache1d<IDisplacement1d, IDisplacement,
    ResultVector6<ExtremaKey1d>> {

    internal Member1dDisplacementCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Member1dDisplacementCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IApiResult ApiResult { get; set; }

    public ConcurrentDictionary<int, Collection<IDisplacement1d>> Cache { get; }
      = new ConcurrentDictionary<int, Collection<IDisplacement1d>>();

    public IResultSubset1d<IDisplacement1d, IDisplacement, ResultVector6<ExtremaKey1d>>
      ResultSubset(ICollection<int> elementIds, int positionCount) {
      var positions = Enumerable.Range(0, positionCount)
       .Select(i => (double)i / (positionCount - 1)).ToList();
      return ResultSubset(elementIds, new ReadOnlyCollection<double>(positions));
    }

    public IResultSubset1d<IDisplacement1d, IDisplacement, ResultVector6<ExtremaKey1d>>
      ResultSubset(ICollection<int> elementIds, ReadOnlyCollection<double> positions) {
      ConcurrentBag<int> missingIds
        = Cache.GetMissingKeysAndPositions<IDisplacement1d, IDisplacement>(elementIds, positions);
      if (missingIds.Count > 0) {
        string memberList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Double6>> aCaseResults
              = analysisCase.Member1dDisplacement(memberList, 5);
            Parallel.ForEach(aCaseResults.Keys,
              memberId => Cache.AddOrUpdate(memberId,
                ResultsFactory1d.CreateBeamDisplacements(aCaseResults[memberId], positions),
                (key, oldValue)
                  => oldValue.AddMissingPositions(aCaseResults[memberId], positions)));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<Double6>>> cCaseResults
              = combinationCase.Member1dDisplacement(memberList, positions);
            Parallel.ForEach(cCaseResults.Keys,
              memberId => Cache.AddOrUpdate(memberId,
                ResultsFactory1d.CreateBeamDisplacements(cCaseResults[memberId], positions),
                (key, oldValue)
                  => oldValue.AddMissingPositions(cCaseResults[memberId], positions)));
            break;
        }
      }

      return new Displacements1d(Cache.GetSubset(elementIds));
    }
  }
}
