using System;
using System.Drawing;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a Material and ouput the information
  /// </summary>
  public class EditSectionModifier2_OBSOLETE : GH_OasysComponent, IGH_VariableParameterComponent {
    public override Guid ComponentGuid => new Guid("db2046cc-236d-44a5-aa88-1394dbc4558f");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GetSectionModifier;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;
    private LinearDensityUnit _linearDensityUnit = DefaultUnits.LinearDensityUnit;

    public EditSectionModifier2_OBSOLETE() : base("Edit Section Modifier", "ModifierEdit",
      "Modify GSA Section Modifier", CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);

      var lengthUnitsMenu = new ToolStripMenuItem("Length") {
        Enabled = true,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => UpdateLength(unit)) {
          Checked = unit == Length.GetAbbreviation(_lengthUnit),
          Enabled = true,
        };
        lengthUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      var densityUnitsMenu = new ToolStripMenuItem("Density") {
        Enabled = true,
      };
      foreach (string unit in
        UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.LinearDensity)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => UpdateDensity(unit)) {
          Checked = unit == LinearDensity.GetAbbreviation(_linearDensityUnit),
          Enabled = true,
        };
        densityUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      var unitsMenu = new ToolStripMenuItem("Select Units", Resources.Units);
      unitsMenu.DropDownItems.AddRange(new ToolStripItem[] {
        lengthUnitsMenu,
        densityUnitsMenu,
      });
      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;

      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index) {
      return false;
    }

    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index) {
      return false;
    }

    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index) {
      return null;
    }

    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) {
      return false;
    }

    public override bool Read(GH_IReader reader) {
      _lengthUnit
        = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("LengthUnit"));
      _linearDensityUnit = (LinearDensityUnit)UnitsHelper.Parse(typeof(LinearDensityUnit),
        reader.GetString("DensityUnit"));
      return base.Read(reader);
    }

    public virtual void VariableParameterMaintenance() {
      string unit = Length.GetAbbreviation(_lengthUnit);
      string volUnit
        = VolumePerLength.GetAbbreviation(UnitsHelper.GetVolumePerLengthUnit(_lengthUnit));
      Params.Output[1].Name = "Area Modifier [" + unit + "\u00B2]";
      Params.Output[2].Name = "I11 Modifier [" + unit + "\u2074]";
      Params.Output[3].Name = "I22 Modifier [" + unit + "\u2074]";
      Params.Output[7].Name = "Volume Modifier [" + volUnit + "]";
      string unitAbbreviation = LinearDensity.GetAbbreviation(_linearDensityUnit);
      Params.Output[8].Name = "Additional Mass [" + unitAbbreviation + "]";
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetString("LengthUnit", _lengthUnit.ToString());
      writer.SetString("DensityUnit", _linearDensityUnit.ToString());
      return base.Write(writer);
    }

    protected override void BeforeSolveInstance() {
      UpdateMessage();
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaSectionModifierParameter(), GsaSectionModifierGoo.Name,
        GsaSectionModifierGoo.NickName,
        GsaSectionModifierGoo.Description
        + " to get or set information for. Leave blank to create a new "
        + GsaSectionModifierGoo.Name, GH_ParamAccess.item);

      pManager.AddGenericParameter("Area Modifier", "A",
        "Modify the effective Area using either:" + Environment.NewLine
        + "BY using a Percentage UnitNumber (tweaking the existing value BY this percentage)"
        + Environment.NewLine + "TO using an Area UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("I11 Modifier", "I11",
        "Modify the effective Iyy/Iuu using either:" + Environment.NewLine
        + "BY using a Percentage UnitNumber (tweaking the existing value BY this percentage)"
        + Environment.NewLine + "TO using an AreaMomentOfInertia UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("I22 Modifier", "I22",
        "Modify the effective Izz/Ivv using either:" + Environment.NewLine
        + "BY using a Percentage UnitNumber (tweaking the existing value BY this percentage)"
        + Environment.NewLine + "TO using an AreaMomentOfInertia UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("J Modifier", "J",
        "Modify the effective J using either:" + Environment.NewLine
        + "BY using a Percentage UnitNumber (tweaking the existing value BY this percentage)"
        + Environment.NewLine + "TO using an AreaMomentOfInertia UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("K11 Modifier", "K11",
        "Modify the effective Kyy/Kuu using either:" + Environment.NewLine
        + "BY using a Percentage UnitNumber (tweaking the existing value BY this percentage)"
        + Environment.NewLine + "TO using a DecimalFraction UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("K22 Modifier", "K22",
        "Modify the effective Kzz/Kvv using either:" + Environment.NewLine
        + "BY using a Percentage UnitNumber (tweaking the existing value BY this percentage)"
        + Environment.NewLine + "TO using a DecimalFraction UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("Volume Modifier", "V",
        "Modify the effective Volume/Length using either:" + Environment.NewLine
        + "BY using a Percentage UnitNumber (tweaking the existing value BY this percentage)"
        + Environment.NewLine + "TO using a VolumePerLength UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("Additional Mass", "+kg",
        "Additional mass per unit length using a LinearDensity UnitNumber", GH_ParamAccess.item);

      pManager.AddBooleanParameter("Principal Bending Axis", "Ax",
        "[Optional] Set to 'true' to use Principal (u,v) Axis for Bending. If false (and by default), Local (y,z) Axis will be used",
        GH_ParamAccess.item);

      pManager.AddBooleanParameter("Reference Point Centroid", "Ref",
        "[Optional] Set to 'true' to use the Centroid as Analysis Reference Point. If false (and by default), the specified point will be used",
        GH_ParamAccess.item);

      pManager.AddIntegerParameter("Stress Option Type", "Str",
        "Set the Stress Option Type. Accepted inputs are:" + Environment.NewLine
        + "0: No calculation" + Environment.NewLine + "1: Use modified section properties"
        + Environment.NewLine + "2: Use unmodified section properties", GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaSectionModifierParameter(), GsaSectionModifierGoo.Name,
        GsaSectionModifierGoo.NickName,
        GsaSectionModifierGoo.Description + " with applied changes.", GH_ParamAccess.item);

      pManager.AddGenericParameter("Area Modifier", "A",
        "Modified effective Area in either:" + Environment.NewLine + "BY as a Percentage UnitNumber"
        + Environment.NewLine + "TO as an Area UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("I11 Modifier", "I11",
        "Modify the effective Iyy/Iuu in either:" + Environment.NewLine
        + "BY as a Percentage UnitNumber" + Environment.NewLine
        + "TO as an AreaMomentOfInertia UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("I22 Modifier", "I22",
        "Modify the effective Izz/Ivv in either:" + Environment.NewLine
        + "BY as a Percentage UnitNumber" + Environment.NewLine
        + "TO as an AreaMomentOfInertia UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("J Modifier", "J",
        "Modify the effective J in either:" + Environment.NewLine + "BY as a Percentage UnitNumber"
        + Environment.NewLine + "TO as an AreaMomentOfInertia UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("K11 Modifier", "K11",
        "Modify the effective Kyy/Kuu in either:" + Environment.NewLine
        + "BY as a Percentage UnitNumber" + Environment.NewLine
        + "TO as a DecimalFraction UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("K22 Modifier", "K22",
        "Modify the effective Kzz/Kvv in either:" + Environment.NewLine
        + "BY as a Percentage UnitNumber" + Environment.NewLine
        + "TO as a DecimalFraction UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("Volume Modifier", "V",
        "Modify the effective Volume/Length in either:" + Environment.NewLine
        + "BY as a Percentage UnitNumber" + Environment.NewLine
        + "TO as a VolumePerLength UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("Additional Mass", "+kg", "Additional mass per unit length",
        GH_ParamAccess.item);

      pManager.AddBooleanParameter("Principal Bending Axis", "Ax",
        "If 'true' GSA will use Principal (u,v) Axis for Bending. If false, Local (y,z) Axis will be used",
        GH_ParamAccess.item);

      pManager.AddBooleanParameter("Reference Point Centroid", "Ref",
        "If 'true' GSA will use the Centroid as Analysis Reference Point. If false, the specified point will be used",
        GH_ParamAccess.item);

      pManager.AddGenericParameter("Stress Option Type", "Str",
        "Get the Stress Option Type:" + Environment.NewLine + "0: No Calculation"
        + Environment.NewLine + "1: Use Modified section properties" + Environment.NewLine
        + "2: Use Unmodified section properties", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var modifier = new GsaSectionModifier();

      GsaSectionModifierGoo modifierGoo = null;
      if (da.GetData(0, ref modifierGoo)) {
        modifier = modifierGoo.Value.Duplicate();
      }

      if (Params.Input[1].SourceCount > 0) {
        var ghTyp = new GH_ObjectWrapper();
        da.GetData(1, ref ghTyp);
        if (GH_Convert.ToString(ghTyp.Value, out string txt, GH_Conversion.Both)) {
          if (Area.TryParse(txt, out Area res)) {
            modifier.AreaModifier = res;
          } else {
            try {
              modifier.AreaModifier
                = Input.UnitNumberOrDoubleAsRatioToPercentage(this, da, 1, true).Value;
            } catch (Exception e) {
              this.AddRuntimeError(e.Message);
              return;
            }
          }
        }
      }

      if (Params.Input[2].SourceCount > 0) {
        var ghTyp = new GH_ObjectWrapper();
        da.GetData(2, ref ghTyp);
        if (GH_Convert.ToString(ghTyp.Value, out string txt, GH_Conversion.Both)) {
          if (AreaMomentOfInertia.TryParse(txt, out AreaMomentOfInertia res)) {
            modifier.I11Modifier = res;
          } else {
            try {
              modifier.I11Modifier = Input.UnitNumberOrDoubleAsRatioToPercentage(this, da, 2, true)
               .Value;
            } catch (Exception e) {
              this.AddRuntimeError(e.Message);
              return;
            }
          }
        }
      }

      if (Params.Input[3].SourceCount > 0) {
        var ghTyp = new GH_ObjectWrapper();
        da.GetData(3, ref ghTyp);
        if (GH_Convert.ToString(ghTyp.Value, out string txt, GH_Conversion.Both)) {
          if (AreaMomentOfInertia.TryParse(txt, out AreaMomentOfInertia res)) {
            modifier.I22Modifier = res;
          } else {
            try {
              modifier.I22Modifier = Input.UnitNumberOrDoubleAsRatioToPercentage(this, da, 3, true)
               .Value;
            } catch (Exception e) {
              this.AddRuntimeError(e.Message);
              return;
            }
          }
        }
      }

      if (Params.Input[4].SourceCount > 0) {
        var ghTyp = new GH_ObjectWrapper();
        da.GetData(4, ref ghTyp);
        if (GH_Convert.ToString(ghTyp.Value, out string txt, GH_Conversion.Both)) {
          if (AreaMomentOfInertia.TryParse(txt, out AreaMomentOfInertia res)) {
            modifier.JModifier = res;
          } else {
            try {
              modifier.JModifier = Input.UnitNumberOrDoubleAsRatioToPercentage(this, da, 4, true)
               .Value;
            } catch (Exception e) {
              this.AddRuntimeError(e.Message);
              return;
            }
          }
        }
      }

      if (Params.Input[5].SourceCount > 0) {
        modifier.K11Modifier = Input.RatioInDecimalFractionToPercentage(this, da, 5);
      }

      if (Params.Input[6].SourceCount > 0) {
        modifier.K22Modifier = Input.RatioInDecimalFractionToPercentage(this, da, 6);
      }

      if (Params.Input[7].SourceCount > 0) {
        var ghTyp = new GH_ObjectWrapper();
        da.GetData(7, ref ghTyp);
        if (GH_Convert.ToString(ghTyp.Value, out string txt, GH_Conversion.Both)) {
          if (VolumePerLength.TryParse(txt, out VolumePerLength res)) {
            modifier.VolumeModifier = res;
          } else {
            try {
              modifier.VolumeModifier = Input
               .UnitNumberOrDoubleAsRatioToPercentage(this, da, 7, true).Value;
            } catch (Exception e) {
              this.AddRuntimeError(e.Message);
              return;
            }
          }
        }
      }

      if (Params.Input[8].SourceCount > 0) {
        var ghTyp = new GH_ObjectWrapper();
        if (da.GetData(8, ref ghTyp)) {
          if (ghTyp.Value is GH_UnitNumber unitNumber) {
            if (unitNumber.Value.QuantityInfo.UnitType != typeof(LinearDensityUnit)) {
              this.AddRuntimeError("Error in " + Params.Input[8].NickName
                + " input: Wrong unit type" + Environment.NewLine + "Unit type is "
                + unitNumber.Value.QuantityInfo.Name + " but must be LinearDensity");
              return;
            }

            modifier.AdditionalMass = (LinearDensity)unitNumber.Value;
          } else if (GH_Convert.ToDouble(ghTyp.Value, out double val, GH_Conversion.Both)) {
            modifier.AdditionalMass = new LinearDensity(val, _linearDensityUnit);
          } else if (GH_Convert.ToString(ghTyp.Value, out string txt, GH_Conversion.Both)) {
            if (LinearDensity.TryParse(txt, out LinearDensity res)) {
              modifier.AdditionalMass = res;
            } else {
              this.AddRuntimeError("Unable to convert " + Params.Input[8].NickName
                + " to LinearDensity");
            }
          } else {
            this.AddRuntimeError("Unable to convert " + Params.Input[8].NickName
              + " to UnitNumber");
            return;
          }
        }
      }

      bool ax = false;
      if (da.GetData(9, ref ax)) {
        modifier.IsBendingAxesPrincipal = ax;
      }

      bool pt = false;
      if (da.GetData(10, ref pt)) {
        modifier.IsReferencePointCentroid = pt;
      }

      var obj = new GH_ObjectWrapper();
      if (da.GetData(11, ref obj)) {
        if (GH_Convert.ToInt32(obj.Value, out int stress, GH_Conversion.Both)) {
          modifier.StressOption = (GsaSectionModifier.StressOptionType)stress;
        }
      }

      da.SetData(0, new GsaSectionModifierGoo(modifier));
      da.SetData(1,
        modifier._sectionModifier.AreaModifier.Option == SectionModifierOptionType.BY ?
          new GH_UnitNumber(modifier.AreaModifier) :
          new GH_UnitNumber(modifier.AreaModifier.ToUnit(UnitsHelper.GetAreaUnit(_lengthUnit))));

      da.SetData(2,
        modifier._sectionModifier.I11Modifier.Option == SectionModifierOptionType.BY ?
          new GH_UnitNumber(modifier.I11Modifier) : new GH_UnitNumber(
            modifier.I11Modifier.ToUnit(UnitsHelper.GetAreaMomentOfInertiaUnit(_lengthUnit))));

      da.SetData(3,
        modifier._sectionModifier.I22Modifier.Option == SectionModifierOptionType.BY ?
          new GH_UnitNumber(modifier.I22Modifier) : new GH_UnitNumber(
            modifier.I22Modifier.ToUnit(UnitsHelper.GetAreaMomentOfInertiaUnit(_lengthUnit))));

      da.SetData(4,
        modifier._sectionModifier.JModifier.Option == SectionModifierOptionType.BY ?
          new GH_UnitNumber(modifier.JModifier) : new GH_UnitNumber(
            modifier.JModifier.ToUnit(UnitsHelper.GetAreaMomentOfInertiaUnit(_lengthUnit))));

      da.SetData(5, new GH_UnitNumber(modifier.K11Modifier));
      da.SetData(6, new GH_UnitNumber(modifier.K22Modifier));

      da.SetData(7,
        modifier._sectionModifier.VolumeModifier.Option == SectionModifierOptionType.BY ?
          new GH_UnitNumber(modifier.VolumeModifier) : new GH_UnitNumber(
            modifier.VolumeModifier.ToUnit(UnitsHelper.GetVolumePerLengthUnit(_lengthUnit))));

      da.SetData(8, new GH_UnitNumber(modifier.AdditionalMass.ToUnit(_linearDensityUnit)));

      da.SetData(9, modifier.IsBendingAxesPrincipal);
      da.SetData(10, modifier.IsReferencePointCentroid);
      da.SetData(11, (int)modifier.StressOption);
    }

    private void Update() {
      UpdateMessage();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }

    private void UpdateDensity(string unit) {
      _linearDensityUnit = (LinearDensityUnit)UnitsHelper.Parse(typeof(LinearDensityUnit), unit);
      Update();
    }

    private void UpdateLength(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      Update();
    }

    private void UpdateMessage() {
      Message = Length.GetAbbreviation(_lengthUnit) + ", "
        + LinearDensity.GetAbbreviation(_linearDensityUnit);
    }
  }
}
