using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create a GSA Analysis Task
  /// </summary>
  public class CreateAnalysisTask_OBSOLETE : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("6ef86d0b-892c-4b6f-950e-b4477e9f0910");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateAnalysisTask;
    private AnalysisTaskType _tasktype = AnalysisTaskType.Static;

    public CreateAnalysisTask_OBSOLETE() : base(
      "Create " + GsaAnalysisTaskGoo.Name,
      GsaAnalysisTaskGoo.NickName.Replace(" ", string.Empty),
      "Create a " + GsaAnalysisTaskGoo.Description, CategoryName.Name(), SubCategoryName.Cat4()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _tasktype = (AnalysisTaskType)Enum.Parse(typeof(AnalysisTaskType), _selectedItems[i]);
      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Solver",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(new List<string>() {
        AnalysisTaskType.Static.ToString(),
      });
      _selectedItems.Add(_tasktype.ToString());

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddTextParameter("Name", "Na", "Task Name", GH_ParamAccess.item);
      pManager.AddGenericParameter("Analysis Cases", "ΣAs",
        "List of GSA Analysis Cases (if left empty, all load cases in model will be added)",
        GH_ParamAccess.list);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaAnalysisTaskParameter());
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      string name = _tasktype.ToString();
      da.GetData(0, ref name);

      List<GsaAnalysisCase> cases = null;

      var ghTypes = new List<GH_ObjectWrapper>();
      if (da.GetDataList(1, ghTypes)) {
        cases = new List<GsaAnalysisCase>();
        for (int i = 0; i < ghTypes.Count; i++) {
          GH_ObjectWrapper ghTyp = ghTypes[i];
          if (ghTyp == null) {
            Params.Owner.AddRuntimeWarning("Analysis Case input (index: " + i
              + ") is null and has been ignored");
            continue;
          }

          if (ghTyp.Value is GsaAnalysisCaseGoo goo) {
            cases.Add(goo.Value.Duplicate());
          } else {
            string type = ghTyp.Value.GetType().ToString();
            type = type.Replace("GsaGH.Parameters.", string.Empty);
            type = type.Replace("Goo", string.Empty);
            Params.Owner.AddRuntimeError("Unable to convert Analysis Case input parameter of type "
              + type + " to GsaAnalysisCase");
            return;
          }
        }
      }

      if (cases == null) {
        this.AddRuntimeRemark(
          "Default Task has been created; it will by default contain all cases found in model");
      }

      if (_tasktype != AnalysisTaskType.Static) {
        this.AddRuntimeWarning("It is currently not possible to adjust the solver settings. "
          + Environment.NewLine
          + "Please verify the solver settings in GSA ('Task and Cases' -> 'Analysis Tasks')");
      }

      var task = new GsaAnalysisTask {
        Cases = cases,
        ApiTask = AnalysisTaskFactory.CreateStaticAnalysisTask(name)
      };

      da.SetData(0, new GsaAnalysisTaskGoo(task));
    }

    protected override void UpdateUIFromSelectedItems() {
      _tasktype = (AnalysisTaskType)Enum.Parse(typeof(AnalysisTaskType), _selectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
