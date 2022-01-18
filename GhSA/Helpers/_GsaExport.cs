using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using Rhino.Geometry;
using GhSA.Parameters;
using System.Threading;
using System.Collections.Concurrent;
using Grasshopper.Kernel.Parameters;

namespace GhSA.Util.Gsa.ToGSA
{
    public class Models
    {
        public static GsaModel MergeModel(List<GsaModel> models)
        {
            if (models != null)
            {
                if (models.Count > 1)
                {
                    GsaModel model = new GsaModel();
                    models.Reverse();
                    for (int i = 0; i < models.Count - 1; i++)
                    {
                        model = MergeModel(model, models[i].Clone());
                    }
                    GsaModel clone = models[models.Count - 1].Clone();
                    clone = MergeModel(clone, model);
                    return clone;
                }
                else
                {
                    if (models.Count > 0)
                        return models[0].Clone();
                }
            }
            return null;
        }

        public static GsaModel MergeModel(GsaModel mainModel, GsaModel appendModel)
        {
            // open the copyfrom model
            Model model = appendModel.Model;

            // get dictionaries from model
            //IReadOnlyDictionary<int, Node> nDict = model.Nodes();
            //IReadOnlyDictionary<int, Element> eDict = model.Elements();
            //IReadOnlyDictionary<int, Member> mDict = model.Members();
            //IReadOnlyDictionary<int, Section> sDict = model.Sections();
            //IReadOnlyDictionary<int, Prop2D> pDict = model.Prop2Ds();
            ConcurrentDictionary<int, Node> nDict = new ConcurrentDictionary<int, Node>(model.Nodes());
            ConcurrentDictionary<int, Element> eDict = new ConcurrentDictionary<int, Element>(model.Elements());
            ConcurrentDictionary<int, Member> mDict = new ConcurrentDictionary<int, Member>(model.Members());
            ConcurrentDictionary<int, Section> sDict = new ConcurrentDictionary<int, Section>(model.Sections());
            ConcurrentDictionary<int, Prop2D> pDict = new ConcurrentDictionary<int, Prop2D>(model.Prop2Ds());

            // get nodes
            List<GsaNodeGoo> goonodes = Util.Gsa.FromGSA.GetNodes(nDict);
            // convert from Goo-type
            List<GsaNode> nodes = goonodes.Select(n => n.Value).ToList();
            // change all members in List's ID to 0;
            nodes.Select(c => { c.ID = 0; return c; }).ToList();

            // get elements
            Tuple<List<GsaElement1dGoo>, List<GsaElement2dGoo>, List<GsaElement3dGoo>> elementTuple
                = Util.Gsa.FromGSA.GetElements(eDict, nDict, sDict, pDict);
            // convert from Goo-type
            List<GsaElement1d> elem1ds = elementTuple.Item1.Select(n => n.Value).ToList();
            // change all members in List's ID to 0;
            elem1ds.Select(c => { c.ID = 0; return c; }).ToList();
            // convert from Goo-type
            List<GsaElement2d> elem2ds = elementTuple.Item2.Select(n => n.Value).ToList();
            // change all members in List's ID to 0;
            foreach (var elem2d in elem2ds)
                elem2d.ID.Select(c => { c = 0; return c; }).ToList();
            // convert from Goo-type
            List<GsaElement3d> elem3ds = elementTuple.Item3.Select(n => n.Value).ToList();
            // change all members in List's ID to 0;
            foreach (var elem3d in elem3ds)
                elem3d.ID.Select(c => { c = 0; return c; }).ToList();

            // get members
            Tuple<List<GsaMember1dGoo>, List<GsaMember2dGoo>, List<GsaMember3dGoo>> memberTuple
                = Util.Gsa.FromGSA.GetMembers(mDict, nDict, sDict, pDict);
            // convert from Goo-type
            List<GsaMember1d> mem1ds = memberTuple.Item1.Select(n => n.Value).ToList();
            // change all members in List's ID to 0;
            mem1ds.Select(c => { c.ID = 0; return c; }).ToList();
            // convert from Goo-type
            List<GsaMember2d> mem2ds = memberTuple.Item2.Select(n => n.Value).ToList();
            // change all members in List's ID to 0;
            mem2ds.Select(c => { c.ID = 0; return c; }).ToList();

            // get properties
            List<GsaSectionGoo> goosections = FromGSA.GetSections(sDict, model.AnalysisMaterials());
            // convert from Goo-type
            List<GsaSection> sections = goosections.Select(n => n.Value).ToList();
            // change all members in List's ID to 0;
            sections.Select(c => { c.ID = 0; return c; }).ToList();
            List<GsaProp2dGoo> gooprop2Ds = FromGSA.GetProp2ds(pDict, model.AnalysisMaterials());
            // convert from Goo-type
            List<GsaProp2d> prop2Ds = gooprop2Ds.Select(n => n.Value).ToList();
            // change all members in List's ID to 0;
            prop2Ds.Select(c => { c.ID = 0; return c; }).ToList();

            // get loads
            List<GsaLoadGoo> gooloads = new List<GsaLoadGoo>();
            gooloads.AddRange(FromGSA.GetGravityLoads(model.GravityLoads()));
            gooloads.AddRange(FromGSA.GetNodeLoads(model));
            gooloads.AddRange(FromGSA.GetBeamLoads(model.BeamLoads()));
            gooloads.AddRange(FromGSA.GetFaceLoads(model.FaceLoads()));

            IReadOnlyDictionary<int, GridSurface> srfDict = model.GridSurfaces();
            IReadOnlyDictionary<int, GridPlane> plnDict = model.GridPlanes();
            IReadOnlyDictionary<int, Axis> axDict = model.Axes();

            gooloads.AddRange(FromGSA.GetGridPointLoads(model.GridPointLoads(), srfDict, plnDict, axDict));
            gooloads.AddRange(FromGSA.GetGridLineLoads(model.GridLineLoads(), srfDict, plnDict, axDict));
            gooloads.AddRange(FromGSA.GetGridAreaLoads(model.GridAreaLoads(), srfDict, plnDict, axDict));
            List<GsaLoad> loads = gooloads.Select(n => n.Value).ToList();

            // get grid plane surfaces
            List<GsaGridPlaneSurfaceGoo> gpsgoo = new List<GsaGridPlaneSurfaceGoo>();
            foreach (int key in srfDict.Keys)
                gpsgoo.Add(new GsaGridPlaneSurfaceGoo(Util.Gsa.FromGSA.GetGridPlaneSurface(srfDict, plnDict, axDict, key)));
            // convert from Goo-type
            List<GsaGridPlaneSurface> gps = gpsgoo.Select(n => n.Value).ToList();

            // return new assembled model
            mainModel.Model = Assemble.AssembleModel(mainModel, nodes, elem1ds, elem2ds, elem3ds, mem1ds, mem2ds, null, sections, prop2Ds, loads, gps);
            return mainModel;
        }
    }
    public class Assemble
    {
        /// <summary>
        /// Method for assembling GSA model from Members only to create geometry
        /// </summary>
        /// <param name="member3Ds">3D Members</param>
        /// <param name="member2Ds">2D Members</param>
        /// <param name="member1Ds">1D Members</param>
        /// <param name="nodes">Nodes</param>
        /// <returns></returns>
        public static Model AssembleModel(List<GsaMember3d> member3Ds = null, List<GsaMember2d> member2Ds = null, List<GsaMember1d> member1Ds = null, List<GsaNode> nodes = null)
        {
            // new model to set members in
            Model gsa = new Model();

            // list of topology nodes
            List<Node> gsanodes = new List<Node>();
            if (nodes != null)
                gsanodes = nodes.ConvertAll(x => x.API_Node);

            // counter for creating nodes
            int id = 1;

            // Create list of members to write to
            List<Member> mems = new List<Member>();
            
            // add converted 1D members
            mems.AddRange(Members.ConvertMember1D(member1Ds, ref gsanodes, ref id));

            // add converted 2D members
            mems.AddRange(Members.ConvertMember2D(member2Ds, ref gsanodes, ref id));

            // add converted 3D members
            mems.AddRange(Members.ConvertMember3D(member3Ds, ref gsanodes, ref id));

            #region create model
            Dictionary<int, Node> nodeDic = gsanodes
                .Select((s, index) => new { s, index })
                .ToDictionary(x => x.index + 1, x => x.s);
            ReadOnlyDictionary<int, Node> setnodes = new ReadOnlyDictionary<int, Node>(nodeDic);
            gsa.SetNodes(setnodes);

            Dictionary<int, Member> memDic = mems
                .Select((s, index) => new { s, index })
                .ToDictionary(x => x.index + 1, x => x.s);
            ReadOnlyDictionary<int, Member> setmem = new ReadOnlyDictionary<int, Member>(memDic);
            gsa.SetMembers(setmem);
            #endregion

            return gsa;
        }

