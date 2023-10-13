﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaAPI.Materials;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;
using GsaGH.Helpers.GsaApi.EnumMappings;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using EntityType = GsaGH.Parameters.EntityType;
using LengthUnit = OasysUnits.Units.LengthUnit;
using LoadCase = GsaAPI.LoadCase;

namespace GsaGH.Helpers.Assembly {
  internal partial class ModelAssembly {
    private GsaIntDictionary<Axis> _axes;
    private GsaGuidIntListDictionary<Element> _elements;
    private GsaIntDictionary<GridLine> _gridLines;
    private GsaModel _gsaModel;
    private GsaGuidDictionary<EntityList> _lists;
    private ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship;
    private GsaGuidDictionary<Member> _members;
    private Model _model;
    private GsaIntDictionary<Node> _nodes;
    private LengthUnit _unit = LengthUnit.Meter;
    private bool _deleteResults = false;
    private int _initialNodeCount = 0;
    private bool _isSeedModel = true;

    // assemble for local axis
    internal ModelAssembly(GsaMember1d member) {
      SetupModel(null, LengthUnit.Meter);
      var mem1ds = new List<GsaMember1d>() {
        member
      };
      ConvertMembers(mem1ds, null, null);
      AssembleNodesElementsMembersAndLists();
    }

    // assemble for local axis
    internal ModelAssembly(GsaElement1d element) {
      SetupModel(null, LengthUnit.Meter);
      var elem1ds = new List<GsaElement1d>() {
        element
      };
      ConvertElements(elem1ds, null, null);
      AssembleNodesElementsMembersAndLists();
    }

    // assemble for preview
    internal ModelAssembly(GsaModel model, List<GsaList> lists, List<GsaElement1d> elem1ds,
      List<GsaElement2d> elem2ds, List<GsaMember1d> mem1ds,
      List<GsaMember2d> mem2ds, LengthUnit modelUnit) {
      SetupModel(model, modelUnit);
      ConvertElements(elem1ds, elem2ds, null);
      ConvertMembers(mem1ds, mem2ds, null);
      ConvertLists(lists);
      AssembleNodesElementsMembersAndLists();
    }

    internal ModelAssembly(
      GsaModel model, List<GsaList> lists, List<GsaGridLine> gridLines, List<GsaNode> nodes,
      List<GsaElement1d> elem1ds, List<GsaElement2d> elem2ds, List<GsaElement3d> elem3ds,
      List<GsaMember1d> mem1ds, List<GsaMember2d> mem2ds, List<GsaMember3d> mem3ds,
      List<GsaMaterial> mats, List<GsaSection> sections, List<GsaProperty2d> prop2Ds,
      List<GsaProperty3d> prop3Ds, List<IGsaLoad> loads, List<GsaGridPlaneSurface> gridPlaneSurfaces,
      List<GsaLoadCase> loadCases, List<GsaAnalysisTask> analysisTasks,
      List<GsaCombinationCase> combinations, LengthUnit modelUnit,
      Length toleranceCoincidentNodes, bool createElementsFromMembers, GH_Component owner) {

      SetupModel(model, modelUnit);

      ConvertNodes(nodes);
      ConvertProperties(mats, sections, prop2Ds, prop3Ds);
      ConvertElements(elem1ds, elem2ds, elem3ds);
      ConvertMembers(mem1ds, mem2ds, mem3ds);
      ConvertNodeList(lists);
      ConvertNodeLoads(loads);
      AssembleNodesElementsMembersAndLists();
      ElementsFromMembers(createElementsFromMembers, toleranceCoincidentNodes, owner);

      ConvertList(lists, loads, owner);
      ConvertGridPlaneSurface(gridPlaneSurfaces, owner);
      ConvertLoad(loads, owner);
      ConvertLoadCases(loadCases, owner);

      AssembleLoadsCasesAxesGridPlaneSurfacesAndLists(owner);
      ConvertAndAssembleGridLines(gridLines);
      ConvertAndAssembleAnalysisTasks(analysisTasks);
      ConvertAndAssembleCombinations(combinations);

      DeleteExistingResults();
    }

    internal Model GetModel() {
      return _model;
    }

