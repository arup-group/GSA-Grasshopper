using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Helpers.Export
{
  internal class Elements
  {
    private static void AddElement(int id, Guid guid, Element apiElement, bool overwrite, ref GsaGuidIntListDictionary<Element> apiElements)
    {
      if (id > 0)
        apiElements.SetValue(id, guid, apiElement, overwrite);
      else
        apiElements.AddValue(guid, apiElement);
    }

    #region element1d
    internal static void ConvertElement1D(GsaElement1d element1d,
        ref GsaGuidIntListDictionary<Element> apiElements, ref GsaIntDictionary<Node> existingNodes, LengthUnit unit,
        ref GsaGuidDictionary<Section> apiSections, ref GsaIntDictionary<SectionModifier> apiSectionModifiers, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      LineCurve line = element1d.Line;
      Element apiElement = element1d.GetApiElementClone();

      List<int> topo = new List<int>
      {
        Nodes.AddNode(ref existingNodes, line.PointAtStart, unit),
        Nodes.AddNode(ref existingNodes, line.PointAtEnd, unit)
      };
      apiElement.Topology = new ReadOnlyCollection<int>(topo);

      if (element1d.OrientationNode != null)
        apiElement.OrientationNode = Nodes.AddNode(ref existingNodes, element1d.OrientationNode.Point, unit);

      apiElement.Property = Sections.ConvertSection(element1d.Section,
          ref apiSections, ref apiSectionModifiers, ref apiMaterials);

      AddElement(element1d.Id, element1d.Guid, apiElement, true, ref apiElements);
    }

    internal static void ConvertElement1D(List<GsaElement1d> element1ds, ref GsaGuidIntListDictionary<Element> apiElements, ref GsaIntDictionary<Node> existingNodes, LengthUnit unit, ref GsaGuidDictionary<Section> apiSections, ref GsaIntDictionary<SectionModifier> apiSectionModifiers, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      if (element1ds != null)
      {
        element1ds = element1ds.OrderByDescending(x => x.Id).ToList();
        for (int i = 0; i < element1ds.Count; i++)
          if (element1ds[i] != null)
            ConvertElement1D(element1ds[i], ref apiElements, ref existingNodes, unit, ref apiSections, ref apiSectionModifiers, ref apiMaterials);
      }
    }
    #endregion

    #region element2d
    internal static void ConvertElement2D(GsaElement2d element2d,
        ref GsaGuidIntListDictionary<Element> apiElements, ref GsaIntDictionary<Node> existingNodes, LengthUnit unit,
        ref GsaGuidDictionary<Prop2D> apiProp2ds, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      List<Point3d> meshVerticies = element2d.Topology;

      for (int i = 0; i < element2d.API_Elements.Count; i++)
      {
        Element apiMeshElement = element2d.GetApiObjectClone(i);
        List<int> meshVertexIndex = element2d.TopoInt[i];

        List<int> topo = new List<int>();
        for (int j = 0; j < meshVertexIndex.Count; j++)
          topo.Add(Nodes.AddNode(ref existingNodes, meshVerticies[meshVertexIndex[j]], unit));

        apiMeshElement.Topology = new ReadOnlyCollection<int>(topo);

        // take last item if lists doesnt match in length
        GsaProp2d prop = (i > element2d.Properties.Count - 1) ? element2d.Properties.Last() : element2d.Properties[i];
        apiMeshElement.Property = Prop2ds.ConvertProp2d(prop, ref apiProp2ds, ref apiMaterials);

        AddElement(element2d.Ids[i], element2d.Guid, apiMeshElement, false, ref apiElements);
      }
    }

    internal static void ConvertElement2D(List<GsaElement2d> element2ds,
        ref GsaGuidIntListDictionary<Element> apiElements, ref GsaIntDictionary<Node> existingNodes, LengthUnit unit,
        ref GsaGuidDictionary<Prop2D> apiProp2ds, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      if (element2ds != null)
      {
        element2ds = element2ds.OrderByDescending(e => e.Ids.First()).ToList();
        for (int i = 0; i < element2ds.Count; i++)
          if (element2ds[i] != null)
            ConvertElement2D(element2ds[i], ref apiElements, ref existingNodes, unit, ref apiProp2ds, ref apiMaterials);
      }
    }
    #endregion

    #region element3d
    internal static void ConvertElement3D(GsaElement3d element3d,
        ref GsaGuidIntListDictionary<Element> apiElements, ref GsaIntDictionary<Node> existingNodes, LengthUnit unit,
        ref GsaGuidDictionary<Prop3D> apiProp3ds, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      List<Point3d> meshVerticies = element3d.Topology;

      for (int i = 0; i < element3d.API_Elements.Count; i++)
      {
        Element apiMeshElement = element3d.GetApiObjectClone(i);
        List<int> meshVertexIndex = element3d.TopoInt[i];

        List<int> topo = new List<int>();
        for (int j = 0; j < meshVertexIndex.Count; j++)
          topo.Add(Nodes.AddNode(ref existingNodes, meshVerticies[meshVertexIndex[j]], unit));

        apiMeshElement.Topology = new ReadOnlyCollection<int>(topo);

        // take last item if lists doesnt match in length
        GsaProp3d prop = (i > element3d.Properties.Count - 1) ? element3d.Properties.Last() : element3d.Properties[i];
        apiMeshElement.Property = Prop3ds.ConvertProp3d(prop, ref apiProp3ds, ref apiMaterials);

        AddElement(element3d.Ids[i], element3d.Guid, apiMeshElement, false, ref apiElements);
      }
    }

    internal static void ConvertElement3D(List<GsaElement3d> element3ds,
        ref GsaGuidIntListDictionary<Element> apiElements,
        ref GsaIntDictionary<Node> existingNodes, LengthUnit unit,
        ref GsaGuidDictionary<Prop3D> apiProp3ds, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      if (element3ds != null)
      {
        element3ds = element3ds.OrderByDescending(e => e.Ids.First()).ToList();
        for (int i = 0; i < element3ds.Count; i++)
          if (element3ds[i] != null)
            ConvertElement3D(element3ds[i], ref apiElements, ref existingNodes, unit, ref apiProp3ds, ref apiMaterials);
      }
    }
    #endregion
  }
}
