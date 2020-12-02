using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using Rhino.Geometry;
using GhSA.Parameters;

namespace GhSA.Components
{
    public class CreateGridAreaLoad : GH_Component
    {
        #region Name and Ribbon Layout
        public CreateGridAreaLoad()
            : base("Create Grid Area Load", "AreaLoad", "Create GSA Grid Area Load",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat3())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override Guid ComponentGuid => new Guid("9d775938-69fc-441f-b766-9cd1d8259e5a");
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override System.Drawing.Bitmap Icon => GSA.Properties.Resources.GridAreaLoad;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        #endregion

        #region input and output
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
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
            pManager.AddNumberParameter("Value (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + "\xB2)", "V", "Load Value (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + "\xB2)", GH_ParamAccess.item);

            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Grid Area Load", "Load", "GSA Grid Area Load", GH_ParamAccess.item);
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
            Plane pln = Plane.Unset;
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
                }
                else if(gh_typ.Value is Plane)
                {
                    gh_typ.CastTo(ref pln);
                    grdplnsrf = new GsaGridPlaneSurface(pln);
                }
                else if (gh_typ.Value is Int32)
                {
                    int id = 0;
                    gh_typ.CastTo(ref id);
                    gridareaload.GridPlaneSurface.GridSurfaceID = id;
                }
                else
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in GPS input. Accepted inputs are Grid Plane Surface or Plane. " +
                        System.Environment.NewLine + "If no input here then the brep's best-fit plane will be used");
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
                    if (pln == Plane.Unset)
                    {
                        // calculate best fit plane:
                        Plane.FitPlaneToPoints(ctrl_pts, out pln);

                        // create grid plane surface from best fit plane
                        grdplnsrf = new GsaGridPlaneSurface(pln);
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
                    desc += "(" + Util.GsaUnit.LengthLarge + ")";

                    // set polyline in grid line load
                    gridareaload.GridAreaLoad.Type = GridAreaPolyLineType.POLYGON;
                    gridareaload.GridAreaLoad.PolyLineDefinition = desc;
                }
                else
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Could not convert Brep edge to Polyline");
            }

            // now we can set the gridplanesurface:
            if (gridareaload.GridPlaneSurface.GridSurfaceID == 0)
                gridareaload.GridPlaneSurface = grdplnsrf;


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

            // 6 load value
            double load1 = 0;
            if (DA.GetData(6, ref load1))
                load1 *= -1000; //convert to kN
            gridareaload.GridAreaLoad.Value = load1;
                        
            // convert to goo
            GsaLoad gsaLoad = new GsaLoad(gridareaload);
            DA.SetData(0, new GsaLoadGoo(gsaLoad));
        }
    }
}
