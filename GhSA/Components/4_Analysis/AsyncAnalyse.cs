using System;
using System.Linq;
using System.Collections.ObjectModel;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
using GsaAPI;
using Rhino.Geometry;
using GhSA.Parameters;
using GrasshopperAsyncComponent;
using System.Collections.Generic;

namespace GhSA.Components
{
    public class Analyse : GH_AsyncComponent
    {
        #region Name and Ribbon Layout
        public Analyse()
            : base("Analyse Model", "Analyse", "Assemble and Analyse a GSA Model",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat4())
        { BaseWorker = new AnalysisWorker(); }
        public override Guid ComponentGuid => new Guid("b9ca86f7-fda1-4c5e-ae75-5e570d4885e9");
        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => GSA.Properties.Resources.AnalysisTask;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        #endregion

        #region input and output
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Model", "GSA", "(Optional) Existing Model to append to", GH_ParamAccess.item);
            pManager.AddGenericParameter("Nodes", "Nodes", "Nodes to add/set in Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("1D Elements", "Elem1D", "1D Elements to add/set in Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("2D Elements", "Elem2D", "2D Elements to add/set in Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("1D Members", "Mem1D", "1D Members to add/set in Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("2D Members", "Mem2D", "2D Members to add/set in Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Loads", "Loads", "Loads to add/set in Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Section / Prop2D", "PA PB", "Sections and Prop2Ds to set in Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Analysis Settings", "ATasks", "Analysis Method and Settings for Model", GH_ParamAccess.list);
            for (int i = 0; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Model", "GSA", "GSA Model", GH_ParamAccess.item);
        }
        #endregion

        public class AnalysisWorker : WorkerInstance
        {
            public AnalysisWorker() : base(null) { }
            public override WorkerInstance Duplicate() => new AnalysisWorker();

            #region fields
            GsaModel WorkModel = new GsaModel();
            List<GsaNode> Nodes { get; set; }
            List<GsaElement1d> Elem1ds { get; set; }
            List<GsaElement2d> Elem2ds { get; set; }
            List<GsaMember1d> Mem1ds { get; set; }
            List<GsaMember2d> Mem2ds { get; set; }
            List<GsaLoad> Loads { get; set; }
            List<GsaSection> Sections { get; set; }
            List<GsaProp2d> Prop2Ds { get; set; }
            #endregion

            public override void GetData(IGH_DataAccess DA, GH_ComponentParamServer Params)
            {
                // Get Model input
                GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
                if (DA.GetData(0, ref gh_typ))
                {
                    if (gh_typ.Value is GsaModelGoo)
                    {
                        GsaModel in_model = new GsaModel();
                        gh_typ.CastTo(ref in_model);

                        WorkModel = in_model.Duplicate();
                    }
                    else
                    {
                        //AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert GSA input to Model");
                        //return;
                    }
                }

                //Get main data input
                //GH_Structure<GH_Curve> gh_Curves;
                //DA.GetDataTree(0, out gh_Curves);

                //GH_Structure<Grasshopper.Kernel.Types.IGH_GeometricGoo> gh_Geo;
                //DA.GetDataTree(1, out gh_Geo);

                // Get Node input
                List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
                List<GsaNode> in_nodes = new List<GsaNode>();
                if (DA.GetDataList(1, gh_types))
                {
                    for (int i = 0; i < gh_types.Count; i++)
                    {
                        gh_typ = gh_types[i];
                        if (gh_typ.Value is GsaNodeGoo)
                        {
                            GsaNode gsa = new GsaNode();
                            gh_typ.CastTo(ref gsa);
                            in_nodes.Add(gsa);
                        }
                    }
                    Nodes = in_nodes;
                }
                
                // Get Element1d input
                gh_types = new List<GH_ObjectWrapper>();
                List<GsaElement1d> in_elem1ds = new List<GsaElement1d>();
                if (DA.GetDataList(2, gh_types))
                {
                    for (int i = 0; i < gh_types.Count; i++)
                    {
                        gh_typ = gh_types[i];
                        if (gh_typ.Value is GsaElement1dGoo)
                        {
                            GsaElement1d gsa = new GsaElement1d();
                            gh_typ.CastTo(ref gsa);
                            in_elem1ds.Add(gsa);
                        }
                    }
                    Elem1ds = in_elem1ds;
                }

                // Get Element2d input
                gh_types = new List<GH_ObjectWrapper>();
                List<GsaElement2d> in_elem2ds = new List<GsaElement2d>();
                if (DA.GetDataList(3, gh_types))
                {
                    for (int i = 0; i < gh_types.Count; i++)
                    {
                        gh_typ = gh_types[i];
                        if (gh_typ.Value is GsaElement2dGoo)
                        {
                            GsaElement2d gsa = new GsaElement2d();
                            gh_typ.CastTo(ref gsa);
                            in_elem2ds.Add(gsa);
                        }
                    }
                    Elem2ds = in_elem2ds;
                }

                // Get Member1d input
                gh_types = new List<GH_ObjectWrapper>();
                List<GsaMember1d> in_mem1ds = new List<GsaMember1d>();
                if (DA.GetDataList(4, gh_types))
                {
                    for (int i = 0; i < gh_types.Count; i++)
                    {
                        gh_typ = gh_types[i];
                        if (gh_typ.Value is GsaMember1dGoo)
                        {
                            GsaMember1d gsa = new GsaMember1d();
                            gh_typ.CastTo(ref gsa);
                            in_mem1ds.Add(gsa);
                        }
                    }
                    Mem1ds = in_mem1ds;
                }

                // Get Member2d input
                gh_types = new List<GH_ObjectWrapper>();
                List<GsaMember2d> in_mem2ds = new List<GsaMember2d>();
                if (DA.GetDataList(5, gh_types))
                {
                    for (int i = 0; i < gh_types.Count; i++)
                    {
                        gh_typ = gh_types[i];
                        if (gh_typ.Value is GsaMember2dGoo)
                        {
                            GsaMember2d gsa = new GsaMember2d();
                            gh_typ.CastTo(ref gsa);
                            in_mem2ds.Add(gsa);
                        }
                    }
                    Mem2ds = in_mem2ds;
                }

                // Get Loads input
                gh_types = new List<GH_ObjectWrapper>();
                List<GsaLoad> in_loads = new List<GsaLoad>();
                if (DA.GetDataList(6, gh_types))
                {
                    for (int i = 0; i < gh_types.Count; i++)
                    {
                        gh_typ = gh_types[i];
                        if (gh_typ.Value is GsaLoadGoo)
                        {
                            GsaLoad gsa = null;
                            gh_typ.CastTo(ref gsa);
                            in_loads.Add(gsa);
                        }
                    }
                    Loads = in_loads;
                }

                // Get Section Property input
                gh_types = new List<GH_ObjectWrapper>();
                List<GsaSection> in_sect = new List<GsaSection>();
                List<GsaProp2d> in_prop = new List<GsaProp2d>();
                if (DA.GetDataList(7, gh_types))
                {
                    for (int i = 0; i < gh_types.Count; i++)
                    {
                        gh_typ = gh_types[i];
                        if (gh_typ.Value is GsaSectionGoo)
                        {
                            GsaSection gsa = new GsaSection();
                            gh_typ.CastTo(ref gsa);
                            in_sect.Add(gsa);
                        }
                        if (gh_typ.Value is GsaSectionGoo)
                        {
                            GsaProp2d gsa = new GsaProp2d();
                            gh_typ.CastTo(ref gsa);
                            in_prop.Add(gsa);
                        }
                    }
                    Sections = in_sect;
                    Prop2Ds = in_prop;
                }
            }

            public override void SetData(IGH_DataAccess DA)
            {
                // 👉 Checking for cancellation!
                if (CancellationToken.IsCancellationRequested) return;

                DA.SetData(0, new GsaModelGoo(WorkModel));
            }

            public override void DoWork(Action<string, double> ReportProgress, Action Done)
            {
                // 👉 Checking for cancellation!
                if (CancellationToken.IsCancellationRequested) return;

                // Let's work just on the model (not wrapped)
                GsaAPI.Model gsa = WorkModel.Model;
                string progress = "";
                string tempprogress = "";

                #region Nodes
                // ### Nodes ###
                
                // We take out the existing nodes in the model and work on that dictionary

                // Get existing nodes
                IReadOnlyDictionary<int, Node> gsaNodes = gsa.Nodes();
                Dictionary<int, Node> nodes = gsaNodes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                // create a counter for creating new nodes
                int newNodeID = (nodes.Count > 0) ? nodes.Keys.Max() + 1 : 1; //checking the existing model nodes

                // Add/Set Nodes
                if (Nodes != null)
                {
                    tempprogress = "Creating Nodes";
                    //ReportProgress(progress + tempprogress, 0);

                    // update counter if new nodes have set ID higher than existing max
                    int existingNodeMaxID = Nodes.Max(x => x.ID); // max ID in new nodes
                    if (existingNodeMaxID > newNodeID)
                        newNodeID = existingNodeMaxID + 1;

                    for (int i = 0; i < Nodes.Count; i++)
                    {
                        if (Nodes[i] != null)
                        {
                            GsaNode node = Nodes[i];
                            Node apiNode = node.Node;

                            // Add spring to model
                            if (node.Spring != null)
                            {
                                // Spring not implemented in GsaAPI
                                //node.Node.SpringProperty = gsa.AddSpring(node.Spring); // assuming this will send back the spring ID in the model
                            }

                            // Add axis to model
                            if (node.LocalAxis != null)
                            {
                                if (node.LocalAxis != Plane.WorldXY)
                                {
                                    Axis ax = new Axis();
                                    Plane pln = node.LocalAxis;
                                    ax.Origin.X = pln.OriginX;
                                    ax.Origin.Y = pln.OriginY;
                                    ax.Origin.Z = pln.OriginZ;

                                    ax.XVector.X = pln.XAxis.X;
                                    ax.XVector.Y = pln.XAxis.Y;
                                    ax.XVector.Z = pln.XAxis.Z;
                                    ax.XYPlane.X = pln.YAxis.X;
                                    ax.XYPlane.Y = pln.YAxis.Y;
                                    ax.XYPlane.Z = pln.YAxis.Z;

                                    // set Axis property in node
                                    apiNode.AxisProperty = gsa.AddAxis(ax);
                                }
                            }
                            if (node.ID > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
                            {
                                nodes[node.ID] = apiNode;
                            }
                                
                            else
                            {
                                // get existing node id if any:
                                int id = Util.Gsa.ModelNodes.ExistingNode(nodes, apiNode);
                                if (id > 0) // if within tolerance of existing node
                                {
                                    // get GSA node
                                    Node gsaNode = new Node();
                                    nodes.TryGetValue(id, out gsaNode);

                                    // combine restraints always picking true
                                    if (gsaNode.Restraint.X)
                                        apiNode.Restraint.X = true;
                                    if (gsaNode.Restraint.Y)
                                        apiNode.Restraint.Y = true;
                                    if (gsaNode.Restraint.Z)
                                        apiNode.Restraint.Z = true;
                                    if (gsaNode.Restraint.XX)
                                        apiNode.Restraint.XX = true;
                                    if (gsaNode.Restraint.YY)
                                        apiNode.Restraint.YY = true;
                                    if (gsaNode.Restraint.ZZ)
                                        apiNode.Restraint.ZZ = true;

                                    // set local axis if it is set in GH node
                                    if (node.LocalAxis != null)
                                    {
                                        if (gsaNode.SpringProperty > 0)
                                            apiNode.SpringProperty = gsaNode.SpringProperty;
                                    }
                                    // replace existing node with new merged node
                                    nodes[id] = apiNode;
                                }
                                else
                                {
                                    nodes.Add(newNodeID, apiNode);
                                    newNodeID++;
                                }
                            }
                        }

                        // 👉 Checking for cancellation!
                        if (CancellationToken.IsCancellationRequested) return;
                        tempprogress = "Creating Nodes..." + (i / (Nodes.Count - 1)).ToString("0%");
                        //ReportProgress(progress + tempprogress, 0);
                    }
                    tempprogress = "";
                    progress += "Creating Nodes...Done" + System.Environment.NewLine;
                    //ReportProgress(progress + tempprogress, 0);
                }
                #endregion

                #region Elements
                // ### Elements ###
                // We take out the existing elements in the model and work on that dictionary
                
                // Get existing elements
                IReadOnlyDictionary<int, Element> gsaElems = gsa.Elements();
                Dictionary<int, Element> elems = gsaElems.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                // create a counter for creating new elements
                int newElemID = (elems.Count > 0) ? elems.Keys.Max() + 1 : 1; //checking the existing model nodes
                if (Elem1ds != null)
                {
                    int existingElem1dMaxID = Elem1ds.Max(x => x.ID); // max ID in new Elem1ds
                    if (existingElem1dMaxID > newElemID)
                        newElemID = existingElem1dMaxID + 1;
                }
                if (Elem2ds != null)
                {
                    int existingElem2dMaxID = Elem2ds.Max(x => x.ID.Max()); // max ID in new Elem2ds
                    if (existingElem2dMaxID > newElemID)
                        newElemID = existingElem2dMaxID + 1;
                }
                
                // Elem1ds
                if (Elem1ds != null)
                {
                    tempprogress = "Creating Elem1Ds";
                    //ReportProgress(progress + tempprogress, 0);
                    
                    for (int i = 0; i < Elem1ds.Count; i++)
                    {
                        if (Elem1ds[i] != null)
                        {
                            GsaElement1d element1d = Elem1ds[i];
                            LineCurve line = element1d.Line;
                            Element apiElement = element1d.Element;

                            // update topology list to fit model nodes
                            List<int> topo = new List<int>();
                            //Start node
                            int id = Util.Gsa.ModelNodes.ExistingNodePoint(nodes, line.PointAtStart);
                            if (id > 0)
                                topo.Add(id);
                            else
                            {
                                int key = (nodes.Count > 0) ? nodes.Keys.Max() + 1 : 1;
                                nodes.Add(key, Util.Gsa.ModelNodes.NodeFromPoint(line.PointAtStart));
                                topo.Add(key);
                            }

                            //End node
                            id = Util.Gsa.ModelNodes.ExistingNodePoint(nodes, line.PointAtEnd);
                            if (id > 0)
                                topo.Add(id);
                            else
                            {
                                nodes.Add(newNodeID, Util.Gsa.ModelNodes.NodeFromPoint(line.PointAtEnd));
                                topo.Add(newNodeID);
                                newNodeID++;
                            }
                            // update topology in Element
                            apiElement.Topology = new ReadOnlyCollection<int>(topo.ToList());

                            // section
                            if (element1d.Section.ID > 0)
                            {
                                apiElement.Property = element1d.Section.ID; // set section ID in element
                                if (element1d.Section.Section != null) // section can refer to an ID only, meaning that the section must already exist in the model. Else we set it in the model:
                                    gsa.SetSection(element1d.Section.ID, element1d.Section.Section);
                            }
                            else
                            {
                                apiElement.Property = gsa.AddSection(element1d.Section.Section);
                            }

                            // set apielement in dictionary
                            if (element1d.ID > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
                            {
                                elems[element1d.ID] = apiElement;
                            }
                            else
                            {
                                elems.Add(newElemID, apiElement);
                                newElemID++;
                            }
                               
                        }

                        // 👉 Checking for cancellation!
                        if (CancellationToken.IsCancellationRequested) return;
                        tempprogress = "Creating Elem1Ds..." + (i / (Elem1ds.Count - 1)).ToString("0%");
                        //ReportProgress(progress + tempprogress, 0);
                    }
                    tempprogress = "";
                    progress += "Creating Elem1Ds...Done" + System.Environment.NewLine;
                    //ReportProgress(progress + tempprogress, 0);
                }

                // Elem2ds
                if (Elem2ds != null)
                {
                    tempprogress = "Creating Elem2Ds";
                    //ReportProgress(progress + tempprogress, 0);
                    
                    for (int i = 0; i < Elem2ds.Count; i++)
                    {
                        if (Elem2ds[i] != null)
                        {
                            GsaElement2d element2d = Elem2ds[i];
                            List<Point3d> meshVerticies = Elem2ds[i].Topology;

                            //Loop through all faces in mesh to update topology list to fit model nodes
                            for (int j = 0; j < element2d.Elements.Count; j++)
                            {
                                Element apiMeshElement = element2d.Elements[j];
                                List<int> meshVertexIndex = element2d.TopoInt[j];

                                List<int> topo = new List<int>(); // temp topologylist
                                
                                //Loop through topology
                                for (int k = 0; k < meshVertexIndex.Count; k++)
                                {
                                    int id = Util.Gsa.ModelNodes.ExistingNodePoint(nodes, meshVerticies[meshVertexIndex[k]]);
                                    if (id > 0)
                                        topo.Add(id);
                                    else
                                    {
                                        nodes.Add(newNodeID, Util.Gsa.ModelNodes.NodeFromPoint(meshVerticies[meshVertexIndex[k]]));
                                        topo.Add(newNodeID);
                                        newNodeID++;
                                    }
                                }
                                //update topology in Element
                                apiMeshElement.Topology = new ReadOnlyCollection<int>(topo.ToList());

                                // section
                                if (element2d.Properties[j].ID > 0)
                                {
                                    apiMeshElement.Property = element2d.Properties[j].ID;
                                    if (element2d.Properties[j].Prop2d != null)
                                        gsa.SetProp2D(element2d.Properties[j].ID, element2d.Properties[j].Prop2d);
                                }
                                else
                                {
                                    apiMeshElement.Property = gsa.AddProp2D(element2d.Properties[j].Prop2d);
                                }

                                // set api element in dictionary
                                if (element2d.ID[j] > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
                                {
                                    elems[element2d.ID[j]] = apiMeshElement;
                                }
                                else
                                {
                                    elems.Add(newElemID, apiMeshElement);
                                    newElemID++;
                                }
                                    

                                // 👉 Checking for cancellation!
                                if (CancellationToken.IsCancellationRequested) return;
                            }

                            // 👉 Checking for cancellation!
                            if (CancellationToken.IsCancellationRequested) return;
                            tempprogress = "Creating Elem2Ds..." + (i / (Elem2ds.Count - 1)).ToString("0%");
                            //ReportProgress(progress + tempprogress, 0);
                        }
                    }
                    tempprogress = "";
                    progress += "Creating Elem2Ds...Done" + System.Environment.NewLine;
                    //ReportProgress(progress + tempprogress, 0);
                }
                #endregion

                #region Members
                // ### Members ###
                // We take out the existing members in the model and work on that dictionary
               

                // Get existing members
                IReadOnlyDictionary<int, Member> gsaMems = gsa.Members();
                Dictionary<int, Member> mems = gsaMems.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                // create a counter for creating new members
                int newMemberID = (mems.Count > 0) ? mems.Keys.Max() + 1 : 1; //checking the existing model nodes
                if (Mem1ds != null)
                {
                    int existingMem1dMaxID = Mem1ds.Max(x => x.ID); // max ID in new Elem1ds
                    if (existingMem1dMaxID > newMemberID)
                        newMemberID = existingMem1dMaxID + 1;
                }
                if (Mem2ds != null)
                {
                    int existingMem2dMaxID = Mem2ds.Max(x => x.ID); // max ID in new Elem2ds
                    if (existingMem2dMaxID > newMemberID)
                        newMemberID = existingMem2dMaxID + 1;
                }

                // Mem1ds
                if (Mem1ds != null)
                {
                    tempprogress = "Creating Mem1Ds";
                    //ReportProgress(progress + tempprogress, 0);

                    for (int i = 0; i < Mem1ds.Count; i++)
                    {
                        if (Mem1ds[i] != null)
                        {
                            GsaMember1d member1d = Mem1ds[i];
                            Member apiMember = member1d.Member;

                            // update topology list to fit model nodes
                            string topo = "";

                            // Loop through the topology list
                            for (int j = 0; j < member1d.Topology.Count; j++)
                            {
                                string topologyType = member1d.TopologyType[j];
                                Point3d pt = member1d.Topology[j];
                                if (j > 0)
                                {
                                    if (topologyType == "" | topologyType == " ")
                                        topo += " ";
                                    else
                                        topo += topologyType.ToLower() + " "; // add topology type (nothing or "a") in front of node id
                                }

                                int id = Util.Gsa.ModelNodes.ExistingNodePoint(nodes, pt);
                                if (id > 0)
                                    topo += id;
                                else
                                {
                                    nodes.Add(newNodeID, Util.Gsa.ModelNodes.NodeFromPoint(pt));
                                    topo += newNodeID;
                                    newNodeID++;
                                }

                                if (j != member1d.Topology.Count - 1)
                                    topo += " ";

                                // 👉 Checking for cancellation!
                                if (CancellationToken.IsCancellationRequested) return;
                            }
                            // set topology in api member
                            apiMember.Topology = string.Copy(topo);

                            // Section
                            if (member1d.Section.ID > 0)
                            {
                                apiMember.Property = member1d.Section.ID;
                                if (member1d.Section.Section != null)
                                    gsa.SetSection(member1d.Section.ID, member1d.Section.Section);
                            }
                            else
                            {
                                apiMember.Property = gsa.AddSection(member1d.Section.Section);
                            }

                            // set apimember in dictionary
                            if (member1d.ID > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
                            {
                                mems[member1d.ID] = apiMember;
                            }
                            else
                            {
                                mems.Add(newMemberID, apiMember);
                                newMemberID++;
                            }
                                
                        }

                        // 👉 Checking for cancellation!
                        if (CancellationToken.IsCancellationRequested) return;
                        tempprogress = "Creating Mem1Ds..." + (i / (Mem1ds.Count - 1)).ToString("0%");
                        //ReportProgress(progress + tempprogress, 0);
                    }
                    tempprogress = "";
                    progress += "Creating Mem1Ds...Done" + System.Environment.NewLine;
                    //ReportProgress(progress + tempprogress, 0);
                }

                // Mem2ds
                if (Mem2ds != null)
                {
                    tempprogress = "Creating Mem2Ds";
                    //ReportProgress(progress + tempprogress, 0);

                    for (int i = 0; i < Mem2ds.Count; i++)
                    {
                        if (Mem2ds[i] != null)
                        {
                            GsaMember2d member2d = Mem2ds[i];
                            Member apimember = member2d.Member;

                            // update topology list to fit model nodes
                            string topo = "";

                            // Loop through the topology list
                            for (int j = 0; j < member2d.Topology.Count; j++)
                            {
                                Point3d pt = member2d.Topology[j];
                                string topologyType = member2d.TopologyType[j];

                                if (j > 0)
                                {
                                    if (topologyType == "" | topologyType == " ")
                                        topo += " ";
                                    else
                                        topo += topologyType.ToLower() + " "; // add topology type (nothing or "a") in front of node id
                                }

                                int id = Util.Gsa.ModelNodes.ExistingNodePoint(nodes, pt);
                                if (id > 0)
                                    topo += id;
                                else
                                {
                                    nodes.Add(newNodeID, Util.Gsa.ModelNodes.NodeFromPoint(pt));
                                    topo += newNodeID;
                                    newNodeID++;
                                }

                                if (j != member2d.Topology.Count - 1)
                                    topo += " ";

                                // 👉 Checking for cancellation!
                                if (CancellationToken.IsCancellationRequested) return;
                            }

                            // Loop through the voidtopology list
                            for (int j = 0; j < member2d.VoidTopology.Count; j++)
                            {
                                for (int k = 0; k < Mem2ds[i].VoidTopology[j].Count; k++)
                                {
                                    Point3d pt = member2d.VoidTopology[j][k];
                                    string voidtopologytype = member2d.VoidTopologyType[j][k];

                                    if (k == 0)
                                        topo += " V(";
                                    if (voidtopologytype == "" | voidtopologytype == " ")
                                        topo += " ";
                                    else
                                        topo += voidtopologytype.ToLower() + " "; // add topology type (nothing or "a") in front of node id

                                    int id = Util.Gsa.ModelNodes.ExistingNodePoint(nodes, pt);
                                    if (id > 0)
                                        topo += id;
                                    else
                                    {
                                        nodes.Add(newNodeID, Util.Gsa.ModelNodes.NodeFromPoint(pt));
                                        topo += newNodeID;
                                        newNodeID++;
                                    }

                                    if (k != member2d.VoidTopology[j].Count - 1)
                                        topo += " ";
                                    else
                                        topo += ")";

                                    // 👉 Checking for cancellation!
                                    if (CancellationToken.IsCancellationRequested) return;
                                }
                            }
                            // Loop through the inclusion lines topology list
                            for (int j = 0; j < member2d.IncLinesTopology.Count; j++)
                            {
                                for (int k = 0; k < member2d.IncLinesTopology[j].Count; k++)
                                {
                                    Point3d pt = member2d.IncLinesTopology[j][k];
                                    string inclineTopologytype = member2d.IncLinesTopologyType[j][k];

                                    if (k == 0)
                                        topo += " L(";
                                    if (inclineTopologytype == "" | inclineTopologytype == " ")
                                        topo += " ";
                                    else
                                        topo += inclineTopologytype.ToLower() + " "; // add topology type (nothing or "a") in front of node id

                                    int id = Util.Gsa.ModelNodes.ExistingNodePoint(nodes, pt);
                                    if (id > 0)
                                        topo += id;
                                    else
                                    {
                                        nodes.Add(newNodeID, Util.Gsa.ModelNodes.NodeFromPoint(pt));
                                        topo += newNodeID;
                                        newNodeID++;
                                    }

                                    if (k != member2d.IncLinesTopology[j].Count - 1)
                                        topo += " ";
                                    else
                                        topo += ")";

                                    // 👉 Checking for cancellation!
                                    if (CancellationToken.IsCancellationRequested) return;
                                }
                            }
                            // Loop through the inclucion point topology list
                            for (int j = 0; j < member2d.InclusionPoints.Count; j++)
                            {
                                Point3d pt = member2d.InclusionPoints[j];
                                if (j == 0)
                                    topo += " P(";

                                int id = Util.Gsa.ModelNodes.ExistingNodePoint(nodes, pt);
                                if (id > 0)
                                    topo += id;
                                else
                                {
                                    nodes.Add(newNodeID, Util.Gsa.ModelNodes.NodeFromPoint(pt));
                                    topo += newNodeID;
                                    newNodeID++;
                                }

                                if (j != member2d.InclusionPoints.Count - 1)
                                    topo += " ";
                                else
                                    topo += ")";

                                // 👉 Checking for cancellation!
                                if (CancellationToken.IsCancellationRequested) return;
                            }

                            // update topology for api member
                            apimember.Topology = string.Copy(topo);

                            // section
                            if (member2d.Property.ID > 0)
                            {
                                apimember.Property = member2d.Property.ID;
                                if (member2d.Property.Prop2d != null)
                                    gsa.SetProp2D(member2d.Property.ID, member2d.Property.Prop2d);
                            }
                            else
                            {
                                apimember.Property = gsa.AddProp2D(member2d.Property.Prop2d);
                            }

                            // set apimember in dictionary
                            if (member2d.ID > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
                            {
                                mems[member2d.ID] = apimember;
                            }
                            else
                            {
                                mems.Add(newMemberID, apimember);
                                newMemberID++;
                            }
                        }

                        // 👉 Checking for cancellation!
                        if (CancellationToken.IsCancellationRequested) return;
                        tempprogress = "Creating Mem1Ds..." + (i / (Mem2ds.Count - 1)).ToString("0%");
                        ReportProgress(progress + tempprogress, 0);
                    }
                    tempprogress = "";
                    progress += "Creating Mem2Ds...Done" + System.Environment.NewLine;
                    //ReportProgress(progress + tempprogress, 0);
                }

                
                #endregion

                #region Loads
                // ### Loads ###
                // We let the existing loads (if any) survive and just add new loads

                // Get existing loads
                List<GravityLoad> gravityLoads = new List<GravityLoad>();
                List<NodeLoad> nodeLoads_node = new List<NodeLoad>();
                List<NodeLoad> nodeLoads_displ = new List<NodeLoad>();
                List<NodeLoad> nodeLoads_settle = new List<NodeLoad>();
                List<BeamLoad> beamLoads = new List<BeamLoad>();
                List<FaceLoad> faceLoads = new List<FaceLoad>();
                List<GridPointLoad> gridPointLoads = new List<GridPointLoad>();
                List<GridLineLoad> gridLineLoads = new List<GridLineLoad>();
                List<GridAreaLoad> gridAreaLoads = new List<GridAreaLoad>();

                if (Loads != null)
                {
                    tempprogress = "Creating Loads";
                    //ReportProgress(progress + tempprogress, 0);

                    for (int i = 0; i < Loads.Count; i++)
                    {
                        if (Loads[i] != null)
                        {
                            GsaLoad load = Loads[i];
                            switch (load.LoadType)
                            {
                                case GsaLoad.LoadTypes.Gravity:
                                    gravityLoads.Add(load.GravityLoad.GravityLoad);
                                    break;
                                case GsaLoad.LoadTypes.Node:
                                    if (load.NodeLoad.NodeLoadType == GsaNodeLoad.NodeLoadTypes.APPLIED_DISP)
                                        nodeLoads_displ.Add(load.NodeLoad.NodeLoad);
                                    if (load.NodeLoad.NodeLoadType == GsaNodeLoad.NodeLoadTypes.NODE_LOAD)
                                        nodeLoads_node.Add(load.NodeLoad.NodeLoad);
                                    if (load.NodeLoad.NodeLoadType == GsaNodeLoad.NodeLoadTypes.SETTLEMENT)
                                        nodeLoads_settle.Add(load.NodeLoad.NodeLoad);
                                    break;
                                case GsaLoad.LoadTypes.Beam:
                                    beamLoads.Add(load.BeamLoad.BeamLoad);
                                    break;
                                case GsaLoad.LoadTypes.Face:
                                    faceLoads.Add(load.FaceLoad.FaceLoad);
                                    break;
                                case GsaLoad.LoadTypes.GridPoint:
                                    load.PointLoad.GridPlaneSurface.GridPlane.AxisProperty = gsa.AddAxis(load.PointLoad.GridPlaneSurface.Axis);
                                    load.PointLoad.GridPlaneSurface.GridSurface.GridPlane = gsa.AddGridPlane(load.PointLoad.GridPlaneSurface.GridPlane);
                                    load.PointLoad.GridPointLoad.GridSurface = gsa.AddGridSurface(load.PointLoad.GridPlaneSurface.GridSurface);
                                    gridPointLoads.Add(load.PointLoad.GridPointLoad);
                                    break;
                                case GsaLoad.LoadTypes.GridLine:
                                    load.LineLoad.GridPlaneSurface.GridPlane.AxisProperty = gsa.AddAxis(load.LineLoad.GridPlaneSurface.Axis);
                                    load.LineLoad.GridPlaneSurface.GridSurface.GridPlane = gsa.AddGridPlane(load.LineLoad.GridPlaneSurface.GridPlane);
                                    load.LineLoad.GridLineLoad.GridSurface = gsa.AddGridSurface(load.LineLoad.GridPlaneSurface.GridSurface);
                                    gridLineLoads.Add(load.LineLoad.GridLineLoad);
                                    break;
                                case GsaLoad.LoadTypes.GridArea:
                                    load.AreaLoad.GridPlaneSurface.GridPlane.AxisProperty = gsa.AddAxis(load.AreaLoad.GridPlaneSurface.Axis);
                                    load.AreaLoad.GridPlaneSurface.GridSurface.GridPlane = gsa.AddGridPlane(load.AreaLoad.GridPlaneSurface.GridPlane);
                                    load.AreaLoad.GridAreaLoad.GridSurface = gsa.AddGridSurface(load.AreaLoad.GridPlaneSurface.GridSurface);
                                    gridAreaLoads.Add(load.AreaLoad.GridAreaLoad);
                                    break;
                            }
                        }
                        // 👉 Checking for cancellation!
                        if (CancellationToken.IsCancellationRequested) return;
                        tempprogress = "Creating Loads..." + (i / (Loads.Count - 1)).ToString("0%");
                        //ReportProgress(progress + tempprogress, 0);
                    }
                    tempprogress = "";
                    progress += "Creating Loads...Done" + System.Environment.NewLine;
                    //ReportProgress(progress + tempprogress, 0);
                }
                #endregion

                #region Sections
                // ### Sections ###
                // We take out the existing sections in the model and work on that dictionary

                // Get existing nodes
                IReadOnlyDictionary<int, Section> gsaSections = gsa.Sections();
                Dictionary<int, Section> sections = gsaSections.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                // create a counter for creating new nodes
                int newSectionID = (sections.Count > 0) ? sections.Keys.Max() + 1 : 1; //checking the existing model

                // Add/Set Nodes
                if (Sections != null)
                {
                    if (Sections.Count != 0)
                    {
                        tempprogress = "Creating Sections";
                        //ReportProgress(progress + tempprogress, 0);

                        // update counter if new nodes have set ID higher than existing max
                        int existingSectionsMaxID = Sections.Max(x => x.ID); // max ID in new 
                        if (existingSectionsMaxID > newSectionID)
                            newSectionID = existingSectionsMaxID + 1;

                        for (int i = 0; i < Sections.Count; i++)
                        {
                            if (Sections[i] != null)
                            {
                                GsaSection section = Sections[i];
                                Section apiSection = section.Section;

                                if (section.ID > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
                                {
                                    sections[section.ID] = apiSection;
                                }
                                else
                                {
                                    sections.Add(newSectionID, apiSection);
                                    newSectionID++;
                                }
                            }

                            // 👉 Checking for cancellation!
                            if (CancellationToken.IsCancellationRequested) return;
                            tempprogress = "Creating Sections..." + (i / (Sections.Count - 1)).ToString("0%");
                            //ReportProgress(progress + tempprogress, 0);
                        }
                        tempprogress = "";
                        progress += "Creating Sections...Done" + System.Environment.NewLine;
                        //ReportProgress(progress + tempprogress, 0);
                    }
                }
                #endregion

                #region Prop2ds
                // ### Sections ###
                // We take out the existing sections in the model and work on that dictionary

                // Get existing nodes
                IReadOnlyDictionary<int, Prop2D> gsaProp2ds = gsa.Prop2Ds();
                Dictionary<int, Prop2D> prop2ds = gsaProp2ds.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                // create a counter for creating new nodes
                int newProp2dID = (gsaProp2ds.Count > 0) ? gsaProp2ds.Keys.Max() + 1 : 1; //checking the existing model

                // Prop2Ds
                if (Prop2Ds != null)
                {
                    if (Prop2Ds.Count != 0)
                    {
                        tempprogress = "Creating Prop2Ds";
                        //ReportProgress(progress + tempprogress, 0);

                        // update counter if new nodes have set ID higher than existing max
                        int existingProp2dMaxID = Prop2Ds.Max(x => x.ID); // max ID in new 
                        if (existingProp2dMaxID > newProp2dID)
                            newProp2dID = existingProp2dMaxID + 1;

                        for (int i = 0; i < Prop2Ds.Count; i++)
                        {
                            if (Prop2Ds[i] != null)
                            {
                                GsaProp2d prop2d = Prop2Ds[i];
                                Prop2D apiProp2d = prop2d.Prop2d;

                                if (prop2d.ID > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
                                {
                                    prop2ds[prop2d.ID] = apiProp2d;
                                }
                                else
                                {
                                    prop2ds.Add(newProp2dID, apiProp2d);
                                    newProp2dID++;
                                }
                            }

                            // 👉 Checking for cancellation!
                            if (CancellationToken.IsCancellationRequested) return;
                            tempprogress = "Creating Sections..." + (i / (Sections.Count - 1)).ToString("0%");
                            //ReportProgress(progress + tempprogress, 0);
                        }
                        tempprogress = "";
                        progress += "Creating Prop2ds...Done" + System.Environment.NewLine;
                        //ReportProgress(progress + tempprogress, 0);
                    }
                }
                #endregion

                #region set stuff in model
                tempprogress = "Setting nodes";
                //ReportProgress(progress + tempprogress, 0);
                if (CancellationToken.IsCancellationRequested) return;
                ReadOnlyDictionary<int, Node> setnodes = new ReadOnlyDictionary<int, Node>(nodes);
                gsa.SetNodes(setnodes);

                tempprogress = "Setting elements";
                //ReportProgress(progress + tempprogress, 0);
                ReadOnlyDictionary<int, Element> setelem = new ReadOnlyDictionary<int, Element>(elems);
                gsa.SetElements(setelem);

                tempprogress = "Setting members";
                //ReportProgress(progress + tempprogress, 0);
                ReadOnlyDictionary<int, Member> setmem = new ReadOnlyDictionary<int, Member>(mems);
                gsa.SetMembers(setmem);

                tempprogress = "Setting loads";
                //ReportProgress(progress + tempprogress, 0);
                //gravity load
                ReadOnlyCollection<GravityLoad> setgrav = new ReadOnlyCollection<GravityLoad>(gravityLoads);
                gsa.AddGravityLoads(setgrav);
                //node loads
                ReadOnlyCollection<NodeLoad> setnode_disp = new ReadOnlyCollection<NodeLoad>(nodeLoads_displ);
                gsa.AddNodeLoads(NodeLoadType.APPLIED_DISP, setnode_disp);
                ReadOnlyCollection<NodeLoad> setnode_node = new ReadOnlyCollection<NodeLoad>(nodeLoads_node);
                gsa.AddNodeLoads(NodeLoadType.NODE_LOAD, setnode_node);
                ReadOnlyCollection<NodeLoad> setnode_setl = new ReadOnlyCollection<NodeLoad>(nodeLoads_settle);
                gsa.AddNodeLoads(NodeLoadType.SETTLEMENT, setnode_setl);
                //beam loads
                ReadOnlyCollection<BeamLoad> setbeam = new ReadOnlyCollection<BeamLoad>(beamLoads);
                gsa.AddBeamLoads(setbeam);
                //face loads
                ReadOnlyCollection<FaceLoad> setface = new ReadOnlyCollection<FaceLoad>(faceLoads);
                gsa.AddFaceLoads(setface);
                //grid point loads
                ReadOnlyCollection<GridPointLoad> setpoint = new ReadOnlyCollection<GridPointLoad>(gridPointLoads);
                gsa.AddGridPointLoads(setpoint);
                //grid line loads
                ReadOnlyCollection<GridLineLoad> setline = new ReadOnlyCollection<GridLineLoad>(gridLineLoads);
                gsa.AddGridLineLoads(setline);
                //grid area loads
                ReadOnlyCollection<GridAreaLoad> setarea = new ReadOnlyCollection<GridAreaLoad>(gridAreaLoads);
                gsa.AddGridAreaLoads(setarea);

                tempprogress = "Setting sections";
                //ReportProgress(progress + tempprogress, 0);
                ReadOnlyDictionary<int, Section> setsect = new ReadOnlyDictionary<int, Section>(sections);
                gsa.SetSections(setsect);

                tempprogress = "Setting prop2ds";
                //ReportProgress(progress + tempprogress, 0);
                ReadOnlyDictionary<int, Prop2D> setpr2d = new ReadOnlyDictionary<int, Prop2D>(prop2ds);
                gsa.SetProp2Ds(setpr2d);

                tempprogress = "";
                progress += "Setting objects...Done" + System.Environment.NewLine;
                //ReportProgress(progress + tempprogress, 0);
                #endregion

                #region meshing
                // Create elements from members
                tempprogress = "Meshing members...";
                //ReportProgress(progress + tempprogress, 0);
                
                gsa.CreateElementsFromMembers();

                tempprogress = "";
                progress += "Meshing members...Done" + System.Environment.NewLine;
                //ReportProgress(progress + tempprogress, 0);
                #endregion

                #region analysis
                //analysis
                tempprogress = "Running analysis...";
                //ReportProgress(progress + tempprogress, 0);
                IReadOnlyDictionary<int, AnalysisTask> gsaTasks = gsa.AnalysisTasks();
                foreach (KeyValuePair<int, AnalysisTask> task in gsaTasks)
                {
                    tempprogress = "Running analysis Task" + task.Key;
                    //ReportProgress(progress + tempprogress, 0);
                    gsa.Analyse(task.Key);
                }

                tempprogress = "";
                //progress += "Analysis...Done" + System.Environment.NewLine;
                //ReportProgress(progress + tempprogress, 0);

                #endregion 

                WorkModel.Model = gsa;

                //gsa.SaveAs("C:\\Users\\Kristjan.Nielsen\\Desktop\\GsaGH_test.gwb");
                Done();
            }
        }
    }
}