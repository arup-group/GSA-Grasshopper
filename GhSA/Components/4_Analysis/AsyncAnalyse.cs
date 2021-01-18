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
using Grasshopper.Kernel.Data;

namespace GhSA.Components
{
    public class Analyse : GH_AsyncComponent
    {
        #region Name and Ribbon Layout
        public Analyse()
            : base("Async Analyse Model", "AsyncAnalyse", "Assemble and Analyse a GSA Model using Multi-threading",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat4())
        { BaseWorker = new AnalysisWorker(); this.Hidden = true; }

        public override Guid ComponentGuid => new Guid("b9ca86f7-fda1-4c5e-ae75-5e570d4885e9");
        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.AnalyseAsync;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        #endregion

        #region input and output
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Model", "GSA", "(Optional) Existing Model to append to", GH_ParamAccess.item);
            pManager.AddGenericParameter("Nodes", "No", "Nodes to add/set in Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("1D Elements", "E1D", "1D Elements to add/set in Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("2D Elements", "E2D", "2D Elements to add/set in Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("1D Members", "M1D", "1D Members to add/set in Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("2D Members", "M2D", "2D Members to add/set in Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Loads", "Ld", "Loads to add/set in Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Section / Prop2D", "P", "Sections and Prop2Ds to set in Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Analysis Settings", "--", "Analysis Method and Settings for Model", GH_ParamAccess.list);
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
            List<GsaMember3d> Mem3ds { get; set; }
            List<GsaLoad> Loads { get; set; }
            List<GsaSection> Sections { get; set; }
            List<GsaProp2d> Prop2Ds { get; set; }
            List<GsaGridPlaneSurface> GridPlaneSurfaces { get; set; }
            #endregion

            public override void GetData(IGH_DataAccess DA, GH_ComponentParamServer Params)
            {
                #region GetData
                WorkModel = new GsaModel();
                Nodes = null;
                Elem1ds = null;
                Elem2ds = null;
                Mem1ds = null;
                Mem2ds = null;
                Mem3ds = null;
                Loads = null;
                Sections = null;
                Prop2Ds = null;
                GridPlaneSurfaces = null;

                // Get Model input
                GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
                if (DA.GetData(0, ref gh_typ))
                {
                    if (gh_typ.Value is GsaModelGoo)
                    {
                        GsaModel in_model = new GsaModel();
                        gh_typ.CastTo(ref in_model);

                        WorkModel = in_model; 
                    }
                    else
                    {
                        this.Parent.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert GSA input to Model");
                        return;
                    }
                }

                // Get Node input
                List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
                GH_Structure<IGH_Goo> ghtree_types = new GH_Structure<IGH_Goo>();
                List<GsaNode> in_nodes = new List<GsaNode>();
                if (DA.GetDataList(1, gh_types))
                {
                    //ghtree_types.Flatten();
                    //gh_types = ghtree_types[0];
                    for (int i = 0; i < gh_types.Count; i++)
                    {
                        gh_typ = gh_types[i];
                        if (gh_typ.Value is GsaNodeGoo)
                        {
                            GsaNode gsanode = new GsaNode();
                            gh_typ.CastTo(ref gsanode);
                            in_nodes.Add(gsanode);
                        }
                        else
                        {
                            this.Parent.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in Nodes input");
                            return;
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
                            GsaElement1d gsaelem1 = new GsaElement1d();
                            gh_typ.CastTo(ref gsaelem1);
                            in_elem1ds.Add(gsaelem1);
                        }
                        else
                        {
                            this.Parent.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in Elem1D input");
                            
                            return;
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
                            GsaElement2d gsaelem2 = new GsaElement2d();
                            gh_typ.CastTo(ref gsaelem2);
                            in_elem2ds.Add(gsaelem2);
                        }
                        else
                        {
                            this.Parent.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in Elem2D input");
                            return;
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
                            GsaMember1d gsamem1 = new GsaMember1d();
                            gh_typ.CastTo(ref gsamem1);
                            in_mem1ds.Add(gsamem1);
                        }
                        else
                        {
                            this.Parent.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in Mem1D input");
                            return;
                        }
                    }
                    Mem1ds = in_mem1ds;
                }

                // Get Member2d input
                gh_types = new List<GH_ObjectWrapper>();
                List<GsaMember2d> in_mem2ds = new List<GsaMember2d>();
                List<GsaMember3d> in_mem3ds = new List<GsaMember3d>();
                if (DA.GetDataList(5, gh_types))
                {
                    for (int i = 0; i < gh_types.Count; i++)
                    {
                        gh_typ = gh_types[i];
                        if (gh_typ.Value is GsaMember2dGoo)
                        {
                            GsaMember2d gsamem2 = new GsaMember2d();
                            gh_typ.CastTo(ref gsamem2);
                            in_mem2ds.Add(gsamem2);
                        }
                        else if (gh_typ.Value is GsaMember3dGoo)
                        {
                            GsaMember3d gsamem3 = new GsaMember3d();
                            gh_typ.CastTo(ref gsamem3);
                            in_mem3ds.Add(gsamem3);
                        }
                        else
                        {
                            this.Parent.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in Mem2D input");
                            return;
                        }
                    }
                    Mem2ds = in_mem2ds;
                    Mem3ds = in_mem3ds;
                }

                // Get Loads input
                gh_types = new List<GH_ObjectWrapper>();
                List<GsaLoad> in_loads = new List<GsaLoad>();
                List<GsaGridPlaneSurface> in_gps = new List<GsaGridPlaneSurface>();
                if (DA.GetDataList(6, gh_types))
                {
                    for (int i = 0; i < gh_types.Count; i++)
                    {
                        gh_typ = gh_types[i];
                        if (gh_typ.Value is GsaLoadGoo)
                        {
                            GsaLoad gsaload = null;
                            gh_typ.CastTo(ref gsaload);
                            in_loads.Add(gsaload);
                        }
                        else if (gh_typ.Value is GsaGridPlaneSurfaceGoo)
                        {
                            GsaGridPlaneSurface gsaGPS = new GsaGridPlaneSurface();
                            gh_typ.CastTo(ref gsaGPS);
                            in_gps.Add(gsaGPS);
                        }
                        else
                        {
                            this.Parent.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in Loads input");
                            return;
                        }
                    }
                    Loads = in_loads;
                    GridPlaneSurfaces = in_gps;
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
                            GsaSection gsasection = new GsaSection();
                            gh_typ.CastTo(ref gsasection);
                            in_sect.Add(gsasection);
                        }
                        else if (gh_typ.Value is GsaProp2dGoo)
                        {
                            GsaProp2d gsaprop = new GsaProp2d();
                            gh_typ.CastTo(ref gsaprop);
                            in_prop.Add(gsaprop);
                        }
                        else
                        {
                            this.Parent.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in Property (PA PB) input");
                            return;
                        }
                    }
                    Sections = in_sect;
                    Prop2Ds = in_prop;
                }
                #endregion
            }
            public override void SetData(IGH_DataAccess DA)
            {
                // 👉 Checking for cancellation!
                if (CancellationToken.IsCancellationRequested) return;

                DA.SetData(0, new GsaModelGoo(WorkModel));
                //base.Parent.Message = analysed ? "Analysis complete" : "Model assembled";
            }

