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
using System.Linq;

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
                    if (gh_typ.Value is GsaModel)
                    {
                        GsaModel in_model = new GsaModel();
                        gh_typ.CastTo(ref in_model);
                        WorkModel = in_model;
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
                        if (gh_typ.Value is GsaNode)
                            in_nodes.Add((GsaNode)gh_typ.Value);
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
                        if (gh_typ.Value is GsaElement1d)
                            in_elem1ds.Add((GsaElement1d)gh_typ.Value);
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
                        if (gh_typ.Value is GsaElement2d)
                            in_elem2ds.Add((GsaElement2d)gh_typ.Value);
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
                        if (gh_typ.Value is GsaMember1d)
                            in_mem1ds.Add((GsaMember1d)gh_typ.Value);
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
                        if (gh_typ.Value is GsaMember2d)
                            in_mem2ds.Add((GsaMember2d)gh_typ.Value);
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
                        if (gh_typ.Value is GsaLoad)
                            in_loads.Add((GsaLoad)gh_typ.Value);
                    }
                    Loads = in_loads;
                }

            }

            public override void SetData(IGH_DataAccess DA)
            {
                // 👉 Checking for cancellation!
                if (CancellationToken.IsCancellationRequested) return;

                DA.SetData(0, WorkModel);
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
                int nodeid = 0;
                if (gsaNodes.Count > 0)
                    nodeid = gsaNodes.Keys.Max() + 1; //counter that we keep up to date for every new node added to dictionary

                Dictionary<int, Node> nodes = gsaNodes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                // Add/Set Nodes
                if (Nodes != null)
                {
                    for (int i = 0; i < Nodes.Count; i++)
                    {
                        // Add spring to model
                        if (Nodes[i].Spring != null)
                        {
                            // Spring not implemented in GsaAPI
                            //Nodes[i].Node.SpringProperty = gsa.AddSpring(Nodes[i].Spring); // assuming this will send back the spring ID in the model
                        }

                        // Add axis to model
                        if (Nodes[i].LocalAxis != null)
                        {
                            if (Nodes[i].LocalAxis != Plane.WorldXY)
                            {
                                Axis ax = new Axis();
                                ax.Origin.X = Nodes[i].LocalAxis.OriginX;
                                ax.Origin.Y = Nodes[i].LocalAxis.OriginY;
                                ax.Origin.Z = Nodes[i].LocalAxis.OriginZ;

                                ax.XVector.X = Nodes[i].LocalAxis.XAxis.X;
                                ax.XVector.Y = Nodes[i].LocalAxis.XAxis.Y;
                                ax.XVector.Z = Nodes[i].LocalAxis.XAxis.Z;
                                ax.XYPlane.X = Nodes[i].LocalAxis.YAxis.X;
                                ax.XYPlane.Y = Nodes[i].LocalAxis.YAxis.Y;
                                ax.XYPlane.Z = Nodes[i].LocalAxis.YAxis.Z;

                                // set Axis property in node
                                Nodes[i].Node.AxisProperty = gsa.AddAxis(ax);
                            }
                        }
                        if (Nodes[i].ID > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
                            nodes[Nodes[i].ID] = Nodes[i].Node;
                        else
                        {
                            // get existing node id if any:
                            int id = Util.Gsa.ModelNodes.ExistingNode(nodes, Nodes[i].Node);
                            if (id > 0) // if within tolerance of existing node
                            {
                                // get GSA node
                                Node gsaNode = new Node();
                                nodes.TryGetValue(id, out gsaNode);

                                // combine restraints always picking true
                                if (Nodes[i].Node.Restraint.X)
                                    gsaNode.Restraint.X = true;
                                if (Nodes[i].Node.Restraint.Y)
                                    gsaNode.Restraint.Y = true;
                                if (Nodes[i].Node.Restraint.Z)
                                    gsaNode.Restraint.Z = true;
                                if (Nodes[i].Node.Restraint.XX)
                                    gsaNode.Restraint.XX = true;
                                if (Nodes[i].Node.Restraint.YY)
                                    gsaNode.Restraint.YY = true;
                                if (Nodes[i].Node.Restraint.ZZ)
                                    gsaNode.Restraint.ZZ = true;

                                // set local axis if it is set in GH node
                                if (Nodes[i].LocalAxis != null)
                                    gsaNode.SpringProperty = Nodes[i].Node.SpringProperty;

                                nodes[id] = gsaNode;
                            }
                            else
                                nodes.Add(nodeid++, Nodes[i].Node);
                        }

                        // 👉 Checking for cancellation!
                        if (CancellationToken.IsCancellationRequested) return;
                        ReportProgress("Creating Nodes" + (i / (Nodes.Count - 1)).ToString("0%"));
                    }

                }

                #endregion

                #region Elements
                // ### Elements ###
                // We take out the existing elements in the model and work on that dictionary

                // Get existing elements
                IReadOnlyDictionary<int, Element> gsaElems = gsa.Elements();
                int elemid = 0;
                if (gsaElems.Count > 0)
                    elemid = gsaElems.Keys.Max() + 1;
                Dictionary<int, Element> elems = gsaElems.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                if (Elem1ds != null)
                {
                    // Elem1ds
                    for (int i = 0; i < Elem1ds.Count; i++)
                    {
                        // update topology list to fit model nodes
                        Collection<int> topo = new Collection<int>();
                        //Start node
                        int id = Util.Gsa.ModelNodes.ExistingNodePoint(nodes, Elem1ds[i].Line.PointAtStart);
                        if (id > 0)
                            topo.Add(id);
                        else
                        {
                            nodes.Add(nodeid, Util.Gsa.ModelNodes.NodeFromPoint(Elem1ds[i].Line.PointAtStart));
                            topo.Add(nodeid++);
                        }

                        //End node
                        id = Util.Gsa.ModelNodes.ExistingNodePoint(nodes, Elem1ds[i].Line.PointAtEnd);
                        if (id > 0)
                            topo.Add(id);
                        else
                        {
                            nodes.Add(nodeid, Util.Gsa.ModelNodes.NodeFromPoint(Elem1ds[i].Line.PointAtEnd));
                            topo.Add(nodeid++);
                        }
                        // update topology
                        Elem1ds[i].Element.Topology = new ReadOnlyCollection<int>(topo);


                        if (Elem1ds[i].ID > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
                        {
                            elems[Elem1ds[i].ID] = Elem1ds[i].Element;
                        }
                        else
                            elems.Add(elemid++, Elem1ds[i].Element);

                        // 👉 Checking for cancellation!
                        if (CancellationToken.IsCancellationRequested) return;
                        ReportProgress("Creating Elem1Ds" + (i / (Elem1ds.Count - 1)).ToString("0%"));
                    }
                }
                
                if (Elem2ds != null)
                {
                    // Elem2ds
                    for (int i = 0; i < Elem2ds.Count; i++)
                    {
                        //Loop through all faces in mesh
                        for (int j = 0; j < Elem2ds[i].Elements.Count; j++)
                        {
                            // update topology list to fit model nodes
                            Collection<int> topo = new Collection<int>();
                            //Loop through topology
                            for (int k = 0; k < Elem2ds[i].TopoInt[j].Count; k++)
                            {
                                int id = Util.Gsa.ModelNodes.ExistingNodePoint(nodes, Elem2ds[i].Topology[Elem2ds[i].TopoInt[j][k]]);
                                if (id > 0)
                                    topo.Add(id);
                                else
                                {
                                    nodes.Add(nodeid, Util.Gsa.ModelNodes.NodeFromPoint(Elem2ds[i].Topology[j]));
                                    topo.Add(nodeid++);
                                }
                            }
                            //update topology in Element
                            Elem2ds[i].Elements[j].Topology = new ReadOnlyCollection<int>(topo);

                            if (Elem2ds[i].ID[j] > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
                            {
                                elems[Elem2ds[i].ID[j]] = Elem2ds[i].Elements[j];
                            }
                            else
                                elems.Add(elemid++, Elem2ds[i].Elements[j]);

                            // 👉 Checking for cancellation!
                            if (CancellationToken.IsCancellationRequested) return;
                        }

                        // 👉 Checking for cancellation!
                        if (CancellationToken.IsCancellationRequested) return;
                        ReportProgress("Creating Elem2Ds" + (i / (Elem1ds.Count - 1)).ToString("0%"));
                    }
                }
                
                #endregion

                #region Members
                // ### Members ###
                // We take out the existing members in the model and work on that dictionary

                // Get existing members
                IReadOnlyDictionary<int, Member> gsaMems = gsa.Members();
                int memid = 0;
                if (gsaMems.Count > 0)
                    memid = gsaMems.Keys.Max() + 1;
                Dictionary<int, Member> mems = gsaMems.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                if (Mem1ds != null)
                {
                    // Mem1ds
                    for (int i = 0; i < Mem1ds.Count; i++)
                    {
                        // update topology list to fit model nodes
                        string topo = "";

                        // Loop through the topology list
                        for (int j = 0; j < Mem1ds[i].Topology.Count; j++)
                        {
                            if (j > 0)
                            {
                                if (Mem1ds[i].TopologyType[j] == "" | Mem1ds[i].TopologyType[j] == " ")
                                    topo += " ";
                                else
                                    topo += Mem1ds[i].TopologyType[j].ToLower() + " "; // add topology type (nothing or "a") in front of node id
                            }

                            int id = Util.Gsa.ModelNodes.ExistingNodePoint(nodes, Mem1ds[i].Topology[j]);
                            if (id > 0)
                                topo += id;
                            else
                            {
                                nodes.Add(nodeid, Util.Gsa.ModelNodes.NodeFromPoint(Mem1ds[i].Topology[j]));
                                topo += nodeid++;
                            }

                            if (j != Mem1ds[i].Topology.Count - 1)
                                topo += " ";

                            // 👉 Checking for cancellation!
                            if (CancellationToken.IsCancellationRequested) return;
                        }
                        Mem1ds[i].Member.Topology = topo;

                        //Mem1ds[i].Section.

                        if (Mem1ds[i].ID > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
                        {
                            mems[Mem1ds[i].ID] = Mem1ds[i].Member;
                        }
                        else
                            mems.Add(memid++, Mem1ds[i].Member);

                        // 👉 Checking for cancellation!
                        if (CancellationToken.IsCancellationRequested) return;
                        ReportProgress("Creating Mem1Ds" + (i / (Elem1ds.Count - 1)).ToString("0%"));
                    }
                }
                
                if (Mem2ds != null)
                {
                    // Mem2ds
                    for (int i = 0; i < Mem2ds.Count; i++)
                    {
                        // update topology list to fit model nodes
                        string topo = "";

                        // Loop through the topology list
                        for (int j = 0; j < Mem2ds[i].Topology.Count; j++)
                        {
                            if (j > 0)
                            {
                                if (Mem2ds[i].TopologyType[j] == "" | Mem2ds[i].TopologyType[j] == " ")
                                    topo += " ";
                                else
                                    topo += Mem2ds[i].TopologyType[j].ToLower() + " "; // add topology type (nothing or "a") in front of node id
                            }

                            int id = Util.Gsa.ModelNodes.ExistingNodePoint(nodes, Mem2ds[i].Topology[j]);
                            if (id > 0)
                                topo += id;
                            else
                            {
                                nodes.Add(nodeid, Util.Gsa.ModelNodes.NodeFromPoint(Mem2ds[i].Topology[j]));
                                topo += nodeid++;
                            }

                            if (j != Mem2ds[i].Topology.Count - 1)
                                topo += " ";

                            // 👉 Checking for cancellation!
                            if (CancellationToken.IsCancellationRequested) return;
                        }
                        // Loop through the voidtopology list
                        for (int j = 0; j < Mem2ds[i].VoidTopology.Count; j++)
                        {
                            for (int k = 0; k < Mem2ds[i].VoidTopology[j].Count; k++)
                            {
                                if (k == 0)
                                    topo += "V(";
                                if (Mem2ds[i].VoidTopologyType[j][k] == "" | Mem2ds[i].VoidTopologyType[j][k] == " ")
                                    topo += " ";
                                else
                                    topo += Mem2ds[i].VoidTopologyType[j][k].ToLower() + " "; // add topology type (nothing or "a") in front of node id

                                int id = Util.Gsa.ModelNodes.ExistingNodePoint(nodes, Mem2ds[i].VoidTopology[j][k]);
                                if (id > 0)
                                    topo += id;
                                else
                                {
                                    nodes.Add(nodeid, Util.Gsa.ModelNodes.NodeFromPoint(Mem2ds[i].VoidTopology[j][k]));
                                    topo += nodeid++;
                                }

                                if (k != Mem2ds[i].VoidTopology[j].Count - 1)
                                    topo += " ";
                                else
                                    topo += ")";

                                // 👉 Checking for cancellation!
                                if (CancellationToken.IsCancellationRequested) return;
                            }
                        }
                        // Loop through the inclusion lines topology list
                        for (int j = 0; j < Mem2ds[i].IncLinesTopology.Count; j++)
                        {
                            for (int k = 0; k < Mem2ds[i].IncLinesTopology[j].Count; k++)
                            {
                                if (k == 0)
                                    topo += "L(";
                                if (Mem2ds[i].IncLinesTopologyType[j][k] == "" | Mem2ds[i].IncLinesTopologyType[j][k] == " ")
                                    topo += " ";
                                else
                                    topo += Mem2ds[i].IncLinesTopologyType[j][k].ToLower() + " "; // add topology type (nothing or "a") in front of node id

                                int id = Util.Gsa.ModelNodes.ExistingNodePoint(nodes, Mem2ds[i].IncLinesTopology[j][k]);
                                if (id > 0)
                                    topo += id;
                                else
                                {
                                    nodes.Add(nodeid, Util.Gsa.ModelNodes.NodeFromPoint(Mem2ds[i].IncLinesTopology[j][k]));
                                    topo += nodeid++;
                                }

                                if (k != Mem2ds[i].IncLinesTopology[j].Count - 1)
                                    topo += " ";
                                else
                                    topo += ")";

                                // 👉 Checking for cancellation!
                                if (CancellationToken.IsCancellationRequested) return;
                            }
                        }
                        // Loop through the inclucion point topology list
                        for (int j = 0; j < Mem2ds[i].InclusionPoints.Count; j++)
                        {
                            if (j == 0)
                                topo += "P(";

                            int id = Util.Gsa.ModelNodes.ExistingNodePoint(nodes, Mem2ds[i].InclusionPoints[j]);
                            if (id > 0)
                                topo += id;
                            else
                            {
                                nodes.Add(nodeid, Util.Gsa.ModelNodes.NodeFromPoint(Mem2ds[i].InclusionPoints[j]));
                                topo += nodeid++;
                            }

                            if (j != Mem2ds[i].InclusionPoints.Count - 1)
                                topo += " ";
                            else
                                topo += ")";

                            // 👉 Checking for cancellation!
                            if (CancellationToken.IsCancellationRequested) return;
                        }

                        // update topology for member
                        Mem2ds[i].Member.Topology = topo;

                        //Mem1ds[i].Section.

                        if (Mem2ds[i].ID > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
                        {
                            mems[Mem2ds[i].ID] = Mem2ds[i].Member;
                        }
                        else
                            mems.Add(memid++, Mem2ds[i].Member);

                        // 👉 Checking for cancellation!
                        if (CancellationToken.IsCancellationRequested) return;
                        ReportProgress("Creating Mem2Ds" + (i / (Elem1ds.Count - 1)).ToString("0%"));
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

                IReadOnlyDictionary<int, AnalysisTask> gsaTasks = gsa.AnalysisTasks();
                foreach (KeyValuePair<int, AnalysisTask> task in gsaTasks)
                {
                    ReportProgress("Analysing Task " + task.Key);
                    gsa.Analyse(task.Key);
                }

                ReportProgress("Analysis done");
                WorkModel.Model = gsa;

                Done();
            }
        }
    }
}
;