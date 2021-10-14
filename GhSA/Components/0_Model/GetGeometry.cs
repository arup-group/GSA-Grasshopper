using System;
using System.Linq;
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
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace GhSA.Components
{
    /// <summary>
    /// Component to retrieve geometric objects from a GSA model
    /// </summary>
    public class GetGeometry : GH_TaskCapableComponent<GetGeometry.SolveResults>, IGH_PreviewObject, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("6c4cb686-a6d1-4a79-b01b-fadc5d6da520");
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
            pManager.AddTextParameter("Node filter list", "No", "Filter import by list." + System.Environment.NewLine +
                "Node list should take the form:" + System.Environment.NewLine +
                " 1 11 to 72 step 2 not (XY3 31 to 45)" + System.Environment.NewLine +
                "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
            pManager.AddTextParameter("Element filter list", "El", "Filter import by list." + System.Environment.NewLine +
                "Element list should take the form:" + System.Environment.NewLine +
                " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + System.Environment.NewLine +
                "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
            pManager.AddTextParameter("Member filter list", "Me", "Filter import by list." + System.Environment.NewLine +
                "Member list should take the form:" + System.Environment.NewLine +
                " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (Z4 XY55)" + System.Environment.NewLine +
                "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;

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
            //pManager.HideParameter(3);
            pManager.AddGenericParameter("1D Members", "M1D", "1D Members (Design Layer) from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("2D Members", "M2D", "2D Members (Design Layer) from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("3D Members", "M3D", "3D Members (Design Layer) from GSA Model", GH_ParamAccess.list);
        }
        #endregion

        public class SolveResults
        {
            public List<GsaNodeGoo> Nodes { get; set; }
            internal List<GsaNodeGoo> displaySupports { get; set; }
            public List<GsaElement1dGoo> Elem1ds { get; set; }
            public List<GsaElement2dGoo> Elem2ds { get; set; }
            public List<GsaElement3dGoo> Elem3ds { get; set; }
            public List<GsaMember1dGoo> Mem1ds { get; set; }
            public List<GsaMember2dGoo> Mem2ds { get; set; }
            public List<GsaMember3dGoo> Mem3ds { get; set; }
        }
        SolveResults Compute(GsaModel gsaModel, string nodeList, string elemList, string memList)
        {
            SolveResults results = new SolveResults();
            List<int> steps = new List<int> { 0, 1, 2 };

            ConcurrentDictionary<int, Axis> axDict;
            ConcurrentDictionary<int, Node> out_nDict;
            ConcurrentDictionary<int, Element> eDict;
            ConcurrentDictionary<int, Member> mDict;

            Model model = gsaModel.Model;

            // get dictionaries from model
            ConcurrentDictionary<int, Node> nDict = new ConcurrentDictionary<int, Node>(model.Nodes());
            ConcurrentDictionary<int, Section> sDict = new ConcurrentDictionary<int, Section>(model.Sections());
            ConcurrentDictionary<int, Prop2D> pDict = new ConcurrentDictionary<int, Prop2D>(model.Prop2Ds());
            try
            {

            
            Parallel.ForEach(steps, i =>
                {
                    if (i == 0)
                    {
                        axDict = new ConcurrentDictionary<int, Axis>(model.Axes());
                        out_nDict = (nodeList.ToLower() == "all") ? nDict : new ConcurrentDictionary<int, Node>(model.Nodes(nodeList));

                        // create nodes
                        results.Nodes = Util.Gsa.FromGSA.GetNodes(out_nDict, axDict);
                        results.displaySupports = results.Nodes.AsParallel().Where(n => n.Value.isSupport).ToList();
                    }

                    if (i == 1)
                    {
                        eDict = new ConcurrentDictionary<int, Element>(model.Elements(elemList));

                        // create elements
                        Tuple<List<GsaElement1dGoo>, List<GsaElement2dGoo>, List<GsaElement3dGoo>> elementTuple
                            = Util.Gsa.FromGSA.GetElements(eDict, nDict, sDict, pDict);

                        results.Elem1ds = elementTuple.Item1;
                        results.Elem2ds = elementTuple.Item2;
                        results.Elem3ds = elementTuple.Item3;
                    }

                    if (i == 2)
                    {
                        mDict = new ConcurrentDictionary<int, Member>(model.Members(memList));

                        // create members
                        Tuple<List<GsaMember1dGoo>, List<GsaMember2dGoo>, List<GsaMember3dGoo>> memberTuple
                            = Util.Gsa.FromGSA.GetMembers(mDict, nDict, sDict, pDict);

                        results.Mem1ds = memberTuple.Item1;
                        results.Mem2ds = memberTuple.Item2;
                        results.Mem3ds = memberTuple.Item3;
                    }

                });
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }
            return results;
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            if (InPreSolve)
            {
                // First pass; collect data and construct tasks
                GsaModel gsaModel = new GsaModel();
                GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
                Task<SolveResults> tsk = null;
                if (data.GetData(0, ref gh_typ))
                {
                    if (gh_typ.Value is GsaModelGoo)
                        gh_typ.CastTo(ref gsaModel);
                    else
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Model");
                        return;
                    }

                    //GsaModel in_Model = gsaModel.Clone();

                    // import lists
                    string nodeList = "all";
                    if (data.GetData(1, ref nodeList))
                        nodeList = nodeList.ToString();
                    string elemList = "all";
                    if (data.GetData(2, ref elemList))
                        elemList = elemList.ToString();
                    string memList = "all";
                    if (data.GetData(3, ref memList))
                        memList = memList.ToString();

                    tsk = Task.Run(() => Compute(gsaModel, nodeList, elemList, memList), CancelToken);
                }
                // Add a null task even if data collection fails. This keeps the
                // list size in sync with the iterations
                TaskList.Add(tsk);
                return;
            }
            SolveResults results;
            try
            {
            
            if (!GetSolveResults(data, out results))
            {
                // Compute right here, right now.
                // 1. Collect
                GsaModel gsaModel = new GsaModel();
                GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
                if (data.GetData(0, ref gh_typ))
                {
                    if (gh_typ.Value is GsaModelGoo)
                        gh_typ.CastTo(ref gsaModel);
                    else
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Model");
                        return;
                    }

                    // import lists
                    string nodeList = "all";
                    if (data.GetData(1, ref nodeList))
                        nodeList = nodeList.ToString();
                    string elemList = "all";
                    if (data.GetData(2, ref elemList))
                        elemList = elemList.ToString();
                    string memList = "all";
                    if (data.GetData(3, ref memList))
                        memList = memList.ToString();

                    // 2. Compute
                    results = Compute(gsaModel, nodeList, elemList, memList);
                }
                else return;
            }
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }

            // 3. Set
            if (results != null)
            {
                if (results.Nodes != null)
                {
                    data.SetDataList(0, results.Nodes);
                    supportNodes = results.displaySupports;
                }
                if (results.Elem1ds != null)
                {
                    data.SetDataList(1, results.Elem1ds);
                }
                if (results.Elem2ds != null)
                {
                    data.SetDataList(2, results.Elem2ds);
                    element2ds = results.Elem2ds;
                }
                if (results.Elem3ds != null)
                {
                    data.SetDataList(3, results.Elem3ds);
                }
                if (results.Mem1ds != null)
                {
                    data.SetDataList(4, results.Mem1ds);
                }
                if (results.Mem2ds != null)
                {
                    data.SetDataList(5, results.Mem2ds);
                }
                if (results.Mem3ds != null)
                {
                    data.SetDataList(6, results.Mem3ds);
                }
                update = true;
            }
        }

        bool update;
        List<GsaElement2dGoo> element2ds;
        ConcurrentBag<GsaElement2dGoo> element2dsShaded;
        ConcurrentBag<GsaElement2dGoo> element2dsNotShaded;
        List<GsaNodeGoo> supportNodes;

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            base.DrawViewportWires(args);

            if (update & element2ds != null)
            {
                element2dsShaded = new ConcurrentBag<GsaElement2dGoo>();
                element2dsNotShaded = new ConcurrentBag<GsaElement2dGoo>();
                Parallel.ForEach(element2ds, elem =>
                {
                    if (elem.Value.API_Elements[0].ParentMember.Member > 0)
                        element2dsShaded.Add(elem);
                    else
                        element2dsNotShaded.Add(elem);
                });
                update = false;
            }

            if (element2dsShaded != null)
            {
                foreach (GsaElement2dGoo element in element2dsShaded)
                {
                    if (element == null) { continue; }
                    //Draw lines
                    if (element.Value.Mesh != null)
                    {
                        args.Display.DrawMeshWires(element.Value.Mesh, System.Drawing.Color.FromArgb(255, 229, 229, 229), 1);
                    }
                }
            }
            if (element2dsNotShaded != null)
            {
                foreach (GsaElement2dGoo element in element2dsNotShaded)
                {
                    if (element == null) { continue; }
                    //Draw lines
                    if (element.Value.Mesh != null)
                    {
                        if (this.Attributes.Selected)
                        {
                            args.Display.DrawMeshWires(element.Value.Mesh, UI.Colour.Element2dEdgeSelected, 2);
                        }
                        else
                        {
                            args.Display.DrawMeshWires(element.Value.Mesh, UI.Colour.Element2dEdge, 1);
                        }
                    }
                }
            }

            if (supportNodes != null)
            {
                foreach (GsaNodeGoo node in supportNodes)
                if (node.Value.Point.IsValid)
                {
                        // draw the point
                    if (!this.Attributes.Selected)
                        {
                        if ((System.Drawing.Color)node.Value.Colour != System.Drawing.Color.FromArgb(0, 0, 0))
                        {
                            args.Display.DrawPoint(node.Value.Point, Rhino.Display.PointStyle.RoundSimple, 3, (System.Drawing.Color)node.Value.Colour);
                        }
                        else
                        {
                            System.Drawing.Color col = UI.Colour.Node;
                            args.Display.DrawPoint(node.Value.Point, Rhino.Display.PointStyle.RoundSimple, 3, col);
                        }
                        if (node.Value.previewSupportSymbol != null)
                            args.Display.DrawBrepShaded(node.Value.previewSupportSymbol, UI.Colour.SupportSymbol);
                        if (node.Value.previewText != null)
                            args.Display.Draw3dText(node.Value.previewText, UI.Colour.Support);
                    }
                    else
                    {
                        args.Display.DrawPoint(node.Value.Point, Rhino.Display.PointStyle.RoundControlPoint, 3, UI.Colour.NodeSelected);
                        if (node.Value.previewSupportSymbol != null)
                            args.Display.DrawBrepShaded(node.Value.previewSupportSymbol, UI.Colour.SupportSymbolSelected);
                        if (node.Value.previewText != null)
                            args.Display.Draw3dText(node.Value.previewText, UI.Colour.NodeSelected);
                    }

                    // local axis
                    if (node.Value.LocalAxis != Plane.WorldXY & node.Value.LocalAxis != new Plane() & node.Value.LocalAxis != Plane.Unset)
                    {
                        args.Display.DrawLine(node.Value.previewXaxis, System.Drawing.Color.FromArgb(255, 244, 96, 96), 1);
                        args.Display.DrawLine(node.Value.previewYaxis, System.Drawing.Color.FromArgb(255, 96, 244, 96), 1);
                        args.Display.DrawLine(node.Value.previewZaxis, System.Drawing.Color.FromArgb(255, 96, 96, 234), 1);
                    }
                }
            }
        }
    }
}

