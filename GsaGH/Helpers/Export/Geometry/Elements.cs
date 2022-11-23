using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Helpers.Export
{
  internal class Elements
  {
    #region element1d
    internal static void ConvertElement1D(GsaElement1d element1d,
        ref Dictionary<int, Element> existingElements, ref int elementidcounter,
        ref GsaDictionary<Node> existingNodes, LengthUnit unit,
        ref Dictionary<int, Section> existingSections, ref Dictionary<Guid, int> sections_guid,
        ref Dictionary<int, SectionModifier> existingSectionModifiers,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      LineCurve line = element1d.Line;
      Element apiElement = element1d.GetAPI_ElementClone();

      // update topology list to fit model nodes
      List<int> topo = new List<int>();
      //Start node
      topo.Add(Nodes.AddNode(ref existingNodes, line.PointAtStart, unit));
      //End node
      topo.Add(Nodes.AddNode(ref existingNodes, line.PointAtEnd, unit));
      //Orientation node
      if (element1d.OrientationNode != null)
        apiElement.OrientationNode = Nodes.AddNode(ref existingNodes, element1d.OrientationNode.Point, unit);

      // update topology in Element
      apiElement.Topology = new ReadOnlyCollection<int>(topo);

      // section
      apiElement.Property = Sections.ConvertSection(element1d.Section,
          ref existingSections, ref sections_guid, ref existingSectionModifiers,
          ref existingMaterials, ref materials_guid);

      // set apielement in dictionary
      if (element1d.Id > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
        existingElements[element1d.Id] = apiElement;
      else
        existingElements.Add(elementidcounter++, apiElement);
    }

    internal static void ConvertElement1D(List<GsaElement1d> element1ds,
        ref Dictionary<int, Element> existingElements, ref int elementidcounter,
        ref GsaDictionary<Node> existingNodes, LengthUnit unit,
        ref Dictionary<int, Section> existingSections, ref Dictionary<Guid, int> sections_guid,
        ref Dictionary<int, SectionModifier> existingSectionModifiers,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      if (element1ds != null)
        for (int i = 0; i < element1ds.Count; i++)
          if (element1ds[i] != null)
            ConvertElement1D(element1ds[i], ref existingElements, ref elementidcounter,
                ref existingNodes, unit, ref existingSections, ref sections_guid,
                ref existingSectionModifiers,
                ref existingMaterials, ref materials_guid);
    }
    #endregion

    #region element2d
    internal static void ConvertElement2D(GsaElement2d element2d,
        ref Dictionary<int, Element> existingElements, ref int elementidcounter,
        ref GsaDictionary<Node> existingNodes, LengthUnit unit,
        ref Dictionary<int, Prop2D> existingProp2Ds, ref Dictionary<Guid, int> prop2d_guid,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      List<Point3d> meshVerticies = element2d.Topology;

      //Loop through all faces in mesh to update topology list to fit model nodes
      for (int i = 0; i < element2d.API_Elements.Count; i++)
      {
        Element apiMeshElement = element2d.GetApiObjectClone(i);
        List<int> meshVertexIndex = element2d.TopoInt[i];

        List<int> topo = new List<int>();
        for (int j = 0; j < meshVertexIndex.Count; j++)
          topo.Add(Nodes.AddNode(ref existingNodes, meshVerticies[meshVertexIndex[j]], unit));
        
        apiMeshElement.Topology = new ReadOnlyCollection<int>(topo);

        // Prop2d
        GsaProp2d prop = (i > element2d.Properties.Count - 1) ? element2d.Properties.Last() : element2d.Properties[i];
        apiMeshElement.Property = Prop2ds.ConvertProp2d(prop, ref existingProp2Ds, ref prop2d_guid, ref existingMaterials, ref materials_guid);

        // set api element in dictionary
        if (element2d.Ids[i] > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
        {
          existingElements[element2d.Ids[i]] = apiMeshElement;
        }
        else
        {
          existingElements.Add(elementidcounter, apiMeshElement);
          elementidcounter++;
        }
      }
    }

    internal static void ConvertElement2D(List<GsaElement2d> element2ds,
        ref Dictionary<int, Element> existingElements, ref int elementidcounter,
        ref GsaDictionary<Node> existingNodes, LengthUnit unit,
        ref Dictionary<int, Prop2D> existingProp2Ds, ref Dictionary<Guid, int> prop2d_guid,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      if (element2ds != null)
        for (int i = 0; i < element2ds.Count; i++)
          if (element2ds[i] != null)
            ConvertElement2D(element2ds[i],
                ref existingElements, ref elementidcounter,
                ref existingNodes, unit,
                ref existingProp2Ds, ref prop2d_guid,
                ref existingMaterials, ref materials_guid);
    }
    #endregion

    #region element3d
    internal static void ConvertElement3D(GsaElement3d element3d,
        ref Dictionary<int, Element> existingElements, ref int elementidcounter,
        ref GsaDictionary<Node> existingNodes, LengthUnit unit,
        ref Dictionary<int, Prop3D> existingProp3Ds, ref Dictionary<Guid, int> prop3d_guid,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      List<Point3d> meshVerticies = element3d.Topology;

      //Loop through all faces in mesh to update topology list to fit model nodes
      for (int i = 0; i < element3d.API_Elements.Count; i++)
      {
        Element apiMeshElement = element3d.GetApiObjectClone(i);
        List<int> meshVertexIndex = element3d.TopoInt[i];

        List<int> topo = new List<int>();
        for (int j = 0; j < meshVertexIndex.Count; j++)
          topo.Add(Nodes.AddNode(ref existingNodes, meshVerticies[meshVertexIndex[j]], unit));

        apiMeshElement.Topology = new ReadOnlyCollection<int>(topo);

        // Prop3d
        GsaProp3d prop = (i > element3d.Properties.Count - 1) ? element3d.Properties.Last() : element3d.Properties[i];
        apiMeshElement.Property = Prop3ds.ConvertProp3d(prop, ref existingProp3Ds, ref prop3d_guid, ref existingMaterials, ref materials_guid);

        // set api element in dictionary
        if (element3d.IDs[i] > 0) 
          existingElements[element3d.IDs[i]] = apiMeshElement;
        else
          existingElements.Add(elementidcounter++, apiMeshElement);
      }
    }

    internal static void ConvertElement3D(List<GsaElement3d> element3ds,
        ref Dictionary<int, Element> existingElements, ref int elementidcounter,
        ref GsaDictionary<Node> existingNodes, LengthUnit unit,
        ref Dictionary<int, Prop3D> existingProp3Ds, ref Dictionary<Guid, int> prop3d_guid,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      if (element3ds != null)
        for (int i = 0; i < element3ds.Count; i++)
          if (element3ds[i] != null)
            ConvertElement3D(element3ds[i],
                ref existingElements, ref elementidcounter,
                ref existingNodes, unit,
                ref existingProp3Ds, ref prop3d_guid,
                ref existingMaterials, ref materials_guid);
    }
    #endregion
  }
}
