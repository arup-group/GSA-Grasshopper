﻿using System;
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
    /// Component to edit a Node
    /// </summary>
    public class EditNode : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("26c324cb-6f2e-4fdb-8539-e7d5cc89b2fc");
        public EditNode()
          : base("Edit Node", "NodeEdit", "Modify GSA Node",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat2())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.EditNode;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout


        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            
            pManager.AddGenericParameter("Node", "No", "GSA Node to Edit. If no input a new node will be created.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Node number", "ID", "Set Node number (ID) - if Node ID is set it will replace any existing nodes in the model", GH_ParamAccess.item);
            pManager.AddPointParameter("Node Position", "Pt", "Set new Position (x, y, z) of Node", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Node local axis", "Pl", "Set Local axis (Plane) of Node", GH_ParamAccess.item);
            pManager.AddGenericParameter("Node Restraints", "B6", "Set Restraints (Bool6) of Node", GH_ParamAccess.item);
            pManager.AddGenericParameter("Node Spring", "PS", "Set Spring (Type: General)", GH_ParamAccess.item);
            pManager.AddTextParameter("Node Name", "Na", "Set Name of Node", GH_ParamAccess.item);
            pManager.AddColourParameter("Node Colour", "Co", "Set colour of node", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Node", "No", "Modified GSA Node", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Node number", "ID", "Original Node number (ID) if Node ever belonged to a GSA Model", GH_ParamAccess.item);
            pManager.AddPointParameter("Node Position", "Pt", "Position (x, y, z) of Node", GH_ParamAccess.item);
            pManager.HideParameter(2);
            pManager.AddPlaneParameter("Node local axis", "Pl", "Local axis (Plane) of Node", GH_ParamAccess.item);
            pManager.HideParameter(3);
            pManager.AddGenericParameter("Node Restraints", "B6", "Restraints (Bool6) of Node", GH_ParamAccess.item);
            pManager.AddGenericParameter("Node Spring", "PS", "Spring attached to Node", GH_ParamAccess.item);
            pManager.AddTextParameter("Node Name", "Na", "Name of Node", GH_ParamAccess.item);
            pManager.AddColourParameter("Node Colour", "Co", "Get colour of node", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Connected Elements", "El", "Connected Element IDs in Model that Node once belonged to", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Connected Members", "Me", "Connected Member IDs in Model that Node once belonged to", GH_ParamAccess.list);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaNode gsaNode = new GsaNode();
            DA.GetData(0, ref gsaNode);

            if (gsaNode != null)
            {
                // #### inputs ####
                
                // 1 ID
                GH_Integer ghInt = new GH_Integer();
                if (DA.GetData(1, ref ghInt))
                {
                    if (GH_Convert.ToInt32(ghInt, out int id, GH_Conversion.Both))
                        gsaNode.ID = id;
                }

                // 2 Point
                GH_Point ghPt = new GH_Point();
                if (DA.GetData(2, ref ghPt))
                {
                    Point3d pt = new Point3d();
                    if (GH_Convert.ToPoint3d(ghPt, ref pt, GH_Conversion.Both))
                    {
                        gsaNode.Point = pt;
                        gsaNode.Node.Position.X = pt.X;
                        gsaNode.Node.Position.Y = pt.Y;
                        gsaNode.Node.Position.Z = pt.Z;
                    }
                }

                // 3 plane
                GH_Plane ghPln = new GH_Plane();
                if (DA.GetData(3, ref ghPln))
                {
                    Plane pln = new Plane();
                    if (GH_Convert.ToPlane(ghPln, ref pln, GH_Conversion.Both))
                    {
                        pln.Origin = gsaNode.Point;
                        gsaNode.LocalAxis = pln;
                    }
                }

                // 4 Restraint
                GsaBool6 restraint = new GsaBool6();
                if (DA.GetData(4, ref restraint))
                {
                    gsaNode.Node.Restraint.X = restraint.X;
                    gsaNode.Node.Restraint.Y = restraint.Y;
                    gsaNode.Node.Restraint.Z = restraint.Z;
                    gsaNode.Node.Restraint.XX = restraint.XX;
                    gsaNode.Node.Restraint.YY = restraint.YY;
                    gsaNode.Node.Restraint.ZZ = restraint.ZZ;
                }

                // 5 Spring
                GsaSpring spring = new GsaSpring();
                if (DA.GetData(5, ref spring))
                {
                    if (spring != null)
                        gsaNode.Spring = spring;
                }

                // 6 Name
                GH_String ghStr = new GH_String();
                if (DA.GetData(6, ref ghStr))
                {
                    if (GH_Convert.ToString(ghStr, out string name, GH_Conversion.Both))
                        gsaNode.Node.Name = name;
                }

                // 7 Colour
                GH_Colour ghcol = new GH_Colour();
                if (DA.GetData(7, ref ghcol))
                {
                    if (GH_Convert.ToColor(ghcol, out System.Drawing.Color col, GH_Conversion.Both))
                        gsaNode.Colour = col;
                }

                // #### outputs ####
                DA.SetData(0, new GsaNodeGoo(gsaNode));
                DA.SetData(1, gsaNode.ID);
                DA.SetData(2, gsaNode.Point);
                DA.SetData(3, gsaNode.LocalAxis);
                GsaBool6 restraint1 = new GsaBool6
                {
                    X = gsaNode.Node.Restraint.X,
                    Y = gsaNode.Node.Restraint.Y,
                    Z = gsaNode.Node.Restraint.Z,
                    XX = gsaNode.Node.Restraint.XX,
                    YY = gsaNode.Node.Restraint.YY,
                    ZZ = gsaNode.Node.Restraint.ZZ
                };
                DA.SetData(4, restraint1);
                GsaSpring spring1 = new GsaSpring();
                if (gsaNode.Spring != null)
                {
                    spring1 = gsaNode.Spring.Duplicate();
                }
                DA.SetData(5, new GsaSpringGoo(spring1));
                DA.SetData(6, gsaNode.Node.Name);
                DA.SetData(7, gsaNode.Colour);
                try { DA.SetDataList(8, gsaNode.Node.ConnectedElements); } catch (Exception) { }
                try { DA.SetDataList(9, gsaNode.Node.ConnectedMembers); } catch (Exception) { }
                
            }
            else { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in Node input"); }
        }
    }
}

