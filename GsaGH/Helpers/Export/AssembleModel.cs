using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Helpers.GH;
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
        LengthUnit modelUnit, Length toleranceCoincidentNodes, bool createElementsFromMembers, GH_Component owner)
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
      Prop2ds.ConvertProp2d(prop2Ds, ref apiProp2ds, ref apiMaterials, ref apiaxes, modelUnit);

      // ### Prop3ds ###
      GsaGuidDictionary<Prop3D> apiProp3ds = new GsaGuidDictionary<Prop3D>(gsa.Prop3Ds());
      Prop3ds.ConvertProp3d(prop3Ds, ref apiProp3ds, ref apiMaterials);
      #endregion

      #region Elements
      GsaGuidIntListDictionary<Element> apiElements = new GsaGuidIntListDictionary<Element>(gsa.Elements());
      Elements.ConvertElement1D(elem1ds, ref apiElements, ref apiNodes, modelUnit, ref apiSections, ref apiSectionModifiers, ref apiMaterials);
      Elements.ConvertElement2D(elem2ds, ref apiElements, ref apiNodes, modelUnit, ref apiProp2ds, ref apiMaterials, ref apiaxes);
      Elements.ConvertElement3D(elem3ds, ref apiElements, ref apiNodes, modelUnit, ref apiProp3ds, ref apiMaterials);
      #endregion

      #region Members
      GsaGuidDictionary<Member> apiMembers = new GsaGuidDictionary<Member>(gsa.Members());
      Members.ConvertMember1D(mem1ds, ref apiMembers, ref apiNodes, modelUnit, ref apiSections, ref apiSectionModifiers, ref apiMaterials);
      Members.ConvertMember2D(mem2ds, ref apiMembers, ref apiNodes, modelUnit, ref apiProp2ds, ref apiMaterials, ref apiaxes);
      Members.ConvertMember3D(mem3ds, ref apiMembers, ref apiNodes, modelUnit, ref apiProp3ds, ref apiMaterials);
      #endregion

      #region Node loads
      List<NodeLoad> nodeLoads_node = new List<NodeLoad>();
      List<NodeLoad> nodeLoads_displ = new List<NodeLoad>();
      List<NodeLoad> nodeLoads_settle = new List<NodeLoad>();
      Loads.ConvertNodeLoad(loads, ref nodeLoads_node, ref nodeLoads_displ, ref nodeLoads_settle, ref apiNodes, modelUnit);
      #endregion

      #region set geometry in model
      // Geometry
      ReadOnlyDictionary<int, Node> apiNodeDict = apiNodes.Dictionary;
      gsa.SetNodes(apiNodeDict);
      ReadOnlyDictionary<int, Element> apiElemDict = apiElements.Dictionary;
      gsa.SetElements(apiElemDict);
      ReadOnlyDictionary<int, Member> apiMemDict = apiMembers.Dictionary;
      gsa.SetMembers(apiMemDict);
      // node loads
      gsa.AddNodeLoads(NodeLoadType.APPL_DISP, new ReadOnlyCollection<NodeLoad>(nodeLoads_displ));
      gsa.AddNodeLoads(NodeLoadType.NODE_LOAD, new ReadOnlyCollection<NodeLoad>(nodeLoads_node));
      gsa.AddNodeLoads(NodeLoadType.SETTLEMENT, new ReadOnlyCollection<NodeLoad>(nodeLoads_settle));

      int initialNodeCount = apiNodeDict.Keys.Count;
      if (createElementsFromMembers && apiMembers.Count > 0)
        gsa.CreateElementsFromMembers();
      if (toleranceCoincidentNodes.Value > 0)
      {
        gsa.CollapseCoincidentNodes(toleranceCoincidentNodes.Meters);
        
        // provide feedback to user if we have removed unreasonable amount of nodes
        if (owner != null)
        {
          try
          {
            double minMeshSize = apiMemDict.Values.Where(x => x.MeshSize != 0).Select(x => x.MeshSize).Min();
            if (minMeshSize < toleranceCoincidentNodes.Meters)
              owner.AddRuntimeWarning(
                "The smallest mesh size (" + minMeshSize + ") is smaller than the set tolerance (" + toleranceCoincidentNodes.Meters + ")."
                + System.Environment.NewLine + "This is likely to produce an undisarable mesh."
                + System.Environment.NewLine + "Right-click the component to change the tolerance.");
          }
          catch (System.InvalidOperationException)
          {
            // if linq .Where returns an empty list (all mesh sizes are zero)
          }

          int newNodeCount = gsa.Nodes().Keys.Count;
          
          if (elem2ds != null && elem2ds.Count > 0)
          {
            foreach (GsaElement2d e2d in elem2ds) 
            {
              int expectedCollapsedNodeCount = e2d.Mesh.TopologyVertices.Count;
              int actualNodeCount = 0;
              foreach(List<int> topoint in e2d.TopoInt)
                actualNodeCount+= topoint.Count;
              int difference = actualNodeCount - expectedCollapsedNodeCount;
              initialNodeCount -= difference;
            }
          }
          if (elem3ds != null && elem3ds.Count > 0)
          {
            foreach (GsaElement3d e3d in elem3ds)
            {
              int expectedCollapsedNodeCount = e3d.NgonMesh.TopologyVertices.Count;
              int actualNodeCount = 0;
              foreach (List<int> topoint in e3d.TopoInt)
                actualNodeCount += topoint.Count;
              int difference = actualNodeCount - expectedCollapsedNodeCount;
              initialNodeCount -= difference;
            }
          }
          double nodeSurvivalRate = (double)newNodeCount / (double)initialNodeCount;
          
          int elemCount = apiElemDict.Count;
          int memCount = apiMemDict.Count;
          double warningSurvivalRate = elemCount > memCount ? 0.1 : 0.5;
          double remarkSurvivalRate = elemCount > memCount ? 0.5 : 0.75;

          if (newNodeCount == 1)
            owner.AddRuntimeWarning(
              "After collapsing coincident nodes only one node remained." + System.Environment.NewLine
              + "This indicates that you have set a tolerance that is too low."
              + System.Environment.NewLine + "Right-click the component to change the tolerance.");
          else if (nodeSurvivalRate < warningSurvivalRate)
            owner.AddRuntimeWarning(
              new Ratio(1 - nodeSurvivalRate, RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent).ToString("g0").Replace(" ", string.Empty)
              + " of the nodes were removed after collapsing coincident nodes." + System.Environment.NewLine
              + "This indicates that you have set a tolerance that is too low."
              + System.Environment.NewLine + "Right-click the component to change the tolerance.");
          else if (nodeSurvivalRate < remarkSurvivalRate)
            owner.AddRuntimeRemark(
              new Ratio(1 - nodeSurvivalRate, RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent).ToString("g0").Replace(" ", string.Empty)
              + " of the nodes were removed after collapsing coincident nodes." + System.Environment.NewLine
              + "This indicates that you have set a tolerance that is too low."
              + System.Environment.NewLine + "Right-click the component to change the tolerance.");
        }
      }
      #endregion

      #region Loads
      // ### Loads ###
      // We let the existing loads (if any) survive and just add new loads
      // Get existing loads
      List<GravityLoad> gravityLoads = new List<GravityLoad>();
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

      // member-element relationship, not used if not needed
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship = null;

      // Set / add Grid plane surfaces - do this first to set any GridPlane and GridSurfaces with IDs.
      Loads.ConvertGridPlaneSurface(gridPlaneSurfaces, ref apiaxes, ref apiGridPlanes, ref apiGridSurfaces, ref gp_guid, ref gs_guid, modelUnit, ref memberElementRelationship, gsa, apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers);

      // Set / add loads to lists
      Loads.ConvertLoad(loads, ref gravityLoads, ref beamLoads, ref faceLoads, ref gridPointLoads, ref gridLineLoads, ref gridAreaLoads, ref apiaxes, ref apiGridPlanes, ref apiGridSurfaces, ref gp_guid, ref gs_guid, modelUnit, ref memberElementRelationship, gsa, apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers, owner);
      #endregion

      #region set rest in model
      // Loads
      gsa.AddGravityLoads(new ReadOnlyCollection<GravityLoad>(gravityLoads));
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

          if (task.Cases == null || task.Cases.Count == 0)
            task.CreateDefaultCases(gsa);

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
