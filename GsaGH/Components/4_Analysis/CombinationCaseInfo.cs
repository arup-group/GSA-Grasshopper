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
  ///   Component to create GSA Combination Case
  /// </summary>
  public class CombinationCaseInfo : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("d480f5d5-8df4-43a6-8bd1-dc2bab5c1e70");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CombinationCaseInfo;

    public CombinationCaseInfo() : base("Combination Case Info", "CombinationInfo",
      "Get information about a GSA Combination Case", CategoryName.Name(), SubCategoryName.Cat4()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaCombinationCaseParameter());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddIntegerParameter("ID", "ID", "Combination Case number", GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Combination Case Name", GH_ParamAccess.item);
      pManager.AddTextParameter("Description", "De",
        "The description should take the form: 1.5A1 + 0.4A3." + Environment.NewLine
        + "Use 'or' for enveloping cases eg (1 or -1.4)A1," + Environment.NewLine
        + "'to' for enveloping a range of cases eg (C1 to C3)", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaCombinationCaseGoo caseGoo = null;
      da.GetData(0, ref caseGoo);
      da.SetData(0, caseGoo.Value.Id);
      da.SetData(1, caseGoo.Value.Name);
      da.SetData(2, caseGoo.Value.Definition);
    }
  }
}
