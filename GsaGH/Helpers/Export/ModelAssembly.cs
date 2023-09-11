﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Components;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi.EnumMappings;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using LengthUnit = OasysUnits.Units.LengthUnit;
using LoadCase = GsaAPI.LoadCase;

namespace GsaGH.Helpers.Export {
  internal class ModelAssembly {
    internal Model Model;
    internal GsaIntDictionary<Node> Nodes;
    internal GsaIntDictionary<Axis> Axes;
    internal Properties Properties;
    internal GsaGuidIntListDictionary<Element> Elements;
    internal GsaGuidDictionary<Member> Members;
    internal GsaGuidDictionary<EntityList> Lists;
    internal GsaIntDictionary<GridLine> _gridLines;
    internal Loads Loads;
    internal ConcurrentDictionary<int, ConcurrentBag<int>> MemberElementRelationship;
    internal LengthUnit Unit = LengthUnit.Meter;
    private bool _deleteResults = false;
    private int _initialNodeCount = 0;
    private bool _isSeedModel = true;

    internal ModelAssembly(GsaModel model, LengthUnit unit) {
      model ??= new GsaModel();
      Model = model.Model;
      Unit = unit;
      Model.UiUnits().LengthLarge = UnitMapping.GetApiUnit(Unit);
      UiUnits units = Model.UiUnits();
      Nodes = new GsaIntDictionary<Node>(model.ApiNodes);
      Axes = new GsaIntDictionary<Axis>(model.ApiAxis);
      Properties = new Properties(model);
      Elements = new GsaGuidIntListDictionary<Element>(Model.Elements());
      Members = new GsaGuidDictionary<Member>(Model.Members());
      Lists = new GsaGuidDictionary<EntityList>(Model.Lists());
      _gridLines = new GsaIntDictionary<GridLine>(Model.GridLines());
      Loads = new Loads(Model);
      CheckIfModelIsEmpty();
    }

    internal void ConvertNodes(List<GsaNode> nodes) {
      Export.Nodes.ConvertNodes(nodes, ref Nodes, ref Axes, Unit);
      Nodes.UpdateFirstEmptyKeyToMaxKey();
    }

    internal void ConvertProperties(List<GsaSection> sections,
      List<GsaProperty2d> prop2Ds,
      List<GsaProperty3d> prop3Ds) {
      if ((!sections.IsNullOrEmpty()) || (!prop2Ds.IsNullOrEmpty()) || (!prop3Ds.IsNullOrEmpty())) {
        _deleteResults = true;
      }

      Sections.ConvertSections(sections, ref Properties);
      Prop2ds.ConvertProp2ds(prop2Ds, ref Properties, ref Axes, Unit);
      Prop3ds.ConvertProp3ds(prop3Ds, ref Properties);
    }

    internal void ConvertElements(
      List<GsaElement1d> element1ds,
      List<GsaElement2d> element2ds,
      List<GsaElement3d> element3ds) {
      if ((!element1ds.IsNullOrEmpty()) || (!element2ds.IsNullOrEmpty())
        || (!element3ds.IsNullOrEmpty())) {
        _deleteResults = true;
      }

      Export.Elements.ConvertElement1ds(element1ds, ref Elements, ref Nodes, Unit, ref Properties);
      Export.Elements.ConvertElement2ds(
        element2ds, ref Elements, ref Nodes, Unit, ref Properties, ref Axes);
      Export.Elements.ConvertElement3ds(element3ds, ref Elements, ref Nodes, Unit, ref Properties);

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

      Export.Members.ConvertMember1ds(member1ds, ref Members, ref Nodes, Unit, ref Properties);
      Export.Members.ConvertMember2ds(
        member2ds, ref Members, ref Nodes, Unit, ref Properties, ref Axes);
      Export.Members.ConvertMember3ds(member3ds, ref Members, ref Nodes, Unit, ref Properties);
    }

    internal void ConvertNodeList(List<GsaList> lists) {
      int nodeCountBefore = Nodes.Count;
      Export.Lists.ConvertNodeLists(lists, ref Lists, ref Nodes, Unit);
      if (nodeCountBefore > Nodes.Count) {
        _deleteResults = true;
      }
    }

    internal void ConvertNodeLoads(List<IGsaLoad> loads) {
      if (!loads.IsNullOrEmpty()) {
        _deleteResults = true;
      }

      Load.NodeLoads.ConvertNodeLoads(loads, ref Loads.Nodes, ref Nodes, ref Lists, Unit);
    }

    internal void ConvertLoadCases(List<GsaLoadCase> loadCases, GH_Component owner) {
      if (loadCases.IsNullOrEmpty()) {
        return;
      }

      foreach (GsaLoadCase loadCase in loadCases) {
        if (Loads.LoadCases.ContainsKey(loadCase.Id)) {
          LoadCase existingCase = Loads.LoadCases[loadCase.Id];
          LoadCase newCase = loadCase.LoadCase;
          if (newCase.CaseType != existingCase.CaseType || newCase.Name != existingCase.Name) {
            Loads.LoadCases[loadCase.Id] = newCase;
            owner?.AddRuntimeRemark($"LoadCase {loadCase.Id} either already existed in the model " +
             $"or two load cases with ID:{loadCase.Id} was added.{Environment.NewLine}" +
             $"{newCase.Name} - {newCase.CaseType} replaced previous LoadCase");
          }
        } else {
          Loads.LoadCases.Add(loadCase.Id, loadCase.LoadCase);
        }
      }
    }

