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
    public class gsaMember2dInfo : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("e6203d83-c499-43d0-83a1-ccb8892478c9");
        public gsaMember2dInfo()
          : base("GSA 2D Member Info", "Mem2dInfo", "Get GSA Member2d Info",
                Ribbon.CategoryName.name(),
                Ribbon.SubCategoryName.cat2())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.hidden;

        //protected override Bitmap Icon => Resources.CrossSections;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        

        #endregion

        #region Input and output
        
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA Member2d", "Mem2d", "GSA 2D Member to get a bit more info out of", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA 1D Member", "Mem2d", "GSA 1D Member", GH_ParamAccess.item);
            pManager.HideParameter(0);
            pManager.AddBrepParameter("Brep", "Brep", "Member Brep", GH_ParamAccess.item);
            pManager.AddGenericParameter("Section", "Section", "Change Section Property", GH_ParamAccess.item);
            pManager.AddGenericParameter("Offset", "Offset", "Get Member Offset", GH_ParamAccess.item);
            pManager.AddPointParameter("Incl. Points", "inclPt", "Get Inclusion points", GH_ParamAccess.list);
            pManager.AddCurveParameter("Incl. Curves", "inclCrv", "Get Inclusion curves", GH_ParamAccess.list);
            pManager.AddNumberParameter("Mesh Size", "MSize", "Get Targe mesh size", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Mesh With Others", "Mesh/O", "Get if to mesh with others", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member Type", "Type", "Get 2D Member Type", GH_ParamAccess.item);
            pManager.AddIntegerParameter("2D Analysis Type", "Typ2D", "Get Member 2D Analysis Type", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member Number", "ID", "Get Member Number", GH_ParamAccess.item);
            pManager.AddTextParameter("Member Name", "Name", "Get Name of Member", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member Group", "Group", "Get Member Group", GH_ParamAccess.item);
            pManager.AddColourParameter("Member Colour", "Colour", "Get Member Colour", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Dummy Member", "Dummy", "Get if Member is Dummy", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaMember2d gsaMember2d = new GsaMember2d();
            if (DA.GetData(0, ref gsaMember2d))
            {
                if (gsaMember2d != null)
                {
                    DA.SetData(0, new GsaMember2dGoo(gsaMember2d));

                    DA.SetData(1, gsaMember2d.Brep);

                    DA.SetData(2, gsaMember2d.member.Property); //section property to be added
                    
                    GsaOffset gsaOffset = new GsaOffset();
                    gsaOffset.Z = gsaMember2d.member.Offset.Z;
                    DA.SetData(3, gsaOffset);

                    DA.SetDataList(4, gsaMember2d.InclusionPoints);
                    DA.SetDataList(5, gsaMember2d.InclusionLines);

                    DA.SetData(6, gsaMember2d.member.MeshSize);
                    //DA.SetData(7, gsaMember2d.member.MeshWithOthers);

                    DA.SetData(8, gsaMember2d.member.Type);
                    DA.SetData(9, gsaMember2d.member.Type2D);

                    DA.SetData(10, gsaMember2d.ID);
                    DA.SetData(11, gsaMember2d.member.Name);
                    DA.SetData(12, gsaMember2d.member.Group);
                    DA.SetData(13, gsaMember2d.member.Colour);

                    DA.SetData(14, gsaMember2d.member.IsDummy);
                }
            }
        }
    }
}

