using GsaAPI;
using GsaGH.Parameters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;

namespace GsaGH.Util.Gsa.ToGSA
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
            ConcurrentDictionary<int, Node> nDict = new ConcurrentDictionary<int, Node>(model.Nodes());
            ConcurrentDictionary<int, Element> eDict = new ConcurrentDictionary<int, Element>(model.Elements());
            ConcurrentDictionary<int, Member> mDict = new ConcurrentDictionary<int, Member>(model.Members());
            ConcurrentDictionary<int, Section> sDict = new ConcurrentDictionary<int, Section>(model.Sections());
            ConcurrentDictionary<int, Prop2D> pDict = new ConcurrentDictionary<int, Prop2D>(model.Prop2Ds());
            ConcurrentDictionary<int, Prop3D> p3Dict = new ConcurrentDictionary<int, Prop3D>(model.Prop3Ds());
            ConcurrentDictionary<int, AnalysisMaterial> amDict = new ConcurrentDictionary<int, AnalysisMaterial>(model.AnalysisMaterials());

            // get nodes
            ConcurrentBag<GsaNodeGoo> goonodes = Util.Gsa.FromGSA.GetNodes(nDict, LengthUnit.Meter);
            // convert from Goo-type
            List<GsaNode> nodes = goonodes.Select(n => n.Value).ToList();
            // change all members in List's ID to 0;
            nodes.Select(c => { c.ID = 0; return c; }).ToList();

            // get elements
            Tuple<ConcurrentBag<GsaElement1dGoo>, ConcurrentBag<GsaElement2dGoo>, ConcurrentBag<GsaElement3dGoo>> elementTuple
                = Util.Gsa.FromGSA.GetElements(eDict, nDict, sDict, pDict, p3Dict, amDict, LengthUnit.Meter);
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
            Tuple<ConcurrentBag<GsaMember1dGoo>, ConcurrentBag<GsaMember2dGoo>, ConcurrentBag<GsaMember3dGoo>> memberTuple
                = Util.Gsa.FromGSA.GetMembers(mDict, nDict, LengthUnit.Meter, sDict, pDict);
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
            List<GsaProp3dGoo> gooprop3Ds = FromGSA.GetProp3ds(p3Dict, model.AnalysisMaterials());
            // convert from Goo-type
            List<GsaProp3d> prop3Ds = gooprop3Ds.Select(n => n.Value).ToList();
            // change all members in List's ID to 0;
            prop3Ds.Select(c => { c.ID = 0; return c; }).ToList();

            // get loads
            List<GsaLoadGoo> gooloads = new List<GsaLoadGoo>();
            gooloads.AddRange(FromGSA.GetGravityLoads(model.GravityLoads()));
            gooloads.AddRange(FromGSA.GetNodeLoads(model));
            gooloads.AddRange(FromGSA.GetBeamLoads(model.BeamLoads()));
            gooloads.AddRange(FromGSA.GetFaceLoads(model.FaceLoads()));

            IReadOnlyDictionary<int, GridSurface> srfDict = model.GridSurfaces();
            IReadOnlyDictionary<int, GridPlane> plnDict = model.GridPlanes();
            IReadOnlyDictionary<int, Axis> axDict = model.Axes();

            gooloads.AddRange(FromGSA.GetGridPointLoads(model.GridPointLoads(), srfDict, plnDict, axDict, LengthUnit.Meter));
            gooloads.AddRange(FromGSA.GetGridLineLoads(model.GridLineLoads(), srfDict, plnDict, axDict, LengthUnit.Meter));
            gooloads.AddRange(FromGSA.GetGridAreaLoads(model.GridAreaLoads(), srfDict, plnDict, axDict, LengthUnit.Meter));
            List<GsaLoad> loads = gooloads.Select(n => n.Value).ToList();

            // get grid plane surfaces
            List<GsaGridPlaneSurfaceGoo> gpsgoo = new List<GsaGridPlaneSurfaceGoo>();
            foreach (int key in srfDict.Keys)
                gpsgoo.Add(new GsaGridPlaneSurfaceGoo(Util.Gsa.FromGSA.GetGridPlaneSurface(srfDict, plnDict, axDict, key, LengthUnit.Meter)));
            // convert from Goo-type
            List<GsaGridPlaneSurface> gps = gpsgoo.Select(n => n.Value).ToList();

            // return new assembled model
            mainModel.Model = Assemble.AssembleModel(mainModel, nodes, elem1ds, elem2ds, elem3ds, mem1ds, mem2ds, null, sections, prop2Ds, prop3Ds, loads, gps, null, null, LengthUnit.Meter);
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
        public static Model AssembleModel(LengthUnit lengthUnit, List<GsaMember3d> member3Ds = null, List<GsaMember2d> member2Ds = null, List<GsaMember1d> member1Ds = null, 
            List<GsaNode> nodes = null)
        {
            // new model to set members in
            Model gsa = new Model();

            // list of topology nodes
            List<Node> gsanodes = new List<Node>();
            if (nodes != null)
            {
                gsanodes = nodes.ConvertAll(x => x.GetApiNodeToUnit(lengthUnit));
            }

            // counter for creating nodes
            int id = 1;

            // Create list of members to write to
            List<Member> mems = new List<Member>();
            
            // add converted 1D members
            mems.AddRange(Members.ConvertMember1D(member1Ds, ref gsanodes, ref id, lengthUnit));

            // add converted 2D members
            mems.AddRange(Members.ConvertMember2D(member2Ds, ref gsanodes, ref id, lengthUnit));

            // add converted 3D members
            mems.AddRange(Members.ConvertMember3D(member3Ds, ref gsanodes, ref id, lengthUnit));

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
            List<GsaSection> sections, List<GsaProp2d> prop2Ds, List<GsaProp3d> prop3Ds,
            List<GsaLoad> loads, List<GsaGridPlaneSurface> gridPlaneSurfaces,
            List<GsaAnalysisTask> analysisTasks, List<GsaCombinationCase> combinations,
            LengthUnit lengthUnit)
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
            Nodes.ConvertNode(nodes, ref apinodes, ref apiaxes, lengthUnit);
            #endregion

            #region Properties
            // ### Sections ###
            // list to keep track of duplicated sections
            Dictionary<Guid, int> sections_guid = new Dictionary<Guid, int>();
            
            // Get existing sections
            IReadOnlyDictionary<int, Section> gsaSections = gsa.Sections();
            Dictionary<int, Section> apisections = gsaSections.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Get existing materials
            IReadOnlyDictionary<int, AnalysisMaterial> gsaMaterials = gsa.AnalysisMaterials();
            Dictionary<int, AnalysisMaterial> apimaterials = gsaMaterials.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            // list to keep track of duplicated materials
            Dictionary<Guid, int> materials_guid = new Dictionary<Guid, int>();

            // add / set sections
            Sections.ConvertSection(sections, ref apisections, ref sections_guid, ref apimaterials, ref materials_guid);

            // ### Prop2ds ###
            // list to keep track of duplicated sextions
            Dictionary<Guid, int> prop2d_guid = new Dictionary<Guid, int>();
            // Get existing prop2ds
            IReadOnlyDictionary<int, Prop2D> gsaProp2ds = gsa.Prop2Ds();
            Dictionary<int, Prop2D> apiprop2ds = gsaProp2ds.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            // add / set prop2ds
            Prop2ds.ConvertProp2d(prop2Ds, ref apiprop2ds, ref prop2d_guid, ref apimaterials, ref materials_guid);

            // ### Prop3ds ###
            // list to keep track of duplicated sextions
            Dictionary<Guid, int> prop3d_guid = new Dictionary<Guid, int>();
            // Get existing prop2ds
            IReadOnlyDictionary<int, Prop3D> gsaProp3ds = gsa.Prop3Ds();
            Dictionary<int, Prop3D> apiprop3ds = gsaProp3ds.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            // add / set prop2ds
            Prop3ds.ConvertProp3d(prop3Ds, ref apiprop3ds, ref prop3d_guid, ref apimaterials, ref materials_guid);
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
            Elements.ConvertElement1D(elem1ds, ref elems, ref newElementID, ref apinodes, lengthUnit, ref apisections, ref sections_guid, ref apimaterials, ref materials_guid);

            // Set / add 2D elements to dictionary
            Elements.ConvertElement2D(elem2ds, ref elems, ref newElementID, ref apinodes, lengthUnit, ref apiprop2ds, ref prop2d_guid, ref apimaterials, ref materials_guid);

            // Set / add 3D elements to dictionary
            Elements.ConvertElement3D(elem3ds, ref elems, ref newElementID, ref apinodes, lengthUnit);
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
            Members.ConvertMember1D(mem1ds, ref mems, ref newMemberID, ref apinodes, lengthUnit, ref apisections, ref sections_guid, ref apimaterials, ref materials_guid);

            // Set / add 2D members to dictionary
            Members.ConvertMember2D(mem2ds, ref mems, ref newMemberID, ref apinodes, lengthUnit, ref apiprop2ds, ref prop2d_guid, ref apimaterials, ref materials_guid);

            // Set / add 3D members to dictionary
            Members.ConvertMember3D(mem3ds, ref mems, ref newMemberID, ref apinodes, lengthUnit);
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
                ref gp_guid, ref gs_guid, lengthUnit);

            // Set / add loads to lists
            Loads.ConvertLoad(loads, ref gravityLoads, ref nodeLoads_node, ref nodeLoads_displ, ref nodeLoads_settle,
                ref beamLoads, ref faceLoads, ref gridPointLoads, ref gridLineLoads, ref gridAreaLoads,
                ref apiaxes, ref apiGridPlanes, ref apiGridSurfaces, ref gp_guid, ref gs_guid, lengthUnit);
            #endregion
            

            #region set stuff in model
            //nodes
            gsa.SetNodes(new ReadOnlyDictionary<int, Node>(apinodes));
            //elements
            gsa.SetElements(new ReadOnlyDictionary<int, Element>(elems));
            //members
            gsa.SetMembers(new ReadOnlyDictionary<int, Member>(mems));
            //gravity load
            gsa.AddGravityLoads(new ReadOnlyCollection<GravityLoad>(gravityLoads));
            //node loads
            gsa.AddNodeLoads(NodeLoadType.APPL_DISP, new ReadOnlyCollection<NodeLoad>(nodeLoads_displ));
            gsa.AddNodeLoads(NodeLoadType.NODE_LOAD, new ReadOnlyCollection<NodeLoad>(nodeLoads_node));
            gsa.AddNodeLoads(NodeLoadType.SETTLEMENT, new ReadOnlyCollection<NodeLoad>(nodeLoads_settle));
            //beam loads
            gsa.AddBeamLoads(new ReadOnlyCollection<BeamLoad>(beamLoads));
            //face loads
            gsa.AddFaceLoads(new ReadOnlyCollection<FaceLoad>(faceLoads));
            //grid point loads
            gsa.AddGridPointLoads(new ReadOnlyCollection<GridPointLoad>(gridPointLoads));
            //grid line loads
            gsa.AddGridLineLoads(new ReadOnlyCollection<GridLineLoad>(gridLineLoads));
            //grid area loads
            gsa.AddGridAreaLoads(new ReadOnlyCollection<GridAreaLoad>(gridAreaLoads));
            //axes
            gsa.SetAxes(new ReadOnlyDictionary<int, Axis>(apiaxes));
            //gridplanes
            gsa.SetGridPlanes(new ReadOnlyDictionary<int, GridPlane>(apiGridPlanes));
            //gridsurfaces
            gsa.SetGridSurfaces(new ReadOnlyDictionary<int, GridSurface>(apiGridSurfaces));
            //sections
            gsa.SetSections(new ReadOnlyDictionary<int, Section>(apisections));
            //prop2ds
            gsa.SetProp2Ds(new ReadOnlyDictionary<int, Prop2D>(apiprop2ds));
            //prop2ds
            gsa.SetProp3Ds(new ReadOnlyDictionary<int, Prop3D>(apiprop3ds));
            //materials
            if (apimaterials.Count > 0)
            {
                foreach (KeyValuePair<int, AnalysisMaterial> mat in apimaterials)
                    gsa.SetAnalysisMaterial(mat.Key, mat.Value);
            }
            //tasks
            if (analysisTasks != null)
            {
                ReadOnlyDictionary<int, AnalysisTask> existingTasks = gsa.AnalysisTasks();
                foreach (GsaAnalysisTask task in analysisTasks)
                {
                    if (!existingTasks.Keys.Contains(task.ID))
                        task.SetID(gsa.AddAnalysisTask());

                    if (task.Cases == null)
                        task.CreateDeafultCases(gsa);

                    foreach (GsaAnalysisCase ca in task.Cases)
                        gsa.AddAnalysisCaseToTask(task.ID, ca.Name, ca.Description);
                }
            }
            //combinations
            if (combinations != null)
            {
                foreach (GsaCombinationCase co in combinations)
                    gsa.AddCombinationCase(co.Name, co.Description);
            }
            #endregion

            return gsa;
        }
    }
}
