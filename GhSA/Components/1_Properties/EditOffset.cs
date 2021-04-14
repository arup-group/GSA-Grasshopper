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
    /// Component to edit an Offset and ouput the information
    /// </summary>
    public class EditOffset : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("1e094fcd-8f5f-4047-983c-e0e57a83ae52");
        public EditOffset()
          : base("Edit Offset", "OffsetEdit", "Modify GSA Offset or just get information about existing",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat1())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.EditOffset;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout

        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Offset", "Of", "GSA Offset", GH_ParamAccess.item);
            pManager.AddNumberParameter("Offset X1", "X1", "Set X1 - Start axial offset (" + Units.LengthLarge + ")", GH_ParamAccess.item);
            pManager.AddNumberParameter("Offset X2", "X2", "Set X2 - End axial offset (" + Units.LengthLarge + ")", GH_ParamAccess.item);
            pManager.AddNumberParameter("Offset Y", "Y", "Set Y Offset (" + Units.LengthLarge + ")", GH_ParamAccess.item);
            pManager.AddNumberParameter("Offset Z", "Z", "Set Z Offset (" + Units.LengthLarge + ")", GH_ParamAccess.item);
            for (int i = 0; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Offset", "Of", "GSA Offset", GH_ParamAccess.item);
            pManager.AddNumberParameter("Offset X1", "X1", "X1 - Start axial offset (" + Units.LengthLarge + ")", GH_ParamAccess.item);
            pManager.AddNumberParameter("Offset X2", "X2", "X2 - End axial offset (" + Units.LengthLarge + ")", GH_ParamAccess.item);
            pManager.AddNumberParameter("Offset Y", "Y", "Y Offset (" + Units.LengthLarge + ")", GH_ParamAccess.item);
            pManager.AddNumberParameter("Offset Z", "Z", "Z Offset (" + Units.LengthLarge + ")", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaOffset offset = new GsaOffset();
            GsaOffset gsaoffset = new GsaOffset();
            if (DA.GetData(0, ref gsaoffset))
            {
                offset = gsaoffset.Duplicate();
            }
            if (offset != null)
            {
                //inputs
                double x1 = 0;
                if (DA.GetData(1, ref x1))
                    offset.X1 = x1;
                double x2 = 0;
                if (DA.GetData(2, ref x2))
                    offset.X2 = x2;
                double y = 0;
                if (DA.GetData(3, ref y))
                    offset.Y = y;
                double z = 0;
                if (DA.GetData(4, ref z))
                    offset.Z = z;

                //outputs
                DA.SetData(0, new GsaOffsetGoo(offset));
                DA.SetData(1, offset.X1);
                DA.SetData(2, offset.X2);
                DA.SetData(3, offset.Y);
                DA.SetData(4, offset.Z);
            }
        }
    }
}

