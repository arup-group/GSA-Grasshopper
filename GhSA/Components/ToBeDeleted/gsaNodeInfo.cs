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
    public class gsaNodeInfo : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("f9d5f2f4-0529-4173-8ae7-20d0d95952aa");
        public gsaNodeInfo()
          : base("GSA Node Info", "NodeInfo", "Get GSA Node Info",
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
            pManager.AddGenericParameter("GSA Node", "Node", "GSA Node to get a bit more info out of", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA Node", "Node", "GSA Node", GH_ParamAccess.item);
            pManager.HideParameter(0);
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
            if (DA.GetData(0, ref gsaNode))
            {
                if (gsaNode != null)
                {
                    DA.SetData(0, new GsaNodeGoo(gsaNode));

                    DA.SetData(1, gsaNode.ID);
                    DA.SetData(2, gsaNode.node.Name);

                    DA.SetData(3, gsaNode.point);

                    DA.SetData(4, gsaNode.localAxis);

                    GsaBool6 restraint = new GsaBool6();
                    restraint.X = gsaNode.node.Restraint.X;
                    restraint.Y = gsaNode.node.Restraint.Y;
                    restraint.Z = gsaNode.node.Restraint.Z;
                    restraint.XX = gsaNode.node.Restraint.XX;
                    restraint.YY = gsaNode.node.Restraint.YY;
                    restraint.ZZ = gsaNode.node.Restraint.ZZ;
                    DA.SetData(5, restraint);

                    if (gsaNode.Spring != null)
                    {
                        GsaSpring spring = new GsaSpring();
                        spring = gsaNode.Spring.Duplicate();
                    }
                    DA.SetData(6, gsaNode.Spring);

                    try { DA.SetDataList(7, gsaNode.node.ConnectedElements);} catch (Exception) {}

                    try { DA.SetDataList(8, gsaNode.node.ConnectedMembers);} catch (Exception) {}
                }
            }
        }
    }
}

