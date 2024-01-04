﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;
using GsaGH.Helpers.Import;

namespace GsaGH.Parameters.Results {
  public class Member1dDisplacementCache : IEntity1dResultCache<IDisplacement, ResultVector6<Entity1dExtremaKey>> {
    public IApiResult ApiResult { get; set; }

    public IDictionary<int, IList<IEntity1dQuantity<IDisplacement>>> Cache { get; }
      = new ConcurrentDictionary<int, IList<IEntity1dQuantity<IDisplacement>>>();

    internal Member1dDisplacementCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Member1dDisplacementCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IEntity1dResultSubset<IDisplacement, ResultVector6<Entity1dExtremaKey>>
      ResultSubset(ICollection<int> memberIds, int positionCount) {
      var positions = Enumerable.Range(0, positionCount)
       .Select(i => (double)i / (positionCount - 1)).ToList();
      return ResultSubset(memberIds, new ReadOnlyCollection<double>(positions));
    }

    public IEntity1dResultSubset<IDisplacement, ResultVector6<Entity1dExtremaKey>>
      ResultSubset(ICollection<int> memberIds, ReadOnlyCollection<double> positions) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeysAndPositions(memberIds, positions);
      if (missingIds.Count > 0) {
        string memberList = string.Join(" ", missingIds);
        var concurrent = Cache as ConcurrentDictionary<int, IList<IEntity1dQuantity<IDisplacement>>>;
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Double6>> aCaseResults
              = analysisCase.Member1dDisplacement(memberList, positions);
            Parallel.ForEach(aCaseResults.Keys, memberId =>
              concurrent.AddOrUpdate(
              memberId,
              // Add
              Entity1dResultsFactory.CreateResults(
                aCaseResults[memberId], positions, (a, b) => new Entity1dDisplacement(a, b)),
              // Update
              (key, oldValue) => oldValue.AddMissingPositions(
                aCaseResults[memberId], positions, (a) => new Displacement(a))));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<Double6>>> cCaseResults
              = combinationCase.Member1dDisplacement(memberList, positions);
            Parallel.ForEach(cCaseResults.Keys, memberId =>
             concurrent.AddOrUpdate(
              memberId,
              // Add
              Entity1dResultsFactory.CreateResults(
                cCaseResults[memberId], positions, (a, b) => new Entity1dDisplacement(a, b)),
              // Update
              (key, oldValue) => oldValue.AddMissingPositions(
                cCaseResults[memberId], positions, (a) => new Displacement(a))));
            break;
        }
      }

      return new Entity1dDisplacements(Cache.GetSubset(memberIds, positions));
    }
  }
}