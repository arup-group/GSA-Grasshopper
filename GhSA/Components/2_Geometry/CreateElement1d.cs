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
    public class gsaCreateElement1d : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("88c58aae-4cd8-4d37-b63f-d828571e6941");
        public gsaCreateElement1d()
          : base("Create 1D Element", "Elem1D", "Create GSA 1D Element",
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
            pManager.AddCurveParameter("Line", "Ln", "Line to create GSA Element", GH_ParamAccess.item);
            pManager.AddGenericParameter("Section", "PB", "GSA Section Property. Input either a GSA Section or an Integer to use a Section already defined in model", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager.HideParameter(0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("1D Element", "Elem1d", "GSA 1D Element", GH_ParamAccess.item);
            
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Line ghln = new GH_Line();
            if (DA.GetData(0, ref ghln))
            {
                Line ln = new Line();
                if (GH_Convert.ToLine(ghln, ref ln, GH_Conversion.Both))
                {
                    GsaElement1d elem = new GsaElement1d(new LineCurve(ln));

                    // 1 section
                    GsaSection section = new GsaSection();
                    GH_Integer gh_sec_idd = new GH_Integer();
                    if (DA.GetData(1, ref section))
                        elem.Section = section;
                    else if (DA.GetData(1, ref gh_sec_idd))
                    {
                        int idd = 0;
                        if (GH_Convert.ToInt32(gh_sec_idd, out idd, GH_Conversion.Both))
                            elem.Section.ID = idd;
                    }
                    else
                        elem.Section.ID = 1;

                    DA.SetData(0, new GsaElement1dGoo(elem));
                }
            }
        }
    }
}

