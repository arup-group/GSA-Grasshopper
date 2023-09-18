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
  ///   Component to edit GSA analysis tasks
  /// </summary>
  public class EditAnalysisTask : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("efc2aae5-7ebf-4032-89d5-8fec8830989d");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditAnalysisTask;

    public EditAnalysisTask() : base("Edit Analysis Task", "EditTask", "Modify GSA Analysis Tasks",
      CategoryName.Name(), SubCategoryName.Cat4()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaAnalysisTaskParameter(), GsaAnalysisTaskGoo.Name,
        GsaAnalysisTaskGoo.NickName, GsaAnalysisTaskGoo.Name + " to Edit", GH_ParamAccess.item);
      pManager.AddParameter(new GsaAnalysisCaseParameter(), GsaAnalysisCaseGoo.Name + "(s)",
        GsaAnalysisCaseGoo.NickName, "Add list of " + GsaAnalysisCaseGoo.Name + " to task",
        GH_ParamAccess.list);
      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaAnalysisTaskParameter(), GsaAnalysisTaskGoo.Name,
        GsaAnalysisTaskGoo.NickName, "Modified " + GsaAnalysisTaskGoo.Name, GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Task Name", GH_ParamAccess.item);
      pManager.AddParameter(new GsaAnalysisCaseParameter(), GsaAnalysisCaseGoo.Name + "(s)",
        GsaAnalysisCaseGoo.NickName, "List of " + GsaAnalysisCaseGoo.Description,
        GH_ParamAccess.list);
      pManager.AddTextParameter("Solver Type", "sT", "Solver Type", GH_ParamAccess.item);
      pManager.AddIntegerParameter("TaskID", "ID",
        "The Task number if the Analysis Case ever belonged to a model", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaTask = new GsaAnalysisTask();
      GsaAnalysisTaskGoo analysisTaskGoo = null;
      if (da.GetData(0, ref analysisTaskGoo)) {
        gsaTask = analysisTaskGoo.Value.Duplicate();
      }

      if (gsaTask != null) {
        var ghTypes = new List<GH_ObjectWrapper>();
        if (da.GetDataList(1, ghTypes)) {
          var cases = new List<GsaAnalysisCase>();
          for (int i = 0; i < ghTypes.Count; i++) {
            GH_ObjectWrapper ghTyp2 = ghTypes[i];
            if (ghTyp2 == null) {
              Params.Owner.AddRuntimeWarning("Analysis Case input (index: " + i
                + ") is null and has been ignored");
              continue;
            }

            if (ghTyp2.Value is GsaAnalysisCaseGoo goo) {
              cases.Add(goo.Value.Duplicate());
            } else {
              string typ = ghTyp2.Value.GetType().ToString();
              typ = typ.Replace("GsaGH.Parameters.", string.Empty);
              typ = typ.Replace("Goo", string.Empty);
              Params.Owner.AddRuntimeError(
                "Unable to convert Analysis Case input parameter of type " + typ
                + " to GsaAnalysisCase");
              return;
            }
          }

          gsaTask.Cases = cases;
        }

        da.SetData(0, new GsaAnalysisTaskGoo(gsaTask));
        da.SetData(1, gsaTask.Name);
        if (gsaTask.Cases != null) {
          da.SetDataList(2,
            new List<GsaAnalysisCaseGoo>(gsaTask.Cases.Select(x => new GsaAnalysisCaseGoo(x))));
        } else {
          da.SetData(2, null);
        }

        da.SetData(3, gsaTask.Type.ToString());
        da.SetData(4, gsaTask.Id);
      } else {
        string type = analysisTaskGoo.Value.GetType().ToString();
        type = type.Replace("GsaGH.Parameters.", string.Empty);
        type = type.Replace("Goo", string.Empty);
        Params.Owner.AddRuntimeError("Unable to convert Analysis Task input parameter of type "
          + type + " to GsaAnalysisTask");
      }
    }
  }
}
