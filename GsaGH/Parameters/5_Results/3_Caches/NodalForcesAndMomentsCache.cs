using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class NodalForcesAndMomentsCache {
    public IApiResult ApiResult { get; set; }

    private readonly Model _model;

    public IDictionary<int, IDictionary<int, IList<IInternalForce>>> Cache { get; }
      = new ConcurrentDictionary<int, IDictionary<int, IList<IInternalForce>>>();

    internal NodalForcesAndMomentsCache(AnalysisCaseResult result, Model model) {
      ApiResult = new ApiResult(result);
      _model = model;
    }

    internal NodalForcesAndMomentsCache(CombinationCaseResult result, Model model) {
      ApiResult = new ApiResult(result);
      _model = model;
    }

    public NodalForcesAndMomentsSubset ResultSubset(ICollection<int> nodeIds) {
      ConcurrentBag<int> missingIds = Cache.GetMissingKeys(nodeIds);

      if (missingIds.Count > 0) {
        string nodelist = string.Join(" ", missingIds);
        ReadOnlyDictionary<int, Node> nodes = _model.Nodes();
        ReadOnlyDictionary<int, Element> elements = _model.Elements();
        Parallel.ForEach(missingIds, nodeId => {
          Node node = nodes[nodeId];
          string elementList = string.Join(" ", node.ConnectedElements);

          switch (ApiResult.Result) {
            case AnalysisCaseResult analysisCase:
              ReadOnlyDictionary<int, ReadOnlyCollection<Double6>> aCaseResults = analysisCase.Element1dForce(elementList, 2);
              foreach (KeyValuePair<int, ReadOnlyCollection<Double6>> resultKvp in aCaseResults) {
                Element element = elements[resultKvp.Key];

                int position = resultKvp.Key == element.Topology[0] ? 0 : 1;
                var res = new InternalForce(resultKvp.Value[position]);

                IDictionary<int, IList<IInternalForce>> value = new ConcurrentDictionary<int, IList<IInternalForce>>();
                value.Add(resultKvp.Key, new Collection<IInternalForce>() { res });

                ((ConcurrentDictionary<int, IDictionary<int, IList<IInternalForce>>>)Cache).TryAdd(nodeId, value);
              }
              break;

            case CombinationCaseResult combinationCase:
              ReadOnlyDictionary<int, ReadOnlyCollection<ReadOnlyCollection<Double6>>> cCaseResults = combinationCase.Element1dForce(elementList, 2);
              foreach (KeyValuePair<int, ReadOnlyCollection<ReadOnlyCollection<Double6>>> resultKvp in cCaseResults) {
                Element element = elements[resultKvp.Key];

                int position = resultKvp.Key == element.Topology[0] ? 0 : 1;
                var permutationResults = new Collection<IInternalForce>();
                foreach (ReadOnlyCollection<Double6> permutation in resultKvp.Value) {
                  permutationResults.Add(new InternalForce(permutation[position]));
                }

                IDictionary<int, IList<IInternalForce>> value = new ConcurrentDictionary<int, IList<IInternalForce>>();
                value.Add(resultKvp.Key, permutationResults);

                ((ConcurrentDictionary<int, IDictionary<int, IList<IInternalForce>>>)Cache).TryAdd(nodeId, value);
              }
              break;
          }
        });
      }

      return new NodalForcesAndMomentsSubset(Cache.GetSubset(nodeIds));
    }
  }
}
