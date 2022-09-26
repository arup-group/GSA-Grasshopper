using System;
using Grasshopper.Kernel;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  public class CreateAnalysisCase : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("75bf9454-92c4-4a3c-8abf-75f1d449bb85");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateAnalysisCase;

    public CreateAnalysisCase()      : base("Create Analysis Case", 
      "CreateCase", 
      "Create a new GSA Analysis Case (Load Case or Combination)",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat4())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddTextParameter("Name", "Na", "Case Name", GH_ParamAccess.item);
      pManager.AddTextParameter("Description", "De",
          "The description should take the form: 1.4L1 + 0.8L3." + System.Environment.NewLine +
          "It may also take the form: 1.4A4 or 1.6C2." + System.Environment.NewLine +
          "The referenced loads (L#), analysis (A#), and combination (C#) cases must exist in model", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Analysis Case", "ΣA", "GSA Analysis Case", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      string name = "";
      DA.GetData(0, ref name);

      string desc = "";
      DA.GetData(1, ref desc);

      DA.SetData(0, new GsaAnalysisCaseGoo(new GsaAnalysisCase(name, desc)));
    }
  }
}

