using System;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units.Helpers;
using OasysGH.Units;
using OasysUnits.Units;
using OasysUnits;
using OasysGH.Helpers;
using System.Collections.Generic;
using System.Linq;
using GsaGH.Helpers.GH;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to get geometric properties of a section
    /// </summary>
    public class GetSectionProperties : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("fc59d2f7-496e-4862-8f66-31f1068fcab7");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.SectionProperties;

    public GetSectionProperties() : base("Section Properties",
      "SectProp",
      "Get GSA Section Properties",
      CategoryName.Name(),
      SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaSectionParameter(), GsaSectionGoo.Name, GsaSectionGoo.NickName, GsaSectionGoo.Description + " to get a bit more info out of.", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      AreaUnit areaUnit = UnitsHelper.GetAreaUnit(this.LengthUnit);
      AreaMomentOfInertiaUnit inertiaUnit = UnitsHelper.GetAreaMomentOfInertiaUnit(this.LengthUnit);

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

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaSection gsaSection = new GsaSection();
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        if (gh_typ.Value is GsaSectionGoo)
          gh_typ.CastTo(ref gsaSection);
        else
        {
          string profile = "";
          gh_typ.CastTo(ref profile);
          if (GsaSection.ValidProfile(profile))
            gsaSection = new GsaSection(profile);
          else
          {
            this.AddRuntimeWarning("Invalid profile syntax: " + profile);
            return;
          }
        }

        AreaUnit areaUnit = UnitsHelper.GetAreaUnit(this.LengthUnit);
        AreaMomentOfInertiaUnit inertiaUnit = UnitsHelper.GetAreaMomentOfInertiaUnit(this.LengthUnit);

        DA.SetData(0, new GH_UnitNumber(new Area(gsaSection.Area.As(areaUnit), areaUnit)));
        DA.SetData(1, new GH_UnitNumber(new AreaMomentOfInertia(gsaSection.Iyy.As(inertiaUnit), inertiaUnit)));
        DA.SetData(2, new GH_UnitNumber(new AreaMomentOfInertia(gsaSection.Izz.As(inertiaUnit), inertiaUnit)));
        DA.SetData(3, new GH_UnitNumber(new AreaMomentOfInertia(gsaSection.Iyz.As(inertiaUnit), inertiaUnit)));
        DA.SetData(4, new GH_UnitNumber(new AreaMomentOfInertia(gsaSection.J.As(inertiaUnit), inertiaUnit)));
        DA.SetData(5, gsaSection.Ky);
        DA.SetData(6, gsaSection.Kz);
        DA.SetData(7, new GH_UnitNumber(gsaSection.SurfaceAreaPerLength));
        DA.SetData(8, new GH_UnitNumber(gsaSection.VolumePerLength));
      }
    }

    #region Custom UI
    protected override void BeforeSolveInstance()
    {
      this.Message = Length.GetAbbreviation(this.LengthUnit);
    }

    LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;
    public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
    {
      Menu_AppendSeparator(menu);

      ToolStripMenuItem unitsMenu = new ToolStripMenuItem("Select unit", Properties.Resources.Units);
      unitsMenu.Enabled = true;
      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length))
      {
        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { Update(unit); });
        toolStripMenuItem.Checked = unit == Length.GetAbbreviation(this.LengthUnit);
        toolStripMenuItem.Enabled = true;
        unitsMenu.DropDownItems.Add(toolStripMenuItem);
      }
      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }
    private void Update(string unit)
    {
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      this.Message = unit;
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetString("LengthUnit", this.LengthUnit.ToString());
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("LengthUnit"));
      return base.Read(reader);
    }

    #region IGH_VariableParameterComponent null implementation
    public virtual void VariableParameterMaintenance()
    {
      AreaUnit areaUnit = UnitsHelper.GetAreaUnit(this.LengthUnit);
      AreaMomentOfInertiaUnit inertiaUnit = UnitsHelper.GetAreaMomentOfInertiaUnit(this.LengthUnit);

      this.Params.Output[0].Name = "Area [" + Area.GetAbbreviation(areaUnit) + "]";
      this.Params.Output[1].Name = "Moment of Inertia y-y [" + AreaMomentOfInertia.GetAbbreviation(inertiaUnit) + "]";
      this.Params.Output[2].Name = "Moment of Inertia z-z [" + AreaMomentOfInertia.GetAbbreviation(inertiaUnit) + "]";
      this.Params.Output[3].Name = "Moment of Inertia y-z [" + AreaMomentOfInertia.GetAbbreviation(inertiaUnit) + "]";
      this.Params.Output[4].Name = "Torsion constant [" + AreaMomentOfInertia.GetAbbreviation(inertiaUnit) + "]";
    }

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index) => false;

    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index) => false;

    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index) => null;

    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) => false;
    #endregion
    #endregion
  }
}

