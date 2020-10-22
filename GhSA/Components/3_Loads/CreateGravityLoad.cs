using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using Rhino.Geometry;
using GhSA.Parameters;

namespace GhSA.Components
{
    public class CreateGravityLoad : GH_Component
    {
        #region Name and Ribbon Layout
        public CreateGravityLoad()
            : base("Create Gravity Load", "GravityLoad", "Create GSA Gravity Load",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat3())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override Guid ComponentGuid => new Guid("f9099874-92fa-4608-b4ed-a788df85a407");
        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => GSA.Properties.Resources.GravityLoad;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        #endregion

        #region input and output
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Load case", "LC", "Load case number (default 1)", GH_ParamAccess.item, 1);
            pManager.AddTextParameter("Elements", "El", "Element list (by default all)", GH_ParamAccess.item, "all");
            pManager.AddVectorParameter("Gravity factor", "G", "Gravity vector factor (default z = -1)", GH_ParamAccess.item, new Vector3d(0, 0, -1));
            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Gravity load", "Load", "GSA Gravity Load", GH_ParamAccess.item);
        }
        #endregion
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            GsaGravityLoad gravityLoad = new GsaGravityLoad();

            //Load case
            int lc = 1;
            GH_Integer gh_lc = new GH_Integer();
            if (DA.GetData(0, ref gh_lc))
                GH_Convert.ToInt32(gh_lc, out lc, GH_Conversion.Both);
            gravityLoad.GravityLoad.Case = lc;

            //element/beam list
            string beamList = "all"; 
            GH_String gh_bl = new GH_String();
            if (DA.GetData(1, ref gh_bl))
                GH_Convert.ToString(gh_bl, out beamList, GH_Conversion.Both);
            gravityLoad.GravityLoad.Elements = beamList;

            //factor
            Vector3 factor = new Vector3();
            Vector3d vect = new Vector3d(0, 0, -1);
            GH_Vector gh_factor = new GH_Vector();
            if (DA.GetData(2, ref gh_factor))
                GH_Convert.ToVector3d(gh_factor, ref vect, GH_Conversion.Both);
            factor.X = vect.X; factor.Y = vect.Y; factor.Z = vect.Z;
            gravityLoad.GravityLoad.Factor = factor;

            GsaLoad gsaLoad = new GsaLoad(gravityLoad);
            DA.SetData(0, new GsaLoadGoo(gsaLoad));
            
        }
    }
}
