using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  /// Component to create a new Offset
  /// </summary>
  public class CreateOffset : GH_OasysDropDownComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("ba73abd3-cd48-4dd2-9cd1-d89c921dd108");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateOffset;

    public CreateOffset() : base("Create Offset",
      "Offset",
      "Create GSA Offset",
      CategoryName.Name(),
      SubCategoryName.Cat1()) {
      Hidden = true;
    }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      pManager.AddGenericParameter("Offset X1 [" + unitAbbreviation + "]", "X1", "X1 - Start axial offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset X2 [" + unitAbbreviation + "]", "X2", "X2 - End axial offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset Y [" + unitAbbreviation + "]", "Y", "Y Offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset Z [" + unitAbbreviation + "]", "Z", "Z Offset", GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaOffsetParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var offset = new GsaOffset {
        X1 = (Length)Input.UnitNumber(this, da, 0, _lengthUnit, true),
        X2 = (Length)Input.UnitNumber(this, da, 1, _lengthUnit, true),
        Y = (Length)Input.UnitNumber(this, da, 2, _lengthUnit, true),
        Z = (Length)Input.UnitNumber(this, da, 3, _lengthUnit, true),
      };

      da.SetData(0, new GsaOffsetGoo(offset));
    }

    #region Custom UI
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;

    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new[]
        {
          "Measure",
        });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      SelectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      IsInitialised = true;
    }

    public override void SetSelected(int i, int j) {
      SelectedItems[i] = DropDownItems[i][j];

      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      Params.Input[0].Name = "Offset X1 [" + unitAbbreviation + "]";
      Params.Input[1].Name = "Offset X2 [" + unitAbbreviation + "]";
      Params.Input[2].Name = "Offset Y [" + unitAbbreviation + "]";
      Params.Input[3].Name = "Offset Z [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
