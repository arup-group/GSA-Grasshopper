using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GhSA.Parameters;


namespace GhSA.Components
{
    /// <summary>
    /// Component to retrieve non-geometric objects from a GSA model
    /// </summary>
    public class GetLoads : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("87ff28e5-a1a6-4d78-ba71-e930e01dca13");
        public GetLoads()
          : base("Get Model Loads", "GetLoads", "Get Loads and Grid Planes/Surfaces from GSA model",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat0())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.GetLoads;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA Model", "GSA", "GSA model containing some loads", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Gravity Loads", "Gr", "Gravity Loads from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Node Loads", "No", "Node Loads from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Beam Loads", "Be", "Beam Loads from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Face Loads", "Fa", "Face Loads from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Grid Point Loads", "Pt", "Grid Point Loads from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Grid Line Loads", "Ln", "Grid Line Loads from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Grid Area Loads", "Ar", "Grid Area Loads from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Grid Plane Surfaces", "GPS", "Grid Plane Surfaces from GSA Model", GH_ParamAccess.list);
            pManager.HideParameter(7);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaModel gsaModel = new GsaModel();
            if (DA.GetData(0, ref gsaModel))
            {
                Model model = new Model();
                model = gsaModel.Model;

                List<GsaLoadGoo> gravity = Util.Gsa.FromGSA.GetGravityLoads(model.GravityLoads());
                List<GsaLoadGoo> node = Util.Gsa.FromGSA.GetNodeLoads(model);
                List<GsaLoadGoo> beam = Util.Gsa.FromGSA.GetBeamLoads(model.BeamLoads());
                List<GsaLoadGoo> face = Util.Gsa.FromGSA.GetFaceLoads(model.FaceLoads());

                IReadOnlyDictionary<int, GridSurface> srfDict = model.GridSurfaces();
                IReadOnlyDictionary<int, GridPlane> plnDict = model.GridPlanes();
                IReadOnlyDictionary<int, Axis> axDict = model.Axes();
                List<GsaLoadGoo> point = Util.Gsa.FromGSA.GetGridPointLoads(model.GridPointLoads(), srfDict, plnDict, axDict);
                List<GsaLoadGoo> line = Util.Gsa.FromGSA.GetGridLineLoads(model.GridLineLoads(), srfDict, plnDict, axDict);
                List<GsaLoadGoo> area = Util.Gsa.FromGSA.GetGridAreaLoads(model.GridAreaLoads(), srfDict, plnDict, axDict);
                
                List<GsaGridPlaneSurfaceGoo> gps = new List<GsaGridPlaneSurfaceGoo>();
                
                foreach (int key in srfDict.Keys)
                    gps.Add(new GsaGridPlaneSurfaceGoo(Util.Gsa.FromGSA.GetGridPlaneSurface(srfDict, plnDict, axDict, key)));

                DA.SetDataList(0, gravity);
                DA.SetDataList(1, node);
                DA.SetDataList(2, beam);
                DA.SetDataList(3, face);
                DA.SetDataList(4, point);
                DA.SetDataList(5, line);
                DA.SetDataList(6, area);
                DA.SetDataList(7, gps);
            }
        }
    }
}

