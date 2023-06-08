using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaAPI.Materials;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi.EnumMappings;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Helpers.Export {
  internal class AssembleModel {

    /// <summary>
    /// </summary>
    /// <param name="model">Existing models to be merged</param>
    /// <param name="nodes">List of nodes with properties like support conditions</param>
    /// <param name="elem1ds">
    ///   List of 1D elements. Nodes at end-points will automatically be added to the model, using existing
    ///   nodes in model within tolerance. Section will automatically be added to model
    /// </param>
    /// <param name="elem2ds">
    ///   List of 2D elements. Nodes at mesh-verticies will automatically be added to the model, using
    ///   existing nodes in model within tolerance. Prop2d will automatically be added to model
    /// </param>
    /// <param name="elem3ds">
    ///   List of 3D elements. Nodes at mesh-verticies will automatically be added to the model, using
    ///   existing nodes in model within tolerance
    /// </param>
    /// <param name="mem1ds">
    ///   List of 1D members. Topology nodes will automatically be added to the model, using existing nodes
    ///   in model within tolerance. Section will automatically be added to model
    /// </param>
    /// <param name="mem2ds">
    ///   List of 2D members. Topology nodes will automatically be added to the model, using existing nodes
    ///   in model within tolerance. Prop2d will automatically be added to model
    /// </param>
    /// <param name="mem3ds">
    ///   List of 3D members. Topology nodes will automatically be added to the model, using existing nodes
    ///   in model within tolerance
    /// </param>
    /// <param name="sections">List of Sections</param>
    /// <param name="prop2Ds">List of 2D Properties</param>
    /// <param name="prop3Ds"></param>
    /// <param name="loads">
    ///   List of Loads. For Grid loads the Axis, GridPlane and GridSurface will automatically be added to
    ///   the model using existing objects where possible within tolerance.
    /// </param>
    /// <param name="gridPlaneSurfaces">List of GridPlaneSurfaces</param>
    /// <param name="analysisTasks"></param>
    /// <param name="combinations"></param>
    /// <param name="modelUnit"></param>
    /// ///
    /// <param name="toleranceCoincidentNodes">Set to zero to skip collapsing coincident nodes</param>
    /// <param name="createElementsFromMembers"></param>
    /// <param name="owner"></param>
    /// <returns></returns>
    internal static Model Assemble(
      GsaModel model, List<GsaList> lists, List<GsaNode> nodes, List<GsaElement1d> elem1ds, List<GsaElement2d> elem2ds,
      List<GsaElement3d> elem3ds, List<GsaMember1d> mem1ds, List<GsaMember2d> mem2ds,
      List<GsaMember3d> mem3ds, List<GsaSection> sections, List<GsaProp2d> prop2Ds,
      List<GsaProp3d> prop3Ds, List<GsaLoad> loads, List<GsaGridPlaneSurface> gridPlaneSurfaces,
      List<GsaAnalysisTask> analysisTasks, List<GsaCombinationCase> combinations,
      LengthUnit modelUnit, Length toleranceCoincidentNodes, bool createElementsFromMembers,
      GH_Component owner) {
      var gsa = new Model();
      if (model != null) {
        gsa = model.Model;
      } else {
        gsa.UiUnits().LengthLarge = UnitMapping.GetApiUnit(modelUnit);
      }

      // Convert GsaGH Nodes to API Objects
      var apiNodes = new GsaIntDictionary<Node>(gsa.Nodes());
      IReadOnlyDictionary<int, Axis> gsaAxes = gsa.Axes();
      var apiaxes = gsaAxes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
      Nodes.ConvertNodes(nodes, ref apiNodes, ref apiaxes, modelUnit);

      // Convert GsaGH Sections & Materials to API Objects
      var apiSections = new GsaGuidDictionary<Section>(gsa.Sections());
      var apiSectionModifiers = new GsaIntDictionary<SectionModifier>(gsa.SectionModifiers());
      var apiMaterials = new GsaGuidDictionary<AnalysisMaterial>(gsa.AnalysisMaterials());
      Sections.ConvertSection(sections, ref apiSections, ref apiSectionModifiers, ref apiMaterials);

      var apiProp2ds = new GsaGuidDictionary<Prop2D>(gsa.Prop2Ds());
      Prop2ds.ConvertProp2d(prop2Ds, ref apiProp2ds, ref apiMaterials, ref apiaxes, modelUnit);

      var apiProp3ds = new GsaGuidDictionary<Prop3D>(gsa.Prop3Ds());
      Prop3ds.ConvertProp3d(prop3Ds, ref apiProp3ds, ref apiMaterials);

      // Convert GsaGH Elements to API Objects
      var apiElements = new GsaGuidIntListDictionary<Element>(gsa.Elements());
      Elements.ConvertElement1D(elem1ds, ref apiElements, ref apiNodes, modelUnit, ref apiSections,
        ref apiSectionModifiers, ref apiMaterials);
      Elements.ConvertElement2D(elem2ds, ref apiElements, ref apiNodes, modelUnit, ref apiProp2ds,
        ref apiMaterials, ref apiaxes);
      Elements.ConvertElement3D(elem3ds, ref apiElements, ref apiNodes, modelUnit, ref apiProp3ds,
        ref apiMaterials);

      // Convert GsaGH Members to API Objects
      var apiMembers = new GsaGuidDictionary<Member>(gsa.Members());
      Members.ConvertMember1D(mem1ds, ref apiMembers, ref apiNodes, modelUnit, ref apiSections,
        ref apiSectionModifiers, ref apiMaterials);
      Members.ConvertMember2D(mem2ds, ref apiMembers, ref apiNodes, modelUnit, ref apiProp2ds,
        ref apiMaterials, ref apiaxes);
      Members.ConvertMember3D(mem3ds, ref apiMembers, ref apiNodes, modelUnit, ref apiProp3ds,
        ref apiMaterials);

      // Convert GsaGH Node lists to API Objects
      var apiNodeLists = new GsaGuidDictionary<EntityList>(gsa.Lists());
      Lists.ConvertNodeList(lists, ref apiNodeLists, ref apiNodes, modelUnit);
      
      // Convert GsaGH Node loads to API Objects
      var nodeLoadsNode = new List<NodeLoad>();
      var nodeLoadsDispl = new List<NodeLoad>();
      var nodeLoadsSettle = new List<NodeLoad>();
      Loads.ConvertNodeLoad(loads, ref nodeLoadsNode, ref nodeLoadsDispl, ref nodeLoadsSettle,
        ref apiNodes, ref apiNodeLists, modelUnit);

      // Set API Nodes, Elements and Members in model
      ReadOnlyDictionary<int, Node> apiNodeDict = apiNodes.ReadOnlyDictionary;
      gsa.SetNodes(apiNodeDict);
      ReadOnlyDictionary<int, Element> apiElemDict = apiElements.ReadOnlyDictionary;
      gsa.SetElements(apiElemDict);
      ReadOnlyDictionary<int, Member> apiMemDict = apiMembers.ReadOnlyDictionary;
      gsa.SetMembers(apiMemDict);

      // Set API Sections and Materials in model
      gsa.SetSections(apiSections.ReadOnlyDictionary);
      gsa.SetSectionModifiers(apiSectionModifiers.ReadOnlyDictionary);
      gsa.SetProp2Ds(apiProp2ds.ReadOnlyDictionary);
      gsa.SetProp3Ds(apiProp3ds.ReadOnlyDictionary);
      ReadOnlyDictionary<int, AnalysisMaterial> materials = apiMaterials.ReadOnlyDictionary;
      if (materials.Count > 0) {
        foreach (KeyValuePair<int, AnalysisMaterial> mat in materials) {
          gsa.SetAnalysisMaterial(mat.Key, mat.Value);
        }
      }

      // Add API Node loads to model
      gsa.AddNodeLoads(NodeLoadType.APPL_DISP, new ReadOnlyCollection<NodeLoad>(nodeLoadsDispl));
      gsa.AddNodeLoads(NodeLoadType.NODE_LOAD, new ReadOnlyCollection<NodeLoad>(nodeLoadsNode));
      gsa.AddNodeLoads(NodeLoadType.SETTLEMENT, new ReadOnlyCollection<NodeLoad>(nodeLoadsSettle));

      // Set API lists for Nodes in model
      gsa.SetLists(apiNodeLists.ReadOnlyDictionary);

      // Meshing / Create Elements from Members
      int initialNodeCount = apiNodeDict.Keys.Count;
      if (createElementsFromMembers && apiMembers.Count > 0) {
        gsa.CreateElementsFromMembers();
      }
      // Sense-checking model after Elements from Members
      if (toleranceCoincidentNodes.Value > 0) {
        gsa.CollapseCoincidentNodes(toleranceCoincidentNodes.Meters);
        if (owner != null) {
          try {
            double minMeshSize = apiMemDict.Values.Where(x => x.MeshSize != 0)
             .Select(x => x.MeshSize).Min();
            if (minMeshSize < toleranceCoincidentNodes.Meters) {
              owner.AddRuntimeWarning("The smallest mesh size (" + minMeshSize
                + ") is smaller than the set tolerance (" + toleranceCoincidentNodes.Meters + ")."
                + Environment.NewLine + "This is likely to produce an undisarable mesh."
                + Environment.NewLine + "Right-click the component to change the tolerance.");
            }
          } catch (InvalidOperationException) {
            // if linq .Where returns an empty list (all mesh sizes are zero)
          }

          int newNodeCount = gsa.Nodes().Keys.Count;

          if (elem2ds != null && elem2ds.Count > 0) {
            foreach (GsaElement2d e2d in elem2ds) {
              int expectedCollapsedNodeCount = e2d.Mesh.TopologyVertices.Count;
              int actualNodeCount = e2d.TopoInt.Sum(topoint => topoint.Count);
              int difference = actualNodeCount - expectedCollapsedNodeCount;
              initialNodeCount -= difference;
            }
          }

          if (elem3ds != null && elem3ds.Count > 0) {
            foreach (GsaElement3d e3d in elem3ds) {
              int expectedCollapsedNodeCount = e3d.NgonMesh.TopologyVertices.Count;
              int actualNodeCount = e3d.TopoInt.Sum(topoint => topoint.Count);
              int difference = actualNodeCount - expectedCollapsedNodeCount;
              initialNodeCount -= difference;
            }
          }

          double nodeSurvivalRate = newNodeCount / (double)initialNodeCount;

          int elemCount = apiElemDict.Count;
          int memCount = apiMemDict.Count;
          double warningSurvivalRate
            = elemCount > memCount ? 0.05 :
              0.2; // warning if >95% of nodes are removed for elements or >80% for members
          double remarkSurvivalRate
            = elemCount > memCount ? 0.2 :
              0.33; // remark if >80% of nodes are removed for elements or >66% for members

          if (newNodeCount == 1) {
            owner.AddRuntimeWarning("After collapsing coincident nodes only one node remained."
              + Environment.NewLine
              + "This indicates that you have set a tolerance that is too low."
              + Environment.NewLine + "Right-click the component to change the tolerance.");
          } else if (nodeSurvivalRate < warningSurvivalRate) {
            owner.AddRuntimeWarning(
              new Ratio(1 - nodeSurvivalRate, RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent)
               .ToString("g0").Replace(" ", string.Empty)
              + " of the nodes were removed after collapsing coincident nodes."
              + Environment.NewLine
              + "This indicates that you have set a tolerance that is too low."
              + Environment.NewLine + "Right-click the component to change the tolerance.");
          } else if (nodeSurvivalRate < remarkSurvivalRate) {
            owner.AddRuntimeRemark(
              new Ratio(1 - nodeSurvivalRate, RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent)
               .ToString("g0").Replace(" ", string.Empty)
              + " of the nodes were removed after collapsing coincident nodes."
              + Environment.NewLine
              + "This indicates that you have set a tolerance that is too low."
              + Environment.NewLine + "Right-click the component to change the tolerance.");
          }
        }
      }

      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship 
        = ElementListFromReference.GetMemberElementRelationship(gsa);

      // Convert rest of GsaGH Lists to API Objects (nodes done prior to collapse coinciding nodes)
      // Convert GsaGH Node lists to API Objects
      var apiLists = new GsaGuidDictionary<EntityList>(gsa.Lists());
      Lists.ConvertList(lists, ref apiLists, apiMaterials, apiSections, apiProp2ds, apiProp3ds, 
        apiElements, apiMembers, memberElementRelationship, owner);

      // Convert rest of GsaGH Loads to API objects (nodes done prior to collapse coinciding nodes)
      var gravityLoads = new List<GravityLoad>();
      var beamLoads = new List<BeamLoad>();
      var faceLoads = new List<FaceLoad>();
      var gridPointLoads = new List<GridPointLoad>();
      var gridLineLoads = new List<GridLineLoad>();
      var gridAreaLoads = new List<GridAreaLoad>();

      IReadOnlyDictionary<int, GridPlane> gsaGPln = gsa.GridPlanes();
      var apiGridPlanes = gsaGPln.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

      IReadOnlyDictionary<int, GridSurface> gsaGSrf = gsa.GridSurfaces();
      var apiGridSurfaces = gsaGSrf.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

      var gpGuid = new Dictionary<Guid, int>();
      var gsGuid = new Dictionary<Guid, int>();

      Loads.ConvertGridPlaneSurface(gridPlaneSurfaces, ref apiaxes, ref apiGridPlanes,
        ref apiGridSurfaces, ref gpGuid, ref gsGuid, ref apiLists, modelUnit, memberElementRelationship, 
        gsa, apiMaterials, apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers, owner);

      Loads.ConvertLoad(loads, ref gravityLoads, ref beamLoads, ref faceLoads, ref gridPointLoads,
        ref gridLineLoads, ref gridAreaLoads, ref apiaxes, ref apiGridPlanes, ref apiGridSurfaces,
        ref gpGuid, ref gsGuid, ref apiLists, modelUnit, memberElementRelationship, gsa, apiMaterials, 
        apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers, owner);

      // Add API Loads in model
      gsa.AddGravityLoads(new ReadOnlyCollection<GravityLoad>(gravityLoads));
      gsa.AddBeamLoads(new ReadOnlyCollection<BeamLoad>(beamLoads));
      gsa.AddFaceLoads(new ReadOnlyCollection<FaceLoad>(faceLoads));
      gsa.AddGridPointLoads(new ReadOnlyCollection<GridPointLoad>(gridPointLoads));
      gsa.AddGridLineLoads(new ReadOnlyCollection<GridLineLoad>(gridLineLoads));
      gsa.AddGridAreaLoads(new ReadOnlyCollection<GridAreaLoad>(gridAreaLoads));
      // Set API Axis, GridPlanes and GridSurface in model
      gsa.SetAxes(new ReadOnlyDictionary<int, Axis>(apiaxes));
      gsa.SetGridPlanes(new ReadOnlyDictionary<int, GridPlane>(apiGridPlanes));
      gsa.SetGridSurfaces(new ReadOnlyDictionary<int, GridSurface>(apiGridSurfaces));

      // Set API list in model
      gsa.SetLists(apiLists.ReadOnlyDictionary);

      // Set Analysis Tasks in model
      if (analysisTasks != null) {
        ReadOnlyDictionary<int, AnalysisTask> existingTasks = gsa.AnalysisTasks();
        foreach (GsaAnalysisTask task in analysisTasks) {
          if (!existingTasks.Keys.Contains(task.Id)) {
            task.Id = gsa.AddAnalysisTask();
          }

          if (task.Cases == null || task.Cases.Count == 0) {
            task.CreateDefaultCases(gsa);
          }

          if (task.Cases == null) {
            continue;
          }

          foreach (GsaAnalysisCase ca in task.Cases) {
            gsa.AddAnalysisCaseToTask(task.Id, ca.Name, ca.Description);
          }
        }
      }

      // Add Combination Cases to model
      if (combinations != null && combinations.Count > 0) {
        foreach (GsaCombinationCase co in combinations) {
          gsa.AddCombinationCase(co.Name, co.Description);
        }
      }

      return gsa;
    }
  }
}
