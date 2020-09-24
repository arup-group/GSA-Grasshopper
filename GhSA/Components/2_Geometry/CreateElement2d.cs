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
using System.Resources;

namespace GhSA.Components
{
    public class gsaCreateElement2d : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("8f83d32a-c2df-4f47-9cfc-d2d4253703e1");
        public gsaCreateElement2d()
          : base("Create 2D Element", "Elem2D", "Create GSA 2D Element",
                Ribbon.CategoryName.name(),
                Ribbon.SubCategoryName.cat2())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;

        //protected override Bitmap Icon => Resources.CrossSections;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout


        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh to create GSA Element", GH_ParamAccess.item);
            pManager.AddGenericParameter("2D Property", "PA", "GSA 2D Property", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager.HideParameter(0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("2D Element", "Elem2d", "GSA 2D Element", GH_ParamAccess.item);
            
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Mesh ghmesh = new GH_Mesh();
            if (DA.GetData(0, ref ghmesh))
            {
                Mesh mesh = new Mesh();
                if (GH_Convert.ToMesh(ghmesh, ref mesh, GH_Conversion.Both))
                {
                    DA.SetData(0, new GsaElement2dGoo(new GsaElement2d(mesh)));
                }
            }
        }
    }
}

