﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Eto.Forms;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element1dDerivedStressCache
    : IElement1dResultCache<IElement1dDerivedStress, IStress1dDerived, ResultDerivedStress1d<Element1dExtremaKey>> {
    public IApiResult ApiResult { get; set; }

    public ConcurrentDictionary<int, Collection<IElement1dDerivedStress>> Cache { get; }
      = new ConcurrentDictionary<int, Collection<IElement1dDerivedStress>>();

    internal Element1dDerivedStressCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element1dDerivedStressCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IElement1dResultSubset<IElement1dDerivedStress, IStress1dDerived, ResultDerivedStress1d<Element1dExtremaKey>>
      ResultSubset(ICollection<int> elementIds, int positionCount) {
      var positions = Enumerable.Range(0, positionCount).Select(
        i => (double)i / (positionCount - 1)).ToList();
      return ResultSubset(elementIds, new ReadOnlyCollection<double>(positions));
    }

    public IElement1dResultSubset<IElement1dDerivedStress, IStress1dDerived, ResultDerivedStress1d<Element1dExtremaKey>>
      ResultSubset(ICollection<int> elementIds, ReadOnlyCollection<double> positions) {
      ConcurrentBag<int> missingIds
        = Cache.GetMissingKeysAndPositions<IElement1dDerivedStress, IStress1dDerived>(elementIds, positions);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<DerivedStressResult1d>> aCaseResults
              = analysisCase.Element1dDerivedStress(elementList, positions);
            Parallel.ForEach(aCaseResults.Keys, elementId => Cache.AddOrUpdate(
              elementId, Element1dResultsFactory.CreateBeamDerivedStresses(aCaseResults[elementId], positions),
              (key, oldValue) => oldValue.AddMissingPositions(aCaseResults[elementId], positions)));
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<DerivedStressResult1d>>> cCaseResults
              = combinationCase.Element1dDerivedStress(elementList, positions);
            Parallel.ForEach(cCaseResults.Keys, elementId => Cache.AddOrUpdate(
              elementId, Element1dResultsFactory.CreateBeamDerivedStresses(cCaseResults[elementId], positions),
              (key, oldValue) => oldValue.AddMissingPositions(cCaseResults[elementId], positions)));
            break;
        }
      }

      return new Element1dDerivedStresses(Cache.GetSubset(elementIds));
    }
  }
}
