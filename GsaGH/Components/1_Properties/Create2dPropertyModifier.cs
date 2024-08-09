using System;
using System.Collections.Generic;
using System.Drawing;

using GH_IO.Serialization;

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
  ///   Component to create a new Prop2dModifier
  /// </summary>
  public class Create2dPropertyModifier : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("7399c0a4-7395-4e25-a615-be6c5803ecb7");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Create2dPropertyModifier;
    private readonly List<string> _optionTypes = new List<string>(new[] {
      "Modify by",
      "Modify to",
    });
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;
    private bool _toMode = false;

    public Create2dPropertyModifier() : base("Create 2D Property Modifier", "Prop2dModifier", "Create GSA 2D Property Modifier",
      CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    public override bool Read(GH_IReader reader) {
      _toMode = reader.GetBoolean("toMode");
      return base.Read(reader);
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      switch (i) {
        case 0 when j == 0: {
            if (_toMode) {
              _dropDownItems.RemoveAt(1);
              _selectedItems.RemoveAt(1);
              _spacerDescriptions.RemoveAt(1);
            }

            _toMode = false;
            break;
          }
        case 0: {
            if (!_toMode) {
              _dropDownItems.Insert(1, FilteredUnits.FilteredLengthUnits);
              _selectedItems.Insert(1, _lengthUnit.ToString());
              _spacerDescriptions.Insert(1, "Length unit");
            }

            _toMode = true;
            break;
          }
        case 1:
          _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
          break;
      }

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      if (_toMode) {
        string lengthUnitAbbreviation = Length.GetAbbreviation(_lengthUnit);
        string areaUnitAbbreviation = Area.GetAbbreviation(UnitsHelper.GetAreaUnit(_lengthUnit));
        string volumeUnitAbbreviation =
          VolumePerLength.GetAbbreviation(UnitsHelper.GetVolumePerLengthUnit(_lengthUnit));
        string inertiaUnitAbbreviation =
          AreaMomentOfInertia.GetAbbreviation(UnitsHelper.GetAreaMomentOfInertiaUnit(_lengthUnit));

        Params.Input[0].Name = "In-plane Modifier [" + areaUnitAbbreviation + "/" + lengthUnitAbbreviation + "]";
        Params.Input[0].Description = "[Optional] Modify the effective in-plane stiffness TO this value";
        Params.Input[1].Name = "Bending Modifier [" + inertiaUnitAbbreviation + "/" + lengthUnitAbbreviation + "]";
        Params.Input[1].Description = "[Optional] Modify the effective bending stiffness TO this value";
        Params.Input[2].Name = "Shear Modifier [" + areaUnitAbbreviation + "/" + lengthUnitAbbreviation + "]";
        Params.Input[2].Description = "[Optional] Modify the effective shear stiffness TO this value"; // shear stiffness?
        Params.Input[3].Name = "Volume Modifier [" + volumeUnitAbbreviation + "/" + areaUnitAbbreviation + "]";
        Params.Input[3].Description = "[Optional] Modify the effective volume this value";
      } else {
        Params.Input[0].Name = "In-plane Modifier";
        Params.Input[0].Description = "[Optional] Modify the effective in-plane stiffness BY this" +
          " decimal fraction value (Default = 1.0 -> 100%)";
        Params.Input[1].Name = "Bending Modifier";
        Params.Input[1].Description = "[Optional] Modify the effective bending stiffness BY this" +
          " decimal fraction value (Default = 1.0 -> 100%)";
        Params.Input[2].Name = "Shear Modifier";
        Params.Input[2].Description = "[Optional] Modify the effective shear stiffness BY this" +
          "decimal fraction value (Default = 1.0 -> 100%)";
        Params.Input[3].Name = "Volume Modifier";
        Params.Input[3].Description = "[Optional] Modify the effective volume BY this decimal" +
          " fraction value (Default = 1.0 -> 100%)";
      }

      Params.Input[4].Name = "Additional Mass [" + AreaDensity.GetAbbreviation(AreaDensityUnit.KilogramPerSquareMeter) + "]";
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetBoolean("toMode", _toMode);
      return base.Write(writer);
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Modify type",
        "Density unit"
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(_optionTypes);
      _selectedItems.Add(_optionTypes[0]);

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.LinearDensity));
      _selectedItems.Add(AreaDensity.GetAbbreviation(AreaDensityUnit.KilogramPerSquareMeter));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("In-plane Modifier", "Ip", "[Optional] Modify the effective" +
        " in-plane stiffness BY this decimal fraction value (Default = 1.0 -> 100%)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Bending Modifier", "B", "[Optional] Modify the effective" +
        " bending stiffness BY this decimal fraction value (Default = 1.0 -> 100%)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Shear Modifier", "S", "[Optional] Modify the effective shear" +
        " stiffness BY this decimal fraction value (Default = 1.0 -> 100%)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Volume Modifier", "V", "[Optional] Modify the effective volume" +
        " stiffness BY this decimal fraction value (Default = 1.0 -> 100%)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Additional Mass [" +
        AreaDensity.GetAbbreviation(AreaDensityUnit.KilogramPerSquareMeter) + "]", "+kg",
        "[Optional] Additional mass per unit length (Default = 0 -> no additional mass)", GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaProperty2dModifierParameter());
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var modifier = new GsaProperty2dModifier();
      if (_toMode) {
        AreaUnit areaUnit = UnitsHelper.GetAreaUnit(_lengthUnit);
        VolumeUnit volumeUnit = UnitsHelper.GetVolumeUnit(_lengthUnit);

        if (Params.Input[0].SourceCount > 0) {
          modifier.InPlane = Input.UnitNumber(this, da, 0, _lengthUnit, true);
        }

        if (Params.Input[1].SourceCount > 0) {
          modifier.Bending = Input.UnitNumber(this, da, 1, volumeUnit, true);
        }

        if (Params.Input[2].SourceCount > 0) {
          modifier.Shear = Input.UnitNumber(this, da, 2, _lengthUnit, true);
        }

        if (Params.Input[3].SourceCount > 0) {
          modifier.Volume = Input.UnitNumber(this, da, 3, _lengthUnit, true);
        }

      } else {
        modifier.InPlane = Input.RatioInDecimalFractionToPercentage(this, da, 0);
        modifier.Bending = Input.RatioInDecimalFractionToPercentage(this, da, 1);
        modifier.Shear = Input.RatioInDecimalFractionToPercentage(this, da, 2);
        modifier.Volume = Input.RatioInDecimalFractionToPercentage(this, da, 3);
      }

      modifier.AdditionalMass = (AreaDensity)Input.UnitNumber(this, da, 4, AreaDensityUnit.KilogramPerSquareMeter, true);

      da.SetData(0, new GsaProperty2dModifierGoo(modifier));
    }

    protected override void UpdateUIFromSelectedItems() {
      if (_toMode) {
        _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);
      }

      base.UpdateUIFromSelectedItems();
    }
  }
}
