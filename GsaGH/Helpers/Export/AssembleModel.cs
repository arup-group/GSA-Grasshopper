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
        LengthUnit modelUnit, double toleranceCoincidentNodes, bool createElementsFromMembers)
    {
      // Set model to work on
      Model gsa = new Model();
      if (model != null)
        gsa = model.Model;

      #region Nodes
      // ### Nodes ###
      // We take out the existing nodes in the model and work on that dictionary
      // Get existing nodes
      GsaIntDictionary<Node> apiNodes = new GsaIntDictionary<Node>(gsa.Nodes());

      // Get existing axes
      IReadOnlyDictionary<int, Axis> gsaAxes = gsa.Axes();
      Dictionary<int, Axis> apiaxes = gsaAxes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
      
      // Set / add nodes to dictionary
      Nodes.ConvertNodes(nodes, ref apiNodes, ref apiaxes, modelUnit);
      #endregion

      #region Properties
      // ### Sections ###
      GsaGuidDictionary<Section> apiSections = new GsaGuidDictionary<Section>(gsa.Sections());
      GsaIntDictionary<SectionModifier> apiSectionModifiers = new GsaIntDictionary<SectionModifier>(gsa.SectionModifiers());
      GsaGuidDictionary<AnalysisMaterial> apiMaterials = new GsaGuidDictionary<AnalysisMaterial>(gsa.AnalysisMaterials());
      Sections.ConvertSection(sections, ref apiSections, ref apiSectionModifiers, ref apiMaterials);

      // ### Prop2ds ###
      GsaGuidDictionary<Prop2D> apiProp2ds = new GsaGuidDictionary<Prop2D>(gsa.Prop2Ds());
      Prop2ds.ConvertProp2d(prop2Ds, ref apiProp2ds, ref apiMaterials);

      // ### Prop3ds ###
      GsaGuidDictionary<Prop3D> apiProp3ds = new GsaGuidDictionary<Prop3D>(gsa.Prop3Ds());
      Prop3ds.ConvertProp3d(prop3Ds, ref apiProp3ds, ref apiMaterials);
      #endregion

      #region Elements
      GsaIntDictionary<Element> apiElements = new GsaIntDictionary<Element>(gsa.Elements());
      Elements.ConvertElement1D(elem1ds, ref apiElements, ref apiNodes, modelUnit, ref apiSections, ref apiSectionModifiers, ref apiMaterials);
      Elements.ConvertElement2D(elem2ds, ref apiElements, ref apiNodes, modelUnit, ref apiProp2ds, ref apiMaterials);
      Elements.ConvertElement3D(elem3ds, ref apiElements, ref apiNodes, modelUnit, ref apiProp3ds, ref apiMaterials);
      #endregion

      #region Members
      GsaIntDictionary<Member> apiMembers = new GsaIntDictionary<Member>(gsa.Members());
      Members.ConvertMember1D(mem1ds, ref apiMembers, ref apiNodes, modelUnit, ref apiSections, ref apiSectionModifiers, ref apiMaterials);
      Members.ConvertMember2D(mem2ds, ref apiMembers, ref apiNodes, modelUnit, ref apiProp2ds, ref apiMaterials);
      Members.ConvertMember3D(mem3ds, ref apiMembers, ref apiNodes, modelUnit, ref apiProp3ds, ref apiMaterials);
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
      gsa.SetNodes(apiNodes.Dictionary);
      gsa.SetElements(apiElements.Dictionary);
      gsa.SetMembers(apiMembers.Dictionary);

      if (createElementsFromMembers && apiMembers.Count > 0)
        gsa.CreateElementsFromMembers();
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
      gsa.SetSections(apiSections.Dictionary);
      gsa.SetSectionModifiers(apiSectionModifiers.Dictionary);
      gsa.SetProp2Ds(apiProp2ds.Dictionary);
      gsa.SetProp3Ds(apiProp3ds.Dictionary);
      ReadOnlyDictionary<int, AnalysisMaterial> materials = apiMaterials.Dictionary;
      if (materials.Count > 0)
      {
        foreach (KeyValuePair<int, AnalysisMaterial> mat in materials)
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
