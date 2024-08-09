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
  public class CreateCombinationCase : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("8adcdf01-364b-4141-865d-02dc70577afd");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateCombinationCase;

    public CreateCombinationCase() : base("Create Combination Case", "CreateCombination",
      "Create a new GSA Combination Case", CategoryName.Name(), SubCategoryName.Cat4()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddIntegerParameter("ID", "ID", "Combination Case number (use '0' " +
        "to append Combination case)", GH_ParamAccess.item, 0);
      pManager.AddTextParameter("Name", "Na", "Case Name", GH_ParamAccess.item);
      pManager.AddTextParameter("Description", "De",
        "The description should take the form: 1.5A1 + 0.4A3." + Environment.NewLine
        + "Use 'or' for enveloping cases eg (1 or -1.4)A1," + Environment.NewLine
        + "'to' for enveloping a range of cases eg (C1 to C3)", GH_ParamAccess.item);
      pManager[0].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaCombinationCaseParameter());
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      int id = 0;
      da.GetData(0, ref id);
      string name = string.Empty;
      da.GetData(1, ref name);
      string desc = string.Empty;
      da.GetData(2, ref desc);
      da.SetData(0, new GsaCombinationCaseGoo(new GsaCombinationCase(id, name, desc)));
    }
  }
}
