using System;
using System.Windows.Forms;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units.Helpers;
using OasysGH.Units;
using OasysUnits.Units;
using OasysUnits;
using Rhino.Geometry;
using OasysGH.Parameters;

namespace GsaGH.Components
{
  public class GridPlaneSurfaceProperties : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("cb5c1d72-e414-447b-b5db-ce18d76e2f4d");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.GridPlaneProperties;

    public GridPlaneSurfaceProperties() : base("Grid Plane Surface Properties",
      "GridPlaneSurfaceProp",
      "Get GSA Grid Plane Surface Properties",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat3())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaGridPlaneParameter(), "Grid Plane Surface", "GPS", "Grid Plane Surface to get a bit more info out of.", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddPlaneParameter("Grid Plane", "P", "Grid Plane (Axis + Elevation)", GH_ParamAccess.item); //0
      pManager.AddIntegerParameter("Grid Plane ID", "IdG", " Grid Plane ID", GH_ParamAccess.item); //1
      pManager.AddTextParameter("Grid Plane Name", "NaP", "Grid Plane Name", GH_ParamAccess.item); //2
      pManager.AddBooleanParameter("is Storey?", "St", "Grid Plane is Storey type", GH_ParamAccess.item); //3
      pManager.AddPlaneParameter("Axis", "Ax", "Grid Plane Axis as plane", GH_ParamAccess.item); //4
      pManager.AddIntegerParameter("Axis ID", "IdA", "Axis ID", GH_ParamAccess.item); //5
      pManager.AddGenericParameter("Elevation", "Ev", "Grid Plane Elevation", GH_ParamAccess.item); //6
      pManager.AddGenericParameter("Grid Plane Tolerance Above", "tA", "Grid Plane Tolerance Above (for Storey Type)", GH_ParamAccess.item); //7
      pManager.AddGenericParameter("Grid Plane Tolerance Below", "tB", "Grid Plane Tolerance Below (for Storey Type)", GH_ParamAccess.item); //8

      pManager.AddIntegerParameter("Grid Surface ID", "IdS", "Grid Surface ID", GH_ParamAccess.item); //9
      pManager.AddTextParameter("Grid Surface Name", "NaS", "Grid Surface Name", GH_ParamAccess.item); //10
      pManager.AddTextParameter("Elements", "El", "Elements that Grid Surface will try to expand load to", GH_ParamAccess.item); //11
      pManager.AddTextParameter("Element Type", "Ty", "Grid Surface Element Type", GH_ParamAccess.item); //12
      pManager.AddGenericParameter("Grid Surface Tolerance", "To", "Grid Surface Tolerance", GH_ParamAccess.item); //13
      pManager.AddTextParameter("Span Type", "Sp", "Grid Surface Span Type", GH_ParamAccess.item); //14
      pManager.AddNumberParameter("Span Direction", "Di", "Grid Surface Span Direction", GH_ParamAccess.item); //15
      pManager.AddTextParameter("Expansion Type", "Ex", "Grid Surface Expansion Type", GH_ParamAccess.item); //16
      pManager.AddBooleanParameter("Simplified Tributary Area", "Sf", "Grid Surface Simplified Tributary Area", GH_ParamAccess.item); //17
    }
    #endregion
    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaGridPlaneSurface gps = new GsaGridPlaneSurface();
      if (DA.GetData(0, ref gps))
      {
        if (gps == null)
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Null GridPlaneSurface");
          return;
        }

        DA.SetData(0, gps == null ? Plane.Unset : gps.Plane);
        DA.SetData(1, gps.GridPlane == null ? 0 : gps.GridPlaneID);
        DA.SetData(2, gps.GridPlane == null ? null : gps.GridPlane.Name);
        DA.SetData(3, gps.GridPlane == null ? false : gps.GridPlane.IsStoreyType);
        Plane axis = new Plane();
        if (gps.GridPlane != null)
        {
          axis = new Plane(new Point3d(gps.Axis.Origin.X, gps.Axis.Origin.Y, gps.Axis.Origin.Z),
          new Vector3d(gps.Axis.XVector.X, gps.Axis.XVector.Y, gps.Axis.XVector.Z),
          new Vector3d(gps.Axis.XYPlane.X, gps.Axis.XYPlane.Y, gps.Axis.XYPlane.Z)
          );
        }
        DA.SetData(4, gps.GridPlane == null ? Plane.Unset : axis);
        DA.SetData(5, gps.AxisID);
        DA.SetData(6, new GH_UnitNumber(new Length(gps.GridPlane == null ? 0 : gps.GridPlane.Elevation, this.LengthUnit)));
        DA.SetData(7, new GH_UnitNumber(new Length(gps.GridPlane == null ? 0 : gps.GridPlane.ToleranceAbove, this.LengthUnit)));
        DA.SetData(8, new GH_UnitNumber(new Length(gps.GridPlane == null ? 0 : gps.GridPlane.ToleranceBelow, this.LengthUnit)));

        DA.SetData(9, gps.GridSurfaceID);
        DA.SetData(10, gps.GridSurface.Name);
        DA.SetData(11, gps.GridSurface.Elements);
        string elemtype = gps.GridSurface.ElementType.ToString();
        DA.SetData(12, Char.ToUpper(elemtype[0]) + elemtype.Substring(1).ToLower().Replace("_", " "));
        DA.SetData(13, new GH_UnitNumber(new Length(gps.GridSurface.Tolerance, this.LengthUnit)));
        string spantype = gps.GridSurface.SpanType.ToString();
        DA.SetData(14, Char.ToUpper(spantype[0]) + spantype.Substring(1).ToLower().Replace("_", " "));
        DA.SetData(15, gps.GridSurface.Direction);
        string expantype = gps.GridSurface.ExpansionType.ToString();
        DA.SetData(16, Char.ToUpper(expantype[0]) + expantype.Substring(1).ToLower().Replace("_", " "));
        bool simple = false;
        if (gps.GridSurface.SpanType == GridSurface.Span_Type.TWO_WAY_SIMPLIFIED_TRIBUTARY_AREAS)
          simple = true;
        DA.SetData(17, simple);
      }
    }

    #region Custom UI
    protected override void BeforeSolveInstance()
    {
      this.Message = Length.GetAbbreviation(this.LengthUnit);
    }

    LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;
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
      this.LengthUnit = Length.ParseUnit(unit);
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
      try
      {
        this.LengthUnit = Length.ParseUnit(reader.GetString("LengthUnit"));
      }
      catch (Exception)
      {
        this.LengthUnit = LengthUnit.Meter;
      }
      return base.Read(reader);
    }

    #region IGH_VariableParameterComponent null implementation
    public virtual void VariableParameterMaintenance()
    {
      string unit = Length.GetAbbreviation(this.LengthUnit);

      this.Params.Output[6].Name = "Elevation [" + unit + "]";
      this.Params.Output[7].Name = "Grid Plane Tolerance Above [" + unit + "]";
      this.Params.Output[8].Name = "Grid Plane Tolerance Below [" + unit + "]";
      this.Params.Output[13].Name = "Grid Surface Tolerance [" + unit + "]";
    }

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index) => false;

    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index) => false;

    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index) => null;

    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) => false;
    #endregion
    #endregion
  }
}
