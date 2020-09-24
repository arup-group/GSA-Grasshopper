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
    public class gsaElement1dInfo : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("0cd72170-d351-481c-b2bb-c65048de4291");
        public gsaElement1dInfo()
          : base("GSA 1D Element Info", "Elem1dInfo", "Get GSA Element1d Info",
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
            pManager.AddGenericParameter("GSA Element1d", "Elem1d", "GSA 1D Element to get a bit more info out of", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA 1D Element", "Elem1d", "GSA 1D Element", GH_ParamAccess.item);
            pManager.HideParameter(0);
            pManager.AddLineParameter("Line", "Line", "Element Line", GH_ParamAccess.item);
            pManager.AddGenericParameter("Section", "Section", "Get Section Property", GH_ParamAccess.item);
            pManager.AddGenericParameter("Offset", "Offset", "Get Element Offset", GH_ParamAccess.item);
            pManager.AddGenericParameter("Start release", "R-End1", "Get Release (Bool6) at Start of Element", GH_ParamAccess.item);
            pManager.AddGenericParameter("End release", "R-End2", "Get Release (Bool6) at End of Element", GH_ParamAccess.item);
            pManager.AddNumberParameter("Orientation Angle", "OriAng", "Get Element Orientation Angle in degrees", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Orientation Node", "OriNod", "Get Element Orientation Node (ID referring to node number in model)", GH_ParamAccess.item);
            pManager.AddIntegerParameter("1D Element Type", "Typ1D", "Get Element 1D Type", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Element1d Number", "ID", "Get Element Number. If ID is set it will replace any existing 1D Element in the model", GH_ParamAccess.item);
            pManager.AddTextParameter("Element1d Name", "Name", "Get Name of Element1d", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Element1d Group", "Group", "Get Element 1D Group", GH_ParamAccess.item);
            pManager.AddColourParameter("Element1d Colour", "Colour", "Get Element 1D Colour", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Parent Members", "ParentM", "Get Parent Member IDs in Model that Element was created from", GH_ParamAccess.list);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaElement1d gsaElement1d = new GsaElement1d();
            if (DA.GetData(0, ref gsaElement1d))
            {
                if (gsaElement1d != null)
                {
                    DA.SetData(0, new GsaElement1dGoo(gsaElement1d));

                    DA.SetData(1, gsaElement1d.Line);
                    DA.SetData(2, gsaElement1d.Element.Property); //section property to be added

                    GsaOffset offset = new GsaOffset();
                    offset.X1 = gsaElement1d.Element.Offset.X1;
                    offset.X2 = gsaElement1d.Element.Offset.X2;
                    offset.Y = gsaElement1d.Element.Offset.Y;
                    offset.Z = gsaElement1d.Element.Offset.Z;
                    DA.SetData(3, offset);

                    DA.SetData(4, gsaElement1d.ReleaseStart);
                    DA.SetData(5, gsaElement1d.ReleaseEnd);
                    
                    DA.SetData(6, gsaElement1d.Element.OrientationAngle);
                    DA.SetData(7, gsaElement1d.Element.OrientationNode);

                    DA.SetData(8, gsaElement1d.Element.Type);

                    DA.SetData(9, gsaElement1d.ID);
                    DA.SetData(10, gsaElement1d.Element.Name);
                    DA.SetData(11, gsaElement1d.Element.Group);
                    DA.SetData(12, gsaElement1d.Element.Colour);

                    try { DA.SetData(13, gsaElement1d.Element.ParentMember.Member); } catch (Exception) { }
                    //DA.SetData(16, gsaElement1d.Element.IsDummy);
                }
            }
        }
    }
}

