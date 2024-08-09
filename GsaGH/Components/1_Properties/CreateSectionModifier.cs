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
  ///   Component to create a new Offset
  /// </summary>
  public class CreateSectionModifier : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("e65d2554-75a9-4fac-9f12-1400e84aeee9");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateSectionModifier;
    private readonly List<string> _optionTypes = new List<string>(new[] {
      "Modify by",
      "Modify to",
    });
    private readonly List<string> _stressOptions = new List<string>(new[] {
      "Don't calculate",
      "Use unmodified",
      "Use modified",
    });
    private LinearDensityUnit _densityUnit = DefaultUnits.LinearDensityUnit;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;
    private StressOptionType _stressOption = StressOptionType.NoCalculation;
    private bool _toMode = false;

    public CreateSectionModifier() : base("Create Section Modifier", "SectionModifier",
      "Create a GSA Section Modifier", CategoryName.Name(), SubCategoryName.Cat1()) {
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
        case 1 when !_toMode:
          _densityUnit
            = (LinearDensityUnit)UnitsHelper.Parse(typeof(LinearDensityUnit), _selectedItems[i]);
          break;

        case 1:
          _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
          break;

        case 2 when !_toMode:
          switch (j) {
            case 0:
              _stressOption = StressOptionType.NoCalculation;
              break;

            case 1:
              _stressOption = StressOptionType.UseUnmodified;
              break;

            case 2:
              _stressOption = StressOptionType.UseModified;
              break;
          }

          break;

        case 2:
          _densityUnit
            = (LinearDensityUnit)UnitsHelper.Parse(typeof(LinearDensityUnit), _selectedItems[i]);
          break;

        case 3:
          switch (j) {
            case 0:
              _stressOption = StressOptionType.NoCalculation;
              break;

            case 1:
              _stressOption = StressOptionType.UseUnmodified;
              break;

            case 2:
              _stressOption = StressOptionType.UseModified;
              break;
          }

          break;
      }

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      Params.Input[7].Name
        = "Additional Mass [" + LinearDensity.GetAbbreviation(_densityUnit) + "]";
      if (_toMode) {
        string unit = Length.GetAbbreviation(_lengthUnit);
        string volUnit
          = VolumePerLength.GetAbbreviation(UnitsHelper.GetVolumePerLengthUnit(_lengthUnit));

        Params.Input[0].Name = "Area Modifier [" + unit + "\u00B2]";
        Params.Input[0].Description = "[Optional] Modify the effective Area TO this value";
        Params.Input[1].Name = "I11 Modifier [" + unit + "\u2074]";
        Params.Input[1].Description = "[Optional] Modify the effective Iyy/Iuu TO this value";
        Params.Input[2].Name = "I22 Modifier [" + unit + "\u2074]";
        Params.Input[2].Description = "[Optional] Modify the effective Izz/Ivv TO this value";
        Params.Input[3].Name = "J Modifier [" + unit + "\u2074]";
        Params.Input[3].Description = "[Optional] Modify the effective J TO this value";
        Params.Input[4].Name = "K11 Modifier [-]";
        Params.Input[4].Description = "[Optional] Modify the effective Kyy/Kuu TO this value";
        Params.Input[5].Name = "K22 Modifier [-]";
        Params.Input[5].Description = "[Optional] Modify the effective Kzz/Kvv TO this value";
        Params.Input[6].Name = "Volume Modifier [" + volUnit + "]";
        Params.Input[6].Description = "[Optional] Modify the effective Volume/Length TO this value";
      } else {
        Params.Input[0].Name = "Area Modifier";
        Params.Input[0].Description
          = "[Optional] Modify the effective Area BY this decimal fraction value (Default = 1.0 -> 100%)";
        Params.Input[1].Name = "I11 Modifier";
        Params.Input[1].Description
          = "[Optional] Modify the effective Iyy/Iuu BY this decimal fraction value (Default = 1.0 -> 100%)";
        Params.Input[2].Name = "I22 Modifier";
        Params.Input[2].Description
          = "[Optional] Modify the effective Izz/Ivv BY this decimal fraction value (Default = 1.0 -> 100%)";
        Params.Input[3].Name = "J Modifier";
        Params.Input[3].Description
          = "[Optional] Modify the effective J BY this decimal fraction value (Default = 1.0 -> 100%)";
        Params.Input[4].Name = "K11 Modifier";
        Params.Input[4].Description
          = "[Optional] Modify the effective Kyy/Kuu BY this decimal fraction value (Default = 1.0 -> 100%)";
        Params.Input[5].Name = "K22 Modifier";
        Params.Input[5].Description
          = "[Optional] Modify the effective Kzz/Kvv BY this decimal fraction value (Default = 1.0 -> 100%)";
        Params.Input[6].Name = "Volume Modifier";
        Params.Input[6].Description
          = "[Optional] Modify the effective Volume/Length BY this decimal fraction value (Default = 1.0 -> 100%)";
      }
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetBoolean("toMode", _toMode);
      return base.Write(writer);
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Modify type",
        "Density unit",
        "Stress calc.",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(_optionTypes);
      _selectedItems.Add(_optionTypes[0]);

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.LinearDensity));
      _selectedItems.Add(LinearDensity.GetAbbreviation(_densityUnit));

      _dropDownItems.Add(_stressOptions);
      _selectedItems.Add(_stressOptions[0]);

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = LinearDensity.GetAbbreviation(_densityUnit);

      pManager.AddGenericParameter("Area Modifier", "A",
        "[Optional] Modify the effective Area BY this decimal fraction value (Default = 1.0 -> 100%)",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("I11 Modifier", "I11",
        "[Optional] Modify the effective Iyy/Iuu BY this decimal fraction value (Default = 1.0 -> 100%)",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("I22 Modifier", "I22",
        "[Optional] Modify the effective Izz/Ivv BY this decimal fraction value (Default = 1.0 -> 100%)",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("J Modifier", "J",
        "[Optional] Modify the effective J BY this decimal fraction value (Default = 1.0 -> 100%)",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("K11 Modifier", "K11",
        "[Optional] Modify the effective Kyy/Kuu BY this decimal fraction value (Default = 1.0 -> 100%)",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("K22 Modifier", "K22",
        "[Optional] Modify the effective Kzz/Kvv BY this decimal fraction value (Default = 1.0 -> 100%)",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Volume Modifier", "V",
        "[Optional] Modify the effective Volume/Length BY this decimal fraction value (Default = 1.0 -> 100%)",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Additional Mass [" + unitAbbreviation + "]", "+kg",
        "[Optional] Additional mass per unit length (Default = 0 -> no additional mass)",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Principal Bending Axis", "Ax",
        "[Optional] Set to 'true' to use Principal (u,v) Axis for Bending. If false (and by default), Local (y,z) Axis will be used",
        GH_ParamAccess.item, false);
      pManager.AddBooleanParameter("Reference Point Centroid", "Ref",
        "[Optional] Set to 'true' to use the Centroid as Analysis Reference Point. If false (and by default), the specified point will be used",
        GH_ParamAccess.item, false);

      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaSectionModifierParameter());
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var modifier = new GsaSectionModifier();
      if (_toMode) {
        AreaUnit areaUnit = UnitsHelper.GetAreaUnit(_lengthUnit);
        AreaMomentOfInertiaUnit inertiaUnit = UnitsHelper.GetAreaMomentOfInertiaUnit(_lengthUnit);
        VolumePerLengthUnit volUnit = UnitsHelper.GetVolumePerLengthUnit(_lengthUnit);

        if (Params.Input[0].SourceCount > 0) {
          modifier.AreaModifier = Input.UnitNumber(this, da, 0, areaUnit, true);
        }

        if (Params.Input[1].SourceCount > 0) {
          modifier.I11Modifier = Input.UnitNumber(this, da, 1, inertiaUnit, true);
        }

        if (Params.Input[2].SourceCount > 0) {
          modifier.I22Modifier = Input.UnitNumber(this, da, 2, inertiaUnit, true);
        }

        if (Params.Input[3].SourceCount > 0) {
          modifier.JModifier = Input.UnitNumber(this, da, 3, inertiaUnit, true);
        }

        if (Params.Input[4].SourceCount > 0) {
          modifier.K11Modifier = Input.RatioInDecimalFractionToDecimalFraction(this, da, 4);
        }

        if (Params.Input[5].SourceCount > 0) {
          modifier.K22Modifier = Input.RatioInDecimalFractionToDecimalFraction(this, da, 5);
        }

        if (Params.Input[6].SourceCount > 0) {
          modifier.VolumeModifier = Input.UnitNumber(this, da, 6, volUnit, true);
        }
      } else {
        modifier.AreaModifier = Input.RatioInDecimalFractionToPercentage(this, da, 0);
        modifier.I11Modifier = Input.RatioInDecimalFractionToPercentage(this, da, 1);
        modifier.I22Modifier = Input.RatioInDecimalFractionToPercentage(this, da, 2);
        modifier.JModifier = Input.RatioInDecimalFractionToPercentage(this, da, 3);
        modifier.K11Modifier = Input.RatioInDecimalFractionToPercentage(this, da, 4);
        modifier.K22Modifier = Input.RatioInDecimalFractionToPercentage(this, da, 5);
        modifier.VolumeModifier = Input.RatioInDecimalFractionToPercentage(this, da, 6);
      }

      modifier.AdditionalMass = (LinearDensity)Input.UnitNumber(this, da, 7, _densityUnit, true);

      bool ax = false;
      if (da.GetData(8, ref ax)) {
        modifier.IsBendingAxesPrincipal = ax;
      }

      bool pt = false;
      if (da.GetData(9, ref pt)) {
        modifier.IsReferencePointCentroid = pt;
      }

      modifier.StressOption = _stressOption;

      da.SetData(0, new GsaSectionModifierGoo(modifier));
    }

    protected override void UpdateUIFromSelectedItems() {
      if (_toMode) {
        _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);
        _densityUnit
          = (LinearDensityUnit)UnitsHelper.Parse(typeof(LinearDensityUnit), _selectedItems[2]);
        if (_selectedItems[3] == _stressOptions[0]) {
          _stressOption = StressOptionType.NoCalculation;
        }

        if (_selectedItems[3] == _stressOptions[1]) {
          _stressOption = StressOptionType.UseUnmodified;
        }

        if (_selectedItems[3] == _stressOptions[2]) {
          _stressOption = StressOptionType.UseModified;
        }
      } else {
        _densityUnit
          = (LinearDensityUnit)UnitsHelper.Parse(typeof(LinearDensityUnit), _selectedItems[1]);
        if (_selectedItems[2] == _stressOptions[0]) {
          _stressOption = StressOptionType.NoCalculation;
        }

        if (_selectedItems[2] == _stressOptions[1]) {
          _stressOption = StressOptionType.UseUnmodified;
        }

        if (_selectedItems[2] == _stressOptions[2]) {
          _stressOption = StressOptionType.UseModified;
        }
      }

      base.UpdateUIFromSelectedItems();
    }
  }
}
