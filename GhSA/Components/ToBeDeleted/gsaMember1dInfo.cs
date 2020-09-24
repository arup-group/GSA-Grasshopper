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
    public class gsaMember1dInfo : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("f932fa90-aad4-4cd7-bb64-804bcc6d1cc0");
        public gsaMember1dInfo()
          : base("GSA 1D Member Info", "Mem1dInfo", "Get GSA Member1d Info",
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
            pManager.AddGenericParameter("GSA Member1d", "Mem1d", "GSA 1D Member to get a bit more info out of", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA 1D Member", "Mem1d", "GSA 1D Member", GH_ParamAccess.item);
            pManager.HideParameter(0);
            pManager.AddCurveParameter("Curve", "Crv", "Member Curve", GH_ParamAccess.item);
            pManager.AddGenericParameter("Section", "Sec", "Change Section Property", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member Type", "Type", "Get 1D Member Type", GH_ParamAccess.item);
            pManager.AddIntegerParameter("1D Element Type", "Typ1D", "Get Element 1D Type", GH_ParamAccess.item);
            pManager.AddGenericParameter("Offset", "Off", "Get Member Offset", GH_ParamAccess.item);
            pManager.AddGenericParameter("Start release", "ReS", "Get Release (Bool6) at Start of Member", GH_ParamAccess.item);
            pManager.AddGenericParameter("End release", "ReE", "Get Release (Bool6) at End of Member", GH_ParamAccess.item);
            pManager.AddNumberParameter("Orientation Angle", "OrA", "Get Member Orientation Angle in degrees", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Orientation Node", "OrN", "Get Member Orientation Node (ID referring to node number in model)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Mesh Size", "MSz", "Get Member Mesh Size", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Mesh With Others", "MwO", "Get if to mesh with others", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member1d Number", "ID", "Get Member Number", GH_ParamAccess.item);
            pManager.AddTextParameter("Member Name", "Na", "Get Name of Member1d", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member Group", "Grp", "Get Member Group", GH_ParamAccess.item);
            pManager.AddColourParameter("Member Colour", "Col", "Get Member Colour", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Dummy Member", "Dum", "Get it Member is Dummy", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaMember1d gsaMember1d = new GsaMember1d();
            if (DA.GetData(0, ref gsaMember1d))
            {
                if (gsaMember1d != null)
                {
                    DA.SetData(0, new GsaMember1dGoo(gsaMember1d));

                    DA.SetData(1, gsaMember1d.PolyCurve);
                    DA.SetData(2, gsaMember1d.member.Property); //section property to be added

                    DA.SetData(3, gsaMember1d.member.Type);

                    DA.SetData(4, gsaMember1d.member.Type1D);

                    GsaOffset gsaOffset = new GsaOffset();
                    gsaOffset.X1 = gsaMember1d.member.Offset.X1;
                    gsaOffset.X2 = gsaMember1d.member.Offset.X2;
                    gsaOffset.Y = gsaMember1d.member.Offset.Y;
                    gsaOffset.Z = gsaMember1d.member.Offset.Z;
                    DA.SetData(5, gsaOffset);

                    DA.SetData(6, gsaMember1d.ReleaseStart);
                    DA.SetData(7, gsaMember1d.ReleaseEnd);
                    
                    DA.SetData(8, gsaMember1d.member.OrientationAngle);
                    DA.SetData(9, gsaMember1d.member.OrientationNode);

                    DA.SetData(10, gsaMember1d.member.MeshSize);
                    //DA.SetData(11, gsaMember1d.member.MeshSize); //mesh with others bool

                    DA.SetData(12, gsaMember1d.ID);
                    DA.SetData(13, gsaMember1d.member.Name);
                    DA.SetData(14, gsaMember1d.member.Group);
                    DA.SetData(15, gsaMember1d.member.Colour);

                    DA.SetData(16, gsaMember1d.member.IsDummy);
                }
            }
        }
    }
}

