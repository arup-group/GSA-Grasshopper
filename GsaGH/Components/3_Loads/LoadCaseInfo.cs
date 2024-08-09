using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  public class LoadCaseInfo : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("1eda4553-0fb7-4aef-a350-3bd7b374cd6d");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.LoadCaseInfo;

    public LoadCaseInfo() : base("Load Case Info", "LCInfo",
      "Get the parameters of a Load Case", CategoryName.Name(), SubCategoryName.Cat3()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaLoadCaseParameter());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddIntegerParameter("Case ID", "ID", "Load Case number",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Load Case Name", GH_ParamAccess.item);
      pManager.AddTextParameter("Type", "Ty", "Load Case Type", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaLoadCaseGoo loadCaseGoo = null;
      da.GetData(0, ref loadCaseGoo);

      da.SetData(0, loadCaseGoo.Value.Id);
      da.SetData(1, loadCaseGoo.Value.LoadCase.Name);
      da.SetData(2, loadCaseGoo.Value.LoadCase.CaseType.ToString());
    }
  }
}