    internal void ConvertLists(List<GsaList> lists) {
      if (lists.IsNullOrEmpty()) {
        return;
      }

      foreach (GsaList list in lists) {
        switch (list.EntityType) {
          case Parameters.Enums.EntityType.Element:
            if (list._elements == (null, null, null)) {
              continue;
            }

            ConvertElements(
              list._elements.e1d.Select(x => x.Value).ToList(),
              list._elements.e2d.Select(x => x.Value).ToList(),
              list._elements.e3d.Select(x => x.Value).ToList());
            break;

          case Parameters.Enums.EntityType.Member:
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
      Properties.Assemble(ref Model);

      // Add API Node loads to model
      Loads.Nodes.Assemble(ref Model);

      // Set API lists for Nodes in model
      Model.SetLists(Lists.ReadOnlyDictionary);
    }

    internal void ElementsFromMembers(bool createElementsFromMembers, Length toleranceCoincidentNodes, GH_Component owner) {
      _initialNodeCount += Nodes.Count;

      if (createElementsFromMembers && Members.Count != 0) {
        ConcurrentDictionary<int, ConcurrentBag<int>> initialMemberElementRelationship
        = ElementListFromReference.GetMemberElementRelationship(Model);
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

      MemberElementRelationship = ElementListFromReference.GetMemberElementRelationship(Model);
    }

    internal void AssembleLoadsCasesAxesGridPlaneSurfacesAndLists(GH_Component owner) {
      // Add API Loads in model
      Model.SetLoadCases(new ReadOnlyDictionary<int, LoadCase>(Loads.LoadCases));
      Model.AddGravityLoads(new ReadOnlyCollection<GravityLoad>(Loads.Gravities));
      Model.AddBeamLoads(new ReadOnlyCollection<BeamLoad>(Loads.Beams));
      Model.AddBeamThermalLoads(new ReadOnlyCollection<BeamThermalLoad>(Loads.BeamThermals));
      Model.AddFaceLoads(new ReadOnlyCollection<FaceLoad>(Loads.Faces));
      Model.AddFaceThermalLoads(new ReadOnlyCollection<FaceThermalLoad>(Loads.FaceThermals));
      Model.AddGridPointLoads(new ReadOnlyCollection<GridPointLoad>(Loads.GridPoints));
      Model.AddGridLineLoads(new ReadOnlyCollection<GridLineLoad>(Loads.GridLines));
      Model.AddGridAreaLoads(new ReadOnlyCollection<GridAreaLoad>(Loads.GridAreas));
      // Set API Axis, GridPlanes and GridSurface in model
      Model.SetAxes(Axes.ReadOnlyDictionary);
      Model.SetGridPlanes(Loads.GridPlaneSurfaces.GridPlanes.ReadOnlyDictionary);
      foreach (int gridSurfaceId in Loads.GridPlaneSurfaces.GridSurfaces.ReadOnlyDictionary.Keys) {
        try {
          Model.SetGridSurface(gridSurfaceId, Loads.GridPlaneSurfaces.GridSurfaces.ReadOnlyDictionary[gridSurfaceId]);
        } catch (ArgumentException e) {
          ReportWarningFromAddingGridSurfacesOrList(
            e.Message, Loads.GridPlaneSurfaces.GridSurfaces.ReadOnlyDictionary[gridSurfaceId].Name,
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
        && Properties.Materials.Count == 0
        && Properties.Count == 0
        && Elements.Count == 0
        && Members.Count == 0
        && Model.ConcreteDesignCode() == string.Empty
        && Model.SteelDesignCode() == string.Empty) {
        _isSeedModel = false;
      }
    }

    private void CreateModelFromDesignCodes() {
      string concreteCode = Properties.Materials.ConcreteDesignCode;
      if (concreteCode == string.Empty) {
        if (Model.ConcreteDesignCode() != string.Empty) {
          concreteCode = Model.ConcreteDesignCode();
        } else {
          concreteCode = DesignCode.GetConcreteDesignCodeNames()[8];
        }
      }

      string steelCode = Properties.Materials.SteelDesignCode;
      if (steelCode == string.Empty) {
        if (Model.SteelDesignCode() != string.Empty) {
          steelCode = Model.SteelDesignCode();
        } else {
          steelCode = DesignCode.GetSteelDesignCodeNames()[8];
        }
      }

      Model = GsaModel.CreateModelFromCodes(concreteCode, steelCode);
      Properties.Materials.ConcreteDesignCode = concreteCode;
      Properties.Materials.SteelDesignCode = steelCode;
    }

    internal void ConvertAndAssembleGridLines(List<GsaGridLine> gridLines) {
      if (gridLines != null) {
        int id = 1;
        foreach (GsaGridLine gridLine in gridLines.OrderBy(x => x._gridLine.Label)) {
          _gridLines.SetValue(id, gridLine._gridLine);
          id++;
        }
        Model.SetGridLines(_gridLines.ReadOnlyDictionary);
      }
    }
  }
}
