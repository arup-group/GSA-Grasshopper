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
using Grasshopper.Kernel.Data;

namespace GhSA.Components
{
    /// <summary>
    /// Component to retrieve geometric objects from a GSA model
    /// </summary>
    public class GetGeometry : GH_Component, IGH_PreviewObject, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("7879a335-cdf3-4412-9a29-c710778430ff");
        public GetGeometry()
          : base("Get Model Geometry", "GetGeo", "Get nodes, elements and members from GSA model",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat0())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.GetGeometry;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        #region menu override
        private enum FoldMode
        {
            Graft,
            List
        }

        private FoldMode _mode = FoldMode.Graft;

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            Menu_AppendItem(menu, "Graft by Property", GraftModeClicked, true, _mode == FoldMode.Graft);
            Menu_AppendItem(menu, "List", ListModeClicked, true, _mode == FoldMode.List);
            
        }
        private void GraftModeClicked(object sender, EventArgs e)
        {
            if (_mode == FoldMode.Graft)
                return;

            RecordUndoEvent("Graft by Property");
            _mode = FoldMode.Graft;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            Message = "Graft by Property";
            ExpireSolution(true);
        }

        private void ListModeClicked(object sender, EventArgs e)
        {
            if (_mode == FoldMode.List)
                return;

            RecordUndoEvent("List");
            _mode = FoldMode.List;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            Message = "Import as List";
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

        bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
        {
            return false;
        }
        bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
        {
            return false;
        }
        IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
        {
            return null;
        }
        bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
        {
            return false;
        }

        void IGH_VariableParameterComponent.VariableParameterMaintenance()
        {
        }

        #endregion
        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA Model", "GSA", "GSA model containing some geometry", GH_ParamAccess.item);
            //pManager.AddTextParameter("Node filter list", "No", "Filter import by list." + System.Environment.NewLine +
            //    "Node list should take the form:" + System.Environment.NewLine +
            //    " 1 11 to 72 step 2 not (XY3 31 to 45)" + System.Environment.NewLine +
            //    "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
            //pManager.AddTextParameter("Element filter list", "El", "Filter import by list." + System.Environment.NewLine +
            //    "Element list should take the form:" + System.Environment.NewLine +
            //    " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + System.Environment.NewLine +
            //    "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
            //pManager.AddTextParameter("Member filter list", "Me", "Filter import by list." + System.Environment.NewLine +
            //    "Member list should take the form:" + System.Environment.NewLine +
            //    " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (Z4 XY55)" + System.Environment.NewLine +
            //    "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
            //pManager[1].Optional = true;
            //pManager[2].Optional = true;
            //pManager[3].Optional = true;

            //_mode = FoldMode.Graft;
            //Message = "Graft by Property" + System.Environment.NewLine + "Right-click to change";
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Nodes", "No", "Nodes from GSA Model", GH_ParamAccess.list);
            pManager.HideParameter(0);
            pManager.AddGenericParameter("1D Elements", "E1D", "1D Elements (Analysis Layer) from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("2D Elements", "E2D", "2D Elements (Analysis Layer) from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("3D Elements", "E3D", "3D Elements (Analysis Layer) from GSA Model", GH_ParamAccess.list);
            pManager.HideParameter(2);
            pManager.HideParameter(3);
            pManager.AddGenericParameter("1D Members", "M1D", "1D Members (Design Layer) from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("2D Members", "M2D", "2D Members (Design Layer) from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("3D Members", "M3D", "3D Members (Design Layer) from GSA Model", GH_ParamAccess.list);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaModel gsaModel = new GsaModel();
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(0, ref gh_typ))
            {
                if (gh_typ.Value is GsaModelGoo)
                    gh_typ.CastTo(ref gsaModel);
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Model");
                    return;
                }
            
                // import lists
                //string nodeList = "all";
                //if (DA.GetData(1, ref nodeList))
                //    nodeList = nodeList.ToString();
                //string elemList = "all";
                //if (DA.GetData(2, ref elemList))
                //    elemList = elemList.ToString();
                //string memList = "all";
                //if (DA.GetData(3, ref memList))
                //    memList = memList.ToString();

                Model model = gsaModel.Model;

                //bool graft = false;
                //if (_mode == FoldMode.Graft)
                //    graft = true;

                // get dictionaries from model
                IReadOnlyDictionary<int, Node> nDict = model.Nodes();
                IReadOnlyDictionary<int, Element> eDict = model.Elements();
                IReadOnlyDictionary<int, Member> mDict = model.Members();
                IReadOnlyDictionary<int, Section> sDict = model.Sections();
                IReadOnlyDictionary<int, Prop2D> pDict = model.Prop2Ds();

                // create nodes
                List<GsaNodeGoo> nodes = Util.Gsa.FromGSA.GetNodes(nDict, model);
                // create elements
                Tuple<List<GsaElement1dGoo>, List<GsaElement2dGoo>> elementTuple
                    = Util.Gsa.FromGSA.GetElements(eDict, nDict, sDict, pDict);
                // create members
                Tuple<List<GsaMember1dGoo>, List<GsaMember2dGoo>, List<GsaMember3dGoo>> memberTuple
                    = Util.Gsa.FromGSA.GetMembers(mDict, nDict, sDict, pDict);

                DA.SetDataList(0, nodes);

                DA.SetDataList(1, elementTuple.Item1);
                DA.SetDataList(2, elementTuple.Item2);

                DA.SetDataList(4, memberTuple.Item1);
                DA.SetDataList(5, memberTuple.Item2);
                DA.SetDataList(6, memberTuple.Item3);

                element2ds = elementTuple.Item2;
            }
        }

        List<GsaElement2dGoo> element2ds;
        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            
            base.DrawViewportMeshes(args);

            if (element2ds != null)
            {
                foreach (GsaElement2dGoo element in element2ds)
                {
                    if (element == null) { continue; }
                    //Draw shape.
                    if (element.Value.Mesh != null)
                    {
                        if (!(element.Value.Elements[0].ParentMember.Member > 0)) // only draw mesh shading if no parent member exist.
                        {
                            if (this.Attributes.Selected)
                                args.Display.DrawMeshShaded(element.Value.Mesh, UI.Colour.Element2dFaceSelected);
                            else
                                args.Display.DrawMeshShaded(element.Value.Mesh, UI.Colour.Element2dFace);
                        }
                    }
                }
            }
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            base.DrawViewportWires(args);

            if (element2ds != null)
            {
                foreach (GsaElement2dGoo element in element2ds)
                {
                    if (element == null) { continue; }
                    //Draw lines
                    if (element.Value.Mesh != null)
                    {
                        if (element.Value.Elements[0].ParentMember.Member > 0) // only draw mesh shading if no parent member exist.
                        {
                            for (int i = 0; i < element.Value.Mesh.TopologyEdges.Count; i++)
                            {
                                if (element.Value.Mesh.TopologyEdges.GetConnectedFaces(i).Length > 1)
                                    args.Display.DrawLine(element.Value.Mesh.TopologyEdges.EdgeLine(i), System.Drawing.Color.FromArgb(255, 229, 229, 229), 1);
                            }
                        }
                        else
                        {
                            if (this.Attributes.Selected)
                            {
                                for (int i = 0; i < element.Value.Mesh.TopologyEdges.Count; i++)
                                    args.Display.DrawLine(element.Value.Mesh.TopologyEdges.EdgeLine(i), UI.Colour.Element2dEdgeSelected, 2);
                            }
                            else
                            {
                                for (int i = 0; i < element.Value.Mesh.TopologyEdges.Count; i++)
                                    args.Display.DrawLine(element.Value.Mesh.TopologyEdges.EdgeLine(i), UI.Colour.Element2dEdge, 1);
                            }
                        }
                    }
                }
            }
        }
    }
}

