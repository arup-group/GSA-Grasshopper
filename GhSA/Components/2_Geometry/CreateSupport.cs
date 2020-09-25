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
    /// Component to create new Node with restraints (support)
    /// </summary>
    public class CreateSupport : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("d808e81f-6ae1-49d9-a8a5-2424a1763a69");
        public CreateSupport()
          : base("Create Support", "Support", "Create GSA Node Support",
                Ribbon.CategoryName.name(),
                Ribbon.SubCategoryName.cat2())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        //protected override Bitmap Icon => Resources.CrossSections;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            m_attributes = new UI.SupportComponentUI(this, setRestraints, "Restraints", x, y, z, xx, yy, zz);
        }

        public void setRestraints(bool resx, bool resy, bool resz, bool resxx, bool resyy, bool reszz)
        {
            x = resx;
            y = resy;
            z = resz;
            xx = resxx;
            yy = resyy;
            zz = reszz;
        }

        #endregion

        #region Input and output
        bool x;
        bool y;
        bool z;
        bool xx;
        bool yy;
        bool zz;

        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            // we need to save all the items that we want to reappear when a GH file is saved and re-opened
            writer.SetBoolean("x", (bool)x);
            writer.SetBoolean("y", (bool)y);
            writer.SetBoolean("z", (bool)z);
            writer.SetBoolean("xx", (bool)xx);
            writer.SetBoolean("yy", (bool)yy);
            writer.SetBoolean("zz", (bool)zz);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            // when a GH file is opened we need to read in the data that was previously set by user
            x = (bool)reader.GetBoolean("x");
            y = (bool)reader.GetBoolean("y");
            z = (bool)reader.GetBoolean("z");
            xx = (bool)reader.GetBoolean("xx");
            yy = (bool)reader.GetBoolean("yy");
            zz = (bool)reader.GetBoolean("zz");
            // we need to recreate the custom UI again as this is created before this read IO is called
            // otherwise the component will not display the selected item on the canvas
            this.CreateAttributes();
            return base.Read(reader);
        }
        #endregion

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "Pt", "Point (x, y, z) location of support", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Pl", "(Optional) Plane for local axis", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddGenericParameter("Restraints", "B6", "(Optional) Restraint in Bool6 form", GH_ParamAccess.item);
            pManager.AddGenericParameter("Spring", "Spr", "(Optional) GSA Spring", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager.HideParameter(1);
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Node", "No", "GSA Node with Restraint", GH_ParamAccess.list);
            
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Point ghpt = new GH_Point();
            if (DA.GetData(0, ref ghpt))
            {
                Point3d pt = new Point3d();
                if (GH_Convert.ToPoint3d(ghpt, ref pt, GH_Conversion.Both))
                {
                    GH_Plane gH_Plane = new GH_Plane();
                    Plane localAxis = Plane.WorldXY;
                    if (DA.GetData(1, ref gH_Plane))
                        GH_Convert.ToPlane(gH_Plane, ref localAxis, GH_Conversion.Both);

                    GsaNode node = new GsaNode(pt);
                    
                    GsaBool6 bool6 = new GsaBool6();
                    if (DA.GetData(2, ref bool6))
                    {
                        x = bool6.X;
                        y = bool6.Y;
                        z = bool6.Z;
                        xx = bool6.XX;
                        yy = bool6.YY;
                        zz = bool6.ZZ;
                    }

                    GsaSpring spring = new GsaSpring();
                    if (DA.GetData(3, ref spring))
                        node.Spring = spring;

                    node.localAxis = localAxis;
                    
                    node.node.Restraint.X = x;
                    node.node.Restraint.Y = y;
                    node.node.Restraint.Z = z;
                    node.node.Restraint.XX = xx;
                    node.node.Restraint.YY = yy;
                    node.node.Restraint.ZZ = zz;

                    DA.SetData(0, new GsaNodeGoo(node.Duplicate()));
                }
            }
        }
    }
}

