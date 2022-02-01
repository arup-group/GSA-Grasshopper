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
using Grasshopper.Kernel.Parameters;

namespace GhSA.Components
{
    /// <summary>
    /// Component to edit a Node
    /// </summary>
    public class EditNode : GH_Component, IGH_PreviewObject, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("de176ec0-0516-4634-8f04-82017e502e1e");
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

        protected override void RegisterInputParams(GH_InputParamManager pManager)
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

            pManager.HideParameter(0);
            pManager.HideParameter(2);
            pManager.HideParameter(3);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Node", "No", "Modified GSA Node", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Node number", "ID", "Original Node number (ID) if Node ever belonged to a GSA Model", GH_ParamAccess.item);
            pManager.AddPointParameter("Node Position", "Pt", "Position (x, y, z) of Node. Setting a new position will clear any existing ID", GH_ParamAccess.item);
            pManager.HideParameter(2);
            pManager.AddPlaneParameter("Node local axis", "Pl", "Local axis (Plane) of Node", GH_ParamAccess.item);
            pManager.HideParameter(3);
            pManager.AddGenericParameter("Node Restraints", "B6", "Restraints (Bool6) of Node", GH_ParamAccess.item);
            pManager.AddGenericParameter("Node Spring", "PS", "Spring attached to Node", GH_ParamAccess.item);
            pManager.AddTextParameter("Node Name", "Na", "Name of Node", GH_ParamAccess.item);
            pManager.AddColourParameter("Node Colour", "Co", "Get colour of node", GH_ParamAccess.item);
            if (_mode == FoldMode.GetConnected)
            {
                pManager.AddIntegerParameter("Connected Elements", "El", "Connected Element IDs in Model that Node once belonged to", GH_ParamAccess.list);
                pManager.AddIntegerParameter("Connected Members", "Me", "Connected Member IDs in Model that Node once belonged to", GH_ParamAccess.list);
            }
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(0, ref gh_typ))
            {
                GsaNode node = new GsaNode();
                Point3d tempPt = new Point3d();
                if (gh_typ.Value is GsaNodeGoo)
                {
                    gh_typ.CastTo(ref node);
                    if (node == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Node input is null"); }
                    if (node.API_Node == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Node input is null"); }
                }
                else if (GH_Convert.ToPoint3d(gh_typ.Value, ref tempPt, GH_Conversion.Both))
                {
                    node.Point = tempPt;
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert input to Node");
                    return;
                }

                // #### inputs ####
                // 2 Point
                GH_Point ghPt = new GH_Point();
                if (DA.GetData(2, ref ghPt))
                {
                    Point3d pt = new Point3d();
                    if (GH_Convert.ToPoint3d(ghPt, ref pt, GH_Conversion.Both))
                    {
                        node.Point = pt;
                    }
                }

                // 1 ID (do ID after point, as setting point will clear the Node.ID value
                GH_Integer ghInt = new GH_Integer();
                if (DA.GetData(1, ref ghInt))
                {
                    if (GH_Convert.ToInt32(ghInt, out int id, GH_Conversion.Both))
                        node.ID = id;
                }

                // 3 plane
                GH_Plane ghPln = new GH_Plane();
                if (DA.GetData(3, ref ghPln))
                {
                    Plane pln = new Plane();
                    if (GH_Convert.ToPlane(ghPln, ref pln, GH_Conversion.Both))
                    {
                        pln.Origin = node.Point;
                        node.LocalAxis = pln;
                    }
                }

                // 4 Restraint
                GsaBool6 restraint = new GsaBool6();
                if (DA.GetData(4, ref restraint))
                {
                    node.Restraint = restraint;
                }

                // 5 Spring
                GsaSpring spring = new GsaSpring();
                if (DA.GetData(5, ref spring))
                {
                    if (spring != null)
                        node.Spring = spring;
                }

                // 6 Name
                GH_String ghStr = new GH_String();
                if (DA.GetData(6, ref ghStr))
                {
                    if (GH_Convert.ToString(ghStr, out string name, GH_Conversion.Both))
                        node.Name = name;
                }

                // 7 Colour
                GH_Colour ghcol = new GH_Colour();
                if (DA.GetData(7, ref ghcol))
                {
                    if (GH_Convert.ToColor(ghcol, out System.Drawing.Color col, GH_Conversion.Both))
                        node.Colour = col;
                }
                
                // #### outputs ####
                DA.SetData(0, new GsaNodeGoo(node));
                DA.SetData(1, node.ID);
                DA.SetData(2, node.Point);
                DA.SetData(3, new GH_Plane(node.LocalAxis));
                DA.SetData(4, node.Restraint);
                DA.SetData(5, new GsaSpringGoo(node.Spring));
                DA.SetData(6, node.API_Node.Name);
                DA.SetData(7, node.Colour);

                // only get connected elements/members if enabled (computationally heavy)
                if (_mode == FoldMode.GetConnected)
                {
                    try { DA.SetDataList(8, node.API_Node.ConnectedElements); } catch (Exception) { }
                    try { DA.SetDataList(9, node.API_Node.ConnectedMembers); } catch (Exception) { }
                }
            }
        }

        #region menu override
        private enum FoldMode
        {
            GetConnected,
            DoNotGetConnected
        }

        private FoldMode _mode = FoldMode.DoNotGetConnected;

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendItem(menu, "Try get connected Element & Members", FlipMode, true, _mode == FoldMode.GetConnected);
        }
        private void FlipMode(object sender, EventArgs e)
        {
            RecordUndoEvent("GetConnected Parameters");
            if (_mode == FoldMode.GetConnected)
            {
                //flip mode
                _mode = FoldMode.DoNotGetConnected;
                
                //remove input parameters
                while (Params.Output.Count > 8)
                    Params.UnregisterOutputParameter(Params.Output[8], true);
            }
            else
            {
                // flip mode
                _mode = FoldMode.GetConnected;

                // add output parameters
                Params.RegisterOutputParam(new Param_Integer());
                Params.RegisterOutputParam(new Param_Integer());
                (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            }
            
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        
        #endregion

        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetInt32("Mode", (int)_mode);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            _mode = (FoldMode)reader.GetInt32("Mode");
            return base.Read(reader);
        }
        #endregion

        public void VariableParameterMaintenance()
        {
            if (_mode == FoldMode.GetConnected)
            {
                Params.Output[8].NickName = "El";
                Params.Output[8].Name = "Connected Elements";
                Params.Output[8].Description = "Connected Element IDs in Model that Node once belonged to";
                Params.Output[8].Access = GH_ParamAccess.list;

                Params.Output[9].NickName = "Me";
                Params.Output[9].Name = "Connected Members";
                Params.Output[9].Description = "Connected Member IDs in Model that Node once belonged to";
                Params.Output[9].Access = GH_ParamAccess.list;
            }
        }

        #region IGH_variable parameter null implementation
        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            throw new NotImplementedException();
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            throw new NotImplementedException();
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            throw new NotImplementedException();
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

