using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.GsaApi;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters._5_Results.Refactor {
  public class GsaNodeDisplacements {
    public string FilterBy { get; private set; }
    internal List<GsaResultsValues> ResultValues { get; private set; }
    internal List<int> Ids { get; private set; }

    public GsaNodeDisplacements(GsaResult result, string filter, LengthUnit unit) {
      FilterBy = filter.ToLower() == "all" || filter == string.Empty ? "All" : filter;

      if (result.SelectedPermutationIds != null) {
        ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> nodeResults
          = result.CombinationCaseResult.NodeResults(FilterBy);
        ResultValues = ResultHelper
         .GetNodeResultValues(nodeResults, unit, result.SelectedPermutationIds).Values.ToList();
        Ids = nodeResults.Keys.ToList();
      } else {
        ReadOnlyDictionary<int, NodeResult> nodeResults
          = result.AnalysisCaseResult.NodeResults(FilterBy);
        ResultValues = new List<GsaResultsValues> {
          ResultHelper.GetNodeResultValues(nodeResults, unit),
        };
        Ids = nodeResults.Keys.ToList();
      }
    }
  }
}
