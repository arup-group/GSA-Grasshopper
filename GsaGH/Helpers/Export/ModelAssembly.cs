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
using GsaGH.Helpers.Import;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using EntityType = GsaGH.Parameters.EntityType;
using LengthUnit = OasysUnits.Units.LengthUnit;
using LoadCase = GsaAPI.LoadCase;

namespace GsaGH.Helpers.Export {
  internal partial class ModelAssembly {
    internal Model Model;
    private GsaIntDictionary<Node> Nodes;
    private GsaIntDictionary<Axis> Axes;
    private GsaGuidIntListDictionary<Element> Elements;
    private GsaGuidDictionary<Member> Members;
    private GsaGuidDictionary<EntityList> Lists;
    private GsaIntDictionary<GridLine> _gridLines;

    private ConcurrentDictionary<int, ConcurrentBag<int>> MemberElementRelationship;
    private LengthUnit Unit = LengthUnit.Meter;
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
    internal ModelAssembly(GsaModel model, List<GsaList> lists,
      List<GsaElement1d> elem1ds, List<GsaElement2d> elem2ds, List<GsaMember1d> mem1ds,
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



    public Model GetModel() {
      return Model;
    }






    internal void SetupModel(GsaModel model, LengthUnit unit) {
      model ??= new GsaModel();
      Model = model.Model;
      Unit = unit;
      Model.UiUnits().LengthLarge = UnitMapping.GetApiUnit(Unit);
      UiUnits units = Model.UiUnits();
      Nodes = new GsaIntDictionary<Node>(model.ApiNodes);
      Axes = new GsaIntDictionary<Axis>(model.ApiAxis);
      Elements = new GsaGuidIntListDictionary<Element>(Model.Elements());
      Members = new GsaGuidDictionary<Member>(Model.Members());
      Lists = new GsaGuidDictionary<EntityList>(Model.Lists());
      _gridLines = new GsaIntDictionary<GridLine>(Model.GridLines());



      GetLoadCasesFromModel(Model);
      _gridPlanes = new GsaGuidDictionary<GridPlane>(Model.GridPlanes());
      _gridSurfaces = new GsaGuidDictionary<GridSurface>(Model.GridSurfaces());

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

    internal void ConvertProperties(List<GsaMaterial> materials, List<GsaSection> sections,
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

    internal void ConvertElements(
      List<GsaElement1d> element1ds,
      List<GsaElement2d> element2ds,
      List<GsaElement3d> element3ds) {
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

    internal void ConvertMembers(
      List<GsaMember1d> member1ds,
      List<GsaMember2d> member2ds,
      List<GsaMember3d> member3ds) {
      if ((!member1ds.IsNullOrEmpty()) || (!member2ds.IsNullOrEmpty())
        || (!member3ds.IsNullOrEmpty())) {
        _deleteResults = true;
      }

      ConvertMember1ds(member1ds);
      ConvertMember2ds(member2ds);
      ConvertMember3ds(member3ds);
    }

    internal void ConvertNodeList(List<GsaList> lists) {
      int nodeCountBefore = Nodes.Count;
      ConvertNodeLists(lists);
      if (nodeCountBefore > Nodes.Count) {
        _deleteResults = true;
      }
    }

    internal void ConvertLoadCases(List<GsaLoadCase> loadCases, GH_Component owner) {
      if (loadCases.IsNullOrEmpty()) {
        return;
      }

      foreach (GsaLoadCase loadCase in loadCases) {
        if (_loadCases.ContainsKey(loadCase.Id)) {
          LoadCase existingCase = _loadCases[loadCase.Id];
          LoadCase newCase = loadCase.LoadCase;
          if (newCase.CaseType != existingCase.CaseType || newCase.Name != existingCase.Name) {
            _loadCases[loadCase.Id] = newCase;
            owner?.AddRuntimeRemark($"LoadCase {loadCase.Id} either already existed in the model " +
             $"or two load cases with ID:{loadCase.Id} was added.{Environment.NewLine}" +
             $"{newCase.Name} - {newCase.CaseType} replaced previous LoadCase");
          }
        } else {
          _loadCases.Add(loadCase.Id, loadCase.LoadCase);
        }
      }
    }

    internal void ConvertLists(List<GsaList> lists) {
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

    internal void AssembleNodesElementsMembersAndLists() {
      if (!_isSeedModel) {
        CreateModelFromDesignCodes();
        GsaModel.SetUserDefaultUnits(Model.UiUnits());
        Model.UiUnits().LengthLarge = UnitMapping.GetApiUnit(Unit);
      }

      // Set API Nodes, Elements and Members in model
      Model.SetNodes(Nodes.ReadOnlyDictionary);
      Model.SetElements(Elements.ReadOnlyDictionary);
      Model.SetMembers(Members.ReadOnlyDictionary);

      // Set API Sections and Materials in model
      Model.SetSections(_sections.ReadOnlyDictionary);
      Model.SetSectionModifiers(_secionModifiers.ReadOnlyDictionary);
      Model.SetProp2Ds(_prop2ds.ReadOnlyDictionary);
      Model.SetProp3Ds(_prop3ds.ReadOnlyDictionary);

      // Add API Node loads to model
      Model.AddNodeLoads(GsaAPI.NodeLoadType.APPL_DISP, new ReadOnlyCollection<NodeLoad>(_displacements));
      Model.AddNodeLoads(GsaAPI.NodeLoadType.NODE_LOAD, new ReadOnlyCollection<NodeLoad>(_nodeLoads));
      Model.AddNodeLoads(GsaAPI.NodeLoadType.SETTLEMENT, new ReadOnlyCollection<NodeLoad>(_settlements));

      // Set API lists for Nodes in model
      Model.SetLists(Lists.ReadOnlyDictionary);
    }

    internal void ElementsFromMembers(bool createElementsFromMembers, Length toleranceCoincidentNodes, GH_Component owner) {
      _initialNodeCount += Nodes.Count;

      if (createElementsFromMembers && Members.Count != 0) {
        ConcurrentDictionary<int, ConcurrentBag<int>> initialMemberElementRelationship
        = GetMemberElementRelationship(Model);
        var elemIds = new List<int>();
        foreach (int id in Members.ReadOnlyDictionary.Keys) {
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

        Model.CreateElementsFromMembers();
      }

      // Sense-checking model after Elements from Members
      if (toleranceCoincidentNodes.Value > 0) {
        Model.CollapseCoincidentNodes(toleranceCoincidentNodes.Meters);
        if (owner != null) {
          try {
            double minMeshSize = Members.ReadOnlyDictionary.Values.Where(x => x.MeshSize != 0)
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

          int newNodeCount = Model.Nodes().Keys.Count;
          double nodeSurvivalRate = newNodeCount / (double)_initialNodeCount;

          int elemCount = Elements.Count;
          int memCount = Members.Count;
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

      MemberElementRelationship = GetMemberElementRelationship(Model);
    }

    internal void AssembleLoadsCasesAxesGridPlaneSurfacesAndLists(GH_Component owner) {
      // Add API Loads in model
      Model.SetLoadCases(new ReadOnlyDictionary<int, LoadCase>(_loadCases));
      Model.AddGravityLoads(new ReadOnlyCollection<GravityLoad>(_gravityLoads));
      Model.AddBeamLoads(new ReadOnlyCollection<BeamLoad>(_beamLoads));
      Model.AddBeamThermalLoads(new ReadOnlyCollection<BeamThermalLoad>(_beamThermalLoads));
      Model.AddFaceLoads(new ReadOnlyCollection<FaceLoad>(_faceLoads));
      Model.AddFaceThermalLoads(new ReadOnlyCollection<FaceThermalLoad>(_faceThermalLoads));
      Model.AddGridPointLoads(new ReadOnlyCollection<GridPointLoad>(_gridPointLoads));
      Model.AddGridLineLoads(new ReadOnlyCollection<GridLineLoad>(_gridLineLoads));
      Model.AddGridAreaLoads(new ReadOnlyCollection<GridAreaLoad>(_gridAreaLoads));
      // Set API Axis, GridPlanes and GridSurface in model
      Model.SetAxes(Axes.ReadOnlyDictionary);
      Model.SetGridPlanes(_gridPlanes.ReadOnlyDictionary);
      foreach (int gridSurfaceId in _gridSurfaces.ReadOnlyDictionary.Keys) {
        try {
          Model.SetGridSurface(gridSurfaceId, _gridSurfaces.ReadOnlyDictionary[gridSurfaceId]);
        } catch (ArgumentException e) {
          ReportWarningFromAddingGridSurfacesOrList(
            e.Message, _gridSurfaces.ReadOnlyDictionary[gridSurfaceId].Name,
            "Grid Surface", owner);
        }
      }
      // Set API list in model
      foreach (int listId in Lists.ReadOnlyDictionary.Keys) {
        try {
          Model.SetList(listId, Lists.ReadOnlyDictionary[listId]);
        } catch (ArgumentException e) {
          ReportWarningFromAddingGridSurfacesOrList(
            e.Message, Lists.ReadOnlyDictionary[listId].Name, "List", owner);
        }
      }
    }

    private void ReportWarningFromAddingGridSurfacesOrList(
      string mes, string troubleName, string type, GH_Component owner) {
      foreach (KeyValuePair<int, GridSurface> gridSurface in Model.GridSurfaces()) {
        if (gridSurface.Value.Name == troubleName) {
          mes += $"\nA Grid Surface called '{troubleName}' was already added as a GridSurface at index {gridSurface.Key}";
          mes += $"\nThe {type} '{troubleName}' was skipped";
          owner.AddRuntimeWarning(mes);
          return;
        }
      }

      foreach (KeyValuePair<int, EntityList> list in Model.Lists()) {
        if (list.Value.Name == troubleName) {
          mes += $"\nA List called '{troubleName}' was already added as a List at index {list.Key}";
          mes += $"\nThe {type} '{troubleName}' was skipped";
          owner.AddRuntimeWarning(mes);
          return;
        }
      }
    }

    internal void ConvertAndAssembleAnalysisTasks(List<GsaAnalysisTask> analysisTasks) {
      // Set Analysis Tasks in model
      if (analysisTasks != null) {
        ReadOnlyDictionary<int, AnalysisTask> existingTasks = Model.AnalysisTasks();
        foreach (GsaAnalysisTask task in analysisTasks) {
          if (!existingTasks.Keys.Contains(task.Id)) {
            task.Id = Model.AddAnalysisTask();
          }

          if (task.Cases == null || task.Cases.Count == 0) {
            task.CreateDefaultCases(Model);
          }

          if (task.Cases == null) {
            continue;
          }

          foreach (GsaAnalysisCase ca in task.Cases) {
            Model.AddAnalysisCaseToTask(task.Id, ca.Name, ca.Description);
          }
        }
      }
    }

    internal void ConvertAndAssembleCombinations(List<GsaCombinationCase> combinations) {
      if (combinations.IsNullOrEmpty()) {
        return;
      }

      var existing = Model.CombinationCases()
        .ToDictionary(k => k.Key, k => k.Value);
      foreach (GsaCombinationCase co in combinations) {
        var apiCase = new CombinationCase(co.Name, co.Definition);
        if (co.Id > 0 && existing.ContainsKey(co.Id)) {
          existing[co.Id] = apiCase;
        } else {
          existing.Add(co.Id, apiCase);
        }
      }
      Model.SetCombinationCases(new ReadOnlyDictionary<int, CombinationCase>(existing));
    }

    internal void DeleteExistingResults() {
      if (!_deleteResults) {
        return;
      }

      foreach (int taskId in Model.AnalysisTasks().Keys) {
        Model.DeleteResults(taskId);
      }
    }

    private void CheckIfModelIsEmpty() {
      if (Nodes.Count == 0
        && _materialCount == 0
        && _propertiesCount == 0
        && Elements.Count == 0
        && Members.Count == 0
        && Model.ConcreteDesignCode() == string.Empty
        && Model.SteelDesignCode() == string.Empty) {
        _isSeedModel = false;
      }
    }

    private void CreateModelFromDesignCodes() {
      string concreteCode = GetConcreteDesignCode(Model);
      string steelCode = GetSteelDesignCode(Model);

      Model = GsaModel.CreateModelFromCodes(concreteCode, steelCode);
    }

    internal void ConvertAndAssembleGridLines(List<GsaGridLine> gridLines) {
      if (gridLines != null) {
        int id = 1;
        foreach (GsaGridLine gridLine in gridLines.OrderBy(x => x.GridLine.Label)) {
          _gridLines.SetValue(id, gridLine.GridLine);
          id++;
        }
        Model.SetGridLines(_gridLines.ReadOnlyDictionary);
      }
    }
  }
}
