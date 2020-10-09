using System;
using System.Linq;
using System.Collections.ObjectModel;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
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
        { BaseWorker = new PrimeCalculatorWorker(); }
        public override Guid ComponentGuid => new Guid("b9ca86f7-fda1-4c5e-ae75-5e570d4885e9");
        public override GH_Exposure Exposure => GH_Exposure.primary;

        //protected override Bitmap Icon => Resources.CrossSections;
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
            pManager.AddGenericParameter("Loads", "Load", "Loads to add/set in Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Analysis Settings", "ASet", "Analysis Method and Settings for Model", GH_ParamAccess.list);
            for (int i = 0; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Model", "GSA", "GSA Model", GH_ParamAccess.item);
        }
        #endregion

        public class PrimeCalculatorWorker : WorkerInstance
        {
            public override WorkerInstance Duplicate() => new PrimeCalculatorWorker();

            #region fields
            GsaModel WorkModel = new GsaModel();
            List<GsaNode> Nodes { get; set; }
            List<GsaElement1d> Elem1ds { get; set; }
            List<GsaElement2d> Elem2ds { get; set; }
            List<GsaMember1d> Mem1ds { get; set; }
            List<GsaMember2d> Mem2ds { get; set; }
            List<GsaLoad> Loads { get; set; }
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
                        //WorkModel = in_model;
                        WorkModel.Model = in_model.Model;
                    }
                    else
                    {
                        //AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert GSA input to Model");
                        //return;
                    }
                }

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
                if (DA.GetDataList(4, gh_types))
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
            }

            public override void SetData(IGH_DataAccess DA)
            {
                // 👉 Checking for cancellation!
                if (CancellationToken.IsCancellationRequested) return;

                DA.SetData(0, new GsaModelGoo(WorkModel));
            }

            public override void DoWork(Action<string> ReportProgress, Action<string, GH_RuntimeMessageLevel> ReportError, Action Done)
            {
                // 👉 Checking for cancellation!
                if (CancellationToken.IsCancellationRequested) return;

                // Let's work just on the model (not wrapped)
                GsaAPI.Model gsa = WorkModel.Model;
                

                #region nodes
                // ### Nodes ###
                ReportProgress("Creating Nodes");
                // We take out the existing nodes in the model and work on that dictionary

                // Get existing nodes
                IReadOnlyDictionary<int, Node> gsaNodes = gsa.Nodes();
                int nodeid = 1;
                if (gsaNodes.Count > 0)
                    nodeid = gsaNodes.Keys.Max() + 1; //counter that we keep up to date for every new node added to dictionary

                Dictionary<int, Node> nodes = gsaNodes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                // Add/Set Nodes
                if (Nodes != null)
                {
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
                                nodes[node.ID] = apiNode;
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

                                    nodes[id] = apiNode;
                                }
                                else
                                    nodes.Add(nodeid++, apiNode);
                            }
                        }

                        // 👉 Checking for cancellation!
                        if (CancellationToken.IsCancellationRequested) return;
                        //ReportProgress("Creating Nodes " + (i / (Nodes.Count - 1)).ToString("0%"));
                    }

                }

                #endregion

                #region Elements
                // ### Elements ###
                // We take out the existing elements in the model and work on that dictionary
                ReportProgress("Creating Elem1Ds");
                // Get existing elements
                IReadOnlyDictionary<int, Element> gsaElems = gsa.Elements();
                int elemid = 1;
                if (gsaElems.Count > 0)
                    elemid = gsaElems.Keys.Max() + 1;
                Dictionary<int, Element> elems = gsaElems.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                if (Elem1ds != null)
                {
                    // Elem1ds
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
                                nodes.Add(nodeid, Util.Gsa.ModelNodes.NodeFromPoint(line.PointAtStart));
                                topo.Add(nodeid);
                                nodeid++;
                            }

                            //End node
                            id = Util.Gsa.ModelNodes.ExistingNodePoint(nodes, line.PointAtEnd);
                            if (id > 0)
                                topo.Add(id);
                            else
                            {
                                nodes.Add(nodeid, Util.Gsa.ModelNodes.NodeFromPoint(line.PointAtEnd));
                                topo.Add(nodeid);
                                nodeid++;
                            }
                            // update topology in Element
                            apiElement.Topology = new ReadOnlyCollection<int>(topo);

                            // section
                            if (element1d.Section.ID > 0)
                            {
                                gsa.SetSection(element1d.Section.ID, element1d.Section.Section);
                                apiElement.Property = element1d.Section.ID;
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
                                elems.Add(elemid, apiElement);
                                elemid++;
                            }
                               
                        }

                        // 👉 Checking for cancellation!
                        if (CancellationToken.IsCancellationRequested) return;
                        //ReportProgress("Creating Elem1Ds " + (i / (Elem1ds.Count - 1)).ToString("0%"));
                    }
                }
                ReportProgress("Creating Elem2Ds");
                if (Elem2ds != null)
                {
                    // Elem2ds
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
                                        nodes.Add(nodeid, Util.Gsa.ModelNodes.NodeFromPoint(meshVerticies[meshVertexIndex[k]]));
                                        topo.Add(nodeid++);
                                    }
                                }
                                //update topology in Element
                                apiMeshElement.Topology = new ReadOnlyCollection<int>(topo);

                                // section
                                if (element2d.Properties[j].ID > 0)
                                {
                                    gsa.SetProp2D(element2d.Properties[j].ID, element2d.Properties[j].Prop2d);
                                    apiMeshElement.Property = element2d.Properties[j].ID;
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
                                    elems.Add(elemid, apiMeshElement);
                                    elemid++;
                                }
                                    

                                // 👉 Checking for cancellation!
                                if (CancellationToken.IsCancellationRequested) return;
                            }

                            // 👉 Checking for cancellation!
                            if (CancellationToken.IsCancellationRequested) return;
                            //ReportProgress("Creating Elem2Ds " + (i / (Elem2ds.Count - 1)).ToString("0%"));
                        }
                    }
                }

                #endregion

                #region Members
                // ### Members ###
                // We take out the existing members in the model and work on that dictionary
                ReportProgress("Creating Mem1Ds");
                // Get existing members
                IReadOnlyDictionary<int, Member> gsaMems = gsa.Members();
                int memid = 1;
                if (gsaMems.Count > 0)
                    memid = gsaMems.Keys.Max() + 1;
                Dictionary<int, Member> mems = gsaMems.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                if (Mem1ds != null)
                {
                    // Mem1ds
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
                                    nodes.Add(nodeid, Util.Gsa.ModelNodes.NodeFromPoint(pt));
                                    topo += nodeid;
                                    nodeid++;
                                }

                                if (j != member1d.Topology.Count - 1)
                                    topo += " ";

                                // 👉 Checking for cancellation!
                                if (CancellationToken.IsCancellationRequested) return;
                            }
                            // set topology in api member
                            apiMember.Topology = topo;

                            // Section
                            if (member1d.Section.ID > 0)
                            {
                                gsa.SetSection(member1d.Section.ID, member1d.Section.Section);
                                apiMember.Property = member1d.Section.ID;
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
                                mems.Add(memid, apiMember);
                                memid++;
                            }
                                
                        }

                        // 👉 Checking for cancellation!
                        if (CancellationToken.IsCancellationRequested) return;
                        //ReportProgress("Creating Mem1Ds " + (i / (Mem1ds.Count - 1)).ToString("0%"));
                    }
                }
                ReportProgress("Creating Mem2Ds");
                if (Mem2ds != null)
                {
                    // Mem2ds
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
                                    nodes.Add(nodeid, Util.Gsa.ModelNodes.NodeFromPoint(pt));
                                    topo += nodeid;
                                    nodeid++;
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
                                        topo += "V(";
                                    if (voidtopologytype == "" | voidtopologytype == " ")
                                        topo += " ";
                                    else
                                        topo += voidtopologytype.ToLower() + " "; // add topology type (nothing or "a") in front of node id

                                    int id = Util.Gsa.ModelNodes.ExistingNodePoint(nodes, pt);
                                    if (id > 0)
                                        topo += id;
                                    else
                                    {
                                        nodes.Add(nodeid, Util.Gsa.ModelNodes.NodeFromPoint(pt));
                                        topo += nodeid;
                                        nodeid++;
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
                                        topo += "L(";
                                    if (inclineTopologytype == "" | inclineTopologytype == " ")
                                        topo += " ";
                                    else
                                        topo += inclineTopologytype.ToLower() + " "; // add topology type (nothing or "a") in front of node id

                                    int id = Util.Gsa.ModelNodes.ExistingNodePoint(nodes, pt);
                                    if (id > 0)
                                        topo += id;
                                    else
                                    {
                                        nodes.Add(nodeid, Util.Gsa.ModelNodes.NodeFromPoint(pt));
                                        topo += nodeid;
                                        nodeid++;
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
                                    topo += "P(";

                                int id = Util.Gsa.ModelNodes.ExistingNodePoint(nodes, pt);
                                if (id > 0)
                                    topo += id;
                                else
                                {
                                    nodes.Add(nodeid, Util.Gsa.ModelNodes.NodeFromPoint(pt));
                                    topo += nodeid;
                                    nodeid++;
                                }

                                if (j != member2d.InclusionPoints.Count - 1)
                                    topo += " ";
                                else
                                    topo += ")";

                                // 👉 Checking for cancellation!
                                if (CancellationToken.IsCancellationRequested) return;
                            }

                            // update topology for api member
                            apimember.Topology = topo;

                            // section
                            if (member2d.Property.ID > 0)
                            {
                                gsa.SetProp2D(member2d.Property.ID, member2d.Property.Prop2d);
                                apimember.Property = member2d.Property.ID;
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
                                mems.Add(memid, apimember);
                                memid++;
                            }
                        }

                        // 👉 Checking for cancellation!
                        if (CancellationToken.IsCancellationRequested) return;
                        //ReportProgress("Creating Mem2Ds" + (i / (Mem2ds.Count - 1)).ToString("0%"));
                    }
                }
                #endregion

                #region loads
                // ### Loads ###
                ReportProgress("Creating Loads");
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
                        //ReportProgress("Creating Loads " + (i / (Loads.Count - 1)).ToString("0%"));
                    }
                }
                #endregion

                if (CancellationToken.IsCancellationRequested) return;
                ReportProgress("Setting Nodes");
                ReadOnlyDictionary<int, Node> setnodes = new ReadOnlyDictionary<int, Node>(nodes);
                gsa.SetNodes(setnodes);

                ReportProgress("Setting Elements");
                ReadOnlyDictionary<int, Element> setelem = new ReadOnlyDictionary<int, Element>(elems);
                gsa.SetElements(setelem);

                ReportProgress("Setting Members");
                ReadOnlyDictionary<int, Member> setmem = new ReadOnlyDictionary<int, Member>(mems);
                gsa.SetMembers(setmem);

                ReportProgress("Create Elements from Members");
                gsa.CreateElementsFromMembers();

                ReportProgress("Setting Loads");
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

                //analysis
                IReadOnlyDictionary<int, AnalysisTask> gsaTasks = gsa.AnalysisTasks();
                foreach (KeyValuePair<int, AnalysisTask> task in gsaTasks)
                {
                    ReportProgress("Analysing Task " + task.Key);
                    gsa.Analyse(task.Key);
                }

                ReportProgress("Analysis done");
                WorkModel.Model = gsa;

                //gsa.SaveAs("C:\\Users\\Kristjan.Nielsen\\Desktop\\GsaGH_test.gwb");
                Done();
            }
        }
    }
}
;