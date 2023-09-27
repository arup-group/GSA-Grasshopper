using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components.GraveyardComp {
  /// <summary>
  ///   Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  public class GetAnalysis2_OBSOLETE : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("566a94d2-a022-4f12-a645-0366deb1476c");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GetModelAnalysis;

    public GetAnalysis2_OBSOLETE() : base("Get Model Analysis Tasks", "GetAnalysisTasks",
      "Get Analysis Tasks and their Cases from GSA model", CategoryName.Name(),
      SubCategoryName.Cat0()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA",
        "GSA model containing some Analysis Cases and Tasks", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaAnalysisTaskParameter(), "Analysis Tasks", "ΣT",
        "List of Analysis Tasks in model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaAnalysisCaseParameter(), "Analysis Cases", "ΣA",
        "List of Analysis Cases in model", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaModelGoo modelGoo = null;
      da.GetData(0, ref modelGoo);

      Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> tuple
        = Analysis.GetAnalysisTasksAndCombinations(modelGoo.Value);

      da.SetDataList(0, tuple.Item1);
      da.SetDataList(1, tuple.Item2);
    }
  }
}
