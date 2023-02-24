using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components
{
  public class CreateGridPointLoad : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("076f03c6-67ba-49d3-9462-cd4a4b5aff92");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.PointLoad;

    public CreateGridPointLoad() : base("Create Grid Point Load",
      "PointLoad",
      "Create GSA Grid Point Load",
      CategoryName.Name(),
      SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Force.GetAbbreviation(this.ForceUnit);

      pManager.AddIntegerParameter("Load case", "LC", "Load case number (default 1)", GH_ParamAccess.item, 1);
      pManager.AddPointParameter("Point", "Pt", "Point. If you input grid plane below only x and y coordinates will be used from this point, but if not a new Grid Plane Surface (xy-plane) will be created at the z-elevation of this point.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Grid Plane Surface", "GPS", "Grid Plane Surface or Plane (optional). If no input here then the point's z-coordinate will be used for an xy-plane at that elevation.", GH_ParamAccess.item);
      pManager.AddTextParameter("Direction", "Di", "Load direction (default z)." +
              Environment.NewLine + "Accepted inputs are:" +
              Environment.NewLine + "x" +
              Environment.NewLine + "y" +
              Environment.NewLine + "z", GH_ParamAccess.item, "z");
      pManager.AddIntegerParameter("Axis", "Ax", "Load axis (default Global). " +
              Environment.NewLine + "Accepted inputs are:" +
              Environment.NewLine + "0 : Global" +
              Environment.NewLine + "-1 : Local", GH_ParamAccess.item, 0);
      pManager.AddTextParameter("Name", "Na", "Load Name", GH_ParamAccess.item);
      pManager.AddGenericParameter("Value [" + unitAbbreviation + "]", "V", "Load Value", GH_ParamAccess.item);

      pManager[0].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
      pManager[5].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaLoadParameter(), "Grid Point Load", "Ld", "GSA Grid Point Load", GH_ParamAccess.item);
    }
    #endregion
    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaGridPointLoad gridpointload = new GsaGridPointLoad();

      // 0 Load case
      int lc = 1;
      GH_Integer gh_lc = new GH_Integer();
      if (DA.GetData(0, ref gh_lc))
        GH_Convert.ToInt32(gh_lc, out lc, GH_Conversion.Both);
      gridpointload.GridPointLoad.Case = lc;

      // 1 Point
      Point3d pt = new Point3d();
      GH_Point gh_pt = new GH_Point();
      if (DA.GetData(1, ref gh_pt))
        GH_Convert.ToPoint3d(gh_pt, ref pt, GH_Conversion.Both);
      gridpointload.GridPointLoad.X = pt.X;
      gridpointload.GridPointLoad.Y = pt.Y;

      // 2 Plane
      GsaGridPlaneSurface grdplnsrf;
      Plane pln = Plane.WorldXY;
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(2, ref gh_typ))
      {
        if (gh_typ.Value is GsaGridPlaneSurfaceGoo)
        {
          GsaGridPlaneSurface temppln = new GsaGridPlaneSurface();
          gh_typ.CastTo(ref temppln);
          grdplnsrf = temppln.Duplicate();
          gridpointload.GridPlaneSurface = grdplnsrf;
        }
        else if (gh_typ.Value is Plane)
        {
          gh_typ.CastTo(ref pln);
          grdplnsrf = new GsaGridPlaneSurface(pln);
          gridpointload.GridPlaneSurface = grdplnsrf;
        }
        else
        {
          int id = 0;
          if (GH_Convert.ToInt32(gh_typ.Value, out id, GH_Conversion.Both))
          {
            gridpointload.GridPointLoad.GridSurface = id;
            gridpointload.GridPlaneSurface = null;
          }
          else
          {
            this.AddRuntimeError("Error in GPS input. Accepted inputs are Grid Plane Surface or Plane. " +
                Environment.NewLine + "If no input here then the point's z-coordinate will be used for an xy-plane at that elevation");
            return;
          }
        }
      }
      else
      {
        pln = Plane.WorldXY;
        pln.Origin = pt;
        grdplnsrf = new GsaGridPlaneSurface(pln);
        gridpointload.GridPlaneSurface = grdplnsrf;
      }

      // 3 direction
      string dir = "Z";
      Direction direc = Direction.Z;

      GH_String gh_dir = new GH_String();
      if (DA.GetData(3, ref gh_dir))
        GH_Convert.ToString(gh_dir, out dir, GH_Conversion.Both);
      dir = dir.ToUpper();
      if (dir == "X")
        direc = Direction.X;
      if (dir == "Y")
        direc = Direction.Y;

      gridpointload.GridPointLoad.Direction = direc;

      // 4 Axis
      int axis = 0;
      gridpointload.GridPointLoad.AxisProperty = 0;
      GH_Integer gh_ax = new GH_Integer();
      if (DA.GetData(4, ref gh_ax))
      {
        GH_Convert.ToInt32(gh_ax, out axis, GH_Conversion.Both);
        if (axis == 0 || axis == -1)
          gridpointload.GridPointLoad.AxisProperty = axis;
      }

      // 5 Name
      string name = "";
      GH_String gh_name = new GH_String();
      if (DA.GetData(5, ref gh_name))
      {
        if (GH_Convert.ToString(gh_name, out name, GH_Conversion.Both))
          gridpointload.GridPointLoad.Name = name;
      }

      // 6 load value
      gridpointload.GridPointLoad.Value = ((Force)Input.UnitNumber(this, DA, 6, this.ForceUnit)).Newtons;

      // convert to goo
      GsaLoad gsaLoad = new GsaLoad(gridpointload);
      DA.SetData(0, new GsaLoadGoo(gsaLoad));
    }

    #region Custom UI
    ForceUnit ForceUnit = DefaultUnits.ForceUnit;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Unit"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Force
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force));
      this.SelectedItems.Add(Force.GetAbbreviation(this.ForceUnit));

      this.IsInitialised = true;
    }
    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      this.ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), this.SelectedItems[i]);
      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), this.SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Force.GetAbbreviation(this.ForceUnit);
      Params.Input[6].Name = "Value [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
