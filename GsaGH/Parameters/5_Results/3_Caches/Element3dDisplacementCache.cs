﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element3dDisplacementCache
    : IEntity2dResultCache<IEntity2dQuantity<ITranslation>, ITranslation, ResultVector3InAxis<Entity2dExtremaKey>> {
    public IApiResult ApiResult { get; set; }

    public ConcurrentDictionary<int, Collection<IEntity2dQuantity<ITranslation>>> Cache { get; }
      = new ConcurrentDictionary<int, Collection<IEntity2dQuantity<ITranslation>>>();

    internal Element3dDisplacementCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element3dDisplacementCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IEntity2dResultSubset<IEntity2dQuantity<ITranslation>, ITranslation, ResultVector3InAxis<Entity2dExtremaKey>>
      ResultSubset(ICollection<int> elementIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(elementIds);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<Double3>> aCaseResults
              = analysisCase.Element3dDisplacement(elementList);
            Parallel.ForEach(aCaseResults.Keys, elementId => Cache.TryAdd(
              elementId, Entity3dResultsFactory.CreateTranslations(aCaseResults[elementId])));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<Double3>>> cCaseResults
              = combinationCase.Element3dDisplacement(elementList);
            Parallel.ForEach(cCaseResults.Keys, elementId => Cache.TryAdd(
              elementId, Entity3dResultsFactory.CreateTranslations(cCaseResults[elementId])));
            break;
        }
      }

      return new Entity3dDisplacements(Cache.GetSubset(elementIds));
    }
  }
}
