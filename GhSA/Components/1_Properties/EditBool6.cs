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
    /// Component to edit a Bool6 and ouput the information
    /// </summary>
    public class EditBool6 : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("dad5064c-6648-45a5-8d98-afaae861e3b9");
        public EditBool6()
          : base("Edit Bool6", "Bool6Edit", "Modify GSA Bool6 or just get information about existing",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat1())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.EditBool6;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout

        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Bool6", "B6", "GSA Bool6 to set or get releases or restraints for", GH_ParamAccess.item);
            pManager.AddBooleanParameter("X", "X", "X", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Y", "Y", "Y", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Z", "Z", "Z", GH_ParamAccess.item);
            pManager.AddBooleanParameter("XX", "XX", "XX", GH_ParamAccess.item);
            pManager.AddBooleanParameter("YY", "YY", "YY", GH_ParamAccess.item);
            pManager.AddBooleanParameter("ZZ", "ZZ", "ZZ", GH_ParamAccess.item);
            for (int i = 0; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Bool6", "B6", "GSA Bool6 with changes", GH_ParamAccess.item);
            pManager.AddBooleanParameter("X", "X", "X", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Y", "Y", "Y", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Z", "Z", "Z", GH_ParamAccess.item);
            pManager.AddBooleanParameter("XX", "XX", "XX", GH_ParamAccess.item);
            pManager.AddBooleanParameter("YY", "YY", "YY", GH_ParamAccess.item);
            pManager.AddBooleanParameter("ZZ", "ZZ", "ZZ", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaBool6 mybool = new GsaBool6();
            GsaBool6 gsabool = new GsaBool6();
            if (DA.GetData(0, ref gsabool))
            {
                mybool = gsabool.Duplicate();
            }
            if (mybool != null)
            {
                //inputs
                bool x = new bool();
                if (DA.GetData(1, ref x))
                    mybool.X = x;
                bool y = new bool();
                if (DA.GetData(2, ref y))
                    mybool.Y = y;
                bool z = new bool();
                if (DA.GetData(3, ref z))
                    mybool.Z = z;
                bool xx = new bool();
                if (DA.GetData(4, ref xx))
                    mybool.XX = xx;
                bool yy = new bool();
                if (DA.GetData(5, ref yy))
                    mybool.YY = yy;
                bool zz = new bool();
                if (DA.GetData(6, ref zz))
                    mybool.ZZ = zz;

                //outputs
                DA.SetData(0, new GsaBool6Goo(mybool));
                DA.SetData(1, mybool.X);
                DA.SetData(2, mybool.Y);
                DA.SetData(3, mybool.Z);
                DA.SetData(4, mybool.XX);
                DA.SetData(5, mybool.YY);
                DA.SetData(6, mybool.ZZ);

            }
        }
    }
}

