using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using System.Collections.Concurrent;
using UnitsNet;
using System.Linq;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to edit a Node
    /// </summary>
    public class ElemFromMem : GH_Component, IGH_PreviewObject, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("3de73a08-b72c-45e4-a650-e4c6515266c5");
        public ElemFromMem()
          : base("Elements from Members", "ElemFromMem", "Create Elements from Members",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat2())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateElemsFromMems;
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
            ExpireSolution(true);
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
        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            IQuantity length = new Length(0, lengthUnit);
            unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

            pManager.AddGenericParameter("Nodes [" + unitAbbreviation + "]", "No", "Nodes to be included in meshing", GH_ParamAccess.list);
            pManager.AddGenericParameter("1D Members [" + unitAbbreviation + "]", "M1D", "1D Members to create 1D Elements from", GH_ParamAccess.list);
            pManager.AddGenericParameter("2D Members [" + unitAbbreviation + "]", "M2D", "2D Members to create 2D Elements from", GH_ParamAccess.list);
            pManager.AddGenericParameter("3D Members [" + unitAbbreviation + "]", "M3D", "3D Members to create 3D Elements from", GH_ParamAccess.list);

            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;

            pManager.HideParameter(0);
            pManager.HideParameter(1);
            pManager.HideParameter(2);
            pManager.HideParameter(3);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Nodes", "No", "GSA Nodes", GH_ParamAccess.list);
            pManager.HideParameter(0);
            pManager.AddGenericParameter("1D Elements", "E1D", "GSA 1D Elements", GH_ParamAccess.list);
            pManager.AddGenericParameter("2D Elements", "E2D", "GSA 2D Elements", GH_ParamAccess.list);
            pManager.AddGenericParameter("3D Elements", "E3D", "GSA 3D Elements", GH_ParamAccess.item);
            pManager.AddGenericParameter("GSA Model", "GSA", "GSA Model with Elements and Members", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            #region inputs
            // Get Member1d input
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();

            List<GsaNode> in_nodes = new List<GsaNode>();
            if (DA.GetDataList(0, gh_types))
            {
                for (int i = 0; i < gh_types.Count; i++)
                {
                    gh_typ = gh_types[i];
                    if (gh_typ == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Node input (index: "+ i +") is null and has been ignored"); continue; }

                    if (gh_typ.Value is GsaNodeGoo)
                    {
                        GsaNode gsanode = new GsaNode();
                        gh_typ.CastTo(ref gsanode);
                        in_nodes.Add(gsanode);
                    }
                    else
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in Node input");
                        return;
                    }
                }
            }

            List<GsaMember1d> in_mem1ds = new List<GsaMember1d>();
            if (DA.GetDataList(1, gh_types))
            {
                for (int i = 0; i < gh_types.Count; i++)
                {
                    gh_typ = gh_types[i];
                    if (gh_typ == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Member1D input (index: " + i + ") is null and has been ignored"); continue; }

                    if (gh_typ.Value is GsaMember1dGoo)
                    {
                        GsaMember1d gsamem1 = new GsaMember1d();
                        gh_typ.CastTo(ref gsamem1);
                        in_mem1ds.Add(gsamem1);
                    }
                    else
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in Mem1D input");
                        return;
                    }
                }
            }

            // Get Member2d input
            gh_types = new List<GH_ObjectWrapper>();
            List<GsaMember2d> in_mem2ds = new List<GsaMember2d>();
            if (DA.GetDataList(2, gh_types))
            {
                for (int i = 0; i < gh_types.Count; i++)
                {
                    gh_typ = gh_types[i];
                    if (gh_typ == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Member2D input (index: " + i + ") is null and has been ignored"); continue; }

                    if (gh_typ.Value is GsaMember2dGoo)
                    {
                        GsaMember2d gsamem2 = new GsaMember2d();
                        gh_typ.CastTo(ref gsamem2);
                        in_mem2ds.Add(gsamem2);
                    }
                    else
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in Mem2D input");
                        return;
                    }
                }
            }

            // Get Member3d input
            gh_types = new List<GH_ObjectWrapper>();
            List<GsaMember3d> in_mem3ds = new List<GsaMember3d>();
            if (DA.GetDataList(3, gh_types))
            {
                for (int i = 0; i < gh_types.Count; i++)
                {
                    gh_typ = gh_types[i];
                    if (gh_typ == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Member3D input (index: " + i + ") is null and has been ignored"); continue; }

                    if (gh_typ.Value is GsaMember3dGoo)
                    {
                        GsaMember3d gsamem3 = new GsaMember3d();
                        gh_typ.CastTo(ref gsamem3);
                        in_mem3ds.Add(gsamem3);
                    }
                    else
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in Mem3D input");
                        return;
                    }
                }
            }

            // manually add a warning if no input is set, as all three inputs are optional
            if (in_mem1ds.Count < 1 & in_mem2ds.Count < 1 & in_mem3ds.Count < 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameters failed to collect data");
                return;
            }
            #endregion

            // Assemble model
            Model gsa = Util.Gsa.ToGSA.Assemble.AssembleModel(null, in_nodes, null, null, null, in_mem1ds, in_mem2ds, in_mem3ds, null, null, null, null, null, null, null, lengthUnit);

            #region meshing
            // Create elements from members
            gsa.CreateElementsFromMembers();
            #endregion

            // extract nodes from model
            ConcurrentBag<GsaNodeGoo> nodes = Util.Gsa.FromGSA.GetNodes(new ConcurrentDictionary<int, Node>(gsa.Nodes()), lengthUnit);

            // extract elements from model
            Tuple<ConcurrentBag<GsaElement1dGoo>, ConcurrentBag<GsaElement2dGoo>, ConcurrentBag<GsaElement3dGoo>> elementTuple
                = Util.Gsa.FromGSA.GetElements(
                    new ConcurrentDictionary<int, Element>(gsa.Elements()), 
                    new ConcurrentDictionary<int, Node>(gsa.Nodes()),
                    new ConcurrentDictionary<int, Section>(gsa.Sections()),
                    new ConcurrentDictionary<int, Prop2D>(gsa.Prop2Ds()),
                    new ConcurrentDictionary<int, Prop3D>(gsa.Prop3Ds()),
                    new ConcurrentDictionary<int, AnalysisMaterial>(gsa.AnalysisMaterials()),
                    lengthUnit);

            // expose internal model if anyone wants to use it
            GsaModel outModel = new GsaModel();
            outModel.Model = gsa;

            DA.SetDataList(0, nodes.OrderBy(item => item.Value.ID));
            DA.SetDataList(1, elementTuple.Item1.OrderBy(item => item.Value.ID));
            DA.SetDataList(2, elementTuple.Item2.OrderBy(item => item.Value.ID.First()));
            DA.SetDataList(3, elementTuple.Item3.OrderBy(item => item.Value.ID.First()));
            DA.SetData(4, new GsaModelGoo(outModel));
            
            // custom display settings for element2d mesh
            element2ds = elementTuple.Item2;
        }
        ConcurrentBag<GsaElement2dGoo> element2ds;
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
                        if (!(element.Value.API_Elements[0].ParentMember.Member > 0)) // only draw mesh shading if no parent member exist.
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
                        if (element.Value.API_Elements[0].ParentMember.Member > 0) // only draw mesh shading if no parent member exist.
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
        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            Util.GH.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
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
        #endregion

        #region IGH_VariableParameterComponent null implementation
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
            Params.Input[i++].Name = "Nodes [" + unitAbbreviation + "]";
            Params.Input[i++].Name = "1D Members [" + unitAbbreviation + "]";
            Params.Input[i++].Name = "2D Members [" + unitAbbreviation + "]";
            Params.Input[i++].Name = "3D Members [" + unitAbbreviation + "]";

        }
        #endregion  
    }
}