            public override void DoWork(Action<string, double> ReportProgress, Action Done)
            {
                #region DoWork
                ReportProgress("Cloning model...", -2);
                GsaModel analysisModel = WorkModel.Clone(); // use Copy when GsaAPI allows to deepclone;
                ReportProgress("Model cloned", -1);
                // Let's work just on the model (not wrapped)
                GsaAPI.Model gsa = analysisModel.Model;
                
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
                    // update counter if new nodes have set ID higher than existing max
                    int existingNodeMaxID = Nodes.Max(x => x.ID); // max ID in new nodes
                    if (existingNodeMaxID > newNodeID)
                        newNodeID = existingNodeMaxID + 1;

                    for (int i = 0; i < Nodes.Count; i++)
                    {
                        if (CancellationToken.IsCancellationRequested) return;
                        ReportProgress("Nodes ", (double)i / (Nodes.Count - 1));

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
                    }
                }
                ReportProgress("Nodes assembled", -2);
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
                    for (int i = 0; i < Elem1ds.Count; i++)
                    {
                        if (CancellationToken.IsCancellationRequested) return;
                        ReportProgress("Elem1D ", (double)i / (Elem1ds.Count - 1));

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
                                nodes.Add(newNodeID, Util.Gsa.ModelNodes.NodeFromPoint(line.PointAtStart));
                                topo.Add(newNodeID);
                                newNodeID++;
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
                                if (element1d.Section.Section != null)
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
                    }
                }
                ReportProgress("Elem1D assembled", -2);

