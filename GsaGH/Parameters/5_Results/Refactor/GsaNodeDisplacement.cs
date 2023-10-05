using System.Collections.Generic;

namespace GsaGH.Parameters._5_Results.Refactor {
  public class GsaNodeDisplacements {
    //public string NodeList { get; private set; }
    //internal List<GsaResultsValues> ResultValues { get; private set; }
    //internal List<int> Ids { get; private set; }

    //public GsaNodeDisplacements(GsaResult result, string filter, LengthUnit unit) {
    //  NodeList = filter.ToLower() == "all" || filter == string.Empty ? "All" : filter;

    //  if (result.Type == CaseType.Combination) {
    //    ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> nodeResults
    //      = result.CombinationCaseResult.NodeResults(NodeList);
    //    ResultValues = ResultHelper
    //     .GetNodeResultValues(nodeResults, unit, result.SelectedPermutationIds).Values.ToList();
    //    Ids = nodeResults.Keys.ToList();
    //  } else {
    //    ReadOnlyDictionary<int, NodeResult> nodeResults
    //      = result.AnalysisCaseResult.NodeResults(NodeList);
    //    ResultValues = new List<GsaResultsValues> {
    //      ResultHelper.GetNodeResultValues(nodeResults, unit),
    //    };
    //    Ids = nodeResults.Keys.ToList();
    //  }
    //}
    internal Dictionary<string, GsaResultsValues> ACaseResultValues { get; private set; }
      = new Dictionary<string, GsaResultsValues>();

    //internal Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>> CCaseResultValues {
    //  get;
    //  private set;
    //} = new Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>();

    internal void AddACaseValue(string nodelist, GsaResultsValues values) {
      ACaseResultValues.Add(nodelist, values);
    }

    //internal void AddCCaseValue(
    //  string nodelist, ConcurrentDictionary<int, GsaResultsValues> values) {
    //  CCaseResultValues.Add(nodelist, values);
    //}
  }
}
