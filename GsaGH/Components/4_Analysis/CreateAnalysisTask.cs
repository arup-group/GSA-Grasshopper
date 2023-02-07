using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units.Helpers;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create a GSA Analysis Task
  /// </summary>
  public class CreateAnalysisTask : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("6ef86d0b-892c-4b6f-950e-b4477e9f0910");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateAnalysisTask;

    public CreateAnalysisTask() : base("Create " + GsaAnalysisTaskGoo.Name.Replace(" ", string.Empty),
      GsaAnalysisTaskGoo.NickName.Replace(" ", string.Empty),
      "Create a " + GsaAnalysisTaskGoo.Description,
      CategoryName.Name(),
      SubCategoryName.Cat4())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddTextParameter("Name", "Na", "Task Name", GH_ParamAccess.item);
      pManager.AddGenericParameter("Analysis Cases", "ΣAs", "List of GSA Analysis Cases (if left empty, all load cases in model will be added)", GH_ParamAccess.list);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaAnalysisTaskParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      string name = _analtype.ToString();
      DA.GetData(0, ref name);

      List<GsaAnalysisCase> cases = null;

      List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
      if (DA.GetDataList(1, gh_types))
      {
        cases = new List<GsaAnalysisCase>();
        for (int i = 0; i < gh_types.Count; i++)
        {
          GH_ObjectWrapper gh_typ = gh_types[i];
          if (gh_typ == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Analysis Case input (index: " + i + ") is null and has been ignored"); continue; }
          if (gh_typ.Value is GsaAnalysisCaseGoo)
          {
            cases.Add(((GsaAnalysisCaseGoo)gh_typ.Value).Value.Duplicate());
          }
          else
          {
            string type = gh_typ.Value.GetType().ToString();
            type = type.Replace("GsaGH.Parameters.", "");
            type = type.Replace("Goo", "");
            Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert Analysis Case input parameter of type " +
                type + " to GsaAnalysisCase");
            return;
          }
        }
      }

      if (cases == null)
        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Default Task has been created; it will by default contain all cases found in model");

      if (_analtype != GsaAnalysisTask.AnalysisType.Static)
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "It is currently not possible to adjust the solver settings. " +
            Environment.NewLine + "Please verify the solver settings in GSA ('Task and Cases' -> 'Analysis Tasks')");

      GsaAnalysisTask task = new GsaAnalysisTask();
      task.Name = name;
      task.Cases = cases;
      task.Type = _analtype;
      DA.SetData(0, new GsaAnalysisTaskGoo(task));
    }

    #region Custom UI
    private GsaAnalysisTask.AnalysisType _analtype = GsaAnalysisTask.AnalysisType.Static;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Solver"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Type
      this.DropDownItems.Add(new List<string>() { GsaAnalysisTask.AnalysisType.Static.ToString() });
      this.SelectedItems.Add(this._analtype.ToString());

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      this._analtype = (GsaAnalysisTask.AnalysisType)Enum.Parse(typeof(GsaAnalysisTask.AnalysisType), this.SelectedItems[i]);
      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems()
    {
      this._analtype = (GsaAnalysisTask.AnalysisType)Enum.Parse(typeof(GsaAnalysisTask.AnalysisType), this.SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}

