using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element1dAverageStrainEnergyDensityCache : INodeResultCache<IEnergyDensity, NodeExtremaKey> {
    public IApiResult ApiResult { get; set; }

    public IDictionary<int, IList<IEnergyDensity>> Cache { get; }
      = new ConcurrentDictionary<int, IList<IEnergyDensity>>();

    internal Element1dAverageStrainEnergyDensityCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    internal Element1dAverageStrainEnergyDensityCache(CombinationCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public INodeResultSubset<IEnergyDensity, NodeExtremaKey> ResultSubset(ICollection<int> elementIds) {
      var positions = new ReadOnlyCollection<double>(new Collection<double> { 0.5 });
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(elementIds);
      if (missingIds.Count > 0) {
        string elementList = string.Join(" ", missingIds);
        switch (ApiResult.Result) {
          case AnalysisCaseResult analysisCase:
            ReadOnlyDictionary<int, double> aCaseResults = analysisCase.Element1dAverageStrainEnergyDensity(elementList);
            Parallel.ForEach(aCaseResults.Keys, elementId => {
              var res = new StrainEnergyDensity(aCaseResults[elementId]);
              ((ConcurrentDictionary<int, IList<IEnergyDensity>>)Cache).TryAdd(
                elementId, new Collection<IEnergyDensity>() { res });
            });
            break;

          case CombinationCaseResult combinationCase:
            ReadOnlyDictionary<int, ReadOnlyCollection<double>> cCaseResults = combinationCase.Element1dAverageStrainEnergyDensity(elementList);
            Parallel.ForEach(cCaseResults.Keys, elementId => {
              var permutationResults = new Collection<IEnergyDensity>();
              foreach (double permutationResult in cCaseResults[elementId]) {
                permutationResults.Add(new StrainEnergyDensity(permutationResult));
              }

              ((ConcurrentDictionary<int, IList<IEnergyDensity>>)Cache).TryAdd(
                elementId, permutationResults);
            });
            break;
        }
      }

      return new Entity1dAverageStrainEnergyDensity(Cache.GetSubset(elementIds));
    }
  }
}
