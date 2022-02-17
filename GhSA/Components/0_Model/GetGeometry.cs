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
using UnitsNet;

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
        public override void CreateAttributes()
        {
            if (first)
            {
                dropdownitems = new List<List<string>>();
                selecteditems = new List<string>();

                // length
                //dropdownitems.Add(Enum.GetNames(typeof(UnitsNet.Units.LengthUnit)).ToList());
                dropdownitems.Add(Units.FilteredLengthUnits);
                selecteditems.Add(lengthUnit.ToString());

                IQuantity quantity = new Length(0, lengthUnit);
                unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

                first = false;
            }
            m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
        }
        public void SetSelected(int i, int j)
        {
            // change selected item
            selecteditems[i] = dropdownitems[i][j];

            lengthUnit = (UnitsNet.Units.LengthUnit)Enum.Parse(typeof(UnitsNet.Units.LengthUnit), selecteditems[i]);

            // update name of inputs (to display unit on sliders)
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        private void UpdateUIFromSelectedItems()
        {
            lengthUnit = (UnitsNet.Units.LengthUnit)Enum.Parse(typeof(UnitsNet.Units.LengthUnit), selecteditems[0]);

            CreateAttributes();
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        // list of lists with all dropdown lists conctent
        List<List<string>> dropdownitems;
        // list of selected items
        List<string> selecteditems;
        // list of descriptions 
        List<string> spacerDescriptions = new List<string>(new string[]
        {
            "Unit"
        });
        private bool first = true;
        private UnitsNet.Units.LengthUnit lengthUnit = Units.LengthUnitGeometry;
        string unitAbbreviation;
        #region menu override
        private enum FoldMode
        {
            Graft,
            List
        }

        private FoldMode _mode = FoldMode.Graft;

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
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
        
        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
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

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            IQuantity length = new Length(0, lengthUnit);
            unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

            pManager.AddGenericParameter("Nodes [" + unitAbbreviation + "]", "No", "Nodes from GSA Model", GH_ParamAccess.list);
            pManager.HideParameter(0);
            pManager.AddGenericParameter("1D Elements [" + unitAbbreviation + "]", "E1D", "1D Elements (Analysis Layer) from GSA Model imported to selected unit", GH_ParamAccess.list);
            pManager.AddGenericParameter("2D Elements [" + unitAbbreviation + "]", "E2D", "2D Elements (Analysis Layer) from GSA Model imported to selected unit", GH_ParamAccess.list);
            pManager.AddGenericParameter("3D Elements [" + unitAbbreviation + "]", "E3D", "3D Elements (Analysis Layer) from GSA Model imported to selected unit", GH_ParamAccess.list);
            pManager.HideParameter(2);
            //pManager.HideParameter(3);
            pManager.AddGenericParameter("1D Members [" + unitAbbreviation + "]", "M1D", "1D Members (Design Layer) from GSA Model imported to selected unit", GH_ParamAccess.list);
            pManager.AddGenericParameter("2D Members [" + unitAbbreviation + "]", "M2D", "2D Members (Design Layer) from GSA Model imported to selected unit", GH_ParamAccess.list);
            pManager.AddGenericParameter("3D Members [" + unitAbbreviation + "]", "M3D", "3D Members (Design Layer) from GSA Model imported to selected unit", GH_ParamAccess.list);
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
                        results.Nodes = Util.Gsa.FromGSA.GetNodes(out_nDict, lengthUnit, axDict);
                        results.displaySupports = results.Nodes.AsParallel().Where(n => n.Value.isSupport).ToList();
                    }

                    if (i == 1)
                    {
                        eDict = new ConcurrentDictionary<int, Element>(model.Elements(elemList));

                        // create elements
                        Tuple<List<GsaElement1dGoo>, List<GsaElement2dGoo>, List<GsaElement3dGoo>> elementTuple
                            = Util.Gsa.FromGSA.GetElements(eDict, nDict, sDict, pDict, lengthUnit);

                        results.Elem1ds = elementTuple.Item1;
                        results.Elem2ds = elementTuple.Item2;
                        results.Elem3ds = elementTuple.Item3;
                    }

                    if (i == 2)
                    {
                        mDict = new ConcurrentDictionary<int, Member>(model.Members(memList));

                        // create members
                        Tuple<List<GsaMember1dGoo>, List<GsaMember2dGoo>, List<GsaMember3dGoo>> memberTuple
                            = Util.Gsa.FromGSA.GetMembers(mDict, nDict, lengthUnit, sDict, pDict);

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

                    ConcurrentBag<GsaElement2dGoo> element2dsShaded = new ConcurrentBag<GsaElement2dGoo>();
                    ConcurrentBag<GsaElement2dGoo> element2dsNotShaded = new ConcurrentBag<GsaElement2dGoo>();
                    Parallel.ForEach(element2ds, elem =>
                    {
                        if (elem.Value.API_Elements[0].ParentMember.Member > 0)
                            element2dsShaded.Add(elem);
                        else
                            element2dsNotShaded.Add(elem);
                    });
                    cachedDisplayMeshShaded = new Mesh();
                    cachedDisplayMeshShaded.Append(element2dsShaded.Select(e => e.Value.Mesh));
                    cachedDisplayMeshNotShaded = new Mesh();
                    cachedDisplayMeshNotShaded.Append(element2dsNotShaded.Select(e => e.Value.Mesh));
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
        Mesh cachedDisplayMeshShaded;
        Mesh cachedDisplayMeshNotShaded;
        //ConcurrentBag<GsaElement2dGoo> element2dsShaded;
        //ConcurrentBag<GsaElement2dGoo> element2dsNotShaded;
        List<GsaNodeGoo> supportNodes;

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            base.DrawViewportWires(args);

            //if (update & element2ds != null)
            //{
            //    element2dsShaded = new ConcurrentBag<GsaElement2dGoo>();
            //    element2dsNotShaded = new ConcurrentBag<GsaElement2dGoo>();
            //    Parallel.ForEach(element2ds, elem =>
            //    {
            //        if (elem.Value.API_Elements[0].ParentMember.Member > 0)
            //            element2dsShaded.Add(elem);
            //        else
            //            element2dsNotShaded.Add(elem);
            //    });
            //    cachedDisplayMeshShaded = new Mesh();
            //    cachedDisplayMeshShaded.Append(element2dsShaded.Select(e => e.Value.Mesh));
            //    cachedDisplayMeshNotShaded = new Mesh();
            //    cachedDisplayMeshNotShaded.Append(element2dsNotShaded.Select(e => e.Value.Mesh));
            //    update = false;
            //}

            if (cachedDisplayMeshShaded != null)
            {
                args.Display.DrawMeshWires(cachedDisplayMeshShaded, System.Drawing.Color.FromArgb(255, 229, 229, 229), 1);
            }
            //if (element2dsShaded != null)
            //{

            //    //foreach (GsaElement2dGoo element in element2dsShaded)
            //    //{
            //    //    if (element == null) { continue; }
            //    //    //Draw lines
            //    //    if (element.Value.Mesh != null)
            //    //    {
            //    //        args.Display.DrawMeshWires(element.Value.Mesh, System.Drawing.Color.FromArgb(255, 229, 229, 229), 1);
            //    //    }
            //    //}
            //}
            if (cachedDisplayMeshNotShaded != null)
            {
                if (this.Attributes.Selected)
                {
                    args.Display.DrawMeshWires(cachedDisplayMeshNotShaded, UI.Colour.Element2dEdgeSelected, 2);
                }
                else
                {
                    args.Display.DrawMeshWires(cachedDisplayMeshNotShaded, UI.Colour.Element2dEdge, 1);
                }
            }
            //if (element2dsNotShaded != null)
            //{
                
            //    //foreach (GsaElement2dGoo element in element2dsNotShaded)
            //    //{
            //    //    if (element == null) { continue; }
            //    //    //Draw lines
            //    //    if (element.Value.Mesh != null)
            //    //    {
            //    //        if (this.Attributes.Selected)
            //    //        {
            //    //            args.Display.DrawMeshWires(element.Value.Mesh, UI.Colour.Element2dEdgeSelected, 2);
            //    //        }
            //    //        else
            //    //        {
            //    //            args.Display.DrawMeshWires(element.Value.Mesh, UI.Colour.Element2dEdge, 1);
            //    //        }
            //    //    }
            //    //}
            //}

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
        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetInt32("Mode", (int)_mode);
            Util.GH.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            _mode = (FoldMode)reader.GetInt32("Mode");

            try // if users has an old versopm of this component then dropdown menu wont read
            {
                Util.GH.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);
            }
            catch (Exception) // we create the dropdown menu with our chosen default
            {
                dropdownitems = new List<List<string>>();
                selecteditems = new List<string>();

                // set length to meters as this was the only option for old components
                lengthUnit = UnitsNet.Units.LengthUnit.Meter;

                dropdownitems.Add(Units.FilteredLengthUnits);
                selecteditems.Add(lengthUnit.ToString());

                IQuantity quantity = new Length(0, lengthUnit);
                unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

                first = false;
            }

            UpdateUIFromSelectedItems();

            first = false;

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
            IQuantity length = new Length(0, lengthUnit);
            unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

            int i = 0;
            Params.Output[i++].Name = "Nodes [" + unitAbbreviation + "]";
            Params.Output[i++].Name = "1D Elements [" + unitAbbreviation + "]";
            Params.Output[i++].Name = "2D Elements [" + unitAbbreviation + "]";
            Params.Output[i++].Name = "3D Elements [" + unitAbbreviation + "]";
            Params.Output[i++].Name = "1D Members [" + unitAbbreviation + "]";
            Params.Output[i++].Name = "2D Members [" + unitAbbreviation + "]";
            Params.Output[i++].Name = "3D Members [" + unitAbbreviation + "]";
        }

        #endregion
    }
}

