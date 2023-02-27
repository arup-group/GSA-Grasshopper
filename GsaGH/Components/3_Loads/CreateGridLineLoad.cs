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
    public class CreateGridLineLoad : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("e1f22e6f-8550-4078-8613-ea5ed2ede2b9");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.LineLoad;

    public CreateGridLineLoad() : base("Create Grid Line Load",
      "LineLoad",
      "Create GSA Grid Line Load",
      CategoryName.Name(),
      SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = ForcePerLength.GetAbbreviation(this.ForcePerLengthUnit);

      pManager.AddIntegerParameter("Load case", "LC", "Load case number (default 1)", GH_ParamAccess.item, 1);
      pManager.AddCurveParameter("PolyLine", "L", "PolyLine. If you input grid plane below only x and y coordinate positions will be used from this polyline, but if not a new Grid Plane Surface (best-fit plane) will be created from PolyLine control points.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Grid Plane Surface", "GPS", "Grid Plane Surface or Plane (optional). If no input here then the line's best-fit plane will be used", GH_ParamAccess.item);
      pManager.AddTextParameter("Direction", "Di", "Load direction (default z)." +
              Environment.NewLine + "Accepted inputs are:" +
              Environment.NewLine + "x" +
              Environment.NewLine + "y" +
              Environment.NewLine + "z", GH_ParamAccess.item, "z");
      pManager.AddIntegerParameter("Axis", "Ax", "Load axis (default Global). " +
              Environment.NewLine + "Accepted inputs are:" +
              Environment.NewLine + "0 : Global" +
              Environment.NewLine + "-1 : Local", GH_ParamAccess.item, 0);
      pManager.AddBooleanParameter("Projected", "Pj", "Projected (default not)", GH_ParamAccess.item, false);
      pManager.AddTextParameter("Name", "Na", "Load Name", GH_ParamAccess.item);
      pManager.AddGenericParameter("Value Start [" + unitAbbreviation + "]", "V1", "Load Value at Start of Line", GH_ParamAccess.item);
      pManager.AddGenericParameter("Value End [" + unitAbbreviation + "]", "V2", "Load Value at End of Line (default : Start Value)", GH_ParamAccess.item);

      pManager[0].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
      pManager[5].Optional = true;
      pManager[6].Optional = true;
      pManager[8].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaLoadParameter(), "Grid Line Load", "Ld", "GSA Grid Line Load", GH_ParamAccess.item);
    }
    #endregion
    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaGridLineLoad gridlineload = new GsaGridLineLoad();

      // 0 Load case
      int lc = 1;
      GH_Integer gh_lc = new GH_Integer();
      if (DA.GetData(0, ref gh_lc))
        GH_Convert.ToInt32(gh_lc, out lc, GH_Conversion.Both);
      gridlineload.GridLineLoad.Case = lc;

      // Do plane input first as to see if we need to project polyline onto grid plane
      // 2 Plane 
      Plane pln = Plane.WorldXY;
      bool planeSet = false;
      GsaGridPlaneSurface grdplnsrf = new GsaGridPlaneSurface();
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(2, ref gh_typ))
      {
        if (gh_typ.Value is GsaGridPlaneSurfaceGoo)
        {
          GsaGridPlaneSurface temppln = new GsaGridPlaneSurface();
          gh_typ.CastTo(ref temppln);
          grdplnsrf = temppln.Duplicate();
          pln = grdplnsrf.Plane;
          planeSet = true;
        }
        else if (gh_typ.Value is Plane)
        {
          gh_typ.CastTo(ref pln);
          grdplnsrf = new GsaGridPlaneSurface(pln);
          planeSet = true;
        }
        else
        {
          int id = 0;
          if (GH_Convert.ToInt32(gh_typ.Value, out id, GH_Conversion.Both))
          {
            gridlineload.GridLineLoad.GridSurface = id;
            gridlineload.GridPlaneSurface = null;
          }
          else
          {
            this.AddRuntimeError("Error in GPS input. Accepted inputs are Grid Plane Surface or Plane. " +
                Environment.NewLine + "If no input here then the line's best-fit plane will be used");
            return;
          }
        }
      }

      // we wait setting the gridplanesurface until we have run the polyline input

      // 1 Polyline
      Polyline ln = new Polyline();
      GH_Curve gh_crv = new GH_Curve();
      if (DA.GetData(1, ref gh_crv))
      {
        Curve crv = null;
        GH_Convert.ToCurve(gh_crv, ref crv, GH_Conversion.Both);

        //convert to polyline
        if (crv.TryGetPolyline(out ln))
        {
          // get control points
          List<Point3d> ctrl_pts = ln.ToList();

          // plane
          if (!planeSet)
          {
            // create best-fit plane from pts
            pln = Helpers.GH.RhinoConversions.CreateBestFitUnitisedPlaneFromPts(ctrl_pts);

            // create grid plane surface from best fit plane
            grdplnsrf = new GsaGridPlaneSurface(pln, true);
          }
          else
          {
            // project original curve onto grid plane
            crv = Curve.ProjectToPlane(crv, pln);

            // convert to polyline again
            crv.TryGetPolyline(out ln);

            //get control points again
            ctrl_pts = ln.ToList();
          }

          // string to write polyline description to
          string desc = "";

          // loop through all points
          for (int i = 0; i < ctrl_pts.Count; i++)
          {
            if (i > 0)
              desc += " ";

            // get control points in local plane coordinates
            Point3d temppt = new Point3d();
            pln.RemapToPlaneSpace(ctrl_pts[i], out temppt);

            // write point to string
            // format accepted by GSA: (0,0) (0,1) (1,2) (3,4) (4,0)(m)
            desc += "(" + temppt.X + "," + temppt.Y + ")";
          }
          // add units to the end
          desc += "(" + DefaultUnits.LengthUnitGeometry + ")";

          // set polyline in grid line load
          gridlineload.GridLineLoad.Type = GridLineLoad.PolyLineType.EXPLICIT_POLYLINE;
          gridlineload.GridLineLoad.PolyLineDefinition = desc;
        }
        else
          this.AddRuntimeError("Could not convert Curve to Polyline");
      }

      // now we can set the gridplanesurface:
      if (gridlineload.GridPlaneSurface != null)
      {
        if (gridlineload.GridPlaneSurface.GridSurfaceId == 0)
          gridlineload.GridPlaneSurface = grdplnsrf;
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

      gridlineload.GridLineLoad.Direction = direc;

      // 4 Axis
      int axis = 0;
      gridlineload.GridLineLoad.AxisProperty = 0;
      GH_Integer gh_ax = new GH_Integer();
      if (DA.GetData(4, ref gh_ax))
      {
        GH_Convert.ToInt32(gh_ax, out axis, GH_Conversion.Both);
        if (axis == 0 || axis == -1)
          gridlineload.GridLineLoad.AxisProperty = axis;
      }

      // 5 Projected
      bool proj = false;
      GH_Boolean gh_proj = new GH_Boolean();
      if (DA.GetData(5, ref gh_proj))
      {
        if (GH_Convert.ToBoolean(gh_proj, out proj, GH_Conversion.Both))
          gridlineload.GridLineLoad.IsProjected = proj;
      }

      // 6 Name
      string name = "";
      GH_String gh_name = new GH_String();
      if (DA.GetData(6, ref gh_name))
      {
        if (GH_Convert.ToString(gh_name, out name, GH_Conversion.Both))
          gridlineload.GridLineLoad.Name = name;
      }

      // 7 load value
      double load1 = ((ForcePerLength)Input.UnitNumber(this, DA, 7, this.ForcePerLengthUnit)).NewtonsPerMeter;
      gridlineload.GridLineLoad.ValueAtStart = load1;

      // 8 load value
      double load2 = load1;
      if (DA.GetData(8, ref load2))
        load2 = ((ForcePerLength)Input.UnitNumber(this, DA, 8, this.ForcePerLengthUnit, true)).NewtonsPerMeter;
      gridlineload.GridLineLoad.ValueAtEnd = load2;

      // convert to goo
      GsaLoad gsaLoad = new GsaLoad(gridlineload);
      DA.SetData(0, new GsaLoadGoo(gsaLoad));
    }

    #region Custom UI
    ForcePerLengthUnit ForcePerLengthUnit = DefaultUnits.ForcePerLengthUnit;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Unit"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // ForcePerLength
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations((EngineeringUnits.ForcePerLength)));
      this.SelectedItems.Add(ForcePerLength.GetAbbreviation(this.ForcePerLengthUnit));

      this.IsInitialised = true;
    }
    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      this.ForcePerLengthUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), this.SelectedItems[i]);
      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.ForcePerLengthUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), this.SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = ForcePerLength.GetAbbreviation(this.ForcePerLengthUnit);
      Params.Input[7].Name = "Value Start [" + unitAbbreviation + "]";
      Params.Input[8].Name = "Value End [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
