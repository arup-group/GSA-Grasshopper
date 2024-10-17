using System;
using System.Drawing;
using System.Windows.Forms;

using GH_IO.Serialization;

using Grasshopper.Kernel;

using GsaAPI;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;
using OasysUnits.Units;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Components {
  public class GetSpringProperty : GH_OasysComponent, IGH_VariableParameterComponent {
    public override Guid ComponentGuid => new Guid("e6e90fc4-157a-4fad-85ec-6957aed91fa9");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GetSpringProperty;
    private RotationalStiffnessUnit _rotationalStiffnessUnit = RotationalStiffnessUnit.NewtonMeterPerRadian;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;
    private ForcePerLengthUnit _stiffnessUnit = DefaultUnits.ForcePerLengthUnit;

    public GetSpringProperty() : base("Get Spring Property", "GetPS",
      "Get GSA Spring Property", CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);

      var stiffnessUnitsMenu = new ToolStripMenuItem("Stiffness") {
        Enabled = true,
      };
      foreach (string unit in
        UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerLength)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => UpdateStiffness(unit)) {
          Checked = unit == ForcePerLength.GetAbbreviation(_stiffnessUnit),
          Enabled = true,
        };
        stiffnessUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      var rotationalStiffnessUnitsMenu = new ToolStripMenuItem("Rotational Stiffness") {
        Enabled = true,
      };
      foreach (string unit in CreateSpringProperty.FilteredRotationalStiffnessUnits) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => UpdateRotationalStiffness(unit)) {
          Checked = unit == RotationalStiffness.GetAbbreviation(_rotationalStiffnessUnit),
          Enabled = true,
        };
        rotationalStiffnessUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

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

      var unitsMenu = new ToolStripMenuItem("Select Units", Resources.ModelUnits);
      unitsMenu.DropDownItems.AddRange(new ToolStripItem[] {
        stiffnessUnitsMenu,
        rotationalStiffnessUnitsMenu,
        lengthUnitsMenu,
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
      _stiffnessUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit),
        reader.GetString("StiffnessUnit"));
      _rotationalStiffnessUnit = (RotationalStiffnessUnit)UnitsHelper.Parse(typeof(RotationalStiffnessUnit),
        reader.GetString("RotationalStiffnessUnit"));
      return base.Read(reader);
    }

    public virtual void VariableParameterMaintenance() {
      string lengthAbr = Length.GetAbbreviation(_lengthUnit);
      string rotationalStiffnessAbr = RotationalStiffness.GetAbbreviation(_rotationalStiffnessUnit);
      string stiffnessAbr = ForcePerLength.GetAbbreviation(_stiffnessUnit);

      Params.Output[0].Name = "Name";
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetString("StiffnessUnit", _stiffnessUnit.ToString());
      writer.SetString("RotationalStiffnessUnit", _rotationalStiffnessUnit.ToString());
      writer.SetString("LengthUnit", _lengthUnit.ToString());
      return base.Write(writer);
    }

    protected override void BeforeSolveInstance() {
      UpdateMessage();
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaSpringPropertyParameter(), GsaSpringPropertyGoo.Name,
        GsaSpringPropertyGoo.NickName,
        GsaSpringPropertyGoo.Description
        + " to get information for."
        + GsaSpringPropertyGoo.Name, GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string lengthAbr = Length.GetAbbreviation(_lengthUnit);
      string rotationalStiffnessAbr = RotationalStiffness.GetAbbreviation(_rotationalStiffnessUnit);
      string stiffnessAbr = ForcePerLength.GetAbbreviation(_stiffnessUnit);

      pManager.AddTextParameter("Name", "Na", "Spring Property Name", GH_ParamAccess.item);

      pManager.AddIntegerParameter("Spring Curve x", "SCx", "Spring Curve in x direction", GH_ParamAccess.item);

      pManager.AddGenericParameter("Stiffness x [" + stiffnessAbr + "]", "Sx", "Stiffness in x direction", GH_ParamAccess.item);

      pManager.AddIntegerParameter("Spring Curve y", "SCy", "Spring Curve y", GH_ParamAccess.item);

      pManager.AddGenericParameter("Stiffness y [" + stiffnessAbr + "]", "Sy", "Stiffness in y direction", GH_ParamAccess.item);

      pManager.AddIntegerParameter("Spring Curve z", "SCz", "Spring Curve z", GH_ParamAccess.item);

      pManager.AddGenericParameter("Stiffness z [" + stiffnessAbr + "]", "Sz", "Stiffness in z direction", GH_ParamAccess.item);

      pManager.AddIntegerParameter("Spring Curve xx", "SCxx", "Spring Curve xx", GH_ParamAccess.item);

      pManager.AddGenericParameter("Stiffness xx [" + rotationalStiffnessAbr + "]", "Sxx", "Stiffness in xx direction", GH_ParamAccess.item);

      pManager.AddIntegerParameter("Spring Curve yy", "SCyy", "Spring Curve yy", GH_ParamAccess.item);

      pManager.AddGenericParameter("Stiffness yy [" + rotationalStiffnessAbr + "]", "Syy", "Stiffness in yy direction", GH_ParamAccess.item);

      pManager.AddIntegerParameter("Spring Curve zz", "SCzz", "Spring Curve zz", GH_ParamAccess.item);

      pManager.AddGenericParameter("Stiffness zz [" + rotationalStiffnessAbr + "]", "Szz", "Stiffness in zz direction", GH_ParamAccess.item);

      pManager.AddGenericParameter("Spring Matrix", "SM", "Spring Matrix", GH_ParamAccess.item);

      pManager.AddGenericParameter("Lockup -ve [" + lengthAbr + "]", "L-ve", "Lockup -ve", GH_ParamAccess.item);

      pManager.AddGenericParameter("Lockup +ve [" + lengthAbr + "]", "L+ve", "Lockup +ve", GH_ParamAccess.item);

      pManager.AddGenericParameter("Coeff. of Friction [" + stiffnessAbr + "]", "CF", "Coefficient of Friction", GH_ParamAccess.item);

      pManager.AddGenericParameter("Damping Ratio", "DR", "[Optional] Damping Ratio (Default = 0.0 -> 0%)", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaSpringPropertyGoo springPropertyGoo = null;
      if (!da.GetData(0, ref springPropertyGoo)) {
        this.AddRuntimeWarning("Input PS failed to collect data");
        return;
      }
      GsaSpringProperty springProperty = springPropertyGoo.Value;

      da.SetData(0, springProperty.ApiProperty.Name);

      switch (springProperty.ApiProperty) {
        case AxialSpringProperty axial:
          da.SetData(2, new GH_UnitNumber(new ForcePerLength(axial.Stiffness, ForcePerLengthUnit.NewtonPerMeter).ToUnit(_stiffnessUnit)));
          break;

        case TensionSpringProperty tension:
          da.SetData(2, new GH_UnitNumber(new ForcePerLength(tension.Stiffness, ForcePerLengthUnit.NewtonPerMeter).ToUnit(_stiffnessUnit)));
          break;

        case CompressionSpringProperty compression:
          da.SetData(2, new GH_UnitNumber(new ForcePerLength(compression.Stiffness, ForcePerLengthUnit.NewtonPerMeter).ToUnit(_stiffnessUnit)));
          break;

        case GapSpringProperty gap:
          da.SetData(2, new GH_UnitNumber(new ForcePerLength(gap.Stiffness, ForcePerLengthUnit.NewtonPerMeter).ToUnit(_stiffnessUnit)));
          break;

        case TorsionalSpringProperty torsional:
          da.SetData(8, new GH_UnitNumber(new RotationalStiffness(torsional.Stiffness, RotationalStiffnessUnit.NewtonMeterPerRadian).ToUnit(_rotationalStiffnessUnit)));
          break;

        case GeneralSpringProperty general:
          da.SetData(1, general.SpringCurveX);
          da.SetData(3, general.SpringCurveY);
          da.SetData(5, general.SpringCurveZ);
          da.SetData(7, general.SpringCurveXX);
          da.SetData(9, general.SpringCurveYY);
          da.SetData(11, general.SpringCurveZZ);

          if (general.StiffnessX != null) {
            da.SetData(2, new GH_UnitNumber(new ForcePerLength((double)general.StiffnessX, ForcePerLengthUnit.NewtonPerMeter).ToUnit(_stiffnessUnit)));
          }
          if (general.StiffnessY != null) {
            da.SetData(4, new GH_UnitNumber(new ForcePerLength((double)general.StiffnessY, ForcePerLengthUnit.NewtonPerMeter).ToUnit(_stiffnessUnit)));
          }
          if (general.StiffnessZ != null) {
            da.SetData(6, new GH_UnitNumber(new ForcePerLength((double)general.StiffnessZ, ForcePerLengthUnit.NewtonPerMeter).ToUnit(_stiffnessUnit)));
          }
          if (general.StiffnessXX != null) {
            da.SetData(8, new GH_UnitNumber(new RotationalStiffness((double)general.StiffnessXX, RotationalStiffnessUnit.NewtonMeterPerRadian).ToUnit(_rotationalStiffnessUnit)));
          }
          if (general.StiffnessYY != null) {
            da.SetData(10, new GH_UnitNumber(new RotationalStiffness((double)general.StiffnessYY, RotationalStiffnessUnit.NewtonMeterPerRadian).ToUnit(_rotationalStiffnessUnit)));
          }
          if (general.StiffnessZZ != null) {
            da.SetData(12, new GH_UnitNumber(new RotationalStiffness((double)general.StiffnessZZ, RotationalStiffnessUnit.NewtonMeterPerRadian).ToUnit(_rotationalStiffnessUnit)));
          }
          break;

        case MatrixSpringProperty matrix:
          da.SetData(13, matrix.SpringMatrix);
          break;

        case LockupSpringProperty lockup:
          da.SetData(14, new GH_UnitNumber(new Length((double)lockup.NegativeLockup, LengthUnit.Meter).ToUnit(_lengthUnit)));
          da.SetData(15, new GH_UnitNumber(new Length((double)lockup.PositiveLockup, LengthUnit.Meter).ToUnit(_lengthUnit)));
          break;

        case FrictionSpringProperty friction:
          da.SetData(2, new GH_UnitNumber(new ForcePerLength((double)friction.StiffnessX, ForcePerLengthUnit.NewtonPerMeter).ToUnit(_stiffnessUnit)));
          da.SetData(4, new GH_UnitNumber(new ForcePerLength((double)friction.StiffnessY, ForcePerLengthUnit.NewtonPerMeter).ToUnit(_stiffnessUnit)));
          da.SetData(6, new GH_UnitNumber(new ForcePerLength((double)friction.StiffnessZ, ForcePerLengthUnit.NewtonPerMeter).ToUnit(_stiffnessUnit)));
          da.SetData(16, friction.FrictionCoefficient);
          break;

        case ConnectorSpringProperty connector:
        default:
          break;
      }

      da.SetData(17, new GH_UnitNumber(new Ratio(springProperty.ApiProperty.DampingRatio, RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent)));
    }

    internal void UpdateLength(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      Update();
    }

    internal void UpdateStiffness(string unit) {
      _stiffnessUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), unit);
      Update();
    }

    internal void UpdateRotationalStiffness(string unit) {
      _rotationalStiffnessUnit = (RotationalStiffnessUnit)UnitsHelper.Parse(typeof(RotationalStiffnessUnit), unit);
      Update();
    }

    private void Update() {
      UpdateMessage();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }

    private void UpdateMessage() {
      Message = ForcePerLength.GetAbbreviation(_stiffnessUnit) + ", "
        + RotationalStiffness.GetAbbreviation(_rotationalStiffnessUnit) + ", "
        + Length.GetAbbreviation(_lengthUnit);
    }
  }
}