    private void AssembleNodesElementsMembersAndLists() {
      if (!_isSeedModel) {
        CreateModelFromDesignCodes();
        ModelFactory.SetUserDefaultUnits(_model);
        _model.UiUnits().LengthLarge = UnitMapping.GetApiUnit(_unit);
      }

      // Set API Nodes, Elements and Members in model
      _model.SetNodes(_nodes.ReadOnlyDictionary);
      _model.SetElements(_elements.ReadOnlyDictionary);
      _model.SetMembers(_members.ReadOnlyDictionary);

      // Set API Sections and Materials in model
      _model.SetSections(_sections.ReadOnlyDictionary);
      _model.SetSectionModifiers(_secionModifiers.ReadOnlyDictionary);
      _model.SetProp2Ds(_prop2ds.ReadOnlyDictionary);
      _model.SetProp3Ds(_prop3ds.ReadOnlyDictionary);

      ValidateMaterialsToDesignCodes(_model);

      foreach (KeyValuePair<int, AnalysisMaterial> mat in _customMaterials.ReadOnlyDictionary) {
        _model.SetAnalysisMaterial(mat.Key, mat.Value);
      }

      foreach (KeyValuePair<int, AluminiumMaterial> mat in _aluminiumMaterials.ReadOnlyDictionary) {
        _model.SetAluminiumMaterial(mat.Key, mat.Value);
      }

      foreach (KeyValuePair<int, ConcreteMaterial> mat in _concreteMaterials.ReadOnlyDictionary) {
        _model.SetConcreteMaterial(mat.Key, mat.Value);
      }

      foreach (KeyValuePair<int, FabricMaterial> mat in _fabricMaterials.ReadOnlyDictionary) {
        _model.SetFabricMaterial(mat.Key, mat.Value);
      }

      foreach (KeyValuePair<int, FrpMaterial> mat in _frpMaterials.ReadOnlyDictionary) {
        _model.SetFrpMaterial(mat.Key, mat.Value);
      }

      foreach (KeyValuePair<int, GlassMaterial> mat in _glassMaterials.ReadOnlyDictionary) {
        _model.SetGlassMaterial(mat.Key, mat.Value);
      }

      foreach (KeyValuePair<int, SteelMaterial> mat in _steelMaterials.ReadOnlyDictionary) {
        _model.SetSteelMaterial(mat.Key, mat.Value);
      }

      foreach (KeyValuePair<int, TimberMaterial> mat in _timberMaterials.ReadOnlyDictionary) {
        _model.SetTimberMaterial(mat.Key, mat.Value);
      }

      // Add API Node loads to model
      _model.AddNodeLoads(GsaAPI.NodeLoadType.APPL_DISP, new ReadOnlyCollection<NodeLoad>(_displacements));
      _model.AddNodeLoads(GsaAPI.NodeLoadType.NODE_LOAD, new ReadOnlyCollection<NodeLoad>(_nodeLoads));
      _model.AddNodeLoads(GsaAPI.NodeLoadType.SETTLEMENT, new ReadOnlyCollection<NodeLoad>(_settlements));

      // Set API lists for Nodes in model
      _model.SetLists(_lists.ReadOnlyDictionary);
    }

    private void AssembleLoadsCasesAxesGridPlaneSurfacesAndLists(GH_Component owner) {
      // Add API Loads in model
      _model.SetLoadCases(new ReadOnlyDictionary<int, LoadCase>(_loadCases));
      _model.AddGravityLoads(new ReadOnlyCollection<GravityLoad>(_gravityLoads));
      _model.AddBeamLoads(new ReadOnlyCollection<BeamLoad>(_beamLoads));
      _model.AddBeamThermalLoads(new ReadOnlyCollection<BeamThermalLoad>(_beamThermalLoads));
      _model.AddFaceLoads(new ReadOnlyCollection<FaceLoad>(_faceLoads));
      _model.AddFaceThermalLoads(new ReadOnlyCollection<FaceThermalLoad>(_faceThermalLoads));
      _model.AddGridPointLoads(new ReadOnlyCollection<GridPointLoad>(_gridPointLoads));
      _model.AddGridLineLoads(new ReadOnlyCollection<GridLineLoad>(_gridLineLoads));
      _model.AddGridAreaLoads(new ReadOnlyCollection<GridAreaLoad>(_gridAreaLoads));
      // Set API Axis, GridPlanes and GridSurface in model
      _model.SetAxes(_axes.ReadOnlyDictionary);
      _model.SetGridPlanes(_gridPlanes.ReadOnlyDictionary);
      foreach (int gridSurfaceId in _gridSurfaces.ReadOnlyDictionary.Keys) {
        try {
          _model.SetGridSurface(gridSurfaceId, _gridSurfaces.ReadOnlyDictionary[gridSurfaceId]);
        } catch (ArgumentException e) {
          ReportWarningFromAddingGridSurfacesOrList(
            e.Message, _gridSurfaces.ReadOnlyDictionary[gridSurfaceId].Name,
            "Grid Surface", owner);
        }
      }
      // Set API list in model
      foreach (int listId in _lists.ReadOnlyDictionary.Keys) {
        try {
          _model.SetList(listId, _lists.ReadOnlyDictionary[listId]);
        } catch (ArgumentException e) {
          ReportWarningFromAddingGridSurfacesOrList(
            e.Message, _lists.ReadOnlyDictionary[listId].Name, "List", owner);
        }
      }
    }

