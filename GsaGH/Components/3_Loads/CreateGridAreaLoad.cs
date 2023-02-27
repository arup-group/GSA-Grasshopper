using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.GUI;
using System.Windows.Forms;
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
    public class CreateGridAreaLoad : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("146f1bf8-8d2b-468f-bdb8-0237bee75262");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.AreaLoad;

    public CreateGridAreaLoad() : base("Create Grid Area Load",
      "AreaLoad",
      "Create GSA Grid Area Load",
      CategoryName.Name(),
      SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Pressure.GetAbbreviation(this.ForcePerAreaUnit);

      pManager.AddIntegerParameter("Load case", "LC", "Load case number (default 1)", GH_ParamAccess.item, 1);
      pManager.AddBrepParameter("Brep", "B", "(Optional) Brep. If no input the whole plane method will be used. If both Grid Plane Surface and Brep are inputted, this Brep will be projected onto the Grid Plane.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Grid Plane Surface", "GPS", "Grid Plane Surface or Plane (optional). If no input here then the brep's best-fit plane will be used", GH_ParamAccess.item);
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
      pManager.AddNumberParameter("Value [" + unitAbbreviation + "]", "V", "Load Value", GH_ParamAccess.item);

      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
      pManager[5].Optional = true;
      pManager[6].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaLoadParameter(), "Grid Area Load", "Ld", "GSA Grid Area Load", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaGridAreaLoad gridareaload = new GsaGridAreaLoad();

      // 0 Load case
      int lc = 1;
      GH_Integer gh_lc = new GH_Integer();
      if (DA.GetData(0, ref gh_lc))
        GH_Convert.ToInt32(gh_lc, out lc, GH_Conversion.Both);
      gridareaload.GridAreaLoad.Case = lc;

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
          this._expansionType = ExpansionType.Use_GPS_Settings;
          this.UpdateMessage();
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
            gridareaload.GridAreaLoad.GridSurface = id;
            gridareaload.GridPlaneSurface = null;
          }
          else
          {
            this.AddRuntimeError("Error in GPS input. Accepted inputs are Grid Plane Surface or Plane. " +
                Environment.NewLine + "If no input here then the brep's best-fit plane will be used");
            return;
          }
        }
      }

      // we wait setting the gridplanesurface until we have run the polyline input

      // 1 Polyline
      Brep brep = new Brep();


      GH_Brep gh_brep = new GH_Brep();
      if (DA.GetData(1, ref gh_brep))
      {
        GH_Convert.ToBrep(gh_brep, ref brep, GH_Conversion.Both);

        // get edge curves
        Curve[] edgeSegments = brep.DuplicateEdgeCurves();
        Curve[] edges = Curve.JoinCurves(edgeSegments);
        Curve crv = edges[0];

        //convert to polyline
        Polyline ln = new Polyline();
        if (crv.TryGetPolyline(out ln))
        {
          // get control points
          List<Point3d> ctrl_pts = ln.ToList();

          // plane
          if (!planeSet)
          {
            // create nice plane from pts
            pln = Helpers.GH.RhinoConversions.CreateBestFitUnitisedPlaneFromPts(ctrl_pts);

            if (this.Params.Input[2].SourceCount == 0 && this._expansionType == ExpansionType.Use_GPS_Settings)
            {
              this._expansionType = ExpansionType.To_1D;
              this.AddRuntimeRemark("Input Brep has automatically been converted to a GridPlaneSurface." + System.Environment.NewLine + "The default expansion type is set to be onto 1D Elements." + System.Environment.NewLine + "You can change this by right-clicking the component.");
              this.UpdateMessage();
            }

            // create grid plane surface from best fit plane
            grdplnsrf = new GsaGridPlaneSurface(pln, true);
          }

          // project original curve onto grid plane
          crv = Curve.ProjectToPlane(crv, pln);

          // convert to polyline again
          crv.TryGetPolyline(out ln);

          //get control points again
          ctrl_pts = ln.ToList();

          // string to write polyline description to
          string desc = "";

          // loop through all points
          for (int i = 0; i < ctrl_pts.Count - 1; i++)
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

          // set polyline in grid line load
          gridareaload.GridAreaLoad.Type = GridAreaPolyLineType.POLYGON;
          gridareaload.GridAreaLoad.PolyLineDefinition = desc;
        }
        else
          this.AddRuntimeError("Could not convert Brep edge to Polyline");
      }

      // now we can set the gridplanesurface:
      if (gridareaload.GridPlaneSurface != null)
      {
        switch (this._expansionType)
        {
          case ExpansionType.To_1D:
            grdplnsrf.GridSurface.ElementType = GridSurface.Element_Type.ONE_DIMENSIONAL;
            break;
          case ExpansionType.To_2D:
            grdplnsrf.GridSurface.ElementType = GridSurface.Element_Type.TWO_DIMENSIONAL;
            break;

          case ExpansionType.Use_GPS_Settings:
          default:
            break;
        }

        if (gridareaload.GridPlaneSurface.GridSurfaceId == 0)
          gridareaload.GridPlaneSurface = grdplnsrf;
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

      gridareaload.GridAreaLoad.Direction = direc;

      // 4 Axis
      int axis = 0;
      gridareaload.GridAreaLoad.AxisProperty = 0;
      GH_Integer gh_ax = new GH_Integer();
      if (DA.GetData(4, ref gh_ax))
      {
        GH_Convert.ToInt32(gh_ax, out axis, GH_Conversion.Both);
        if (axis == 0 || axis == -1)
          gridareaload.GridAreaLoad.AxisProperty = axis;
      }

      // 5 Projected
      bool proj = false;
      GH_Boolean gh_proj = new GH_Boolean();
      if (DA.GetData(5, ref gh_proj))
      {
        if (GH_Convert.ToBoolean(gh_proj, out proj, GH_Conversion.Both))
          gridareaload.GridAreaLoad.IsProjected = proj;
      }

      // 6 Name
      string name = "";
      GH_String gh_name = new GH_String();
      if (DA.GetData(6, ref gh_name))
      {
        if (GH_Convert.ToString(gh_name, out name, GH_Conversion.Both))
          gridareaload.GridAreaLoad.Name = name;
      }

      // 7 load value
      gridareaload.GridAreaLoad.Value = ((Pressure)Input.UnitNumber(this, DA, 7, ForcePerAreaUnit)).NewtonsPerSquareMeter;

      // convert to goo
      GsaLoad gsaLoad = new GsaLoad(gridareaload);
      DA.SetData(0, new GsaLoadGoo(gsaLoad));
    }

    #region Custom UI
    PressureUnit ForcePerAreaUnit = DefaultUnits.ForcePerAreaUnit;
    private enum ExpansionType
    {
      Use_GPS_Settings = 0,
      To_1D = 1,
      To_2D = 2
    }
    private ExpansionType _expansionType = ExpansionType.Use_GPS_Settings;

    protected override void BeforeSolveInstance()
    {
      base.BeforeSolveInstance();
      this.UpdateMessage();
    }

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Unit"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // ForcePerArea
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations((EngineeringUnits.ForcePerArea)));
      this.SelectedItems.Add(Pressure.GetAbbreviation(this.ForcePerAreaUnit));

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      this.ForcePerAreaUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), this.SelectedItems[i]);
      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.ForcePerAreaUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), this.SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Pressure.GetAbbreviation(this.ForcePerAreaUnit);
      Params.Input[7].Name = "Value [" + unitAbbreviation + "]";
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
    {
      bool noGPS = false;
      if (this.Params.Input[2].SourceCount == 0)
        noGPS = true;
      Menu_AppendItem(menu, "Use GridPlaneSurface", SetUseGPS, !noGPS, _expansionType == ExpansionType.Use_GPS_Settings);
      Menu_AppendItem(menu, "Expand to 1D Elements", SetUse1D, noGPS, _expansionType == ExpansionType.To_1D);
      Menu_AppendItem(menu, "Expand to 2D Elements", SetUse2D, noGPS, _expansionType == ExpansionType.To_2D);
    }
    private void UpdateMessage()
    {
      this.Message = "Expansion: " + this._expansionType.ToString().Replace("_", " ");
    }
    private void SetUseGPS(object s, EventArgs e)
    {
      this._expansionType = ExpansionType.Use_GPS_Settings;
      this.UpdateMessage();
      base.UpdateUI();
    }
    private void SetUse1D(object s, EventArgs e)
    {
      this._expansionType = ExpansionType.To_1D;
      this.UpdateMessage();
      base.UpdateUI();
    }
    private void SetUse2D(object s, EventArgs e)
    {
      this._expansionType = ExpansionType.To_2D;
      this.UpdateMessage();
      base.UpdateUI();
    }
    #endregion

    #region deserialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetString("Mode", this._expansionType.ToString());
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      if (reader.ItemExists("Mode"))
        this._expansionType = (ExpansionType)Enum.Parse(typeof(ExpansionType), reader.GetString("Mode"));
      return base.Read(reader);
    }
    #endregion
  }
}
