using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create a new Offset
  /// </summary>
  public class CreateOffset : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("ba73abd3-cd48-4dd2-9cd1-d89c921dd108");
    public override GH_Exposure Exposure => GH_Exposure.septenary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateOffset;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;

    public CreateOffset() : base("Create Offset", "Offset", "Create an GSA Offset",
      CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      Params.Input[0].Name = "Offset X1 [" + unitAbbreviation + "]";
      Params.Input[1].Name = "Offset X2 [" + unitAbbreviation + "]";
      Params.Input[2].Name = "Offset Y [" + unitAbbreviation + "]";
      Params.Input[3].Name = "Offset Z [" + unitAbbreviation + "]";
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Measure",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      pManager.AddGenericParameter("Offset X1 [" + unitAbbreviation + "]", "X1",
        "X1 - Start axial offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset X2 [" + unitAbbreviation + "]", "X2",
        "X2 - End axial offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset Y [" + unitAbbreviation + "]", "Y", "Y Offset",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset Z [" + unitAbbreviation + "]", "Z", "Z Offset",
        GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaOffsetParameter());
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var offset = new GsaOffset {
        X1 = (Length)Input.UnitNumber(this, da, 0, _lengthUnit, true),
        X2 = (Length)Input.UnitNumber(this, da, 1, _lengthUnit, true),
        Y = (Length)Input.UnitNumber(this, da, 2, _lengthUnit, true),
        Z = (Length)Input.UnitNumber(this, da, 3, _lengthUnit, true),
      };

      da.SetData(0, new GsaOffsetGoo(offset));
    }

    protected override void UpdateUIFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
