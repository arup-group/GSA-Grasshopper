using System;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
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
  // ReSharper disable once InconsistentNaming
  public class GetSectionProperties_OBSOLETE : GH_OasysComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("6504a99f-a4e2-4e30-8251-de31ea83e8cb");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.SectionProperties;

    public GetSectionProperties_OBSOLETE() : base("Section Properties",
      "SectProp",
      "Get GSA Section Properties",
      CategoryName.Name(),
      SubCategoryName.Cat1()) {
        Hidden = true;
    } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaSectionParameter(), GsaSectionGoo.Name, GsaSectionGoo.NickName, GsaSectionGoo.Description + " to get a bit more info out of.", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      AreaUnit areaUnit = UnitsHelper.GetAreaUnit(_lengthUnit);
      AreaMomentOfInertiaUnit inertiaUnit = UnitsHelper.GetAreaMomentOfInertiaUnit(_lengthUnit);

      pManager.AddGenericParameter("Area [" + Area.GetAbbreviation(areaUnit) + "]", "A", "Section Area", GH_ParamAccess.item);
      pManager.AddGenericParameter("Moment of Inertia y-y [" + AreaMomentOfInertia.GetAbbreviation(inertiaUnit) + "]", "Iyy", "Section Moment of Intertia around local y-y axis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Moment of Inertia z-z [" + AreaMomentOfInertia.GetAbbreviation(inertiaUnit) + "]", "Izz", "Section Moment of Intertia around local z-z axis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Moment of Inertia y-z [" + AreaMomentOfInertia.GetAbbreviation(inertiaUnit) + "]", "Iyz", "Section Moment of Intertia around local y-z axis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Torsion constant [" + AreaMomentOfInertia.GetAbbreviation(inertiaUnit) + "]", "J", "Section Torsion constant J", GH_ParamAccess.item);
      pManager.AddGenericParameter("Shear Area Factor in y", "Ky", "Section Shear Area Factor in local y-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Shear Area Factor in z", "Kz", "Section Shear Area Factor in local z-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Surface A/Length [m²/m]", "S/L", "Section Surface Area per Unit Length", GH_ParamAccess.item);
      pManager.AddGenericParameter("Volume/Length [m³/m]", "V/L", "Section Volume per Unit Length", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaSection = new GsaSection();
      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp)) {
        return;
      }

      if (ghTyp.Value is GsaSectionGoo)
        ghTyp.CastTo(ref gsaSection);
      else {
        string profile = "";
        ghTyp.CastTo(ref profile);
        gsaSection = new GsaSection(profile);
      }

      AreaUnit areaUnit = UnitsHelper.GetAreaUnit(_lengthUnit);
      AreaMomentOfInertiaUnit inertiaUnit = UnitsHelper.GetAreaMomentOfInertiaUnit(_lengthUnit);

      da.SetData(0, new GH_UnitNumber(new Area(gsaSection.Area.As(areaUnit), areaUnit)));
      da.SetData(1, new GH_UnitNumber(new AreaMomentOfInertia(gsaSection.Iyy.As(inertiaUnit), inertiaUnit)));
      da.SetData(2, new GH_UnitNumber(new AreaMomentOfInertia(gsaSection.Izz.As(inertiaUnit), inertiaUnit)));
      da.SetData(3, new GH_UnitNumber(new AreaMomentOfInertia(gsaSection.Iyz.As(inertiaUnit), inertiaUnit)));
      da.SetData(4, new GH_UnitNumber(new AreaMomentOfInertia(gsaSection.J.As(inertiaUnit), inertiaUnit)));
      da.SetData(5, gsaSection.Ky);
      da.SetData(6, gsaSection.Kz);
      da.SetData(7, new GH_UnitNumber(gsaSection.SurfaceAreaPerLength));
      da.SetData(8, new GH_UnitNumber(gsaSection.VolumePerLength));
    }

    #region Custom UI
    protected override void BeforeSolveInstance() {
      Message = Length.GetAbbreviation(_lengthUnit);
    }

    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;
    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      Menu_AppendSeparator(menu);

      var unitsMenu = new ToolStripMenuItem("Select unit", Properties.Resources.Units) {
        Enabled = true,
        ImageScaling = ToolStripItemImageScaling.SizeToFit,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { Update(unit); }) {
          Checked = unit == Length.GetAbbreviation(_lengthUnit),
          Enabled = true,
        };
        unitsMenu.DropDownItems.Add(toolStripMenuItem);
      }
      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }
    private void Update(string unit) {
      _lengthUnit = Length.ParseUnit(unit);
      Message = unit;
      ExpireSolution(true);
    }
    public override bool Write(GH_IO.Serialization.GH_IWriter writer) {
      writer.SetString("LengthUnit", _lengthUnit.ToString());
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader) {
      if (reader.ItemExists("LengthUnit")) // = v0.9.33 => saved as IGH_Variableblabla
      {
        _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("LengthUnit"));
        bool flag = base.Read(reader);
        return flag & Params.ReadAllParameterData(reader);
      }

      _lengthUnit = DefaultUnits.LengthUnitSection;
      return base.Read(reader);
    }
    #endregion
  }
}

