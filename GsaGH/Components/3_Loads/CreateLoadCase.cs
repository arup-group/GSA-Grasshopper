using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  public class CreateLoadCase : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("ebae28df-470b-45eb-b57e-a6a519aa0152");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateLoadCase;

    public CreateLoadCase() : base("Create Load Case", "LoadCase", "Create GSA Load Case",
      CategoryName.Name(), SubCategoryName.Cat3()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Type",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(Enum.GetNames(typeof(LoadCaseType)).ToList());
      _selectedItems.Add(_dropDownItems[0][0]);

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddIntegerParameter("Case ID", "ID", "Load Case number",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Load Case Name", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaLoadCaseParameter());
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var type =
        (LoadCaseType)Enum.Parse(typeof(LoadCaseType), _selectedItems[0]);

      int id = 0;
      da.GetData(0, ref id);

      string name = string.Empty;
      da.GetData(1, ref name);

      da.SetData(0, new GsaLoadCaseGoo(new GsaLoadCase(id, type, name)));
    }
  }
}
