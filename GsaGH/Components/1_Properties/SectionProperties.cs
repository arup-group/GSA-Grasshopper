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
  /// Component to get geometric properties of a section
  /// </summary>
  public class SectionProperties : GH_OasysComponent, IGH_VariableParameterComponent {
    public override Guid ComponentGuid => new Guid("8385abdc-d06f-45f1-86ce-412ceb592955");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.SectionProperties;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;

    public SectionProperties() : base("Section Properties", "PropPB",
      "Get GSA Section Properties", CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);

      var unitsMenu = new ToolStripMenuItem("Select unit", Resources.ModelUnits) {
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
      string lengthUnitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      string areaUnitAbbreviation = Area.GetAbbreviation(UnitsHelper.GetAreaUnit(_lengthUnit));
      string volumeUnitAbbreviation = Volume.GetAbbreviation(UnitsHelper.GetVolumeUnit(_lengthUnit));
      string inertiaUnitAbbreviation = AreaMomentOfInertia.GetAbbreviation(UnitsHelper.GetAreaMomentOfInertiaUnit(_lengthUnit));

      Params.Output[0].Name = "Area [" + areaUnitAbbreviation + "]";
      Params.Output[1].Name = "Moment of Inertia y-y [" + inertiaUnitAbbreviation + "]";
      Params.Output[2].Name = "Moment of Inertia z-z [" + inertiaUnitAbbreviation + "]";
      Params.Output[3].Name = "Moment of Inertia y-z [" + inertiaUnitAbbreviation + "]";
      Params.Output[4].Name = "Moment of Inertia u-u [" + inertiaUnitAbbreviation + "]";
      Params.Output[5].Name = "Moment of Inertia v-v [" + inertiaUnitAbbreviation + "]";
      Params.Output[6].Name = "Angle [°]";
      Params.Output[7].Name = "Shear Area Factor y-y";
      Params.Output[8].Name = "Shear Area Factor z-z";
      Params.Output[9].Name = "Shear Area Factor u-u";
      Params.Output[10].Name = "Shear Area Factor v-v";
      Params.Output[11].Name = "Torsion Constant J [" + inertiaUnitAbbreviation + "]";
      Params.Output[12].Name = "Torsion Constant C [" + volumeUnitAbbreviation + "]";
      Params.Output[13].Name = "Section Modulus in y [" + volumeUnitAbbreviation + "]";
      Params.Output[14].Name = "Section Modulus in z [" + volumeUnitAbbreviation + "]";
      Params.Output[15].Name = "Plastic Modulus in y [" + volumeUnitAbbreviation + "]";
      Params.Output[16].Name = "Plastic Modulus in z [" + volumeUnitAbbreviation + "]";
      Params.Output[17].Name = "Elastic Centroid in y [" + lengthUnitAbbreviation + "]";
      Params.Output[18].Name = "Elastic Centroid in z [" + lengthUnitAbbreviation + "]";
      Params.Output[19].Name = "Radius of Gyration in y [" + lengthUnitAbbreviation + "]";
      Params.Output[20].Name = "Radius of Gyration in z [" + lengthUnitAbbreviation + "]";
      Params.Output[21].Name = "Surface Area / Unit Length [" + areaUnitAbbreviation + "/" + lengthUnitAbbreviation + "]";
      Params.Output[22].Name = "Volume / Unit Length [" + volumeUnitAbbreviation + "/" + lengthUnitAbbreviation + "]";
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
      string lengthUnitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      string areaUnitAbbreviation = Area.GetAbbreviation(UnitsHelper.GetAreaUnit(_lengthUnit));
      string volumeUnitAbbreviation = Volume.GetAbbreviation(UnitsHelper.GetVolumeUnit(_lengthUnit));
      string inertiaUnitAbbreviation = AreaMomentOfInertia.GetAbbreviation(UnitsHelper.GetAreaMomentOfInertiaUnit(_lengthUnit));

      pManager.AddGenericParameter("Area [" + areaUnitAbbreviation + "]", "A",
        "Section Area", GH_ParamAccess.item);
      pManager.AddGenericParameter(
        "Moment of Inertia y-y [" + inertiaUnitAbbreviation + "]", "Iyy",
        "Section Moment of Intertia around local y-y axis", GH_ParamAccess.item);
      pManager.AddGenericParameter(
        "Moment of Inertia z-z [" + inertiaUnitAbbreviation + "]", "Izz",
        "Section Moment of Intertia around local z-z axis", GH_ParamAccess.item);
      pManager.AddGenericParameter(
        "Moment of Inertia y-z [" + inertiaUnitAbbreviation + "]", "Iyz",
        "Section Moment of Intertia around local y-z axis", GH_ParamAccess.item);
      pManager.AddGenericParameter(
        "Moment of Inertia u-u [" + inertiaUnitAbbreviation + "]", "Iuu",
        "Section Moment of Intertia around principal u-u axis", GH_ParamAccess.item);
      pManager.AddGenericParameter(
        "Moment of Inertia v-v [" + inertiaUnitAbbreviation + "]", "Ivv",
        "Section Moment of Intertia around principal v-v axis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Angle [°]", "A", "Angle between local and principal axis",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Shear Area Factor y-y", "Kyy",
        "Section Shear Area Factor around local y-y axis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Shear Area Factor z-z", "Kzz",
        "Section Shear Area Factor around local z-z axis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Shear Area Factor u-u", "Kuu",
        "Section Shear Area Factor around local u-u axis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Shear Area Factor v-v", "Kvv",
        "Section Shear Area Factor around local v-v axis", GH_ParamAccess.item);
      pManager.AddGenericParameter(
        "Torsion constant J [" + inertiaUnitAbbreviation + "]", "J",
        "Section Torsion constant J", GH_ParamAccess.item);
      pManager.AddGenericParameter(
        "Torsion constant C [" + volumeUnitAbbreviation + "]", "C",
        "Section Torsion constant C", GH_ParamAccess.item);
      pManager.AddGenericParameter("Section Modulus in y [" + volumeUnitAbbreviation +
        "]", "Zy", "Section Modulus in y-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Section Modulus in z [" + volumeUnitAbbreviation +
        "]", "Zz", "Section Modulus in z-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Plastic Modulus in y [" + volumeUnitAbbreviation +
        "]", "Zpy", "Plastic Section Modulus in y-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Plastic Modulus in z [" + volumeUnitAbbreviation +
        "]", "Zpz", "Plastic Section Modulus in z-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Elastic Centroid in y [" + lengthUnitAbbreviation
        + "]", "Cy", "Elastic Centroid in y-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Elastic Centroid in z [" + lengthUnitAbbreviation
        + "]", "Cz", "Elastic Centroid in z-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Radius of Gyration in y [" + lengthUnitAbbreviation
        + "]", "Ry", "Radius of Gyration in y-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Radius of Gyration in z [" + lengthUnitAbbreviation +
        "]", "Rz", "Radius of Gyration in z-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Surface Area/Length [" + areaUnitAbbreviation + "/" +
        lengthUnitAbbreviation + "]", "S/L", "Section Surface Area per Unit Length",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Volume/Length [" + volumeUnitAbbreviation + "/" +
        lengthUnitAbbreviation + "]", "V/L", "Section Volume per Unit Length",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaSectionGoo sectionGoo = null;
      da.GetData(0, ref sectionGoo);
      GsaSection section = sectionGoo.Value;

      AreaUnit areaUnit = UnitsHelper.GetAreaUnit(_lengthUnit);
      SectionModulusUnit sectionModulusUnit = UnitsHelper.GetSectionModulusUnit(_lengthUnit);
      AreaMomentOfInertiaUnit inertiaUnit = UnitsHelper.GetAreaMomentOfInertiaUnit(_lengthUnit);

      GsaSectionProperties props = section.SectionProperties;

      da.SetData(0, new GH_UnitNumber(props.Area.ToUnit(areaUnit)));

      da.SetData(1, new GH_UnitNumber(props.Iyy.ToUnit(inertiaUnit)));
      da.SetData(2, new GH_UnitNumber(props.Izz.ToUnit(inertiaUnit)));
      da.SetData(3, new GH_UnitNumber(props.Iyz.ToUnit(inertiaUnit)));

      da.SetData(4, new GH_UnitNumber(props.Iuu.ToUnit(inertiaUnit)));
      da.SetData(5, new GH_UnitNumber(props.Ivv.ToUnit(inertiaUnit)));
      da.SetData(6, new GH_UnitNumber(props.Angle.ToUnit(AngleUnit.Degree)));

      da.SetData(7, new GH_Number(props.Kyy));
      da.SetData(8, new GH_Number(props.Kzz));

      da.SetData(9, new GH_Number(props.Kuu));
      da.SetData(10, new GH_Number(props.Kvv));

      da.SetData(11, new GH_UnitNumber(props.J.ToUnit(inertiaUnit)));
      da.SetData(12, new GH_UnitNumber(props.C.ToUnit(sectionModulusUnit)));

      da.SetData(13, new GH_UnitNumber(props.Zy.ToUnit(sectionModulusUnit)));
      da.SetData(14, new GH_UnitNumber(props.Zz.ToUnit(sectionModulusUnit)));

      da.SetData(15, new GH_UnitNumber(props.Zpy.ToUnit(sectionModulusUnit)));
      da.SetData(16, new GH_UnitNumber(props.Zpz.ToUnit(sectionModulusUnit)));

      da.SetData(17, new GH_UnitNumber(props.Cy.ToUnit(_lengthUnit)));
      da.SetData(18, new GH_UnitNumber(props.Cz.ToUnit(_lengthUnit)));

      da.SetData(19, new GH_UnitNumber(props.Ry.ToUnit(_lengthUnit)));
      da.SetData(20, new GH_UnitNumber(props.Rz.ToUnit(_lengthUnit)));

      da.SetData(21, new GH_UnitNumber(props.SurfaceAreaPerLength));
      da.SetData(22, new GH_UnitNumber(props.VolumePerLength));
    }

    private void Update(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      Message = unit;
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }
  }
}
