using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.GsaApi;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters._5_Results.Refactor {
  public class GsaNodeDisplacements {
    public string FilterBy { get; private set; }
    public LengthUnit LengthUnit { get; private set; }
    internal List<GsaResultsValues> Results { get; private set; }
    internal List<int> Ids { get; private set; }

    public GsaNodeDisplacements(GsaResult result, string filter, LengthUnit unit) {
      FilterBy = filter.ToLower() == "all" || filter == string.Empty ? "All" : filter;
      LengthUnit = unit;

      if (result.SelectedPermutationIds != null) {
        ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> a
          = result.CombinationCaseResult.NodeResults(FilterBy);
        Results = ResultHelper.GetNodeResultValues(a, LengthUnit, result.SelectedPermutationIds)
         .Values.ToList();
      } else {
        ReadOnlyDictionary<int, NodeResult> a = result.AnalysisCaseResult.NodeResults(FilterBy);
        Results = new List<GsaResultsValues> {
          ResultHelper.GetNodeResultValues(a, LengthUnit),
        };
      }

      ;
      Ids = result.Model.Model.Nodes(FilterBy).Keys.ToList(); //why not a.keys?
    }
  }
}
