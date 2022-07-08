using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using Rhino.Geometry;
using GsaGH.Parameters;
using UnitsNet.Units;
using UnitsNet;

namespace GsaGH.Components
{
  public class CreateGridAreaLoad : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    public CreateGridAreaLoad()
        : base("Create Grid Area Load", "AreaLoad", "Create GSA Grid Area Load",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override Guid ComponentGuid => new Guid("146f1bf8-8d2b-468f-bdb8-0237bee75262");
    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.AreaLoad;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        dropdownitems.Add(Units.FilteredForcePerAreaUnits);

        selecteditems = new List<string>();
        PressureUnit pressureUnit = (Force.From(1, Units.ForceUnit) / (Length.From(1, Units.LengthUnitGeometry) * Length.From(1, Units.LengthUnitGeometry))).Unit;
        selecteditems.Add(pressureUnit.ToString());

        first = false;
      }

      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      forceAreaUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[0]);

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();

      // update input params
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      forceAreaUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[0]);

      CreateAttributes();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    #endregion

    #region input and output
    // list of lists with all dropdown lists conctent
    List<List<string>> dropdownitems;
    // list of selected items
    List<string> selecteditems;
    // list of descriptions 
    List<string> spacerDescriptions = new List<string>(new string[]
    {
            "Unit",
    });
    bool first = true;
    private PressureUnit forceAreaUnit = (Force.From(1, Units.ForceUnit) / (Length.From(1, Units.LengthUnitGeometry) * Length.From(1, Units.LengthUnitGeometry))).Unit;

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity force = new Pressure(0, forceAreaUnit);
      string unitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));

      pManager.AddIntegerParameter("Load case", "LC", "Load case number (default 1)", GH_ParamAccess.item, 1);
      pManager.AddBrepParameter("Brep", "B", "(Optional) Brep. If no input the whole plane method will be used. If both Grid Plane Surface and Brep are inputted, this Brep will be projected onto the Grid Plane.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Grid Plane Surface", "GPS", "Grid Plane Surface or Plane (optional). If no input here then the brep's best-fit plane will be used", GH_ParamAccess.item);
      pManager.AddTextParameter("Direction", "Di", "Load direction (default z)." +
              System.Environment.NewLine + "Accepted inputs are:" +
              System.Environment.NewLine + "x" +
              System.Environment.NewLine + "y" +
              System.Environment.NewLine + "z", GH_ParamAccess.item, "z");
      pManager.AddIntegerParameter("Axis", "Ax", "Load axis (default Global). " +
              System.Environment.NewLine + "Accepted inputs are:" +
              System.Environment.NewLine + "0 : Global" +
              System.Environment.NewLine + "-1 : Local", GH_ParamAccess.item, 0);
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
      pManager.AddGenericParameter("Grid Area Load", "Ld", "GSA Grid Area Load", GH_ParamAccess.item);
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
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in GPS input. Accepted inputs are Grid Plane Surface or Plane. " +
                System.Environment.NewLine + "If no input here then the brep's best-fit plane will be used");
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
            pln = Util.GH.Convert.CreateBestFitUnitisedPlaneFromPts(ctrl_pts);

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
          // add units to the end
          desc += "(" + Units.LengthUnitGeometry + ")";

          // set polyline in grid line load
          gridareaload.GridAreaLoad.Type = GridAreaPolyLineType.POLYGON;
          gridareaload.GridAreaLoad.PolyLineDefinition = desc;
        }
        else
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Could not convert Brep edge to Polyline");
      }

      // now we can set the gridplanesurface:
      if (gridareaload.GridPlaneSurface != null)
      {
        if (gridareaload.GridPlaneSurface.GridSurfaceID == 0)
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
      gridareaload.GridAreaLoad.Value = GetInput.Stress(this, DA, 7, forceAreaUnit).NewtonsPerSquareMeter;

      // convert to goo
      GsaLoad gsaLoad = new GsaLoad(gridareaload);
      DA.SetData(0, new GsaLoadGoo(gsaLoad));
    }
    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Util.GH.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      try // this will fail if user has an old version of the component
      {
        Util.GH.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);
      }
      catch (Exception) // we set the stored values like first initation of component
      {
        dropdownitems = new List<List<string>>();
        dropdownitems.Add(Units.FilteredStressUnits);

        selecteditems = new List<string>();
        selecteditems.Add(PressureUnit.KilonewtonPerSquareMeter.ToString());
      }
      first = false;

      UpdateUIFromSelectedItems();
      return base.Read(reader);
    }

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
    {
      return null;
    }
    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    #endregion
    #region IGH_VariableParameterComponent null implementation
    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
      IQuantity force = new Pressure(0, forceAreaUnit);
      string unitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));

      Params.Input[7].Name = "Value [" + unitAbbreviation + "]";
    }
    #endregion

  }
}
