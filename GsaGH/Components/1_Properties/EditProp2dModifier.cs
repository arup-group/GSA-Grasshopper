using System;
using System.Drawing;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
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
  public class EditProp2dModifier : GH_OasysComponent, IGH_VariableParameterComponent {
    public override Guid ComponentGuid => new Guid("945d2e3b-4e5d-4a6e-a64b-c2447c0c0723");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditProp2dModifier;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;
    private LinearDensityUnit _linearDensityUnit = DefaultUnits.LinearDensityUnit;

    public EditProp2dModifier() : base("Edit 2D Property Modifier", "ModifierEdit",
      "Edit GSA 2D Property Modifier", CategoryName.Name(), SubCategoryName.Cat1()) {
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
      string lengthUnitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      string areaUnitAbbreviation = Area.GetAbbreviation(UnitsHelper.GetAreaUnit(_lengthUnit));
      string volumeUnitAbbreviation =
        VolumePerLength.GetAbbreviation(UnitsHelper.GetVolumePerLengthUnit(_lengthUnit));
      string inertiaUnitAbbreviation =
        AreaMomentOfInertia.GetAbbreviation(UnitsHelper.GetAreaMomentOfInertiaUnit(_lengthUnit));

      Params.Input[0].Name = "In-plane Modifier [" + areaUnitAbbreviation + "/" + lengthUnitAbbreviation + "]";
      Params.Input[1].Name = "Bending Modifier [" + inertiaUnitAbbreviation + "/" + lengthUnitAbbreviation + "]";
      Params.Input[2].Name = "Shear Modifier [" + areaUnitAbbreviation + "/" + lengthUnitAbbreviation + "]";
      Params.Input[3].Name = "Volume Modifier [" + volumeUnitAbbreviation + "/" + areaUnitAbbreviation + "]";
      Params.Output[5].Name = "Additional Mass [" + LinearDensity.GetAbbreviation(_linearDensityUnit) + "]";
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
      pManager.AddParameter(new GsaProp2dModifierParameter(), GsaProp2dModifierGoo.Name,
        GsaProp2dModifierGoo.NickName, GsaProp2dModifierGoo.Description + " to get or set" +
        "information for. Leave blank to create a new " + GsaProp2dModifierGoo.Name, GH_ParamAccess.item);

      pManager.AddGenericParameter("In-plane Modifier", "Ip", "Modify the effective in-plane" +
        " stiffness using either:" + Environment.NewLine + "BY using a Percentage UnitNumber" +
        " (tweaking the existing value BY this percentage)" + Environment.NewLine +
        "TO using a Length UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("Bending Modifier", "B", "Modify the effective bending stiffness" +
        " using either:" + Environment.NewLine + "BY using a Percentage UnitNumber (tweaking the" +
        " existing value BY this percentage)" + Environment.NewLine + "TO using a Volume UnitNumber",
        GH_ParamAccess.item);

      pManager.AddGenericParameter("Shear Modifier", "S", "Modify the effective shear stiffness" +
        " using either:" + Environment.NewLine + "BY using a Percentage UnitNumber (tweaking the" +
        " existing value BY this percentage)" + Environment.NewLine + "TO using a Length UnitNumber",
        GH_ParamAccess.item);

      pManager.AddGenericParameter("Volume Modifier", "V", "Modify the effective volume using either:"
        + Environment.NewLine + "BY using a Percentage UnitNumber (tweaking the existing value BY" +
        " this percentage)" + Environment.NewLine + "TO using a Length UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("Additional Mass", "+kg", "Additional mass per unit length using" +
        " a LinearDensity UnitNumber", GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaProp2dModifierParameter(), GsaProp2dModifierGoo.Name,
        GsaProp2dModifierGoo.NickName, GsaProp2dModifierGoo.Description +
        " with applied changes.", GH_ParamAccess.item);

      pManager.AddGenericParameter("In-plane Modifier", "Ip", "Modified effective in-plane" +
        " stiffness in either:" + Environment.NewLine + "BY as a Percentage UnitNumber" +
        Environment.NewLine + "TO as a Length UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("Bending Modifier", "B", "Modify the effective bending stiffness" +
        " in either:" + Environment.NewLine + "BY as a Percentage UnitNumber" + Environment.NewLine
        + "TO as a Volume UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("Shear Modifier", "S", "Modify the effective shear stiffness in" +
        " either:" + Environment.NewLine + "BY as a Percentage UnitNumber" + Environment.NewLine +
        "TO as a Length UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("Volume Modifier", "V", "Modify the effective volume in either:"
        + Environment.NewLine + "BY as a Percentage UnitNumber" + Environment.NewLine +
        "TO as a a Lenght UnitNumber", GH_ParamAccess.item);

      pManager.AddGenericParameter("Additional Mass", "+kg", "Additional mass per unit length",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var modifier = new GsaProp2dModifier();

      GsaProp2dModifierGoo modifierGoo = null;
      if (da.GetData(0, ref modifierGoo)) {
        modifier = modifierGoo.Value.Duplicate();
      }

      if (Params.Input[1].SourceCount > 0) {
        var ghTyp = new GH_ObjectWrapper();
        da.GetData(1, ref ghTyp);
        if (GH_Convert.ToString(ghTyp.Value, out string txt, GH_Conversion.Both)) {
          if (Length.TryParse(txt, out Length res)) {
            modifier.InPlane = res;
          } else {
            try {
              modifier.InPlane = Input.UnitNumberOrDoubleAsRatioToPercentage(this, da, 1, true).Value;
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
          if (Volume.TryParse(txt, out Volume res)) {
            modifier.Bending = res;
          } else {
            try {
              modifier.Bending = Input.UnitNumberOrDoubleAsRatioToPercentage(this, da, 2, true).Value;
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
          if (Length.TryParse(txt, out Length res)) {
            modifier.Shear = res;
          } else {
            try {
              modifier.Shear = Input.UnitNumberOrDoubleAsRatioToPercentage(this, da, 3, true).Value;
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
          if (Length.TryParse(txt, out Length res)) {
            modifier.Volume = res;
          } else {
            try {
              modifier.Volume = Input.UnitNumberOrDoubleAsRatioToPercentage(this, da, 4, true).Value;
            } catch (Exception e) {
              this.AddRuntimeError(e.Message);
              return;
            }
          }
        }
      }

      if (Params.Input[5].SourceCount > 0) {
        var ghTyp = new GH_ObjectWrapper();
        if (da.GetData(4, ref ghTyp)) {
          if (ghTyp.Value is GH_UnitNumber unitNumber) {
            if (unitNumber.Value.QuantityInfo.UnitType != typeof(LinearDensityUnit)) {
              this.AddRuntimeError("Error in " + Params.Input[8].NickName + " input: Wrong unit type"
                + Environment.NewLine + "Unit type is " + unitNumber.Value.QuantityInfo.Name +
                " but must be LinearDensity");
              return;
            }

            modifier.AdditionalMass = (LinearDensity)unitNumber.Value;
          } else if (GH_Convert.ToDouble(ghTyp.Value, out double val, GH_Conversion.Both)) {
            modifier.AdditionalMass = new LinearDensity(val, _linearDensityUnit);
          } else if (GH_Convert.ToString(ghTyp.Value, out string txt, GH_Conversion.Both)) {
            if (LinearDensity.TryParse(txt, out LinearDensity res)) {
              modifier.AdditionalMass = res;
            } else {
              this.AddRuntimeError("Unable to convert " + Params.Input[8].NickName + " to LinearDensity");
            }
          } else {
            this.AddRuntimeError("Unable to convert " + Params.Input[8].NickName + " to UnitNumber");
            return;
          }
        }
      }

      da.SetData(0, new GsaProp2dModifierGoo(modifier));
      da.SetData(1, new GH_UnitNumber(modifier.InPlane));
      da.SetData(2, new GH_UnitNumber(modifier.Bending));
      da.SetData(3, new GH_UnitNumber(modifier.Shear));
      da.SetData(4, new GH_UnitNumber(modifier.Volume));
      da.SetData(5, new GH_UnitNumber(modifier.AdditionalMass.ToUnit(_linearDensityUnit)));
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
