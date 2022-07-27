//using System;
//using System.Collections.Generic;
//using Grasshopper.Kernel.Attributes;
//using Grasshopper.GUI.Canvas;
//using Grasshopper.GUI;
//using Grasshopper.Kernel;
//using Grasshopper;
//using Rhino.Geometry;
//using System.Windows.Forms;
//using Grasshopper.Kernel.Types;
//using GsaAPI;
//using GhSA.Parameters;
//using System.Resources;

//namespace GhSA.Components
//{
//    /// <summary>
//    /// Component to create a new Offset
//    /// </summary>
//    public class CreateOffset_OBSOLETE : GH_OasysComponent
//    {
//        #region Name and Ribbon Layout
//        // This region handles how the component in displayed on the ribbon
//        // including name, exposure level and icon
//        public override Guid ComponentGuid => new Guid("ba62abd3-cd48-4dd2-9cd1-d89c921dd108");
//        public CreateOffset_OBSOLETE()
//          : base("Create Offset", "Offset", "Create GSA Offset",
//                Ribbon.CategoryName.Name(),
//                Ribbon.SubCategoryName.Cat1())
//        { this.Hidden = true; } // sets the initial state of the component to hidden
//        public override GH_Exposure Exposure => GH_Exposure.hidden;

//        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.CreateOffset;
//        #endregion

//        #region Custom UI
//        //This region overrides the typical component layout

//        #endregion

//        #region Input and output

//        protected override void RegisterInputParams(GH_InputParamManager pManager)
//        {
//            pManager.AddNumberParameter("Offset X1", "X1", "X1 - Start axial offset (" + Units.LengthUnitGeometry.ToString() + ")", GH_ParamAccess.item, 0);
//            pManager.AddNumberParameter("Offset X2", "X2", "X2 - End axial offset (" + Units.LengthUnitGeometry.ToString() + ")", GH_ParamAccess.item, 0);
//            pManager.AddNumberParameter("Offset Y", "Y", "Y Offset (" + Units.LengthUnitGeometry.ToString() + ")", GH_ParamAccess.item, 0);
//            pManager.AddNumberParameter("Offset Z", "Z", "Z Offset (" + Units.LengthUnitGeometry.ToString() + ")", GH_ParamAccess.item, 0);

//            pManager[0].Optional = true;
//            pManager[1].Optional = true;
//            pManager[2].Optional = true;
//            pManager[3].Optional = true;
//        }

//        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
//        {
//            pManager.AddGenericParameter("Offset", "Of", "GSA Offset", GH_ParamAccess.item);
//        }
//        #endregion

//        protected override void SolveInstance(IGH_DataAccess DA)
//        {
//            double x1 = 0;
//            double x2 = 0;
//            double y = 0;
//            double z = 0;
//            GH_Number ghOffsetX1 = new GH_Number();
//            if (DA.GetData(0, ref ghOffsetX1))
//                GH_Convert.ToDouble(ghOffsetX1, out x1, GH_Conversion.Both);
//            GH_Number ghOffsetX2 = new GH_Number();
//            if (DA.GetData(1, ref ghOffsetX2))
//                GH_Convert.ToDouble(ghOffsetX2, out x2, GH_Conversion.Both);
//            GH_Number ghOffsetY = new GH_Number();
//            if (DA.GetData(2, ref ghOffsetY))
//                GH_Convert.ToDouble(ghOffsetY, out y, GH_Conversion.Both);
//            GH_Number ghOffsetZ = new GH_Number();
//            if (DA.GetData(3, ref ghOffsetZ))
//                GH_Convert.ToDouble(ghOffsetZ, out z, GH_Conversion.Both);

//            GsaOffset offset = new GsaOffset
//            {
//                X1 = x1,
//                X2 = x2,
//                Y = y,
//                Z = z
//            };

//            DA.SetData(0, new GsaOffsetGoo(offset.Duplicate()));
//        }
//    }
//}
