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
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Helpers.Export {
  internal class ModelAssembly {
    internal Model Model;
    internal GsaIntDictionary<Node> Nodes;
    internal GsaIntDictionary<Axis> Axes;
    internal Properties Properties;
    internal GsaGuidIntListDictionary<Element> Elements;
    internal GsaGuidDictionary<Member> Members;
    internal GsaGuidDictionary<EntityList> Lists;
    internal Loads Loads;
    internal ConcurrentDictionary<int, ConcurrentBag<int>> MemberElementRelationship;
    internal LengthUnit Unit = LengthUnit.Meter;
    private bool _deleteResults = false;
    private int _initialNodeCount = 0;
    private bool _isSeedModel = true;

    internal ModelAssembly(GsaModel model, LengthUnit unit) {
      if (model == null) {
        model = new GsaModel();
      }

      Model = model.Model;
      Unit = unit;
      Nodes = new GsaIntDictionary<Node>(model.ApiNodes);
      Axes = new GsaIntDictionary<Axis>(model.ApiAxis);
      Properties = new Properties(model);
      Elements = new GsaGuidIntListDictionary<Element>(Model.Elements());
      Members = new GsaGuidDictionary<Member>(Model.Members());
      Lists = new GsaGuidDictionary<EntityList>(Model.Lists());
      Loads = new Loads(Model);
      CheckIfModelIsEmpty();
    }

    internal void ConvertNodes(List<GsaNode> nodes) {
      Export.Nodes.ConvertNodes(nodes, ref Nodes, ref Axes, Unit);
      Nodes.UpdateFirstEmptyKeyToMaxKey();
    }

    internal void ConvertProperties(List<GsaSection> sections,
      List<GsaProp2d> prop2Ds,
      List<GsaProp3d> prop3Ds) {
      if ((sections != null && sections.Count > 0)
        || (prop2Ds != null && prop2Ds.Count > 0)
        || (prop3Ds != null && prop3Ds.Count > 0)) {
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
      if ((element1ds != null && element1ds.Count > 0)
        || (element2ds != null && element2ds.Count > 0)
        || (element3ds != null && element3ds.Count > 0)) {
        _deleteResults = true;
      }

      Export.Elements.ConvertElement1ds(element1ds, ref Elements, ref Nodes, Unit, ref Properties);
      Export.Elements.ConvertElement2ds(
        element2ds, ref Elements, ref Nodes, Unit, ref Properties, ref Axes);
      Export.Elements.ConvertElement3ds(element3ds, ref Elements, ref Nodes, Unit, ref Properties);

      if (element2ds != null && element2ds.Count > 0) {
        foreach (GsaElement2d e2d in element2ds) {
          int expectedCollapsedNodeCount = e2d.Mesh.TopologyVertices.Count;
          int actualNodeCount = e2d.TopoInt.Sum(topoint => topoint.Count);
          int difference = actualNodeCount - expectedCollapsedNodeCount;
          _initialNodeCount -= difference;
        }
      }

      if (element3ds != null && element3ds.Count > 0) {
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
      if ((member1ds != null && member1ds.Count > 0)
        || (member2ds != null && member2ds.Count > 0)
        || (member3ds != null && member3ds.Count > 0)) {
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
      if (loads != null && loads.Count > 0) {
        _deleteResults = true;
      }

      Load.NodeLoads.ConvertNodeLoads(loads, ref Loads.Nodes, ref Nodes, ref Lists, Unit);
    }

    internal void AssemblePreMeshing() {
      if (!_isSeedModel) {
        CreateModelFromDesignCodes();
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

    internal void AssemblePostMeshing() {
      // Add API Loads in model
      Model.AddGravityLoads(new ReadOnlyCollection<GravityLoad>(Loads.Gravities));
      Model.AddBeamLoads(new ReadOnlyCollection<BeamLoad>(Loads.Beams));
      Model.AddFaceLoads(new ReadOnlyCollection<FaceLoad>(Loads.Faces));
      Model.AddGridPointLoads(new ReadOnlyCollection<GridPointLoad>(Loads.GridPoints));
      Model.AddGridLineLoads(new ReadOnlyCollection<GridLineLoad>(Loads.GridLines));
      Model.AddGridAreaLoads(new ReadOnlyCollection<GridAreaLoad>(Loads.GridAreas));
      // Set API Axis, GridPlanes and GridSurface in model
      Model.SetAxes(Axes.ReadOnlyDictionary);
      Model.SetGridPlanes(Loads.GridPlaneSurfaces.GridPlanes.ReadOnlyDictionary);
      foreach (int gridSurfaceId in Loads.GridPlaneSurfaces.GridSurfaces.ReadOnlyDictionary.Keys) {
        Model.SetGridSurface(gridSurfaceId, Loads.GridPlaneSurfaces.GridSurfaces.ReadOnlyDictionary[gridSurfaceId]);
      }
      // Set API list in model
      Model.SetLists(Lists.ReadOnlyDictionary);
    }

    internal void ConvertAnalysisTasks(List<GsaAnalysisTask> analysisTasks) {
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
    internal void ConvertCombinations(List<GsaCombinationCase> combinations) {
      // Add Combination Cases to model
      if (combinations != null && combinations.Count > 0) {
        foreach (GsaCombinationCase co in combinations) {
          Model.AddCombinationCase(co.Name, co.Description);
        }
      }
    }

    internal void DeleteExistingResults() {
      if (_deleteResults) {
        foreach (int taskId in Model.AnalysisTasks().Keys) {
          Model.DeleteResults(taskId);
        }
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
  }
}