        /// <summary>
        /// Method to assemble full GSA model 
        /// </summary>
        /// <param name="model">Existing models to be merged</param>
        /// <param name="nodes">List of nodes with properties like support conditions</param>
        /// <param name="elem1ds">List of 1D elements. Nodes at end-points will automatically be added to the model, using existing nodes in model within tolerance. Section will automatically be added to model</param>
        /// <param name="elem2ds">List of 2D elements. Nodes at mesh-verticies will automatically be added to the model, using existing nodes in model within tolerance. Prop2d will automatically be added to model</param>
        /// <param name="elem3ds">List of 3D elements. Nodes at mesh-verticies will automatically be added to the model, using existing nodes in model within tolerance</param>
        /// <param name="mem1ds">List of 1D members. Topology nodes will automatically be added to the model, using existing nodes in model within tolerance. Section will automatically be added to model</param>
        /// <param name="mem2ds">List of 2D members. Topology nodes will automatically be added to the model, using existing nodes in model within tolerance. Prop2d will automatically be added to model</param>
        /// <param name="mem3ds">List of 3D members. Topology nodes will automatically be added to the model, using existing nodes in model within tolerance</param>
        /// <param name="sections">List of Sections</param>
        /// <param name="prop2Ds">List of 2D Properties</param>
        /// <param name="loads">List of Loads. For Grid loads the Axis, GridPlane and GridSurface will automatically be added to the model using existing objects where possible within tolerance.</param>
        /// <param name="gridPlaneSurfaces">List of GridPlaneSurfaces</param>
        /// <param name="workerInstance">Optional input for AsyncComponents</param>
        /// <param name="ReportProgress">Optional input for AsyncComponents</param>
        /// <returns></returns>
        public static Model AssembleModel(GsaModel model, List<GsaNode> nodes, 
            List<GsaElement1d> elem1ds, List<GsaElement2d> elem2ds, List<GsaElement3d> elem3ds,
            List<GsaMember1d> mem1ds, List<GsaMember2d> mem2ds, List<GsaMember3d> mem3ds,
            List<GsaSection> sections, List<GsaProp2d> prop2Ds, 
            List<GsaLoad> loads, List<GsaGridPlaneSurface> gridPlaneSurfaces,
            GrasshopperAsyncComponent.WorkerInstance workerInstance = null,
            Action<string, double> ReportProgress = null
            )
        {
            // Set model to work on
            Model gsa = new Model();
            if (model != null)
                gsa = model.Model;

            #region Nodes
            // ### Nodes ###
            // We take out the existing nodes in the model and work on that dictionary

            // Get existing nodes
            IReadOnlyDictionary<int, Node> gsaNodes = gsa.Nodes();
            Dictionary<int, Node> apinodes = gsaNodes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Get existing axes
            IReadOnlyDictionary<int, Axis> gsaAxes = gsa.Axes();
            Dictionary<int, Axis> apiaxes = gsaAxes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Set / add nodes to dictionary
            Nodes.ConvertNode(nodes, ref apinodes, ref apiaxes, workerInstance, ReportProgress);
            #endregion

            #region Properties
            // ### Sections ###
            // list to keep track of duplicated sextions
            Dictionary<Guid, int> sections_guid = new Dictionary<Guid, int>();
            
            // Get existing sections
            IReadOnlyDictionary<int, Section> gsaSections = gsa.Sections();
            Dictionary<int, Section> apisections = gsaSections.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            // add / set sections
            Sections.ConvertSection(sections, ref apisections, ref sections_guid, workerInstance, ReportProgress);

            // ### Prop2ds ###
            // list to keep track of duplicated sextions
            Dictionary<Guid, int> prop2d_guid = new Dictionary<Guid, int>();
            // Get existing prop2ds
            IReadOnlyDictionary<int, Prop2D> gsaProp2ds = gsa.Prop2Ds();
            Dictionary<int, Prop2D> apiprop2ds = gsaProp2ds.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            // add / set prop2ds
            Prop2ds.ConvertProp2d(prop2Ds, ref apiprop2ds, ref prop2d_guid, workerInstance, ReportProgress);
            #endregion

            #region Elements
            // ### Elements ###
            // We take out the existing elements in the model and work on that dictionary

            // Get existing elements
            IReadOnlyDictionary<int, Element> gsaElems = gsa.Elements();
            Dictionary<int, Element> elems = gsaElems.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // create a counter for creating new elements
            int newElementID = (elems.Count > 0) ? elems.Keys.Max() + 1 : 1; //checking the existing model

            // as both elem1d and elem2d will be set in the same table, we check the highest ID that may have
            // been set in the incoming elements and start appending from there to avoid accidentially overwriting 
            if (elem1ds != null)
            {
                if (elem1ds.Count > 0)
                {
                    int existingElem1dMaxID = elem1ds.Max(x => x.ID); // max ID in new Elem1ds
                    if (existingElem1dMaxID > newElementID)
                        newElementID = existingElem1dMaxID + 1;
                }
            }
            if (elem2ds != null)
            {
                if (elem2ds.Count > 0)
                {
                    int existingElem2dMaxID = elem2ds.Max(x => x.ID.Max()); // max ID in new Elem2ds
                    if (existingElem2dMaxID > newElementID)
                        newElementID = existingElem2dMaxID + 1;
                }
            }
            
            // Set / add 1D elements to dictionary
            Elements.ConvertElement1D(elem1ds, ref elems, ref newElementID, ref apinodes, ref apisections, ref sections_guid, workerInstance, ReportProgress);

            // Set / add 2D elements to dictionary
            Elements.ConvertElement2D(elem2ds, ref elems, ref newElementID, ref apinodes, ref apiprop2ds, ref prop2d_guid, workerInstance, ReportProgress);

            // Set / add 3D elements to dictionary
            Elements.ConvertElement3D(elem3ds, ref elems, ref newElementID, ref apinodes, workerInstance, ReportProgress);
            #endregion

            #region Members
            // ### Members ###
            // We take out the existing members in the model and work on that dictionary

            // Get existing members
            IReadOnlyDictionary<int, Member> gsaMems = gsa.Members();
            Dictionary<int, Member> mems = gsaMems.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // create a counter for creating new members
            int newMemberID = (mems.Count > 0) ? mems.Keys.Max() + 1 : 1; //checking the existing model

            // as both mem1d, mem2d and mem3dwill be set in the same table, we check the highest ID that may have
            // been set in the incoming elements and start appending from there to avoid accidentially overwriting 
            if (mem1ds != null)
            {
                if (mem1ds.Count > 0)
                {
                    int existingMem1dMaxID = mem1ds.Max(x => x.ID); // max ID in new Elem1ds
                    if (existingMem1dMaxID > newMemberID)
                        newMemberID = existingMem1dMaxID + 1;
                }
            }

            if (mem2ds != null)
            {
                if (mem2ds.Count > 0)
                {
                    int existingMem2dMaxID = mem2ds.Max(x => x.ID); // max ID in new Elem2ds
                    if (existingMem2dMaxID > newMemberID)
                        newMemberID = existingMem2dMaxID + 1;
                }
            }

            if (mem3ds != null)
            {
                if (mem3ds.Count > 0)
                {
                    int existingMem3dMaxID = mem3ds.Max(x => x.ID); // max ID in new Elem2ds
                    if (existingMem3dMaxID > newMemberID)
                        newMemberID = existingMem3dMaxID + 1;
                }
            }

            // Set / add 1D members to dictionary
            Members.ConvertMember1D(mem1ds, ref mems, ref newMemberID, ref apinodes, ref apisections, ref sections_guid, workerInstance, ReportProgress);

            // Set / add 2D members to dictionary
            Members.ConvertMember2D(mem2ds, ref mems, ref newMemberID, ref apinodes, ref apiprop2ds, ref prop2d_guid, workerInstance, ReportProgress);

            // Set / add 3D members to dictionary
            Members.ConvertMember3D(mem3ds, ref mems, ref newMemberID, ref apinodes, workerInstance, ReportProgress);
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
            
            IReadOnlyDictionary<int, GridPlane> gsaGPln = gsa.GridPlanes();
            Dictionary<int, GridPlane> apiGridPlanes = gsaGPln.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            IReadOnlyDictionary<int, GridSurface> gsaGSrf = gsa.GridSurfaces();
            Dictionary<int, GridSurface> apiGridSurfaces = gsaGSrf.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // lists to keep track of duplicated grid planes and grid surfaces
            Dictionary<Guid, int> gp_guid = new Dictionary<Guid, int>();
            Dictionary<Guid, int> gs_guid = new Dictionary<Guid, int>();

            // Set / add Grid plane surfaces - do this first to set any GridPlane and GridSurfaces with IDs.
            Loads.ConvertGridPlaneSurface(gridPlaneSurfaces, ref apiaxes, ref apiGridPlanes, ref apiGridSurfaces,
                ref gp_guid, ref gs_guid, workerInstance, ReportProgress);

            // Set / add loads to lists
            Loads.ConvertLoad(loads, ref gravityLoads, ref nodeLoads_node, ref nodeLoads_displ, ref nodeLoads_settle,
                ref beamLoads, ref faceLoads, ref gridPointLoads, ref gridLineLoads, ref gridAreaLoads,
                ref apiaxes, ref apiGridPlanes, ref apiGridSurfaces, ref gp_guid, ref gs_guid, 
                workerInstance, ReportProgress);
            #endregion

            

            #region set stuff in model
            if (workerInstance != null)
            {
                if (workerInstance.CancellationToken.IsCancellationRequested) return null;
            }
            //nodes
            ReadOnlyDictionary<int, Node> setnodes = new ReadOnlyDictionary<int, Node>(apinodes);
            gsa.SetNodes(setnodes);
            //elements
            ReadOnlyDictionary<int, Element> setelem = new ReadOnlyDictionary<int, Element>(elems);
            gsa.SetElements(setelem);
            //members
            ReadOnlyDictionary<int, Member> setmem = new ReadOnlyDictionary<int, Member>(mems);
            gsa.SetMembers(setmem);
            //gravity load
            ReadOnlyCollection<GravityLoad> setgrav = new ReadOnlyCollection<GravityLoad>(gravityLoads);
            gsa.AddGravityLoads(setgrav);
            //node loads
            ReadOnlyCollection<NodeLoad> setnode_disp = new ReadOnlyCollection<NodeLoad>(nodeLoads_displ);
            gsa.AddNodeLoads(NodeLoadType.APPL_DISP, setnode_disp);
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
            //axes
            ReadOnlyDictionary<int, Axis> setax = new ReadOnlyDictionary<int, Axis>(apiaxes);
            gsa.SetAxes(setax);
            //gridplanes
            ReadOnlyDictionary<int, GridPlane> setgp = new ReadOnlyDictionary<int, GridPlane>(apiGridPlanes);
            gsa.SetGridPlanes(setgp);
            //gridsurfaces
            ReadOnlyDictionary<int, GridSurface> setgs = new ReadOnlyDictionary<int, GridSurface>(apiGridSurfaces);
            gsa.SetGridSurfaces(setgs);
            //sections
            ReadOnlyDictionary<int, Section> setsect = new ReadOnlyDictionary<int, Section>(apisections);
            gsa.SetSections(setsect);
            //prop2ds
            ReadOnlyDictionary<int, Prop2D> setpr2d = new ReadOnlyDictionary<int, Prop2D>(apiprop2ds);
            gsa.SetProp2Ds(setpr2d);


            if (workerInstance != null)
            {
                ReportProgress("Model assembled", -2);
            }
            #endregion

            return gsa;
        }
    }
}
