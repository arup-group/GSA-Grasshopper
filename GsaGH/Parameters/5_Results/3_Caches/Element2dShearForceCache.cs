﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Eto.Forms;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element2dShearForceCache
    : IEntity2dResultCache<IEntity2dQuantity<IShear2d>, IShear2d, ResultVector2<Entity2dExtremaKey>> {
    public IApiResult ApiResult { get; set; }

    public ConcurrentDictionary<int, Collection<IEntity2dQuantity<IShear2d>>> Cache { get; }
      = new ConcurrentDictionary<int, Collection<IEntity2dQuantity<IShear2d>>>();

    internal Element2dShearForceCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element2dShearForceCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IEntity2dResultSubset<IEntity2dQuantity<IShear2d>, IShear2d, ResultVector2<Entity2dExtremaKey>>
      ResultSubset(ICollection<int> elementIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(elementIds);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Vector2>> aCaseResults
              = analysisCase.Element2dShear(elementList, 0);
            Parallel.ForEach(aCaseResults.Keys, elementId => Cache.TryAdd(
              elementId, Entity2dResultsFactory.CreateShearForce(aCaseResults[elementId])));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<Vector2>>> cCaseResults
              = combinationCase.Element2dShear(elementList, 0);
            Parallel.ForEach(cCaseResults.Keys, elementId => Cache.TryAdd(
              elementId, Entity2dResultsFactory.CreateShearForce(cCaseResults[elementId])));
            break;
        }
      }

      return new Entity2dShearForce(Cache.GetSubset(elementIds));
    }
  }
}