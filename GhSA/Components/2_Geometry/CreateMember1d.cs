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
    /// Component to create new 1D Member
    /// </summary>
    public class CreateMember1d : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("5c5b9efa-cdae-4be5-af40-ff2b590801dd");
        public CreateMember1d()
          : base("Create 1D Member", "Mem1D", "Create GSA 1D Member",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat2())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        //protected override Bitmap Icon => Resources.CrossSections;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            m_attributes = new UI.ReleasesComponentUI(this, SetReleases, "Start Release", "End Release", x1, y1, z1, xx1, yy1, zz1, x2, y2, z2, xx2, yy2, zz2);
        }

        public void SetReleases(bool resx1, bool resy1, bool resz1, bool resxx1, bool resyy1, bool reszz1,
            bool resx2, bool resy2, bool resz2, bool resxx2, bool resyy2, bool reszz2)
        {
            x1 = resx1;
            y1 = resy1;
            z1 = resz1;
            xx1 = resxx1;
            yy1 = resyy1;
            zz1 = reszz1;
            x2 = resx2;
            y2 = resy2;
            z2 = resz2;
            xx2 = resxx2;
            yy2 = resyy2;
            zz2 = reszz2;
        }

        #endregion

        #region Input and output
        bool x1;
        bool y1;
        bool z1;
        bool xx1;
        bool yy1;
        bool zz1;
        bool x2;
        bool y2;
        bool z2;
        bool xx2;
        bool yy2;
        bool zz2;

        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            // we need to save all the items that we want to reappear when a GH file is saved and re-opened
            writer.SetBoolean("x1", (bool)x1);
            writer.SetBoolean("y1", (bool)y1);
            writer.SetBoolean("z1", (bool)z1);
            writer.SetBoolean("xx1", (bool)xx1);
            writer.SetBoolean("yy1", (bool)yy1);
            writer.SetBoolean("zz1", (bool)zz1);
            writer.SetBoolean("x2", (bool)x2);
            writer.SetBoolean("y2", (bool)y2);
            writer.SetBoolean("z2", (bool)z2);
            writer.SetBoolean("xx2", (bool)xx2);
            writer.SetBoolean("yy2", (bool)yy2);
            writer.SetBoolean("zz2", (bool)zz2);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            // when a GH file is opened we need to read in the data that was previously set by user
            x1 = (bool)reader.GetBoolean("x1");
            y1 = (bool)reader.GetBoolean("y1");
            z1 = (bool)reader.GetBoolean("z1");
            xx1 = (bool)reader.GetBoolean("xx1");
            yy1 = (bool)reader.GetBoolean("yy1");
            zz1 = (bool)reader.GetBoolean("zz1");
            x2 = (bool)reader.GetBoolean("x2");
            y2 = (bool)reader.GetBoolean("y2");
            z2 = (bool)reader.GetBoolean("z2");
            xx2 = (bool)reader.GetBoolean("xx2");
            yy2 = (bool)reader.GetBoolean("yy2");
            zz2 = (bool)reader.GetBoolean("zz2");
            // we need to recreate the custom UI again as this is created before this read IO is called
            // otherwise the component will not display the selected item on the canvas
            this.CreateAttributes();
            return base.Read(reader);
        }
        #endregion

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "Crv", "Curve (will be converted to Arcs and Lines automatically if NURBS)", GH_ParamAccess.item);
            pManager.AddGenericParameter("Section", "PB", "GSA Section Property. Input either a GSA Section or an Integer to use a Section already defined in model", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager.HideParameter(0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("1D Member", "Mem1d", "GSA 1D Member", GH_ParamAccess.item);
            
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Curve ghcrv = new GH_Curve();
            if (DA.GetData(0, ref ghcrv))
            {
                Curve crv = null;
                if (GH_Convert.ToCurve(ghcrv, ref crv, GH_Conversion.Both))
                {
                    GsaMember1d mem = new GsaMember1d(crv);

                    GsaBool6 rel1 = new GsaBool6
                    {
                        X = x1,
                        Y = y1,
                        Z = z1,
                        XX = xx1,
                        YY = yy1,
                        ZZ = zz1
                    };

                    mem.ReleaseStart = rel1;

                    GsaBool6 rel2 = new GsaBool6
                    {
                        X = x2,
                        Y = y2,
                        Z = z2,
                        XX = xx2,
                        YY = yy2,
                        ZZ = zz2
                    };
                    mem.ReleaseEnd = rel2;

                    // 1 section
                    GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
                    GsaSection section = new GsaSection();
                    if (DA.GetData(1, ref gh_typ))
                    {
                        if (gh_typ.Value is GsaSection)
                            gh_typ.CastTo(ref section);
                        else if (gh_typ.Value is GH_Number)
                        {
                            if(GH_Convert.ToInt32((GH_Number)gh_typ.Value, out int idd, GH_Conversion.Both))
                                section.ID = idd;
                        }
                        else
                            section.ID = 1;
                    }
                    mem.Section = section;

                    DA.SetData(0, new GsaMember1dGoo(mem));
                }
            }
        }
    }
}

