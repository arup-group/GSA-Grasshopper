using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Helpers.GH;
using OasysUnits.Units;
using OasysUnits;
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


    internal ModelAssembly(GsaModel model) {
      Model = model.Model;
      Nodes = new GsaIntDictionary<Node>(Model.Nodes());
      Axes = new GsaIntDictionary<Axis>(Model.Axes());
      Properties = new Properties(model);
      Elements = new GsaGuidIntListDictionary<Element>(Model.Elements());
      Members = new GsaGuidDictionary<Member>(Model.Members());
      Lists = new GsaGuidDictionary<EntityList>(Model.Lists());
      Loads = new Loads();
      Unit = model.ModelUnit;
    }

    internal void ConvertProperties(List<GsaSection> sections,
      List<GsaProp2d> prop2Ds,
      List<GsaProp3d> prop3Ds) {
      if ((sections != null && sections.Count > 0)
        || (prop2Ds != null && prop2Ds.Count > 0)
        || (prop3Ds != null && prop3Ds.Count > 0)) {
        _deleteResults = true;
      }

      Sections.ConvertSection(sections, ref Properties);
      Prop2ds.ConvertProp2d(prop2Ds, ref Properties, ref Axes, Unit);
      Prop3ds.ConvertProp3d(prop3Ds, ref Properties);
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

      Export.Elements.ConvertElement1D(element1ds, ref Elements, ref Nodes, Unit, ref Properties);
      Export.Elements.ConvertElement2D(
        element2ds, ref Elements, ref Nodes, Unit, ref Properties, ref Axes);
      Export.Elements.ConvertElement3D(element3ds, ref Elements, ref Nodes, Unit, ref Properties);

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

      Export.Members.ConvertMember1D(member1ds, ref Members, ref Nodes, Unit, ref Properties);
      Export.Members.ConvertMember2D(member2ds, ref Members, ref Nodes, Unit, ref Properties, ref Axes);
      Export.Members.ConvertMember3D(member3ds, ref Members, ref Nodes, Unit, ref Properties);
    }

    internal void ConvertNodeList(List<GsaList> lists) {
      int nodeCountBefore = Nodes.Count;
      Export.Lists.ConvertNodeList(lists, ref Lists, ref Nodes, Unit);
      if (nodeCountBefore > Nodes.Count) {
        _deleteResults = true;
      }
    }

    internal void ConvertNodeLoads(List<GsaLoad> loads) {
      if (loads != null && loads.Count > 0) {
        _deleteResults = true;
      }

      Load.NodeLoads.ConvertNodeLoad(loads, ref Loads.Nodes, ref Nodes, ref Lists, Unit);
    }

    internal void AssemblePreMeshing() {
      // Set API Nodes, Elements and Members in model
      Model.SetNodes(Nodes.ReadOnlyDictionary);
      Model.SetElements(Elements.ReadOnlyDictionary);
      Model.SetMembers(Members.ReadOnlyDictionary);

      // Set API Sections and Materials in model
      Properties.Assemble(ref  Model);

      // Add API Node loads to model
      Loads.Nodes.Assemble(ref Model);
      
      // Set API lists for Nodes in model
      Model.SetLists(Lists.ReadOnlyDictionary);
    }

    internal void ElementsFromMembers(Length toleranceCoincidentNodes, GH_Component owner) {
      if (Members.Count == 0) {
        return;
      }
      
      _initialNodeCount += Nodes.Count;

      Model.CreateElementsFromMembers();

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

    internal void ConvertList(List<GsaList> lists, List<GsaLoad> loads, GH_Component owner) {
      Lists = new GsaGuidDictionary<EntityList>(Model.Lists());

      // Add lists embedded in loads as they may have ID > 0 set
      if (lists == null && loads != null && loads.Count > 0) {
        lists = Loads.GetLoadLists(loads);
      } else if (loads != null && loads.Count > 0) {
        lists.AddRange(Loads.GetLoadLists(loads));
      }

      Export.Lists.ConvertList(
        lists, ref Lists, Properties, Elements, Members, MemberElementRelationship, owner);
    }

    internal void ConvertLoads(List<GsaLoad> loads, List<GsaGridPlaneSurface> gridPlaneSurfaces) {
      Loads.GridPlaneSurfaces = new GridPlaneSurfaces(this);

      Loads.ConvertGridPlaneSurface(gridPlaneSurfaces, ref Axes, ref apiGridPlanes,
      ref apiGridSurfaces, ref gpGuid, ref gsGuid, ref apiLists, modelUnit, memberElementRelationship,
        gsa, apiMaterials, apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers, owner);

      Loads.ConvertLoad(loads, ref gravityLoads, ref beamLoads, ref faceLoads, ref gridPointLoads,
        ref gridLineLoads, ref gridAreaLoads, ref apiaxes, ref apiGridPlanes, ref apiGridSurfaces,
      ref gpGuid, ref gsGuid, ref apiLists, modelUnit, memberElementRelationship, gsa, apiMaterials,
        apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers, owner);

      
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
      Model.SetGridPlanes(Loads.GridPlanes.ReadOnlyDictionary);
      Model.SetGridSurfaces(Loads.GridSurfaces.ReadOnlyDictionary);
      // Set API list in model
      Model.SetLists(Lists.ReadOnlyDictionary);


    }
  }
}
