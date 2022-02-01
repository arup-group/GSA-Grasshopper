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
    /// <summary>
    /// Component to create a new spring
    /// </summary>
    public class CreateSpring_OBSOLETE : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("e4b7c688-147b-4d91-b754-1a45c715b8db");
        public CreateSpring_OBSOLETE()
          : base("Create Spring", "Spring", "Create GSA Spring (Type: General)",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat1())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.hidden;// | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.CreateSpring;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout

        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("X", "X", "X", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Y", "Y", "Y", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Z", "Z", "Z", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("XX", "XX", "XX", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("YY", "YY", "YY", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("ZZ", "ZZ", "ZZ", GH_ParamAccess.item, 0);

            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Spring", "PS", "GSA Spring (Type: General)", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double x = 0;
            double y = 0;
            double z = 0;
            double xx = 0;
            double yy = 0;
            double zz = 0;
            GH_Number ghSprX = new GH_Number();
            if (DA.GetData(0, ref ghSprX))
                GH_Convert.ToDouble(ghSprX, out x, GH_Conversion.Both);
            GH_Number ghSprY = new GH_Number();
            if (DA.GetData(1, ref ghSprY))
                GH_Convert.ToDouble(ghSprY, out y, GH_Conversion.Both);
            GH_Number ghSprZ = new GH_Number();
            if (DA.GetData(2, ref ghSprZ))
                GH_Convert.ToDouble(ghSprZ, out z, GH_Conversion.Both);
            GH_Number ghSprXX = new GH_Number();
            if (DA.GetData(3, ref ghSprXX))
                GH_Convert.ToDouble(ghSprXX, out xx, GH_Conversion.Both);
            GH_Number ghSprYY = new GH_Number();
            if (DA.GetData(4, ref ghSprYY))
                GH_Convert.ToDouble(ghSprYY, out yy, GH_Conversion.Both);
            GH_Number ghSprZZ = new GH_Number();
            if (DA.GetData(5, ref ghSprZZ))
                GH_Convert.ToDouble(ghSprZZ, out zz, GH_Conversion.Both);
            GsaSpring Spring = new GsaSpring
            {
                X = x,
                Y = y,
                Z = z,
                XX = xx,
                YY = yy,
                ZZ = zz
            };
            DA.SetData(0, new GsaSpringGoo(Spring.Duplicate()));

            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Please note that springs are not yet supported in GsaGH");
        }
    }
}