                // Elem2ds
                if (Elem2ds != null)
                {
                    for (int i = 0; i < Elem2ds.Count; i++)
                    {
                        if (CancellationToken.IsCancellationRequested) return;
                        ReportProgress("Elem2D ", (double)i / (Elem2ds.Count - 1));

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
                                    if (element2d.Properties[j].Prop2d != null)
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
                            }
                        }
                    }
                }
                ReportProgress("Elem2D assembled", -2);
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
                    for (int i = 0; i < Mem1ds.Count; i++)
                    {
                        if (CancellationToken.IsCancellationRequested) return;
                        ReportProgress("Mem1D ", (double)i / (Mem1ds.Count - 1));

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
                                if (member1d.Section.Section != null)
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
                    }
                }
                ReportProgress("Mem1D assembled", -2);

                // Mem2ds
                if (Mem2ds != null)
                {
                    for (int i = 0; i < Mem2ds.Count; i++)
                    {
                        if (CancellationToken.IsCancellationRequested) return;
                        ReportProgress("Mem2D ", (double)i / (Mem2ds.Count - 1));

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
                            }


                            // Loop through the voidtopology list
                            if (member2d.VoidTopology != null)
                            {
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
                                    }
                                }
                            }

                            // Loop through the inclusion lines topology list  
                            if (member2d.IncLinesTopology != null)
                            {
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
                                    }
                                }
                            }

                            // Loop through the inclucion point topology list
                            if (member2d.InclusionPoints != null)
                            {
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
                                }
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
                                if (member2d.Property.Prop2d != null)
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
                    }
                }
                ReportProgress("Mem2D assembled", -2);

                // Mem3ds
                if (Mem3ds != null)
                {
                    for (int i = 0; i < Mem3ds.Count; i++)
                    {
                        if (CancellationToken.IsCancellationRequested) return;
                        ReportProgress("Mem3D ", (double)i / (Mem3ds.Count - 1));

                        if (Mem3ds[i] != null)
                        {
                            GsaMember3d member3d = Mem3ds[i];
                            Member apiMember = member3d.Member;

                            // update topology list to fit model nodes
                            string topo = "";

                            // Loop through the face list
                            for (int j = 0; j < member3d.SolidMesh.Faces.Count; j++)
                            {
                                for (int k = 0; k < 3; k++)
                                {
                                    int faceint = 0; 
                                    if (k == 0)
                                        faceint = member3d.SolidMesh.Faces[j].A;
                                    if (k == 1)
                                        faceint = member3d.SolidMesh.Faces[j].B;
                                    if (k == 2)
                                        faceint = member3d.SolidMesh.Faces[j].C;
                                    
                                    // vertex point of current face corner
                                    Point3d pt = member3d.SolidMesh.Vertices[faceint];

                                    // add space if we are not in first iteration
                                    if (k > 0)
                                        topo += " ";

                                    // check point against existing nodes in model
                                    int id = Util.Gsa.ModelNodes.ExistingNodePoint(nodes, pt);
                                    if (id > 0)
                                        topo += id;
                                    else
                                    {
                                        nodes.Add(newNodeID, Util.Gsa.ModelNodes.NodeFromPoint(pt));
                                        topo += newNodeID;
                                        newNodeID++;
                                    }
                                }
                                // add ";" between face lists, unless we are in final iteration
                                if (j != member3d.SolidMesh.Faces.Count - 1)
                                    topo += "; ";
                            }
                            // set topology in api member
                            apiMember.Topology = string.Copy(topo);

                            // Section
                            // to be done

                            // set apimember in dictionary
                            if (member3d.ID > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
                            {
                                mems[member3d.ID] = apiMember;
                            }
                            else
                            {
                                mems.Add(newMemberID, apiMember);
                                newMemberID++;
                            }
                        }
                    }
                }
                ReportProgress("Mem3D assembled", -2);
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

                // lists to keep track of duplicated grid planes and grid surfaces
                Dictionary<Guid, int> gp_guid = new Dictionary<Guid, int>();
                Dictionary<Guid, int> gs_guid = new Dictionary<Guid, int>();

                if (Loads != null)
                {
                    for (int i = 0; i < Loads.Count; i++)
                    {
                        if (CancellationToken.IsCancellationRequested) return;
                        ReportProgress("Loads ", (double)i / (Loads.Count - 1));
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
                                    // ### PARAMETERS ###
                                    // the load class is wrapping the grid plane surface class
                                    // grid plane surface is holding GsaAPI axis, grid plane and grid surface, 
                                    // for clarity of the code they are declared separately below
                                    GsaGridPointLoad gridptref = load.PointLoad;
                                    GridPointLoad gridPtLoad = gridptref.GridPointLoad;
                                    GsaGridPlaneSurface gridplnsrf = gridptref.GridPlaneSurface;
                                    GridSurface gridsrf = gridplnsrf.GridSurface;
                                    GridPlane gridpln = gridplnsrf.GridPlane;
                                    Axis axis = gridplnsrf.Axis;

                                    // - grid load references a grid surface number
                                    // -- grid surface references a grid plane number
                                    // --- grid plane references an Axis number
                                    // toggle through the members in reverse order, set/add to model in each step

                                    // ### AXIS ###
                                    // set axis property in grid plane, add/set axis in model

                                    // see if AXIS has been set
                                    if (gridplnsrf.AxisID > 0)
                                    {
                                        // assign the axis property to the grid plane (in the load)
                                        gridpln.AxisProperty = gridplnsrf.AxisID;
                                        // set the axis in model
                                        gsa.SetAxis(gridplnsrf.AxisID, axis);
                                    }
                                    else
                                    {
                                        // check if there's already an axis with same properties in the model:
                                        int axID = Util.Gsa.ModelAxis.ExistingAxis(gsa.Axes(), axis);
                                        if (axID > 0)
                                            gridpln.AxisProperty = axID; // set the id if axis exist
                                        else
                                        {
                                            // else add the axis to the model and assign the new axis number to the grid plane
                                            gridpln.AxisProperty = gsa.AddAxis(axis);
                                        }
                                    }

                                    // ### GRID PLANE ###
                                    // set grid plane number in grid surface, add/set grid plane in model

                                    // see if grid plane ID has been set by user
                                    if (gridplnsrf.GridPlaneID > 0)
                                    {
                                        // assign the grid plane number set by user in the load's grid surface
                                        gridsrf.GridPlane = gridplnsrf.GridPlaneID;
                                        // set grid plane in model
                                        gsa.SetGridPlane(gridplnsrf.GridPlaneID, gridpln);
                                    }
                                    else
                                    {
                                        // check if grid plane has already been added to model by other loads
                                        if (gp_guid.ContainsKey(gridplnsrf.GridPlaneGUID))
                                        {
                                            gp_guid.TryGetValue(gridplnsrf.GridPlaneGUID, out int gpID);
                                            // if guid exist in our dictionary it has been added to the model and we just assign the value to the grid surface
                                            gridsrf.GridPlane = gpID;
                                        }
                                        else
                                        {
                                            // if it does not exist we add the grid plane to the model
                                            int gpID = gsa.AddGridPlane(gridpln);
                                            // then set the id to grid surface
                                            gridsrf.GridPlane = gpID;
                                            // and add it to the our list of grid planes 
                                            gp_guid.Add(gridplnsrf.GridPlaneGUID, gpID);
                                        }
                                    }

                                    // ### GRID SURFACE ###
                                    // set the surface number in the load, add/set the surface in the model
                                    
                                    // see if grid surface ID has been set by user
                                    if (gridplnsrf.GridSurfaceID > 0)
                                    {
                                        // assign the grid surface number set by user in the load
                                        gridPtLoad.GridSurface = gridplnsrf.GridSurfaceID;
                                        // set the grid surface in model
                                        gsa.SetGridSurface(gridplnsrf.GridSurfaceID, gridsrf);
                                    }
                                    else
                                    {
                                        // check if grid surface has already been added to model by other loads
                                        if (gs_guid.ContainsKey(gridplnsrf.GridSurfaceGUID))
                                        {
                                            gs_guid.TryGetValue(gridplnsrf.GridSurfaceGUID, out int gsID);
                                            // if guid exist in our dictionary it has been added to the model 
                                            // and we just assign the value to the load
                                            gridPtLoad.GridSurface = gsID;
                                        }
                                        else
                                        {
                                            // if it does not exist we add the grid surface to the model
                                            int gsID = gsa.AddGridSurface(gridsrf);
                                            // then set the id to load 
                                            gridPtLoad.GridSurface = gsID;
                                            // and add it to the our list of grid surfaces
                                            gs_guid.Add(gridplnsrf.GridSurfaceGUID, gsID);
                                        }
                                    }
                                    // add the load to our list of loads to be set later
                                    gridPointLoads.Add(gridPtLoad);
                                    break;
                                case GsaLoad.LoadTypes.GridLine:
                                    // ### PARAMETERS ###
                                    // the load class is wrapping the grid plane surface class
                                    // grid plane surface is holding GsaAPI axis, grid plane and grid surface, 
                                    // for clarity of the code they are declared separately below
                                    GsaGridLineLoad gridlnref = load.LineLoad;
                                    GridLineLoad gridLnLoad = gridlnref.GridLineLoad;
                                    gridplnsrf = gridlnref.GridPlaneSurface;
                                    gridsrf = gridplnsrf.GridSurface;
                                    gridpln = gridplnsrf.GridPlane;
                                    axis = gridplnsrf.Axis;

                                    // - grid load references a grid surface number
                                    // -- grid surface references a grid plane number
                                    // --- grid plane references an Axis number
                                    // toggle through the members in reverse order, set/add to model in each step

                                    // ### AXIS ###
                                    // set axis property in grid plane, add/set axis in model

                                    // see if AXIS has been set
                                    if (gridplnsrf.AxisID > 0)
                                    {
                                        // assign the axis property to the grid plane (in the load)
                                        gridpln.AxisProperty = gridplnsrf.AxisID;
                                        // set the axis in model
                                        gsa.SetAxis(gridplnsrf.AxisID, axis);
                                    }
                                    else
                                    {
                                        // check if there's already an axis with same properties in the model:
                                        int axID = Util.Gsa.ModelAxis.ExistingAxis(gsa.Axes(), axis);
                                        if (axID > 0)
                                            gridpln.AxisProperty = axID; // set the id if axis exist
                                        else
                                        {
                                            // else add the axis to the model and assign the new axis number to the grid plane
                                            gridpln.AxisProperty = gsa.AddAxis(axis);
                                        }
                                    }

                                    // ### GRID PLANE ###
                                    // set grid plane number in grid surface, add/set grid plane in model

                                    // see if grid plane ID has been set by user
                                    if (gridplnsrf.GridPlaneID > 0)
                                    {
                                        // assign the grid plane number set by user in the load's grid surface
                                        gridsrf.GridPlane = gridplnsrf.GridPlaneID;
                                        // set grid plane in model
                                        gsa.SetGridPlane(gridplnsrf.GridPlaneID, gridpln);
                                    }
                                    else
                                    {
                                        // check if grid plane has already been added to model by other loads
                                        if (gp_guid.ContainsKey(gridplnsrf.GridPlaneGUID))
                                        {
                                            gp_guid.TryGetValue(gridplnsrf.GridPlaneGUID, out int gpID);
                                            // if guid exist in our dictionary it has been added to the model and we just assign the value to the grid surface
                                            gridsrf.GridPlane = gpID;
                                        }
                                        else
                                        {
                                            // if it does not exist we add the grid plane to the model
                                            int gpID = gsa.AddGridPlane(gridpln);
                                            // then set the id to grid surface
                                            gridsrf.GridPlane = gpID;
                                            // and add it to the our list of grid planes 
                                            gp_guid.Add(gridplnsrf.GridPlaneGUID, gpID);
                                        }
                                    }

                                    // ### GRID SURFACE ###
                                    // set the surface number in the load, add/set the surface in the model

                                    // see if grid surface ID has been set by user
                                    if (gridplnsrf.GridSurfaceID > 0)
                                    {
                                        // assign the grid surface number set by user in the load
                                        gridLnLoad.GridSurface = gridplnsrf.GridSurfaceID;
                                        // set the grid surface in model
                                        gsa.SetGridSurface(gridplnsrf.GridSurfaceID, gridsrf);
                                    }
                                    else
                                    {
                                        // check if grid surface has already been added to model by other loads
                                        if (gs_guid.ContainsKey(gridplnsrf.GridSurfaceGUID))
                                        {
                                            gs_guid.TryGetValue(gridplnsrf.GridSurfaceGUID, out int gsID);
                                            // if guid exist in our dictionary it has been added to the model 
                                            // and we just assign the value to the load
                                            gridLnLoad.GridSurface = gsID;
                                        }
                                        else
                                        {
                                            // if it does not exist we add the grid surface to the model
                                            int gsID = gsa.AddGridSurface(gridsrf);
                                            // then set the id to load 
                                            gridLnLoad.GridSurface = gsID;
                                            // and add it to the our list of grid surfaces
                                            gs_guid.Add(gridplnsrf.GridSurfaceGUID, gsID);
                                        }
                                    }
                                    // add the load to our list of loads to be set later
                                    gridLineLoads.Add(gridLnLoad);
                                    break;
                                case GsaLoad.LoadTypes.GridArea:
                                    // ### PARAMETERS ###
                                    // the load class is wrapping the grid plane surface class
                                    // grid plane surface is holding GsaAPI axis, grid plane and grid surface, 
                                    // for clarity of the code they are declared separately below
                                    GsaGridAreaLoad gridarearef = load.AreaLoad;
                                    GridAreaLoad gridALoad = gridarearef.GridAreaLoad;
                                    gridplnsrf = gridarearef.GridPlaneSurface;
                                    gridsrf = gridplnsrf.GridSurface;
                                    gridpln = gridplnsrf.GridPlane;
                                    axis = gridplnsrf.Axis;

                                    // - grid load references a grid surface number
                                    // -- grid surface references a grid plane number
                                    // --- grid plane references an Axis number
                                    // toggle through the members in reverse order, set/add to model in each step

                                    // ### AXIS ###
                                    // set axis property in grid plane, add/set axis in model

                                    // see if AXIS has been set
                                    if (gridplnsrf.AxisID > 0)
                                    {
                                        // assign the axis property to the grid plane (in the load)
                                        gridpln.AxisProperty = gridplnsrf.AxisID;
                                        // set the axis in model
                                        gsa.SetAxis(gridplnsrf.AxisID, axis);
                                    }
                                    else
                                    {
                                        // check if there's already an axis with same properties in the model:
                                        int axID = Util.Gsa.ModelAxis.ExistingAxis(gsa.Axes(), axis);
                                        if (axID > 0)
                                            gridpln.AxisProperty = axID; // set the id if axis exist
                                        else
                                        {
                                            // else add the axis to the model and assign the new axis number to the grid plane
                                            gridpln.AxisProperty = gsa.AddAxis(axis);
                                        }
                                    }

                                    // ### GRID PLANE ###
                                    // set grid plane number in grid surface, add/set grid plane in model

                                    // see if grid plane ID has been set by user
                                    if (gridplnsrf.GridPlaneID > 0)
                                    {
                                        // assign the grid plane number set by user in the load's grid surface
                                        gridsrf.GridPlane = gridplnsrf.GridPlaneID;
                                        // set grid plane in model
                                        gsa.SetGridPlane(gridplnsrf.GridPlaneID, gridpln);
                                    }
                                    else
                                    {
                                        // check if grid plane has already been added to model by other loads
                                        if (gp_guid.ContainsKey(gridplnsrf.GridPlaneGUID))
                                        {
                                            gp_guid.TryGetValue(gridplnsrf.GridPlaneGUID, out int gpID);
                                            // if guid exist in our dictionary it has been added to the model and we just assign the value to the grid surface
                                            gridsrf.GridPlane = gpID;
                                        }
                                        else
                                        {
                                            // if it does not exist we add the grid plane to the model
                                            int gpID = gsa.AddGridPlane(gridpln);
                                            // then set the id to grid surface
                                            gridsrf.GridPlane = gpID;
                                            // and add it to the our list of grid planes 
                                            gp_guid.Add(gridplnsrf.GridPlaneGUID, gpID);
                                        }
                                    }

                                    // ### GRID SURFACE ###
                                    // set the surface number in the load, add/set the surface in the model

                                    // see if grid surface ID has been set by user
                                    if (gridplnsrf.GridSurfaceID > 0)
                                    {
                                        // assign the grid surface number set by user in the load
                                        gridALoad.GridSurface = gridplnsrf.GridSurfaceID;
                                        // set the grid surface in model
                                        gsa.SetGridSurface(gridplnsrf.GridSurfaceID, gridsrf);
                                    }
                                    else
                                    {
                                        // check if grid surface has already been added to model by other loads
                                        if (gs_guid.ContainsKey(gridplnsrf.GridSurfaceGUID))
                                        {
                                            gs_guid.TryGetValue(gridplnsrf.GridSurfaceGUID, out int gsID);
                                            // if guid exist in our dictionary it has been added to the model 
                                            // and we just assign the value to the load
                                            gridALoad.GridSurface = gsID;
                                        }
                                        else
                                        {
                                            // if it does not exist we add the grid surface to the model
                                            int gsID = gsa.AddGridSurface(gridsrf);
                                            // then set the id to load 
                                            gridALoad.GridSurface = gsID;
                                            // and add it to the our list of grid surfaces
                                            gs_guid.Add(gridplnsrf.GridSurfaceGUID, gsID);
                                        }
                                    }
                                    // add the load to our list of loads to be set later
                                    gridAreaLoads.Add(gridALoad);
                                    break;
                            }
                        }
                    }
                }

                if (GridPlaneSurfaces != null)
                {
                    for (int i = 0; i < GridPlaneSurfaces.Count; i++)
                    {
                        if (GridPlaneSurfaces[i] != null)
                        {
                            GsaGridPlaneSurface gps = GridPlaneSurfaces[i];
                            gps.GridPlane.AxisProperty = gsa.AddAxis(gps.Axis);
                            gps.GridSurface.GridPlane = gsa.AddGridPlane(gps.GridPlane);
                            gsa.AddGridSurface(gps.GridSurface);
                        }
                    }
                }
                ReportProgress("Loads assembled", -2);
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
                        // update counter if new nodes have set ID higher than existing max
                        int existingSectionsMaxID = Sections.Max(x => x.ID); // max ID in new 
                        if (existingSectionsMaxID > newSectionID)
                            newSectionID = existingSectionsMaxID + 1;

                        for (int i = 0; i < Sections.Count; i++)
                        {
                            if (CancellationToken.IsCancellationRequested) return;
                            ReportProgress("Sections ", (double)i / (Sections.Count - 1));

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
                        }
                    }
                }
                ReportProgress("Sections assembled", -2);
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
                        // update counter if new nodes have set ID higher than existing max
                        int existingProp2dMaxID = Prop2Ds.Max(x => x.ID); // max ID in new 
                        if (existingProp2dMaxID > newProp2dID)
                            newProp2dID = existingProp2dMaxID + 1;

                        for (int i = 0; i < Prop2Ds.Count; i++)
                        {
                            if (CancellationToken.IsCancellationRequested) return;
                            ReportProgress("Prop2D ", (double)i / (Prop2Ds.Count - 1));

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
                        }
                    }
                }
                ReportProgress("Prop2D assembled", -2);
                #endregion



                #region set stuff in model
                if (CancellationToken.IsCancellationRequested) return;
                //ReportProgress("Assemble model", -2);
                ReadOnlyDictionary<int, Node> setnodes = new ReadOnlyDictionary<int, Node>(nodes);
                gsa.SetNodes(setnodes);

                ReadOnlyDictionary<int, Element> setelem = new ReadOnlyDictionary<int, Element>(elems);
                gsa.SetElements(setelem);

                ReadOnlyDictionary<int, Member> setmem = new ReadOnlyDictionary<int, Member>(mems);
                gsa.SetMembers(setmem);

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

                ReadOnlyDictionary<int, Section> setsect = new ReadOnlyDictionary<int, Section>(sections);
                gsa.SetSections(setsect);

                ReadOnlyDictionary<int, Prop2D> setpr2d = new ReadOnlyDictionary<int, Prop2D>(prop2ds);
                gsa.SetProp2Ds(setpr2d);
                ReportProgress("Model assembled", -2);
                #endregion

                #region meshing
                // Create elements from members
                ReportProgress("Meshing", 0);
                //gsa.CreateElementsFromMembers();
                ReportProgress("Model meshed", -2);
                #endregion

                #region analysis
                //analysis
                IReadOnlyDictionary<int, AnalysisTask> gsaTasks = gsa.AnalysisTasks();
                if (gsaTasks.Count < 1)
                {
                    ReportProgress("Model contains no Analysis Tasks", -255);
                    ReportProgress("Model assembled", -1);
                }
                else
                {
                    foreach (KeyValuePair<int, AnalysisTask> task in gsaTasks)
                    {
                        if (CancellationToken.IsCancellationRequested) return;
                        ReportProgress("Analysing Task " + task.Key.ToString(), -2);

                        if (!(gsa.Analyse(task.Key)))
                        {
                            ReportProgress("Warning Analysis Case " + task.Key + " could not be analysed", -10);
                        }
                    }
                    ReportProgress("Model analysed", -1);
                }

                #endregion
                WorkModel.Model = gsa;
                Done();
                #endregion
            }
        }
    }
}