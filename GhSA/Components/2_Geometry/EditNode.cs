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
    public class gsaNodeEdit : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("26c324cb-6f2e-4fdb-8539-e7d5cc89b2fc");
        public gsaNodeEdit()
          : base("Edit Node", "NodeEdit", "Modify GSA Node",
                Ribbon.CategoryName.name(),
                Ribbon.SubCategoryName.cat2())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        //protected override Bitmap Icon => Resources.CrossSections;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        

        #endregion

        #region Input and output
        
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            
            pManager.AddGenericParameter("Node", "No", "GSA Node to Edit. If no input a new node will be created.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Node number", "ID", "If Node ID is set it will replace any existing nodes in the model", GH_ParamAccess.item);
            pManager.AddTextParameter("Node Name", "Na", "Name of Node", GH_ParamAccess.item);
            pManager.AddPointParameter("Node Position", "Pt", "Position (x, y, z) of Node", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Node local axis", "Pl", "Local axis (Plane) of Node", GH_ParamAccess.item);
            pManager.AddGenericParameter("Node Restraints", "B6", "Restraints (Bool6) of Node", GH_ParamAccess.item);
            pManager.AddGenericParameter("Node Spring", "PS", "Local axis (Plane) of Node", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Node", "No", "Modified GSA Node", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Node number", "ID", "Original Node number (ID) if Node ever belonged to a GSA Model", GH_ParamAccess.item);
            pManager.AddTextParameter("Node Name", "Na", "Name of Node", GH_ParamAccess.item);
            pManager.AddPointParameter("Node Position", "Pt", "Position (x, y, z) of Node", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Node local axis", "Pl", "Local axis (Plane) of Node", GH_ParamAccess.item);
            pManager.HideParameter(4);
            pManager.AddGenericParameter("Node Restraints", "Re", "Restraints (Bool6) of Node", GH_ParamAccess.item);
            pManager.AddGenericParameter("Node Spring", "Sp", "Local axis (Plane) of Node", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Connected Elements", "El", "Connected Element IDs in Model that Node once belonged to", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Connected Members", "Mem", "Connected Member IDs in Model that Node once belonged to", GH_ParamAccess.list);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaNode gsaNode = new GsaNode();
            if (!DA.GetData(0, ref gsaNode))
                gsaNode = new GsaNode(new Point3d(0, 0, 0));

            if (gsaNode != null)
            {
                // #### inputs ####
                
                GH_Integer ghInt = new GH_Integer();
                if (DA.GetData(1, ref ghInt))
                {
                    int id = new int();
                    if (GH_Convert.ToInt32(ghInt, out id, GH_Conversion.Both))
                        gsaNode.ID = id;
                }

                GH_String ghStr = new GH_String();
                if (DA.GetData(2, ref ghStr))
                {
                    string name = "";
                    if (GH_Convert.ToString(ghStr, out name, GH_Conversion.Both))
                        gsaNode.node.Name = name;
                }

                GH_Point ghPt = new GH_Point();
                if (DA.GetData(3, ref ghPt))
                {
                    Point3d pt = new Point3d();
                    if (GH_Convert.ToPoint3d(ghPt, ref pt, GH_Conversion.Both))
                    {
                        gsaNode.point = pt;
                        gsaNode.node.Position.X = pt.X;
                        gsaNode.node.Position.Y = pt.Y;
                        gsaNode.node.Position.Z = pt.Z;
                    }
                }

                GH_Plane ghPln = new GH_Plane();
                if (DA.GetData(4, ref ghPln))
                {
                    Plane pln = new Plane();
                    if (GH_Convert.ToPlane(ghPln, ref pln, GH_Conversion.Both))
                    {
                        pln.Origin = gsaNode.point;
                        gsaNode.localAxis = pln;
                    }
                }

                GsaBool6 restraint = new GsaBool6();
                if (DA.GetData(5, ref restraint))
                {
                    restraint.X = gsaNode.node.Restraint.X;
                    restraint.Y = gsaNode.node.Restraint.Y;
                    restraint.Z = gsaNode.node.Restraint.Z;
                    restraint.XX = gsaNode.node.Restraint.XX;
                    restraint.YY = gsaNode.node.Restraint.YY;
                    restraint.ZZ = gsaNode.node.Restraint.ZZ;
                }

                GsaSpring spring = new GsaSpring();
                if (DA.GetData(6, ref spring))
                {
                    if (gsaNode.Spring != null)
                        gsaNode.Spring = spring;
                }


                // #### outputs ####
                DA.SetData(0, new GsaNodeGoo(gsaNode));

                DA.SetData(1, gsaNode.ID);
                DA.SetData(2, gsaNode.node.Name);

                DA.SetData(3, gsaNode.point);

                DA.SetData(4, gsaNode.localAxis);

                GsaBool6 restraint1 = new GsaBool6();
                restraint1.X = gsaNode.node.Restraint.X;
                restraint1.Y = gsaNode.node.Restraint.Y;
                restraint1.Z = gsaNode.node.Restraint.Z;
                restraint1.XX = gsaNode.node.Restraint.XX;
                restraint1.YY = gsaNode.node.Restraint.YY;
                restraint1.ZZ = gsaNode.node.Restraint.ZZ;
                DA.SetData(5, restraint1);

                if (gsaNode.Spring != null)
                {
                    GsaSpring spring1 = new GsaSpring();
                    spring1 = gsaNode.Spring.Duplicate();
                }
                DA.SetData(6, gsaNode.Spring);

                try { DA.SetDataList(7, gsaNode.node.ConnectedElements); } catch (Exception) { }

                try { DA.SetDataList(8, gsaNode.node.ConnectedMembers); } catch (Exception) { }

            }
        }
    }
}

