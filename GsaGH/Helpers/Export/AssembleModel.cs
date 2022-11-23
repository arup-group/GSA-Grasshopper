using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Helpers.Export
{
  internal class AssembleModel
  {
    /// <summary>
    /// 
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
    /// <param name="prop3Ds"></param>
    /// <param name="loads">List of Loads. For Grid loads the Axis, GridPlane and GridSurface will automatically be added to the model using existing objects where possible within tolerance.</param>
    /// <param name="gridPlaneSurfaces">List of GridPlaneSurfaces</param>
    /// <param name="analysisTasks"></param>
    /// <param name="combinations"></param>
    /// <param name="modelUnit"></param>
    /// /// <param name="toleranceCoincidentNodes">Set to zero to skip collapsing coincident nodes</param>
    /// <returns></returns>
    internal static Model Assemble(GsaModel model, List<GsaNode> nodes,
        List<GsaElement1d> elem1ds, List<GsaElement2d> elem2ds, List<GsaElement3d> elem3ds,
        List<GsaMember1d> mem1ds, List<GsaMember2d> mem2ds, List<GsaMember3d> mem3ds,
        List<GsaSection> sections, List<GsaProp2d> prop2Ds, List<GsaProp3d> prop3Ds,
        List<GsaLoad> loads, List<GsaGridPlaneSurface> gridPlaneSurfaces,
        List<GsaAnalysisTask> analysisTasks, List<GsaCombinationCase> combinations,
        LengthUnit modelUnit, double toleranceCoincidentNodes)
    {
      // Set model to work on
      Model gsa = new Model();
      if (model != null)
        gsa = model.Model;

      #region Nodes
      // ### Nodes ###
      // We take out the existing nodes in the model and work on that dictionary
      // Get existing nodes
      GsaDictionary<Node> apinodes = new GsaDictionary<Node>(gsa.Nodes());

      // Get existing axes
      IReadOnlyDictionary<int, Axis> gsaAxes = gsa.Axes();
      Dictionary<int, Axis> apiaxes = gsaAxes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
      
      // Set / add nodes to dictionary
      Nodes.ConvertNodes(nodes, ref apinodes, ref apiaxes, modelUnit);
      #endregion

      #region Properties
      // ### Sections ###
      // list to keep track of duplicated sections
      Dictionary<Guid, int> sections_guid = new Dictionary<Guid, int>();

      // Get existing sections
      IReadOnlyDictionary<int, Section> gsaSections = gsa.Sections();
      Dictionary<int, Section> apisections = gsaSections.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
      // Get existing modifiers
      IReadOnlyDictionary<int, SectionModifier> gsaSectionModifiers = gsa.SectionModifiers();
      Dictionary<int, SectionModifier> apimodifiers = gsaSectionModifiers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

      // Get existing materials
      IReadOnlyDictionary<int, AnalysisMaterial> gsaMaterials = gsa.AnalysisMaterials();
      Dictionary<int, AnalysisMaterial> apimaterials = gsaMaterials.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
      // list to keep track of duplicated materials
      Dictionary<Guid, int> materials_guid = new Dictionary<Guid, int>();

      // add / set sections
      Sections.ConvertSection(sections, ref apisections, ref sections_guid, ref apimodifiers, ref apimaterials, ref materials_guid);

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
          int existingElem1dMaxID = elem1ds.Max(x => x.Id); // max ID in new Elem1ds
          if (existingElem1dMaxID > newElementID)
            newElementID = existingElem1dMaxID + 1;
        }
      }
      if (elem2ds != null)
      {
        if (elem2ds.Count > 0)
        {
          int existingElem2dMaxID = elem2ds.Max(x => x.Ids.Max()); // max ID in new Elem2ds
          if (existingElem2dMaxID > newElementID)
            newElementID = existingElem2dMaxID + 1;
        }
      }

      // Set / add 1D elements to dictionary
      Elements.ConvertElement1D(elem1ds, ref elems, ref newElementID, ref apinodes, modelUnit, ref apisections, ref sections_guid, ref apimodifiers, ref apimaterials, ref materials_guid);

      // Set / add 2D elements to dictionary
      Elements.ConvertElement2D(elem2ds, ref elems, ref newElementID, ref apinodes, modelUnit, ref apiprop2ds, ref prop2d_guid, ref apimaterials, ref materials_guid);

      // Set / add 3D elements to dictionary
      Elements.ConvertElement3D(elem3ds, ref elems, ref newElementID, ref apinodes, modelUnit, ref apiprop3ds, ref prop3d_guid, ref apimaterials, ref materials_guid);
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
          int existingMem1dMaxID = mem1ds.Max(x => x.Id); // max ID in new Mem1ds
          if (existingMem1dMaxID > newMemberID)
            newMemberID = existingMem1dMaxID + 1;
        }
      }

      if (mem2ds != null)
      {
        if (mem2ds.Count > 0)
        {
          int existingMem2dMaxID = mem2ds.Max(x => x.Id); // max ID in new Mem2ds
          if (existingMem2dMaxID > newMemberID)
            newMemberID = existingMem2dMaxID + 1;
        }
      }

      if (mem3ds != null)
      {
        if (mem3ds.Count > 0)
        {
          int existingMem3dMaxID = mem3ds.Max(x => x.Id); // max ID in new Mem2ds
          if (existingMem3dMaxID > newMemberID)
            newMemberID = existingMem3dMaxID + 1;
        }
      }

      // Set / add 1D members to dictionary
      Members.ConvertMember1D(mem1ds, ref mems, ref newMemberID, ref apinodes, modelUnit, ref apisections, ref sections_guid, ref apimodifiers, ref apimaterials, ref materials_guid, modelUnit);

      // Set / add 2D members to dictionary
      Members.ConvertMember2D(mem2ds, ref mems, ref newMemberID, ref apinodes, modelUnit, ref apiprop2ds, ref prop2d_guid, ref apimaterials, ref materials_guid, modelUnit);

      // Set / add 3D members to dictionary
      Members.ConvertMember3D(mem3ds, ref mems, ref newMemberID, ref apinodes, modelUnit, ref apiprop3ds, ref prop3d_guid, ref apimaterials, ref materials_guid);
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
      Loads.ConvertGridPlaneSurface(gridPlaneSurfaces, ref apiaxes, ref apiGridPlanes, ref apiGridSurfaces, ref gp_guid, ref gs_guid, modelUnit);

      // Set / add loads to lists
      Loads.ConvertLoad(loads, ref gravityLoads, ref nodeLoads_node, ref nodeLoads_displ, ref nodeLoads_settle,
          ref beamLoads, ref faceLoads, ref gridPointLoads, ref gridLineLoads, ref gridAreaLoads,
          ref apiaxes, ref apiGridPlanes, ref apiGridSurfaces, ref gp_guid, ref gs_guid, modelUnit);
      #endregion


      #region set stuff in model
      // Geometry
      gsa.SetNodes(apinodes.Dictionary);
      gsa.SetElements(new ReadOnlyDictionary<int, Element>(elems));
      gsa.SetMembers(new ReadOnlyDictionary<int, Member>(mems));
      if (toleranceCoincidentNodes > 0)
        gsa.CollapseCoincidentNodes(toleranceCoincidentNodes);

      // Loads
      gsa.AddGravityLoads(new ReadOnlyCollection<GravityLoad>(gravityLoads));
      gsa.AddNodeLoads(NodeLoadType.APPL_DISP, new ReadOnlyCollection<NodeLoad>(nodeLoads_displ));
      gsa.AddNodeLoads(NodeLoadType.NODE_LOAD, new ReadOnlyCollection<NodeLoad>(nodeLoads_node));
      gsa.AddNodeLoads(NodeLoadType.SETTLEMENT, new ReadOnlyCollection<NodeLoad>(nodeLoads_settle));
      gsa.AddBeamLoads(new ReadOnlyCollection<BeamLoad>(beamLoads));
      gsa.AddFaceLoads(new ReadOnlyCollection<FaceLoad>(faceLoads));
      gsa.AddGridPointLoads(new ReadOnlyCollection<GridPointLoad>(gridPointLoads));
      gsa.AddGridLineLoads(new ReadOnlyCollection<GridLineLoad>(gridLineLoads));
      gsa.AddGridAreaLoads(new ReadOnlyCollection<GridAreaLoad>(gridAreaLoads));
      gsa.SetAxes(new ReadOnlyDictionary<int, Axis>(apiaxes));
      gsa.SetGridPlanes(new ReadOnlyDictionary<int, GridPlane>(apiGridPlanes));
      gsa.SetGridSurfaces(new ReadOnlyDictionary<int, GridSurface>(apiGridSurfaces));
      
      //properties
      gsa.SetSections(new ReadOnlyDictionary<int, Section>(apisections));
      gsa.SetSectionModifiers(new ReadOnlyDictionary<int, SectionModifier>(apimodifiers));
      gsa.SetProp2Ds(new ReadOnlyDictionary<int, Prop2D>(apiprop2ds));
      gsa.SetProp3Ds(new ReadOnlyDictionary<int, Prop3D>(apiprop3ds));
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
            task.ID = gsa.AddAnalysisTask();

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
