using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {

  /// <summary>
  ///   Component to create a GSA Analysis Case
  /// </summary>
  public class CreateAnalysisCase : GH_OasysComponent {

    #region Properties + Fields
    public override Guid ComponentGuid => new Guid("75bf9454-92c4-4a3c-8abf-75f1d449bb85");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateAnalysisCase;
    #endregion Properties + Fields

    #region Public Constructors
    public CreateAnalysisCase() : base(
      "Create " + GsaAnalysisCaseGoo.Name.Replace(" ", string.Empty),
      GsaAnalysisCaseGoo.NickName.Replace(" ", string.Empty),
      "Create a " + GsaAnalysisCaseGoo.Description,
      CategoryName.Name(),
      SubCategoryName.Cat4())
      => Hidden = true;

    #endregion Public Constructors

    #region Protected Methods
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddTextParameter("Name", "Na", "Case Name", GH_ParamAccess.item);
      pManager.AddTextParameter("Description",
        "De",
        "The description should take the form: 1.4L1 + 0.8L3."
        + Environment.NewLine
        + "It may also take the form: 1.4A4 or 1.6C2."
        + Environment.NewLine
        + "The referenced loads (L#), analysis (A#), and combination (C#) cases must exist in model",
        GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
      => pManager.AddParameter(new GsaAnalysisCaseParameter());

    protected override void SolveInstance(IGH_DataAccess da) {
      string name = "";
      da.GetData(0, ref name);

      string desc = "";
      da.GetData(1, ref desc);

      da.SetData(0, new GsaAnalysisCaseGoo(new GsaAnalysisCase(name, desc)));
    }

    #endregion Protected Methods
  }
}
