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
            pManager.AddPointParameter("Point", "P", "Point. If you input grid plane below only x and y coordinates will be used from this point, but if not a new Grid Plane (xy-plane) will be created at the z-elevation of this point.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Grid Plane", "P", "Grid Plane or plane (optional). If no input here then the point's z-coordinate will be used for an xy-plane at that elevation.", GH_ParamAccess.item);
            pManager.AddTextParameter("Elements", "El", "Element list (by default all)", GH_ParamAccess.item, "all");
            pManager.AddTextParameter("Direction", "Di", "Load direction (default z)." +
                    System.Environment.NewLine + "Accepted inputs are:" +
                    System.Environment.NewLine + "x" +
                    System.Environment.NewLine + "y" +
                    System.Environment.NewLine + "z", GH_ParamAccess.item, "z");
            pManager.AddNumberParameter("Value (" + Util.GsaUnit.Force + ")", "V", "Load Value (" + Util.GsaUnit.Force + ")", GH_ParamAccess.item);

            pManager[0].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Grid Point Load", "Load", "GSA Grid Point Load", GH_ParamAccess.item);
        }
        #endregion
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaGridPointLoad gridpointload = new GsaGridPointLoad();

            // 0 Load case
            int lc = 1;
            GH_Integer gh_lc = new GH_Integer();
            if (DA.GetData(0, ref gh_lc))
                GH_Convert.ToInt32_Primary(gh_lc, ref lc);
            gridpointload.GridPointLoad.Case = lc;

            
            // we want to get the plane in first in order to sort out what point coordinates to be using
            // 2 Plane
            GsaGridSurface gridsrf = new GsaGridSurface();
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(2, ref gh_typ))
            {
                if (gh_typ.Value is GsaGridSurface)
                    gh_typ.CastTo(ref gridsrf);
                else
                {
                    Plane pln = Plane.Unset;
                    gh_typ.CastTo(ref pln);
                    gsaSection = new GsaSection(profile);
                }
            }



            GH_Point gh_pt = new GH_Point();
            if (DA.GetData(1, ref gh_pt))
                GH_Convert.ToPoint3d(gh_pt, ref pt, GH_Conversion.Both);
            gridpointload.GridPointLoad.X = pt.X;
            gridpointload.GridPointLoad.Y = pt.Y;





            // 1 Point
            Point3d pt = new Point3d();
            GH_Point gh_pt = new GH_Point();
            if (DA.GetData(1, ref gh_pt))
                GH_Convert.ToPoint3d(gh_pt, ref pt, GH_Conversion.Both);
            gridpointload.GridPointLoad.X = pt.X;
            gridpointload.GridPointLoad.Y = pt.Y;



            // 3 element/beam list
            string beamList = "all"; 
            GH_String gh_bl = new GH_String();
            if (DA.GetData(3, ref gh_bl))
                GH_Convert.ToString_Primary(gh_bl, ref beamList);
            gridpointload.GridSurface.GridSurface.Elements = beamList;

            //factor
            Vector3 factor = new Vector3();
            Vector3d vect = new Vector3d(0, 0, -1);
            GH_Vector gh_factor = new GH_Vector();
            if (DA.GetData(2, ref gh_factor))
                GH_Convert.ToVector3d_Primary(gh_factor, ref vect);
            factor.X = vect.X; factor.Y = vect.Y; factor.Z = vect.Z;
            gravityLoad.GravityLoad.Factor = factor;

            DA.SetData(0, new GsaLoad(gravityLoad));
            
        }
    }
}
