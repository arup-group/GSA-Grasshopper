using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  /// Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class GetAnalysis_OBSOLETE : GH_OasysComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("fa497db7-8bdd-438d-888f-83a85d6cd48a");

    public GetAnalysis_OBSOLETE()
      : base("Get Model Analysis", "GetAnalysis", "Get Analysis Cases and Tasks from GSA model",
        CategoryName.Name(),
        SubCategoryName.Cat0()) {
          Hidden = true;
    } // sets the initial state of the component to hidden
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetAnalysisTask;
    #endregion
    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("GSA Model", "GSA", "GSA model containing some Analysis Cases and Tasks", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("Analysis Tasks", "Tasks", "List of analysis tasks in model", GH_ParamAccess.list);
      pManager.AddGenericParameter("Analysis Case Names", "Name", "Analysis case name", GH_ParamAccess.list);
      pManager.AddGenericParameter("Load Case/Combination ID", "LC", "Load cases and combinations list", GH_ParamAccess.list);
      pManager.AddGenericParameter("Analysis Case Description", "Desc", "Analysis case description", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaModel = new GsaModel();
      if (!da.GetData(0, ref gsaModel)) {
        return;
      }

      Model model = gsaModel.Model;
      var taskList = new List<string>();
      var descriptionList = new List<string>();
      var caseNameList = new List<string>();
      var analysisIdList = new List<int>();

      foreach (int key in model.AnalysisTasks().Keys) {
        model.AnalysisTasks().TryGetValue(key, out AnalysisTask analTask);
        taskList.Add(analTask?.Name);
      }

      foreach (int key in model.Results().Keys) {
        descriptionList.Add(model.AnalysisCaseDescription(key));
        caseNameList.Add(model.AnalysisCaseName(key));
        analysisIdList.Add(key);
      }

      da.SetDataList(0, taskList);
      da.SetDataList(1, caseNameList);
      da.SetDataList(3, descriptionList);
      da.SetDataList(2, analysisIdList);
    }
  }
}

