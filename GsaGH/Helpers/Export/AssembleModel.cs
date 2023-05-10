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
      GsaModel model, List<GsaNode> nodes, List<GsaElement1d> elem1ds, List<GsaElement2d> elem2ds,
      List<GsaElement3d> elem3ds, List<GsaMember1d> mem1ds, List<GsaMember2d> mem2ds,
      List<GsaMember3d> mem3ds, List<GsaSection> sections, List<GsaProp2d> prop2Ds,
      List<GsaProp3d> prop3Ds, List<GsaLoad> loads, List<GsaGridPlaneSurface> gridPlaneSurfaces,
      List<GsaAnalysisTask> analysisTasks, List<GsaCombinationCase> combinations,
      LengthUnit modelUnit, Length toleranceCoincidentNodes, bool createElementsFromMembers,
      GH_Component owner) {
      var gsa = new Model();
      if (model != null) {
        gsa = model.Model;
      }

      #region Nodes

      var apiNodes = new GsaIntDictionary<Node>(gsa.Nodes());
      IReadOnlyDictionary<int, Axis> gsaAxes = gsa.Axes();
      var apiaxes = gsaAxes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
      Nodes.ConvertNodes(nodes, ref apiNodes, ref apiaxes, modelUnit);

      #endregion

      #region Properties

      var apiSections = new GsaGuidDictionary<Section>(gsa.Sections());
      var apiSectionModifiers = new GsaIntDictionary<SectionModifier>(gsa.SectionModifiers());
      var apiMaterials = new Materials(gsa);
      Sections.ConvertSection(sections, ref apiSections, ref apiSectionModifiers, ref apiMaterials);

      var apiProp2ds = new GsaGuidDictionary<Prop2D>(gsa.Prop2Ds());
      Prop2ds.ConvertProp2d(prop2Ds, ref apiProp2ds, ref apiMaterials, ref apiaxes, modelUnit);

      var apiProp3ds = new GsaGuidDictionary<Prop3D>(gsa.Prop3Ds());
      Prop3ds.ConvertProp3d(prop3Ds, ref apiProp3ds, ref apiMaterials);

      #endregion

      #region Elements

      var apiElements = new GsaGuidIntListDictionary<Element>(gsa.Elements());
      Elements.ConvertElement1D(elem1ds, ref apiElements, ref apiNodes, modelUnit, ref apiSections,
        ref apiSectionModifiers, ref apiMaterials);
      Elements.ConvertElement2D(elem2ds, ref apiElements, ref apiNodes, modelUnit, ref apiProp2ds,
        ref apiMaterials, ref apiaxes);
      Elements.ConvertElement3D(elem3ds, ref apiElements, ref apiNodes, modelUnit, ref apiProp3ds,
        ref apiMaterials);

      #endregion

      #region Members

      var apiMembers = new GsaGuidDictionary<Member>(gsa.Members());
      Members.ConvertMember1D(mem1ds, ref apiMembers, ref apiNodes, modelUnit, ref apiSections,
        ref apiSectionModifiers, ref apiMaterials);
      Members.ConvertMember2D(mem2ds, ref apiMembers, ref apiNodes, modelUnit, ref apiProp2ds,
        ref apiMaterials, ref apiaxes);
      Members.ConvertMember3D(mem3ds, ref apiMembers, ref apiNodes, modelUnit, ref apiProp3ds,
        ref apiMaterials);

      #endregion

      #region Node loads

      var nodeLoadsNode = new List<NodeLoad>();
      var nodeLoadsDispl = new List<NodeLoad>();
      var nodeLoadsSettle = new List<NodeLoad>();
      Loads.ConvertNodeLoad(loads, ref nodeLoadsNode, ref nodeLoadsDispl, ref nodeLoadsSettle,
        ref apiNodes, modelUnit);

      #endregion

      #region set geometry in model

      ReadOnlyDictionary<int, Node> apiNodeDict = apiNodes.Dictionary;
      gsa.SetNodes(apiNodeDict);
      ReadOnlyDictionary<int, Element> apiElemDict = apiElements.Dictionary;
      gsa.SetElements(apiElemDict);
      ReadOnlyDictionary<int, Member> apiMemDict = apiMembers.Dictionary;
      gsa.SetMembers(apiMemDict);

      gsa.SetSections(apiSections.Dictionary);
      gsa.SetSectionModifiers(apiSectionModifiers.Dictionary);
      gsa.SetProp2Ds(apiProp2ds.Dictionary);
      gsa.SetProp3Ds(apiProp3ds.Dictionary);

      ReadOnlyDictionary<int, AnalysisMaterial> customMaterials = 
        apiMaterials.AnalysisMaterials.Dictionary;
      if (customMaterials.Count > 0) {
        foreach (KeyValuePair<int, AnalysisMaterial> mat in customMaterials) {
          gsa.SetAnalysisMaterial(mat.Key, mat.Value);
        }
      }

      ReadOnlyDictionary<int, AluminiumMaterial> aluminiumMaterials =
        apiMaterials.AluminiumMaterials.Dictionary;
      if (aluminiumMaterials.Count > 0) {
        foreach (KeyValuePair<int, AluminiumMaterial> mat in aluminiumMaterials) {
          gsa.SetAluminiumMaterial(mat.Key, mat.Value);
        }
      }

      ReadOnlyDictionary<int, ConcreteMaterial> concreteMaterials =
        apiMaterials.ConcreteMaterials.Dictionary;
      if (concreteMaterials.Count > 0) {
        foreach (KeyValuePair<int, ConcreteMaterial> mat in concreteMaterials) {
          gsa.SetConcreteMaterial(mat.Key, mat.Value);
        }
      }

      ReadOnlyDictionary<int, FabricMaterial> fabricMaterials =
        apiMaterials.FabricMaterials.Dictionary;
      if (fabricMaterials.Count > 0) {
        foreach (KeyValuePair<int, FabricMaterial> mat in fabricMaterials) {
          gsa.SetFabricMaterial(mat.Key, mat.Value);
        }
      }

      ReadOnlyDictionary<int, FrpMaterial> frpMaterials =
        apiMaterials.FrpMaterials.Dictionary;
      if (frpMaterials.Count > 0) {
        foreach (KeyValuePair<int, FrpMaterial> mat in frpMaterials) {
          gsa.SetFrpMaterial(mat.Key, mat.Value);
        }
      }

      ReadOnlyDictionary<int, GlassMaterial> glassMaterials =
        apiMaterials.GlassMaterials.Dictionary;
      if (glassMaterials.Count > 0) {
        foreach (KeyValuePair<int, GlassMaterial> mat in glassMaterials) {
          gsa.SetGlassMaterial(mat.Key, mat.Value);
        }
      }

      ReadOnlyDictionary<int, ReinforcementMaterial> reinforcementMaterials =
        apiMaterials.ReinforcementMaterials.Dictionary;
      if (reinforcementMaterials.Count > 0) {
        foreach (KeyValuePair<int, ReinforcementMaterial> mat in reinforcementMaterials) {
          gsa.SetReinforcementMaterial(mat.Key, mat.Value);
        }
      }

      ReadOnlyDictionary<int, SteelMaterial> steelMaterials =
        apiMaterials.SteelMaterials.Dictionary;
      if (steelMaterials.Count > 0) {
        foreach (KeyValuePair<int, SteelMaterial> mat in steelMaterials) {
          gsa.SetSteelMaterial(mat.Key, mat.Value);
        }
      }

      ReadOnlyDictionary<int, TimberMaterial> timberMaterials =
        apiMaterials.TimberMaterials.Dictionary;
      if (timberMaterials.Count > 0) {
        foreach (KeyValuePair<int, TimberMaterial> mat in timberMaterials) {
          gsa.SetTimberMaterial(mat.Key, mat.Value);
        }
      }

      gsa.AddNodeLoads(NodeLoadType.APPL_DISP, new ReadOnlyCollection<NodeLoad>(nodeLoadsDispl));
      gsa.AddNodeLoads(NodeLoadType.NODE_LOAD, new ReadOnlyCollection<NodeLoad>(nodeLoadsNode));
      gsa.AddNodeLoads(NodeLoadType.SETTLEMENT, new ReadOnlyCollection<NodeLoad>(nodeLoadsSettle));

      int initialNodeCount = apiNodeDict.Keys.Count;
      if (createElementsFromMembers && apiMembers.Count > 0) {
        gsa.CreateElementsFromMembers();
      }

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

      #endregion

      #region Loads

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

      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship = null;
      Loads.ConvertGridPlaneSurface(gridPlaneSurfaces, ref apiaxes, ref apiGridPlanes,
        ref apiGridSurfaces, ref gpGuid, ref gsGuid, modelUnit, ref memberElementRelationship, gsa,
        apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers);

      Loads.ConvertLoad(loads, ref gravityLoads, ref beamLoads, ref faceLoads, ref gridPointLoads,
        ref gridLineLoads, ref gridAreaLoads, ref apiaxes, ref apiGridPlanes, ref apiGridSurfaces,
        ref gpGuid, ref gsGuid, modelUnit, ref memberElementRelationship, gsa, apiSections,
        apiProp2ds, apiProp3ds, apiElements, apiMembers, owner);

      #endregion

      #region set rest in model

      gsa.AddGravityLoads(new ReadOnlyCollection<GravityLoad>(gravityLoads));
      gsa.AddBeamLoads(new ReadOnlyCollection<BeamLoad>(beamLoads));
      gsa.AddFaceLoads(new ReadOnlyCollection<FaceLoad>(faceLoads));
      gsa.AddGridPointLoads(new ReadOnlyCollection<GridPointLoad>(gridPointLoads));
      gsa.AddGridLineLoads(new ReadOnlyCollection<GridLineLoad>(gridLineLoads));
      gsa.AddGridAreaLoads(new ReadOnlyCollection<GridAreaLoad>(gridAreaLoads));
      gsa.SetAxes(new ReadOnlyDictionary<int, Axis>(apiaxes));
      gsa.SetGridPlanes(new ReadOnlyDictionary<int, GridPlane>(apiGridPlanes));
      gsa.SetGridSurfaces(new ReadOnlyDictionary<int, GridSurface>(apiGridSurfaces));

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

      if (combinations == null) {
        return gsa;
      }

      foreach (GsaCombinationCase co in combinations) {
        gsa.AddCombinationCase(co.Name, co.Description);
      }

      #endregion

      return gsa;
    }
  }
}