    private void CheckIfModelIsEmpty() {
      if (_nodes.Count == 0
        && MaterialCount == 0
        && PropertiesCount == 0
        && _elements.Count == 0
        && _members.Count == 0
        && _model.ConcreteDesignCode() == string.Empty
        && _model.SteelDesignCode() == string.Empty) {
        _isSeedModel = false;
      }
    }

    private void ConvertAndAssembleAnalysisTasks(List<GsaAnalysisTask> analysisTasks) {
      // Set Analysis Tasks in model
      if (analysisTasks != null) {
        ReadOnlyDictionary<int, AnalysisTask> existingTasks = _model.AnalysisTasks();
        foreach (GsaAnalysisTask task in analysisTasks) {
          if (!existingTasks.Keys.Contains(task.Id)) {
            task.Id = _model.AddAnalysisTask();
          }

          if (task.Cases == null || task.Cases.Count == 0) {
            task.CreateDefaultCases(_gsaModel);
          }

          if (task.Cases == null) {
            continue;
          }

          foreach (GsaAnalysisCase ca in task.Cases) {
            _model.AddAnalysisCaseToTask(task.Id, ca.Name, ca.Definition);
          }
        }
      }
    }

    private void ConvertAndAssembleCombinations(List<GsaCombinationCase> combinations) {
      if (combinations.IsNullOrEmpty()) {
        return;
      }

      var existing = _model.CombinationCases()
        .ToDictionary(k => k.Key, k => k.Value);
      foreach (GsaCombinationCase co in combinations) {
        var apiCase = new CombinationCase(co.Name, co.Definition);
        if (co.Id > 0 && existing.ContainsKey(co.Id)) {
          existing[co.Id] = apiCase;
        } else {
          existing.Add(co.Id, apiCase);
        }
      }
      _model.SetCombinationCases(new ReadOnlyDictionary<int, CombinationCase>(existing));
    }

    private void ConvertAndAssembleGridLines(List<GsaGridLine> gridLines) {
      if (gridLines != null) {
        int id = 1;
        foreach (GsaGridLine gridLine in gridLines.OrderBy(x => x.GridLine.Label)) {
          _gridLines.SetValue(id, gridLine.GridLine);
          id++;
        }
        _model.SetGridLines(_gridLines.ReadOnlyDictionary);
      }
    }

    private void ConvertElements(List<GsaElement1d> element1ds, List<GsaElement2d> element2ds, List<GsaElement3d> element3ds) {
      if ((!element1ds.IsNullOrEmpty()) || (!element2ds.IsNullOrEmpty())
        || (!element3ds.IsNullOrEmpty())) {
        _deleteResults = true;
      }

      ConvertElement1ds(element1ds);
      ConvertElement2ds(element2ds);
      ConvertElement3ds(element3ds);

      if (!element2ds.IsNullOrEmpty()) {
        foreach (GsaElement2d e2d in element2ds) {
          int expectedCollapsedNodeCount = e2d.Mesh.TopologyVertices.Count;
          int actualNodeCount = e2d.TopoInt.Sum(topoint => topoint.Count);
          int difference = actualNodeCount - expectedCollapsedNodeCount;
          _initialNodeCount -= difference;
        }
      }

      if (!element3ds.IsNullOrEmpty()) {
        foreach (GsaElement3d e3d in element3ds) {
          int expectedCollapsedNodeCount = e3d.NgonMesh.TopologyVertices.Count;
          int actualNodeCount = e3d.TopoInt.Sum(topoint => topoint.Count);
          int difference = actualNodeCount - expectedCollapsedNodeCount;
          _initialNodeCount -= difference;
        }
      }
    }

