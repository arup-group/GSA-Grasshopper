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
  public class GetSectionModifier : GH_OasysComponent, IGH_VariableParameterComponent {
    public override Guid ComponentGuid => new Guid("495dab75-6404-43b5-9f20-4a8caaaf41ab");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GetSectionModifier;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;
    private LinearDensityUnit _linearDensityUnit = DefaultUnits.LinearDensityUnit;

    public GetSectionModifier() : base("Get Section Modifier", "GetPBM",
      "Get GSA Section Modifier", CategoryName.Name(), SubCategoryName.Cat1()) {
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

      var unitsMenu = new ToolStripMenuItem("Select Units", Resources.ModelUnits);
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
        + " to get information for."
        + GsaSectionModifierGoo.Name, GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaSectionModifierParameter(), GsaSectionModifierGoo.Name,
        GsaSectionModifierGoo.NickName,
        GsaSectionModifierGoo.Description, GH_ParamAccess.item);

      pManager.AddGenericParameter("Area Modifier", "A", "Effective Area", GH_ParamAccess.item);

      pManager.AddGenericParameter("I11 Modifier", "I11", "Effective Iyy/Iuu", GH_ParamAccess.item);

      pManager.AddGenericParameter("I22 Modifier", "I22", "Effective Izz/Ivv", GH_ParamAccess.item);

      pManager.AddGenericParameter("J Modifier", "J", "Effective J", GH_ParamAccess.item);

      pManager.AddGenericParameter("K11 Modifier", "K11", "Effective Kyy/Kuu", GH_ParamAccess.item);

      pManager.AddGenericParameter("K22 Modifier", "K22", "Effective Kzz/Kvv", GH_ParamAccess.item);

      pManager.AddGenericParameter("Volume Modifier", "V", "Effective Volume/Length", GH_ParamAccess.item);

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
        modifier = modifierGoo.Value;
      }

      da.SetData(0, new GsaSectionModifierGoo(modifier));
      da.SetData(1,
        modifier.ApiSectionModifier.AreaModifier.Option == SectionModifierOptionType.BY ?
          new GH_UnitNumber(modifier.AreaModifier) :
          new GH_UnitNumber(modifier.AreaModifier.ToUnit(UnitsHelper.GetAreaUnit(_lengthUnit))));

      da.SetData(2,
        modifier.ApiSectionModifier.I11Modifier.Option == SectionModifierOptionType.BY ?
          new GH_UnitNumber(modifier.I11Modifier) : new GH_UnitNumber(
            modifier.I11Modifier.ToUnit(UnitsHelper.GetAreaMomentOfInertiaUnit(_lengthUnit))));

      da.SetData(3,
        modifier.ApiSectionModifier.I22Modifier.Option == SectionModifierOptionType.BY ?
          new GH_UnitNumber(modifier.I22Modifier) : new GH_UnitNumber(
            modifier.I22Modifier.ToUnit(UnitsHelper.GetAreaMomentOfInertiaUnit(_lengthUnit))));

      da.SetData(4,
        modifier.ApiSectionModifier.JModifier.Option == SectionModifierOptionType.BY ?
          new GH_UnitNumber(modifier.JModifier) : new GH_UnitNumber(
            modifier.JModifier.ToUnit(UnitsHelper.GetAreaMomentOfInertiaUnit(_lengthUnit))));

      da.SetData(5, new GH_UnitNumber(modifier.K11Modifier));
      da.SetData(6, new GH_UnitNumber(modifier.K22Modifier));

      da.SetData(7,
        modifier.ApiSectionModifier.VolumeModifier.Option == SectionModifierOptionType.BY ?
          new GH_UnitNumber(modifier.VolumeModifier) : new GH_UnitNumber(
            modifier.VolumeModifier.ToUnit(UnitsHelper.GetVolumePerLengthUnit(_lengthUnit))));

      da.SetData(8, new GH_UnitNumber(modifier.AdditionalMass.ToUnit(_linearDensityUnit)));

      da.SetData(9, modifier.IsBendingAxesPrincipal);
      da.SetData(10, modifier.IsReferencePointCentroid);
      da.SetData(11, (int)modifier.StressOption);
    }

    internal void UpdateDensity(string unit) {
      _linearDensityUnit = (LinearDensityUnit)UnitsHelper.Parse(typeof(LinearDensityUnit), unit);
      Update();
    }

    internal void UpdateLength(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      Update();
    }

    private void Update() {
      UpdateMessage();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }

    private void UpdateMessage() {
      Message = Length.GetAbbreviation(_lengthUnit) + ", "
        + LinearDensity.GetAbbreviation(_linearDensityUnit);
    }
  }
}
