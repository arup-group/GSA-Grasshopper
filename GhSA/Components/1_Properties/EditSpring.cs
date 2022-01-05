﻿using System;
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
    /// Component to edit a Spring and ouput the information
    /// </summary>
    public class EditSpring : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("037f46d0-f0f6-4e99-8851-fc99d5e8205c");
        public EditSpring()
          : base("Edit Spring", "SpringEdit", "Modify GSA Spring",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat1())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.hidden;// | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.EditSpring;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout

        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Spring", "PS", "GSA Spring to get or set information for", GH_ParamAccess.item);
            pManager.AddNumberParameter("X", "X", "X", GH_ParamAccess.item);
            pManager.AddNumberParameter("Y", "Y", "Y", GH_ParamAccess.item);
            pManager.AddNumberParameter("Z", "Z", "Z", GH_ParamAccess.item);
            pManager.AddNumberParameter("XX", "XX", "XX", GH_ParamAccess.item);
            pManager.AddNumberParameter("YY", "YY", "YY", GH_ParamAccess.item);
            pManager.AddNumberParameter("ZZ", "ZZ", "ZZ", GH_ParamAccess.item);
            for (int i = 0; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Spring", "PS", "GSA Spring with changes", GH_ParamAccess.item);
            pManager.AddNumberParameter("X", "X", "X", GH_ParamAccess.item);
            pManager.AddNumberParameter("Y", "Y", "Y", GH_ParamAccess.item);
            pManager.AddNumberParameter("Z", "Z", "Z", GH_ParamAccess.item);
            pManager.AddNumberParameter("XX", "XX", "XX", GH_ParamAccess.item);
            pManager.AddNumberParameter("YY", "YY", "YY", GH_ParamAccess.item);
            pManager.AddNumberParameter("ZZ", "ZZ", "ZZ", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaSpring spring = new GsaSpring();
            GsaSpring gsaspring = new GsaSpring();
            if (DA.GetData(0, ref gsaspring))
            {
                spring = gsaspring.Duplicate();
            }
            if (spring != null)
            {
                //inputs
                double x = 0;
                if (DA.GetData(1, ref x))
                    spring.X = x;
                double y = 0;
                if (DA.GetData(2, ref y))
                    spring.Y = y;
                double z = 0;
                if (DA.GetData(3, ref z))
                    spring.Z = z;
                double xx = 0;
                if (DA.GetData(4, ref xx))
                    spring.XX = xx;
                double yy = 0;
                if (DA.GetData(5, ref yy))
                    spring.YY = yy;
                double zz = 0;
                if (DA.GetData(6, ref zz))
                    spring.ZZ = zz;

                //outputs
                DA.SetData(0, new GsaSpringGoo(spring));
                DA.SetData(1, spring.X);
                DA.SetData(2, spring.Y);
                DA.SetData(3, spring.Z);
                DA.SetData(4, spring.XX);
                DA.SetData(5, spring.YY);
                DA.SetData(6, spring.ZZ);
            }
        }
    }
}