    private void ConvertLists(List<GsaList> lists) {
      if (lists.IsNullOrEmpty()) {
        return;
      }

      foreach (GsaList list in lists) {
        switch (list.EntityType) {
          case EntityType.Element:
            if (list._elements == (null, null, null)) {
              continue;
            }

            ConvertElements(
              list._elements.e1d.Select(x => x.Value).ToList(),
              list._elements.e2d.Select(x => x.Value).ToList(),
              list._elements.e3d.Select(x => x.Value).ToList());
            break;

          case EntityType.Member:
            if (list._members == (null, null, null)) {
              continue;
            }

            ConvertMembers(
              list._members.m1d.Select(x => x.Value).ToList(),
              list._members.m2d.Select(x => x.Value).ToList(),
              list._members.m3d.Select(x => x.Value).ToList());
            break;
        }
      }
    }

    private void ConvertLoadCases(List<GsaLoadCase> loadCases, GH_Component owner) {
      if (loadCases.IsNullOrEmpty()) {
        return;
      }

      foreach (GsaLoadCase loadCase in loadCases) {
        if (_loadCases.ContainsKey(loadCase.Id)) {
          LoadCase existingCase = _loadCases[loadCase.Id];
          LoadCase newCase = loadCase.DuplicateApiObject();
          if (newCase.CaseType != existingCase.CaseType || newCase.Name != existingCase.Name) {
            _loadCases[loadCase.Id] = newCase;
            owner?.AddRuntimeRemark($"LoadCase {loadCase.Id} either already existed in the model " +
             $"or two load cases with ID:{loadCase.Id} was added.{Environment.NewLine}" +
             $"{newCase.Name} - {newCase.CaseType} replaced previous LoadCase");
          }

          _loadCases[loadCase.Id] = newCase;
        } else {
          _loadCases.Add(loadCase.Id, loadCase.DuplicateApiObject());
        }
      }
    }

    private void ConvertMembers(List<GsaMember1d> member1ds, List<GsaMember2d> member2ds, List<GsaMember3d> member3ds) {
      if ((!member1ds.IsNullOrEmpty()) || (!member2ds.IsNullOrEmpty())
        || (!member3ds.IsNullOrEmpty())) {
        _deleteResults = true;
      }

      ConvertMember1ds(member1ds);
      ConvertMember2ds(member2ds);
      ConvertMember3ds(member3ds);
    }

    private void ConvertNodeList(List<GsaList> lists) {
      int nodeCountBefore = _nodes.Count;
      ConvertNodeLists(lists);
      if (nodeCountBefore > _nodes.Count) {
        _deleteResults = true;
      }
    }

    private void ConvertProperties(List<GsaMaterial> materials, List<GsaSection> sections,
      List<GsaProperty2d> prop2Ds, List<GsaProperty3d> prop3Ds) {
      if ((!materials.IsNullOrEmpty()) || (!sections.IsNullOrEmpty())
        || (!prop2Ds.IsNullOrEmpty()) || (!prop3Ds.IsNullOrEmpty())) {
        _deleteResults = true;
      }

      if (!materials.IsNullOrEmpty()) {
        foreach (GsaMaterial material in materials) {
          ConvertMaterial(material);
        }
      }

      ConvertSections(sections);
      ConvertProp2ds(prop2Ds);
      ConvertProp3ds(prop3Ds);
    }

    private void CreateModelFromDesignCodes() {
      string concreteCode = GetConcreteDesignCode(_model);
      string steelCode = GetSteelDesignCode(_model);

      _model = ModelFactory.CreateModelFromCodes(concreteCode, steelCode);
    }

    private void DeleteExistingResults() {
      if (!_deleteResults) {
        return;
      }

      foreach (int taskId in _model.AnalysisTasks().Keys) {
        _model.DeleteResults(taskId);
      }
    }

