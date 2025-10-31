using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get information about GSA Analysis Tasks
  /// </summary>
  public class AnalysisTaskInfo : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("82d76442-cc58-49c6-b6d7-d0ae998ce063");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.AnalysisTaskInfo;

    public AnalysisTaskInfo() : base("Analysis Task Info", "TaskInfo",
      "Get information about a GSA Analysis Task",
      CategoryName.Name(), SubCategoryName.Cat4()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaAnalysisTaskParameter(), GsaAnalysisTaskGoo.Name,
        GsaAnalysisTaskGoo.NickName, GsaAnalysisTaskGoo.Name, GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddTextParameter("Name", "Na", "Analysis Task Name", GH_ParamAccess.item);
      pManager.AddParameter(new GsaAnalysisCaseParameter(), GsaAnalysisCaseGoo.Name + "(s)",
        GsaAnalysisCaseGoo.NickName, "List of " + GsaAnalysisCaseGoo.Description,
        GH_ParamAccess.list);
      pManager.AddTextParameter("Solver Type", "sT", "Solver Type", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Task ID", "ID", "The Task number of the Analysis Task",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp)) {
        return;
      }

      if (ghTyp.Value is GsaAnalysisTaskGoo goo) {
        da.SetData(0, goo.Value.ApiTask.Name);
        if (goo.Value.Cases != null) {
          da.SetDataList(1, new List<GsaAnalysisCaseGoo>(goo.Value.Cases.Select(x => new GsaAnalysisCaseGoo(x))));
        } else {
          da.SetData(1, null);
        }

        var type = (AnalysisTaskType)goo.Value.ApiTask.Type;
        da.SetData(2, type.ToString());
        da.SetData(3, goo.Value.Id);
      } else {
        string type = ghTyp.Value.GetType().ToString();
        type = type.Replace("GsaGH.Parameters.", string.Empty);
        type = type.Replace("Goo", string.Empty);
        Params.Owner.AddRuntimeError("Unable to convert Analysis Task input parameter of type "
          + type + " to GsaAnalysisTask");
      }
    }
  }
}
