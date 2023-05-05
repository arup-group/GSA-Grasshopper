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
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get geometric properties of a section
  /// </summary>
  public class GetSectionProperties : GH_OasysComponent, IGH_VariableParameterComponent {
    public override Guid ComponentGuid => new Guid("f7463c48b-d901-4820-b43f-3d938f614206");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.SectionProperties;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;

    public GetSectionProperties() : base("Section Properties", "SectProp",
      "Get GSA Section Properties", CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);

      var unitsMenu = new ToolStripMenuItem("Select unit", Resources.Units) {
        Enabled = true,
        ImageScaling = ToolStripItemImageScaling.SizeToFit,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => Update(unit)) {
          Checked = unit == Length.GetAbbreviation(_lengthUnit),
          Enabled = true,
        };
        unitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

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
      return base.Read(reader);
    }

    public virtual void VariableParameterMaintenance() {
      AreaUnit areaUnit = UnitsHelper.GetAreaUnit(_lengthUnit);
      VolumeUnit volumeUnit = UnitsHelper.GetVolumeUnit(_lengthUnit);
      AreaMomentOfInertiaUnit inertiaUnit = UnitsHelper.GetAreaMomentOfInertiaUnit(_lengthUnit);

      Params.Output[0].Name = "Area [" + Area.GetAbbreviation(areaUnit) + "]";
      Params.Output[1].Name = "Moment of Inertia y-y ["
        + AreaMomentOfInertia.GetAbbreviation(inertiaUnit) + "]";
      Params.Output[2].Name = "Moment of Inertia z-z ["
        + AreaMomentOfInertia.GetAbbreviation(inertiaUnit) + "]";
      Params.Output[3].Name = "Moment of Inertia y-z ["
        + AreaMomentOfInertia.GetAbbreviation(inertiaUnit) + "]";
      Params.Output[4].Name = "Moment of Inertia u-u ["
        + AreaMomentOfInertia.GetAbbreviation(inertiaUnit) + "]";
      Params.Output[5].Name = "Moment of Inertia v-v ["
        + AreaMomentOfInertia.GetAbbreviation(inertiaUnit) + "]";
      Params.Output[9].Name = "Torsion constant [" + AreaMomentOfInertia.GetAbbreviation(inertiaUnit) +
        "]";
      Params.Output[10].Name = "Section Modulus in y [" + Volume.GetAbbreviation(volumeUnit) + "]";
      Params.Output[11].Name = "Section Modulus in z [" + Volume.GetAbbreviation(volumeUnit) + "]";
      Params.Output[12].Name = "Plastic Modulus in y [" + Volume.GetAbbreviation(volumeUnit) + "]";
      Params.Output[13].Name = "Plastic Modulus in z [" + Volume.GetAbbreviation(volumeUnit) + "]";
      Params.Output[14].Name = "Radius of Gyration in y [" + Length.GetAbbreviation(_lengthUnit) + "]";
      Params.Output[15].Name = "Radius of Gyration in z [" + Length.GetAbbreviation(_lengthUnit) + "]";
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetString("LengthUnit", _lengthUnit.ToString());
      return base.Write(writer);
    }

    protected override void BeforeSolveInstance() {
      Message = Length.GetAbbreviation(_lengthUnit);
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaSectionParameter(), GsaSectionGoo.Name, GsaSectionGoo.NickName,
        GsaSectionGoo.Description + " to get a bit more info out of.", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      AreaUnit areaUnit = UnitsHelper.GetAreaUnit(_lengthUnit);
      VolumeUnit volumeUnit = UnitsHelper.GetVolumeUnit(_lengthUnit);
      AreaMomentOfInertiaUnit inertiaUnit = UnitsHelper.GetAreaMomentOfInertiaUnit(_lengthUnit);

      pManager.AddGenericParameter("Area [" + Area.GetAbbreviation(areaUnit) + "]", "A",
        "Section Area", GH_ParamAccess.item);
      pManager.AddGenericParameter(
        "Moment of Inertia y-y [" + AreaMomentOfInertia.GetAbbreviation(inertiaUnit) + "]", "Iyy",
        "Section Moment of Intertia around local y-y axis", GH_ParamAccess.item);
      pManager.AddGenericParameter(
        "Moment of Inertia z-z [" + AreaMomentOfInertia.GetAbbreviation(inertiaUnit) + "]", "Izz",
        "Section Moment of Intertia around local z-z axis", GH_ParamAccess.item);
      pManager.AddGenericParameter(
        "Moment of Inertia y-z [" + AreaMomentOfInertia.GetAbbreviation(inertiaUnit) + "]", "Iyz",
        "Section Moment of Intertia around local y-z axis", GH_ParamAccess.item);
      pManager.AddGenericParameter(
        "Moment of Inertia u-u [" + AreaMomentOfInertia.GetAbbreviation(inertiaUnit) + "]", "Iuu",
        "Section Moment of Intertia around principal u-u axis", GH_ParamAccess.item);
      pManager.AddGenericParameter(
        "Moment of Inertia v-v [" + AreaMomentOfInertia.GetAbbreviation(inertiaUnit) + "]", "Ivv",
        "Section Moment of Intertia around principal v-v axis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Angle [°]", "A", "Angle between local and principal axis",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Shear Area Factor in y", "Ky",
        "Section Shear Area Factor in local y-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Shear Area Factor in z", "Kz",
        "Section Shear Area Factor in local z-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter(
        "Torsion constant [" + AreaMomentOfInertia.GetAbbreviation(inertiaUnit) + "]", "J",
        "Section Torsion constant J", GH_ParamAccess.item);
      pManager.AddGenericParameter("Section Modulus in y [" + Volume.GetAbbreviation(volumeUnit) +
        "]", "Zy", "Section Modulus in y-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Section Modulus in z [" + Volume.GetAbbreviation(volumeUnit) +
        "]", "Zz", "Section Modulus in z-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Plastic Modulus in y [" + Volume.GetAbbreviation(volumeUnit) +
        "]", "Zpy", "Plastic Section Modulus in y-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Plastic Modulus in z [" + Volume.GetAbbreviation(volumeUnit) +
        "]", "Zpz", "Plastic Section Modulus in z-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Radius of Gyration in y [" + Length.GetAbbreviation(_lengthUnit)
        + "]", "Ry", "Radius of Gyration in y-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Radius of Gyration in z [" + Length.GetAbbreviation(_lengthUnit) +
        "]", "Rz", "Radius of Gyration in z-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Surface A/Length [m²/m]", "S/L",
        "Section Surface Area per Unit Length", GH_ParamAccess.item);
      pManager.AddGenericParameter("Volume/Length [m³/m]", "V/L", "Section Volume per Unit Length",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp)) {
        return;
      }

      GsaSection section;
      if (ghTyp.Value is GsaSectionGoo sectionGoo) {
        section = sectionGoo.Value;
      } else {
        string profile = string.Empty;
        ghTyp.CastTo(ref profile);
        if (GsaSection.ValidProfile(profile)) {
          section = new GsaSection(profile);
        } else {
          this.AddRuntimeWarning("Invalid profile syntax: " + profile);
          return;
        }
      }

      AreaUnit areaUnit = UnitsHelper.GetAreaUnit(_lengthUnit);
      VolumeUnit volumeUnit = UnitsHelper.GetVolumeUnit(_lengthUnit);
      AreaMomentOfInertiaUnit inertiaUnit = UnitsHelper.GetAreaMomentOfInertiaUnit(_lengthUnit);

      da.SetData(0, new GH_UnitNumber(new Area(section.Area.As(areaUnit), areaUnit)));

      da.SetData(1, new GH_UnitNumber(section.Iyy.ToUnit(inertiaUnit)));
      da.SetData(2, new GH_UnitNumber(section.Izz.ToUnit(inertiaUnit)));
      da.SetData(3, new GH_UnitNumber(section.Iyz.ToUnit(inertiaUnit)));

      da.SetData(4, new GH_UnitNumber(section.Iuu.ToUnit(inertiaUnit)));
      da.SetData(5, new GH_UnitNumber(section.Ivv.ToUnit(inertiaUnit)));
      da.SetData(6, new GH_UnitNumber(new Angle(section.Angle, AngleUnit.Degree)));

      da.SetData(7, section.Ky);
      da.SetData(8, section.Kz);

      da.SetData(9, new GH_UnitNumber(section.J.ToUnit(inertiaUnit)));

      da.SetData(10, new GH_UnitNumber(section.Zy.ToUnit(volumeUnit)));
      da.SetData(11, new GH_UnitNumber(section.Zz.ToUnit(volumeUnit)));

      da.SetData(12, new GH_UnitNumber(section.Zpy.ToUnit(volumeUnit)));
      da.SetData(13, new GH_UnitNumber(section.Zpz.ToUnit(volumeUnit)));

      da.SetData(14, new GH_UnitNumber(section.Ry.ToUnit(_lengthUnit)));
      da.SetData(15, new GH_UnitNumber(section.Rz.ToUnit(_lengthUnit)));

      da.SetData(16, new GH_UnitNumber(section.SurfaceAreaPerLength));
      da.SetData(17, new GH_UnitNumber(section.VolumePerLength));
    }

    private void Update(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      Message = unit;
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }
  }
}
