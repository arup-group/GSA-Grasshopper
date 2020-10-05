using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using Rhino.Geometry;
using GhSA.Parameters;

namespace GhSA.Components
{
    public class CreateGridPointLoad : GH_Component
    {
        #region Name and Ribbon Layout
        public CreateGridPointLoad()
            : base("Create Grid Point Load", "PointLoad", "Create GSA Grid Point Load",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat3())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override Guid ComponentGuid => new Guid("844dbf7b-3750-445c-950d-b161b00a6757");
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        //protected override Bitmap Icon => Resources.CrossSections;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        #endregion

        #region input and output
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Load case", "LC", "Load case number (default 1)", GH_ParamAccess.item, 1);
            pManager.AddPointParameter("Point", "Pt", "Point. If you input grid plane below only x and y coordinates will be used from this point, but if not a new Grid Plane (xy-plane) will be created at the z-elevation of this point.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Grid Plane", "GP", "Grid Plane or plane (optional). If no input here then the point's z-coordinate will be used for an xy-plane at that elevation.", GH_ParamAccess.item);
            pManager.AddTextParameter("Elements", "El", "Element list (by default all)", GH_ParamAccess.item, "all");
            pManager.AddTextParameter("Direction", "Di", "Load direction (default z)." +
                    System.Environment.NewLine + "Accepted inputs are:" +
                    System.Environment.NewLine + "x" +
                    System.Environment.NewLine + "y" +
                    System.Environment.NewLine + "z", GH_ParamAccess.item, "z");
            pManager.AddIntegerParameter("Axis", "Ax", "Load axis (default Global). " +
                    System.Environment.NewLine + "Accepted inputs are:" +
                    System.Environment.NewLine + "0 : Global" +
                    System.Environment.NewLine + "-1 : Local", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Value (" + Util.GsaUnit.Force + ")", "V", "Load Value (" + Util.GsaUnit.Force + ")", GH_ParamAccess.item);

            pManager[0].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Grid Point Load", "Load", "GSA Grid Point Load", GH_ParamAccess.item);
        }
        #endregion
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaGridPointLoad gridpointload = new GsaGridPointLoad();
            GsaGridSurface gridsrf = new GsaGridSurface(); 

            // 0 Load case
            int lc = 1;
            GH_Integer gh_lc = new GH_Integer();
            if (DA.GetData(0, ref gh_lc))
                GH_Convert.ToInt32_Primary(gh_lc, ref lc);
            gridpointload.GridPointLoad.Case = lc;

            // 1 Point
            Point3d pt = new Point3d();
            GH_Point gh_pt = new GH_Point();
            if (DA.GetData(1, ref gh_pt))
                GH_Convert.ToPoint3d(gh_pt, ref pt, GH_Conversion.Both);
            gridpointload.GridPointLoad.X = pt.X;
            gridpointload.GridPointLoad.Y = pt.Y;

            // 2 Plane
            GsaGridPlane gridplane;
            Plane pln = Plane.Unset;
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(2, ref gh_typ))
            {
                if (gh_typ.Value is GsaGridPlane)
                {
                    GsaGridPlane temppln = new GsaGridPlane();
                    gh_typ.CastTo(ref temppln);
                    gridplane = temppln.Duplicate();
                }
                else
                {
                    gh_typ.CastTo(ref pln);
                    gridplane = new GsaGridPlane(pln);
                }
            }
            else
            {
                pln = Plane.WorldXY;
                pln.Origin = pt;
                gridplane = new GsaGridPlane(pln);
            }
            gridsrf.GridPlane = gridplane;

            // 3 element/beam list
            string beamList = "all"; 
            GH_String gh_bl = new GH_String();
            if (DA.GetData(3, ref gh_bl))
                GH_Convert.ToString_Primary(gh_bl, ref beamList);

            gridsrf.GridSurface.Elements = beamList;
            
            gridpointload.GridSurface = gridsrf;

            // 4 direction
            string dir = "Z";
            Direction direc = Direction.Z;

            GH_String gh_dir = new GH_String();
            if (DA.GetData(2, ref gh_dir))
                GH_Convert.ToString_Primary(gh_dir, ref dir);
            dir = dir.ToUpper();
            if (dir == "X")
                direc = Direction.X;
            if (dir == "Y")
                direc = Direction.Y;

            gridpointload.GridPointLoad.Direction = direc;

            // 5 Axis
            int axis = 0;
            gridpointload.GridPointLoad.AxisProperty = 0; 
            GH_Integer gh_ax = new GH_Integer();
            if (DA.GetData(5, ref gh_ax))
            {
                GH_Convert.ToInt32_Primary(gh_ax, ref axis);
                if (axis == 0 || axis == -1)
                    gridpointload.GridPointLoad.AxisProperty = axis;
            }

            // 6 load value
            double load = 0;
            if (DA.GetData(6, ref load))
                load = load * -1000; //convert to kN

            gridpointload.GridPointLoad.Value = load;
            GsaLoad gsaLoad = new GsaLoad(gridpointload);
            DA.SetData(0, new GsaLoadGoo(gsaLoad));
        }
    }
}
