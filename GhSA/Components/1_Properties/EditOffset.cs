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
using UnitsNet;
using System.Linq;
using UnitsNet.GH;

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
        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.EditOffset;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout

        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            IQuantity quantity = new Length(0, Units.LengthUnitGeometry);
            string unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

            pManager.AddGenericParameter("Offset", "Of", "GSA Offset", GH_ParamAccess.item);
            pManager.AddGenericParameter("Offset X1 [" + unitAbbreviation + "]", "X1", "X1 - Start axial offset", GH_ParamAccess.item);
            pManager.AddGenericParameter("Offset X2 [" + unitAbbreviation + "]", "X2", "X2 - End axial offset", GH_ParamAccess.item);
            pManager.AddGenericParameter("Offset Y [" + unitAbbreviation + "]", "Y", "Y Offset", GH_ParamAccess.item);
            pManager.AddGenericParameter("Offset Z [" + unitAbbreviation + "]", "Z", "Z Offset", GH_ParamAccess.item);
            for (int i = 1; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            IQuantity quantity = new Length(0, Units.LengthUnitGeometry);
            string unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

            pManager.AddGenericParameter("Offset", "Of", "GSA Offset", GH_ParamAccess.item);
            pManager.AddGenericParameter("Offset X1 [" + unitAbbreviation + "]", "X1", "X1 - Start axial offset", GH_ParamAccess.item);
            pManager.AddGenericParameter("Offset X2 [" + unitAbbreviation + "]", "X2", "X2 - End axial offset", GH_ParamAccess.item);
            pManager.AddGenericParameter("Offset Y [" + unitAbbreviation + "]", "Y", "Y Offset", GH_ParamAccess.item);
            pManager.AddGenericParameter("Offset Z [" + unitAbbreviation + "]", "Z", "Z Offset", GH_ParamAccess.item);
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
                int inp = 0;
                if (this.Params.Input[inp].SourceCount != 0)
                    offset.X1 = GetInput.Length(this, DA, inp++, Units.LengthUnitGeometry, true);

                if (this.Params.Input[inp].SourceCount != 0)
                    offset.X2 = GetInput.Length(this, DA, inp++, Units.LengthUnitGeometry, true);
                
                if (this.Params.Input[inp].SourceCount != 0)
                    offset.Y = GetInput.Length(this, DA, inp++, Units.LengthUnitGeometry, true);
                
                if (this.Params.Input[inp].SourceCount != 0)
                    offset.Z = GetInput.Length(this, DA, inp++, Units.LengthUnitGeometry, true);

                //outputs
                int outp = 0;
                DA.SetData(outp++, new GsaOffsetGoo(offset));
                
                DA.SetData(outp++, new GH_UnitNumber(offset.X1));
                DA.SetData(outp++, new GH_UnitNumber(offset.X2));
                DA.SetData(outp++, new GH_UnitNumber(offset.Y));
                DA.SetData(outp++, new GH_UnitNumber(offset.Z));
            }
        }
    }
}