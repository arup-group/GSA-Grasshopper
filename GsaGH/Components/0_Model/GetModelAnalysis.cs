using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  public class GetModelAnalysis : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("f71f0d68-f121-4081-b53d-896676c34ddb");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GetModelAnalysis;

    public GetModelAnalysis() : base("Get Model Analysis", "GetAnalysis",
      "Get Analysis Tasks and their Cases from GSA model", CategoryName.Name(),
      SubCategoryName.Cat0()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA",
        "GSA model containing some Analysis Cases, Combinations and Tasks", 
        GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaAnalysisTaskParameter(), "Analysis Tasks", "ΣT",
        "List of Analysis Tasks in model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaAnalysisCaseParameter(), "Analysis Cases", "ΣA",
        "List of Analysis Cases in model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaCombinationCaseParameter(), "Combination Cases", "ΣC",
        "List of Combination Cases in model", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaModelGoo modelGoo = null;
      da.GetData(0, ref modelGoo);

      Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> tuple
        = Analyses.GetAnalysisTasksAndCombinations(modelGoo.Value);
      var combinationCaseGoos = modelGoo.Value.Model.CombinationCases().Select(keyValuePair
        => new GsaCombinationCaseGoo(new GsaCombinationCase(keyValuePair))).ToList();

      da.SetDataList(0, tuple.Item1);
      da.SetDataList(1, tuple.Item2);
      da.SetDataList(2, combinationCaseGoos);
    }
  }
}
