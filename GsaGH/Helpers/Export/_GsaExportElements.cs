using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Util.Gsa.ToGSA
{
  class Elements
  {
    #region element1d
    public static void ConvertElement1D(GsaElement1d element1d,
        ref Dictionary<int, Element> existingElements, ref int elementidcounter,
        ref Dictionary<int, Node> existingNodes, ref int nodeidcounter, LengthUnit unit,
        ref Dictionary<int, Section> existingSections, ref Dictionary<Guid, int> sections_guid,
        ref Dictionary<int, SectionModifier> existingSectionModifiers,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      LineCurve line = element1d.Line;
      Element apiElement = element1d.GetAPI_ElementClone();

      // update topology list to fit model nodes
      List<int> topo = new List<int>();
      //Start node
      int id = Nodes.GetExistingNodeID(existingNodes, line.PointAtStart, unit);
      if (id > 0)
        topo.Add(id);
      else
      {
        existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(line.PointAtStart, unit));
        topo.Add(nodeidcounter);
        nodeidcounter++;
      }

      //End node
      id = Nodes.GetExistingNodeID(existingNodes, line.PointAtEnd, unit);
      if (id > 0)
        topo.Add(id);
      else
      {
        existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(line.PointAtEnd, unit));
        topo.Add(nodeidcounter);
        nodeidcounter++;
      }

      //Orientation node
      if (element1d.OrientationNode != null)
      {
        id = Nodes.GetExistingNodeID(existingNodes, element1d.OrientationNode.Point, unit);
        if (id > 0)
          apiElement.OrientationNode = id;
        else
        {
          existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(element1d.OrientationNode.Point, unit));
          apiElement.OrientationNode = nodeidcounter;
          nodeidcounter++;
        }
      }

      // update topology in Element
      apiElement.Topology = new ReadOnlyCollection<int>(topo.ToList());

      // section
      apiElement.Property = Sections.ConvertSection(element1d.Section,
          ref existingSections, ref sections_guid, ref existingSectionModifiers,
          ref existingMaterials, ref materials_guid);

      // set apielement in dictionary
      if (element1d.Id > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
      {
        existingElements[element1d.Id] = apiElement;
      }
      else
      {
        existingElements.Add(elementidcounter, apiElement);
        elementidcounter++;
      }
    }

    public static void ConvertElement1D(List<GsaElement1d> element1ds,
        ref Dictionary<int, Element> existingElements, ref int elementidcounter,
        ref Dictionary<int, Node> existingNodes, LengthUnit unit,
        ref Dictionary<int, Section> existingSections, ref Dictionary<Guid, int> sections_guid,
        ref Dictionary<int, SectionModifier> existingSectionModifiers,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      int nodeidcounter = (existingNodes.Count > 0) ? existingNodes.Keys.Max() + 1 : 1;

      // Elem1ds
      if (element1ds != null)
      {
        for (int i = 0; i < element1ds.Count; i++)
        {

          if (element1ds[i] != null)
          {
            GsaElement1d element1d = element1ds[i];

            // Add/set element
            ConvertElement1D(element1d, ref existingElements, ref elementidcounter,
                ref existingNodes, ref nodeidcounter, unit, ref existingSections, ref sections_guid,
                ref existingSectionModifiers,
                ref existingMaterials, ref materials_guid);
          }
        }
      }
    }
    #endregion

    #region element2d

    public static void ConvertElement2D(GsaElement2d element2d,
        ref Dictionary<int, Element> existingElements, ref int elementidcounter,
        ref Dictionary<int, Node> existingNodes, ref int nodeidcounter, LengthUnit unit,
        ref Dictionary<int, Prop2D> existingProp2Ds, ref Dictionary<Guid, int> prop2d_guid,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      List<Point3d> meshVerticies = element2d.Topology;

      //Loop through all faces in mesh to update topology list to fit model nodes
      for (int i = 0; i < element2d.API_Elements.Count; i++)
      {
        Element apiMeshElement = element2d.GetApiObjectClone(i);
        List<int> meshVertexIndex = element2d.TopoInt[i];

        List<int> topo = new List<int>(); // temp topologylist

        //Loop through topology
        for (int j = 0; j < meshVertexIndex.Count; j++)
        {
          int id = Nodes.GetExistingNodeID(existingNodes, meshVerticies[meshVertexIndex[j]], unit);
          if (id > 0)
            topo.Add(id);
          else
          {
            existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(meshVerticies[meshVertexIndex[j]], unit));
            topo.Add(nodeidcounter);
            nodeidcounter++;
          }
        }
        //update topology in Element
        apiMeshElement.Topology = new ReadOnlyCollection<int>(topo.ToList());

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
    public static void ConvertElement2D(List<GsaElement2d> element2ds,
        ref Dictionary<int, Element> existingElements, ref int elementidcounter,
        ref Dictionary<int, Node> existingNodes, LengthUnit unit,
        ref Dictionary<int, Prop2D> existingProp2Ds, ref Dictionary<Guid, int> prop2d_guid,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      // create a counter for creating new elements, nodes and properties
      int nodeidcounter = (existingNodes.Count > 0) ? existingNodes.Keys.Max() + 1 : 1;
      int prop2didcounter = (existingProp2Ds.Count > 0) ? existingProp2Ds.Keys.Max() + 1 : 1; //checking the existing model

      // Elem2ds
      if (element2ds != null)
      {
        for (int i = 0; i < element2ds.Count; i++)
        {
          if (element2ds[i] != null)
          {
            GsaElement2d element2d = element2ds[i];

            ConvertElement2D(element2d,
                ref existingElements, ref elementidcounter,
                ref existingNodes, ref nodeidcounter, unit,
                ref existingProp2Ds, ref prop2d_guid,
                ref existingMaterials, ref materials_guid);
          }
        }
      }
    }
    #endregion

    #region element3d

    public static void ConvertElement3D(GsaElement3d element3d,
        ref Dictionary<int, Element> existingElements, ref int elementidcounter,
        ref Dictionary<int, Node> existingNodes, ref int nodeidcounter, LengthUnit unit
        )
    {
      List<Point3d> meshVerticies = element3d.Topology;

      //Loop through all faces in mesh to update topology list to fit model nodes
      for (int i = 0; i < element3d.API_Elements.Count; i++)
      {
        Element apiMeshElement = element3d.GetApiObjectClone(i);
        List<int> meshVertexIndex = element3d.TopoInt[i];

        List<int> topo = new List<int>(); // temp topologylist

        //Loop through topology
        for (int j = 0; j < meshVertexIndex.Count; j++)
        {
          int id = Nodes.GetExistingNodeID(existingNodes, meshVerticies[meshVertexIndex[j]], unit);
          if (id > 0)
            topo.Add(id);
          else
          {
            existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(meshVerticies[meshVertexIndex[j]], unit));
            topo.Add(nodeidcounter);
            nodeidcounter++;
          }
        }
        //update topology in Element
        apiMeshElement.Topology = new ReadOnlyCollection<int>(topo.ToList());

        // section
        //if (apiMeshElement.Property == 0)
        //    apiMeshElement.Property = Prop2ds.ConvertProp2d(element2d.Properties[i], ref existingProp2Ds, ref prop2didcounter);


        // set api element in dictionary
        if (element3d.IDs[i] > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
        {
          existingElements[element3d.IDs[i]] = apiMeshElement;
        }
        else
        {
          existingElements.Add(elementidcounter, apiMeshElement);
          elementidcounter++;
        }
      }
    }
    public static void ConvertElement3D(List<GsaElement3d> element3ds,
        ref Dictionary<int, Element> existingElements, ref int elementidcounter,
        ref Dictionary<int, Node> existingNodes, LengthUnit unit)
    {
      // create a counter for creating new elements, nodes and properties
      int nodeidcounter = (existingNodes.Count > 0) ? existingNodes.Keys.Max() + 1 : 1;
      //int prop2didcounter = (existingProp2Ds.Count > 0) ? existingProp2Ds.Keys.Max() + 1 : 1; //checking the existing model

      // Elem3ds
      if (element3ds != null)
      {
        for (int i = 0; i < element3ds.Count; i++)
        {
          if (element3ds[i] != null)
          {
            GsaElement3d element3d = element3ds[i];

            ConvertElement3D(element3d,
                ref existingElements, ref elementidcounter,
                ref existingNodes, ref nodeidcounter, unit);
          }
        }
      }
    }
    #endregion
  }
}
