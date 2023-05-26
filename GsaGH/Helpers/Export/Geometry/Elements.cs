using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Helpers.Export {
  internal class Elements {

    internal static void ConvertElement1D(
      GsaElement1d element1d, ref GsaGuidIntListDictionary<Element> apiElements,
      ref GsaIntDictionary<Node> existingNodes, LengthUnit unit,
      ref GsaGuidDictionary<Section> apiSections,
      ref GsaIntDictionary<SectionModifier> apiSectionModifiers,
      ref GsaGuidDictionary<AnalysisMaterial> apiMaterials) {
      LineCurve line = element1d.Line;
      Element apiElement = element1d.GetApiElementClone();

      var topo = new List<int> {
        Nodes.AddNode(ref existingNodes, line.PointAtStart, unit),
        Nodes.AddNode(ref existingNodes, line.PointAtEnd, unit),
      };
      apiElement.Topology = new ReadOnlyCollection<int>(topo);

      if (element1d.OrientationNode != null) {
        apiElement.OrientationNode
          = Nodes.AddNode(ref existingNodes, element1d.OrientationNode.Point, unit);
      }

      apiElement.Property = Sections.ConvertSection(element1d.Section, ref apiSections,
        ref apiSectionModifiers, ref apiMaterials);

      AddElement(element1d.Id, element1d.Guid, apiElement, true, ref apiElements);
    }

    internal static void ConvertElement1D(
      List<GsaElement1d> element1ds, ref GsaGuidIntListDictionary<Element> apiElements,
      ref GsaIntDictionary<Node> existingNodes, LengthUnit unit,
      ref GsaGuidDictionary<Section> apiSections,
      ref GsaIntDictionary<SectionModifier> apiSectionModifiers,
      ref GsaGuidDictionary<AnalysisMaterial> apiMaterials) {
      if (element1ds == null) {
        return;
      }

      element1ds = element1ds.OrderByDescending(x => x.Id).ToList();
      foreach (GsaElement1d element1d in element1ds.Where(element1d => element1d != null)) {
        ConvertElement1D(element1d, ref apiElements, ref existingNodes, unit, ref apiSections,
          ref apiSectionModifiers, ref apiMaterials);
      }
    }

    internal static void ConvertElement2D(
      GsaElement2d element2d, ref GsaGuidIntListDictionary<Element> apiElements,
      ref GsaIntDictionary<Node> existingNodes, LengthUnit unit,
      ref GsaGuidDictionary<Prop2D> apiProp2ds,
      ref GsaGuidDictionary<AnalysisMaterial> apiMaterials,
      ref Dictionary<int, Axis> existingAxes) {
      List<Point3d> meshVerticies = element2d.Topology;

      for (int i = 0; i < element2d.ApiElements.Count; i++) {
        Element apiMeshElement = element2d.GetApiObjectClone(i);
        List<int> meshVertexIndex = element2d.TopoInt[i];

        var topo = new List<int>();
        foreach (int mesh in meshVertexIndex) {
          topo.Add(Nodes.AddNode(ref existingNodes, meshVerticies[mesh], unit));
        }

        apiMeshElement.Topology = new ReadOnlyCollection<int>(topo);

        GsaProp2d prop = (i > element2d.Prop2ds.Count - 1) ? element2d.Prop2ds.Last() :
          element2d.Prop2ds[i];
        apiMeshElement.Property = Prop2ds.ConvertProp2d(prop, ref apiProp2ds, ref apiMaterials,
          ref existingAxes, unit);

        AddElement(element2d.Ids[i], element2d.Guid, apiMeshElement, false, ref apiElements);
      }
    }

    internal static void ConvertElement2D(
      List<GsaElement2d> element2ds, ref GsaGuidIntListDictionary<Element> apiElements,
      ref GsaIntDictionary<Node> existingNodes, LengthUnit unit,
      ref GsaGuidDictionary<Prop2D> apiProp2ds,
      ref GsaGuidDictionary<AnalysisMaterial> apiMaterials,
      ref Dictionary<int, Axis> existingAxes) {
      if (element2ds == null) {
        return;
      }

      element2ds = element2ds.OrderByDescending(e => e.Ids.First()).ToList();
      foreach (GsaElement2d element2d in element2ds) {
        if (element2d != null) {
          ConvertElement2D(element2d, ref apiElements, ref existingNodes, unit, ref apiProp2ds,
            ref apiMaterials, ref existingAxes);
        }
      }
    }

    internal static void ConvertElement3D(
      GsaElement3d element3d, ref GsaGuidIntListDictionary<Element> apiElements,
      ref GsaIntDictionary<Node> existingNodes, LengthUnit unit,
      ref GsaGuidDictionary<Prop3D> apiProp3ds,
      ref GsaGuidDictionary<AnalysisMaterial> apiMaterials) {
      List<Point3d> meshVerticies = element3d.Topology;

      for (int i = 0; i < element3d.ApiElements.Count; i++) {
        Element apiMeshElement = element3d.GetApiObjectClone(i);
        List<int> meshVertexIndex = element3d.TopoInt[i];

        var topo = new List<int>();
        foreach (int mesh in meshVertexIndex) {
          topo.Add(Nodes.AddNode(ref existingNodes, meshVerticies[mesh], unit));
        }

        apiMeshElement.Topology = new ReadOnlyCollection<int>(topo);

        GsaProp3d prop = (i > element3d.Prop3ds.Count - 1) ? element3d.Prop3ds.Last() :
          element3d.Prop3ds[i];
        apiMeshElement.Property = Prop3ds.ConvertProp3d(prop, ref apiProp3ds, ref apiMaterials);

        AddElement(element3d.Ids[i], element3d.Guid, apiMeshElement, false, ref apiElements);
      }
    }

    internal static void ConvertElement3D(
      List<GsaElement3d> element3ds, ref GsaGuidIntListDictionary<Element> apiElements,
      ref GsaIntDictionary<Node> existingNodes, LengthUnit unit,
      ref GsaGuidDictionary<Prop3D> apiProp3ds,
      ref GsaGuidDictionary<AnalysisMaterial> apiMaterials) {
      if (element3ds == null) {
        return;
      }

      element3ds = element3ds.OrderByDescending(e => e.Ids.First()).ToList();
      foreach (GsaElement3d element3d in element3ds) {
        if (element3d != null) {
          ConvertElement3D(element3d, ref apiElements, ref existingNodes, unit, ref apiProp3ds,
            ref apiMaterials);
        }
      }
    }

    private static void AddElement(
      int id, Guid guid, Element apiElement, bool overwrite,
      ref GsaGuidIntListDictionary<Element> apiElements) {
      if (id > 0) {
        apiElements.SetValue(id, guid, apiElement, overwrite);
      } else {
        apiElements.AddValue(guid, apiElement);
      }
    }
  }
}