    private void ElementsFromMembers(bool createElementsFromMembers, Length toleranceCoincidentNodes, GH_Component owner) {
      _initialNodeCount += _nodes.Count;

      if (createElementsFromMembers && _members.Count != 0) {
        ConcurrentDictionary<int, ConcurrentBag<int>> initialMemberElementRelationship
        = GetMemberElementRelationship(_model);
        var elemIds = new List<int>();
        foreach (int id in _members.ReadOnlyDictionary.Keys) {
          if (initialMemberElementRelationship.ContainsKey(id)) {
            elemIds.AddRange(initialMemberElementRelationship[id]);
          }
        }

        if (elemIds.Count > 0) {
          string warning = "Creating Elements From Members will recreate child Elements." +
            Environment.NewLine + "This will update the Element's property to the parent Member's property, " +
            Environment.NewLine + "and may also renumber element IDs. " + Environment.NewLine +
            Environment.NewLine + "The following former Element IDs were updated:" + Environment.NewLine;

          string ids = GsaList.CreateListDefinition(elemIds);
          owner.AddRuntimeWarning(warning + ids);
        }

        _model.CreateElementsFromMembers();
      }

      // Sense-checking model after Elements from Members
      if (toleranceCoincidentNodes.Value > 0) {
        _model.CollapseCoincidentNodes(toleranceCoincidentNodes.Meters);
        if (owner != null) {
          try {
            double minMeshSize = _members.ReadOnlyDictionary.Values.Where(x => x.MeshSize != 0)
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

          int newNodeCount = _model.Nodes().Keys.Count;
          double nodeSurvivalRate = newNodeCount / (double)_initialNodeCount;

          int elemCount = _elements.Count;
          int memCount = _members.Count;
          // warning if >95% of nodes are removed for elements or >80% for members
          double warningSurvivalRate = elemCount > memCount ? 0.05 : 0.2;
          // remark if >80% of nodes are removed for elements or >66% for members
          double remarkSurvivalRate = elemCount > memCount ? 0.2 : 0.33;

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

      memberElementRelationship = GetMemberElementRelationship(_model);
    }

    private void ReportWarningFromAddingGridSurfacesOrList(string mes, string troubleName, string type, GH_Component owner) {
      foreach (KeyValuePair<int, GridSurface> gridSurface in _model.GridSurfaces()) {
        if (gridSurface.Value.Name == troubleName) {
          mes += $"\nA Grid Surface called '{troubleName}' was already added as a GridSurface at index {gridSurface.Key}";
          mes += $"\nThe {type} '{troubleName}' was skipped";
          owner.AddRuntimeWarning(mes);
          return;
        }
      }

      foreach (KeyValuePair<int, EntityList> list in _model.Lists()) {
        if (list.Value.Name == troubleName) {
          mes += $"\nA List called '{troubleName}' was already added as a List at index {list.Key}";
          mes += $"\nThe {type} '{troubleName}' was skipped";
          owner.AddRuntimeWarning(mes);
          return;
        }
      }
    }

    private void SetupModel(GsaModel model, LengthUnit unit) {
      model ??= new GsaModel();
      _model = model.Model;
      _gsaModel = model;
      _unit = unit;
      _model.UiUnits().LengthLarge = UnitMapping.GetApiUnit(_unit);
      UiUnits units = _model.UiUnits();
      _nodes = new GsaIntDictionary<Node>(model.ApiNodes);
      _axes = new GsaIntDictionary<Axis>(model.ApiAxis);
      _elements = new GsaGuidIntListDictionary<Element>(_model.Elements());
      _members = new GsaGuidDictionary<Member>(_model.Members());
      _lists = new GsaGuidDictionary<EntityList>(_model.Lists());
      _gridLines = new GsaIntDictionary<GridLine>(_model.GridLines());

      GetLoadCasesFromModel(_model);
      _gridPlanes = new GsaGuidDictionary<GridPlane>(_model.GridPlanes());
      _gridSurfaces = new GsaGuidDictionary<GridSurface>(_model.GridSurfaces());

      (_sections, _secionModifiers) = GetSectionDictionary(model);
      _prop2ds = GetProp2dDictionary(model);
      _prop3ds = GetProp3dDictionary(model);

      _steelMaterials = GetStandardMaterialDictionary<SteelMaterial>(model.Materials.SteelMaterials);
      _concreteMaterials = GetStandardMaterialDictionary<ConcreteMaterial>(model.Materials.ConcreteMaterials);
      _frpMaterials = GetStandardMaterialDictionary<FrpMaterial>(model.Materials.FrpMaterials);
      _aluminiumMaterials = GetStandardMaterialDictionary<AluminiumMaterial>(model.Materials.AluminiumMaterials);
      _timberMaterials = GetStandardMaterialDictionary<TimberMaterial>(model.Materials.TimberMaterials);
      _glassMaterials = GetStandardMaterialDictionary<GlassMaterial>(model.Materials.GlassMaterials);
      _fabricMaterials = GetStandardMaterialDictionary<FabricMaterial>(model.Materials.FabricMaterials);
      _customMaterials = GetCustomMaterialDictionary(model.Materials.AnalysisMaterials);
      _concreteDesignCode = model.Model.ConcreteDesignCode();
      _steelDesignCode = model.Model.SteelDesignCode();
      GetGsaGhMaterialsDictionary(model.Materials);

      CheckIfModelIsEmpty();

      _nodeLoads = new List<NodeLoad>();
      _displacements = new List<NodeLoad>();
      _settlements = new List<NodeLoad>();
    }
  }
}
