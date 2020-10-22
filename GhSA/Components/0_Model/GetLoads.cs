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
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override System.Drawing.Bitmap Icon => GSA.Properties.Resources.GetLoads;
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
            pManager.AddGenericParameter("Gravity Loads", "Grav", "Gravity Loads from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Node Loads", "Node", "Node Loads from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Beam Loads", "Beam", "Beam Loads from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Face Loads", "Face", "Face Loads from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Grid Point Loads", "Point", "Grid Point Loads from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Grid Line Loads", "Line", "Grid Line Loads from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Grid Area Loads", "Area", "Grid Area Loads from GSA Model", GH_ParamAccess.list);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaModel gsaModel = new GsaModel();
            if (DA.GetData(0, ref gsaModel))
            {
                Model model = new Model();
                model = gsaModel.Model;

                List<GsaLoadGoo> gravity = Util.Gsa.GsaImport.GsaGetGravityLoads(model);
                List<GsaLoadGoo> node = Util.Gsa.GsaImport.GsaGetNodeLoads(model);
                List<GsaLoadGoo> beam = Util.Gsa.GsaImport.GsaGetBeamLoads(model);
                List<GsaLoadGoo> face = Util.Gsa.GsaImport.GsaGetFaceLoads(model);
                List<GsaLoadGoo> point = Util.Gsa.GsaImport.GsaGetGridPointLoads(model);
                List<GsaLoadGoo> line = Util.Gsa.GsaImport.GsaGetGridLineLoads(model);
                List<GsaLoadGoo> area = Util.Gsa.GsaImport.GsaGetGridAreaLoads(model);

                DA.SetDataList(0, gravity);
                DA.SetDataList(1, node);
                DA.SetDataList(2, beam);
                DA.SetDataList(3, face);
                DA.SetDataList(4, point);
                DA.SetDataList(5, line);
                DA.SetDataList(6, area);
            }
        }
    }
}

