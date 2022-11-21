using System;
using System.Linq;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using GsaAPI;
using Rhino.Geometry;
using GsaGH.Parameters;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using OasysUnits.Units;
using OasysUnits;
using Grasshopper.Kernel;

namespace GsaGH.Util.Gsa
{
  /// <summary>
  /// Class containing functions to import various object types from GSA
  /// </summary>
  public class FromGSA
  {
    #region nodes
    public static GsaNode GetNode(Node node, LengthUnit unit, int ID, ConcurrentDictionary<int, Axis> axDict = null)
    {
      Plane local = new Plane();
      // add local axis if node has Axis property
      if (axDict != null)
      {
        if (node.AxisProperty > 0)
        {
          axDict.TryGetValue(node.AxisProperty, out Axis axis);
          local = AxisToPlane(axis, unit);
        }
        else
        {
          switch (node.AxisProperty)
          {
            case 0:
              // local axis = Global
              // do nothing, XY-plan already set by default
              break;
            case -11:
              // local axis = X-elevation
              local = Plane.WorldYZ;
              break;
            case -12:
              // local axis = X-elevation
              local = Plane.WorldZX;
              break;
            case -13:
              // local axis = vertical
              // GSA naming is confusing, but this is a XY-plane
              local = Plane.WorldXY;
              break;
            case -14:
              // local axis = global cylindric
              // no method in Rhino/GH to handle cylindric coordinate system
              local = Plane.Unset;
              break;
          }
        }
      }

      // create new node with basic Position and ID values

      return new GsaNode(node, ID, unit, local);
    }


    /// <summary>
    /// Method to import Nodes from a GSA model.
    /// Will output a list of GsaNodeGoos.
    /// Input node dictionary pre-filtered for selected nodes to import;
    /// </summary>
    /// <param name="nDict">Dictionary of GSA Nodes pre-filtered for nodes to import</param>
    /// <param name="model">GSA Model, only used in case node refers to a local axis</param>
    /// <returns></returns>
    public static ConcurrentBag<GsaNodeGoo> GetNodes(ConcurrentDictionary<int, Node> nDict, LengthUnit unit, ConcurrentDictionary<int, Axis> axDict = null)
    {
      ConcurrentBag<GsaNodeGoo> outNodes = new ConcurrentBag<GsaNodeGoo>();
      Parallel.ForEach(nDict, node =>
      {
        outNodes.Add(new GsaNodeGoo(GetNode(node.Value, unit, node.Key, axDict)));
      });
      return outNodes;

      // use linq parallel
      //return nDict.AsParallel().Select(node => new GsaNodeGoo(GetNode(node.Value, unit, node.Key, axDict)));
    }
    public static ConcurrentDictionary<int, GsaNodeGoo> GetNodeDictionary(ConcurrentDictionary<int, Node> nDict, LengthUnit unit, ConcurrentDictionary<int, Axis> axDict = null)
    {
      ConcurrentDictionary<int, GsaNodeGoo> outNodes = new ConcurrentDictionary<int, GsaNodeGoo>();
      Parallel.ForEach(nDict, node =>
      {
        outNodes.TryAdd(node.Key, new GsaNodeGoo(GetNode(node.Value, unit, node.Key, axDict)));
      });
      return outNodes;

      // use linq parallel
      //return nDict.AsParallel().Select(node => new GsaNodeGoo(GetNode(node.Value, unit, node.Key, axDict)));
    }

    /// <summary>
    /// Method to create a Rhino Plane from a GSA Axis
    /// </summary>
    /// <param name="axis">GSA Axis to create plane from</param>
    /// <returns></returns>
    public static Plane AxisToPlane(Axis axis, LengthUnit unit)
    {
      if (axis == null) { return Plane.Unset; }

      // origin point from GSA Axis
      Point3d origin = Point3dFromXYZUnit(axis.Origin.X, axis.Origin.Y, axis.Origin.Z, unit);

      // X-axis from GSA Axis
      Vector3d xAxis = Vector3dFromXYZUnit(axis.XVector.X, axis.XVector.Y, axis.XVector.Z, unit);

      // check if vector is zero-length
      if (xAxis.IsZero) { return Plane.Unset; }
      // create unitised vector
      Vector3d xUnit = new Vector3d(xAxis);
      xUnit.Unitize();

      // Y-axis from GSA Axis
      Vector3d yAxis = Vector3dFromXYZUnit(axis.XYPlane.X, axis.XYPlane.Y, axis.XYPlane.Z, unit);

      // check if vector is zero-length
      if (yAxis.IsZero) { return Plane.Unset; }
      // create unitised vector
      Vector3d yUnit = new Vector3d(yAxis);
      yUnit.Unitize();

      // check if x and y unitised are not the same
      if (xUnit.Equals(yUnit)) { return Plane.Unset; }

      // Create new plane with Rhino method
      Plane pln = new Plane(origin, xAxis, yAxis);

      return pln;
    }
    #endregion

    #region elements
    /// <summary>
    /// Method to convert Elements to Element 1D and Element 2D
    /// Element 3Ds to be implemented
    /// Will output a tuple containing a:
    /// 1. List of GsaElement1dGoos and 
    /// 2. List of GsaElement2dGoos
    /// </summary>
    /// <param name="eDict">Dictionary of Elements to import</param>
    /// <param name="nDict">Dictionary of Nodes that elements refers to by topology. Include all Nodes unless you are sure what you are doing</param>
    /// <param name="sDict">Dictionary of Sections (for 1D elements)</param>
    /// <param name="pDict">Dictionary of 2D Properties (for 2D elements)</param>
    /// <returns></returns>
    public static Tuple<ConcurrentBag<GsaElement1dGoo>, ConcurrentBag<GsaElement2dGoo>, ConcurrentBag<GsaElement3dGoo>>
        GetElements(ConcurrentDictionary<int, Element> eDict, ConcurrentDictionary<int, Node> nDict,
        ConcurrentDictionary<int, Section> sDict, ConcurrentDictionary<int, Prop2D> pDict, ConcurrentDictionary<int, Prop3D> p3Dict,
        ConcurrentDictionary<int, AnalysisMaterial> mDict, ConcurrentDictionary<int, SectionModifier> modDict, LengthUnit unit)
    {
      // Create lists for Rhino lines and meshes
      ConcurrentBag<GsaElement1dGoo> elem1ds = new ConcurrentBag<GsaElement1dGoo>();
      ConcurrentBag<GsaElement2dGoo> elem2ds = new ConcurrentBag<GsaElement2dGoo>();
      ConcurrentBag<GsaElement3dGoo> elem3ds = new ConcurrentBag<GsaElement3dGoo>();

      ConcurrentDictionary<int, Element> elem1dDict = new ConcurrentDictionary<int, Element>();
      ConcurrentDictionary<int, Element> elem2dDict = new ConcurrentDictionary<int, Element>();
      ConcurrentDictionary<int, Element> elem3dDict = new ConcurrentDictionary<int, Element>();

      Parallel.ForEach(eDict, item =>
      {
        // find type of element, 1D, 2D or 3D:
        int elemDimension = 1; // default assume 1D element

        // get element type
        ElementType type = item.Value.Type;

        // change to 2D if type is one of these
        if (type == ElementType.TRI3 || type == ElementType.TRI6 ||
                  type == ElementType.QUAD4 || type == ElementType.QUAD8 ||
                  type == ElementType.TWO_D || type == ElementType.TWO_D_FE ||
                  type == ElementType.TWO_D_LOAD)
          elemDimension = 2;
        // change to 3D if type is one of these
        if (type == ElementType.BRICK8 || type == ElementType.WEDGE6 ||
                  type == ElementType.PYRAMID5 || type == ElementType.TETRA4 ||
                  type == ElementType.THREE_D)
          elemDimension = 3;

        switch (elemDimension)
        {
          case 1:
            // create new element from api element;
            elem1dDict.TryAdd(item.Key, item.Value);
            //elem1ds.Add(new GsaElement1dGoo(
            //    ConvertToElement1D(
            //        item.Value, item.Key, nDict, sDict, mDict, unit)));
            break;

          case 2:
            // add 2D element to dictionary to bulk create and combine
            // meshes in one go
            elem2dDict.TryAdd(item.Key, item.Value);
            break;

          case 3:
            // add 3D element to dictionary to bulk create and combine
            // meshes in one go
            elem3dDict.TryAdd(item.Key, item.Value);
            break;
        }
      });

      // if import found any 1D elements add these in one go.
      // GsaElement2d and 3d consist of a list of 2D elements in order
      // to display a combined mesh: each 2D element is a mesh face
      if (elem1dDict.Count > 0)
        elem1ds = new ConcurrentBag<GsaElement1dGoo>(elem1dDict.AsParallel().
            Select(item => new GsaElement1dGoo(
                ConvertToElement1D(item.Value, item.Key, nDict, sDict, mDict, modDict, unit))));

      if (elem2dDict.Count > 0)
        elem2ds = ConvertToElement2Ds(elem2dDict, nDict, pDict, mDict, unit);

      if (elem3dDict.Count > 0)
        elem3ds = ConvertToElement3Ds(elem3dDict, nDict, p3Dict, mDict, unit);


      return new Tuple<ConcurrentBag<GsaElement1dGoo>, ConcurrentBag<GsaElement2dGoo>, ConcurrentBag<GsaElement3dGoo>>
          (elem1ds, elem2ds, elem3ds);
    }

    /// <summary>
    /// Method to convert a single 1D Element to a Element 1D
    /// Will output a GsaElement1d
    /// </summary>
    /// <param name="element">GsaAPI Element to be converted</param>
    /// <param name="ID">Element number (key/ID). Set to 0 if this shall be ignored when exporting from Grasshopper</param>
    /// <param name="nodes">Dictionary of Nodes that elements refers to by topology. Include all Nodes unless you are sure what you are doing</param>
    /// <param name="sections">Dictionary of Sections</param>
    /// <returns></returns>
    public static GsaElement1d ConvertToElement1D(Element element,
        int ID, ConcurrentDictionary<int, Node> nodes, ConcurrentDictionary<int, Section> sections,
        ConcurrentDictionary<int, AnalysisMaterial> materials, ConcurrentDictionary<int, SectionModifier> sectionModifiers, LengthUnit unit)
    {
      // get element's topology
      ReadOnlyCollection<int> topo = element.Topology;

      // ensure the element is a 1D element
      if (topo.Count == 2)
      {
        // get start and end nodes
        List<Point3d> pts = new List<Point3d>();
        for (int i = 0; i <= 1; i++)
        {
          if (nodes.TryGetValue(topo[i], out Node node))
            pts.Add(Point3dFromNode(node, unit));
        }
        // create line
        LineCurve ln = new LineCurve(new Line(pts[0], pts[1]));

        // orientation node
        GsaNode orient = null;
        if (element.OrientationNode > 0)
        {
          if (nodes.TryGetValue(element.OrientationNode, out Node node))
          {
            var p = node.Position;
            orient = new GsaNode(Point3dFromNode(node, unit),
                element.OrientationNode);
          }
        }

        // get section (if it exist)
        GsaSection section = new GsaSection(element.Property);
        if (sections.TryGetValue(element.Property, out Section apisection))
        {
          section = new GsaSection(element.Property);
          section.API_Section = apisection;

          //// get material (if analysis material exist)
          //if (section.API_Section.MaterialAnalysisProperty > 0)
          //{
          //    materials.TryGetValue(apisection.MaterialAnalysisProperty, out AnalysisMaterial apimaterial);
          //    section.Material = new GsaMaterial(section, apimaterial);
          //}
          //else
          //    section.Material = new GsaMaterial(section);

          if (sectionModifiers.TryGetValue(element.Property, out SectionModifier sectionModifier))
            section.Modifier = new GsaSectionModifier(sectionModifier);
        }

        // create GH GsaElement1d
        return new GsaElement1d(element, ln, ID, section, orient);
      }
      return null;
    }

    /// <summary>
    /// Method to get convert an Element to a Mesh with one face (tri, quad or ngon)
    /// </summary>
    /// <param name="element">Element to get mesh face from</param>
    /// <param name="nodes">Dictionary of nodes that includes nodes for the topology which the element.Topology refers to. Typically use all nodes from a GSA model</param>
    /// <returns></returns>
    public static Mesh ConvertElement2D(Element element, IReadOnlyDictionary<int, Node> nodes, LengthUnit unit)
    {
      // get element's topology
      ReadOnlyCollection<int> topo = element.Topology;

      // check if element is 2D
      if (topo.Count < 3 ||
          element.Type == ElementType.THREE_D ||
          element.Type == ElementType.BRICK8 ||
          element.Type == ElementType.WEDGE6 ||
          element.Type == ElementType.PYRAMID5 ||
          element.Type == ElementType.TETRA4)
        return null;

      Mesh outMesh = new Mesh();

      // Get verticies:
      for (int k = 0; k < topo.Count; k++)
      {
        if (nodes.TryGetValue(topo[k], out Node node))
          outMesh.Vertices.Add(Point3dFromNode(node, unit));
      }

      // Create mesh face (Tri- or Quad):
      if (topo.Count == 3)
        outMesh.Faces.AddFace(0, 1, 2);
      else if (topo.Count == 4)
        outMesh.Faces.AddFace(0, 1, 2, 3);
      else if (topo.Count > 4)
      {
        // so we introduce the average middle point and create more faces

        if (topo.Count == 6)
        {
          outMesh.Faces.AddFace(0, 3, 5);
          outMesh.Faces.AddFace(1, 4, 3);
          outMesh.Faces.AddFace(2, 5, 4);
          outMesh.Faces.AddFace(3, 4, 5);
          List<int> tri6Vert = new List<int>() { 0, 3, 1, 4, 2, 5 };
          List<int> tri6Face = new List<int>() { 0, 1, 2, 3 };
          MeshNgon meshGon = MeshNgon.Create(tri6Vert, tri6Face);

          outMesh.Ngons.AddNgon(meshGon);
        }

        if (topo.Count == 8)
        {
          Point3d ave = new Point3d();
          ave.X = 0;
          ave.Y = 0;
          ave.Z = 0;
          for (int k = 0; k < topo.Count; k++)
          {
            ave.X += outMesh.Vertices[k].X;
            ave.Y += outMesh.Vertices[k].Y;
            ave.Z += outMesh.Vertices[k].Z;
          }
          ave.X = ave.X / topo.Count;
          ave.Y = ave.Y / topo.Count;
          ave.Z = ave.Z / topo.Count;

          outMesh.Vertices.Add(ave);

          outMesh.Faces.AddFace(0, 4, 8);
          outMesh.Faces.AddFace(1, 8, 4);
          outMesh.Faces.AddFace(1, 5, 8);
          outMesh.Faces.AddFace(2, 8, 5);
          outMesh.Faces.AddFace(2, 6, 8);
          outMesh.Faces.AddFace(3, 8, 6);
          outMesh.Faces.AddFace(3, 7, 8);
          outMesh.Faces.AddFace(0, 8, 7);
          List<int> quad8vert = new List<int>() { 0, 4, 1, 5, 2, 6, 3, 7 };
          List<int> quad8Face = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };

          MeshNgon meshGon = MeshNgon.Create(quad8vert, quad8Face);

          outMesh.Ngons.AddNgon(meshGon);
        }
      }
      return outMesh;
    }

    /// <summary>
    /// Method to bulk convert 2D Elements to Element 2Ds
    /// Will output a list of GsaElement2dGoos
    /// </summary>
    /// <param name="elements">Dictionary of 2D Elements</param>
    /// <param name="nodes">Dictionary of Nodes that elements refers to by topology. Include all Nodes unless you are sure what you are doing</param>
    /// <param name="properties">Dictionary of 2D Properties</param>
    /// <returns></returns>
    public static ConcurrentBag<GsaElement2dGoo> ConvertToElement2Ds(
        ConcurrentDictionary<int, Element> elements, ConcurrentDictionary<int, Node> nodes,
        ConcurrentDictionary<int, Prop2D> properties, ConcurrentDictionary<int, AnalysisMaterial> materials, LengthUnit unit)
    {
      // main sorted dictionary with 
      // key = parent member
      // value = dictionary of elements belong to that parent:
      //    key = element id
      //    value = element
      ConcurrentDictionary<int, ConcurrentDictionary<int, Element>> sortedElements = new ConcurrentDictionary<int, ConcurrentDictionary<int, Element>>();

      Parallel.ForEach(elements, elem =>
      {
        // get parent member
        int parent = -elem.Value.ParentMember.Member;

        // if no parent member then split by property
        if (parent == 0)
          parent = elem.Value.Property;

        if (!sortedElements.ContainsKey(parent))
        {
          sortedElements.TryAdd(parent, new ConcurrentDictionary<int, Element>());
        }

        // add elements to parent member
        sortedElements[parent][elem.Key] = elem.Value;
      });

      // bag to drop all elements to
      ConcurrentBag<GsaElement2dGoo> elem2dGoos = new ConcurrentBag<GsaElement2dGoo>();

      Parallel.For(0, sortedElements.Count, i =>
      {
        int parentID = sortedElements.Keys.ElementAt(i);

        // list of elements with same parent
        ConcurrentDictionary<int, Element> elems = sortedElements[parentID];

        // create list of Prop2Ds
        ConcurrentDictionary<int, GsaProp2d> prop2Ds = new ConcurrentDictionary<int, GsaProp2d>();

        // create list of meshes
        ConcurrentDictionary<int, Mesh> mList = new ConcurrentDictionary<int, Mesh>();

        Parallel.For(0, elems.Count, j =>
              {
                int elementID = elems.Keys.ElementAt(j);
                Mesh faceMesh = ConvertElement2D(elems[elementID], nodes, unit);
                if (faceMesh == null) { return; }
                mList[elementID] = faceMesh;

                // get prop2d (if it exist)
                int propID = elems[elementID].Property;
                GsaProp2d prop = new GsaProp2d(propID);
                if (properties.TryGetValue(propID, out Prop2D apiprop))
                {
                  prop = new GsaProp2d(propID);
                  prop.API_Prop2d = apiprop;

                  // get material (if analysis material exist)
                  //if (prop.API_Prop2d.MaterialAnalysisProperty > 0)
                  //{
                  //    materials.TryGetValue(apiprop.MaterialAnalysisProperty, out AnalysisMaterial apimaterial);
                  //    prop.Material = new GsaMaterial(prop, apimaterial);
                  //}
                  //else
                  //    prop.Material = new GsaMaterial(prop);
                }

                prop2Ds[elementID] = prop;
              });

        // create one large mesh from single mesh face using
        // append list of meshes (faster than appending each mesh one by one)
        Mesh m = new Mesh(); // new mesh to merge existing into
        m.Append(mList.Values.ToList());

        // if parent member value is 0 then no parent member exist for element
        // we can therefore not be sure all elements with parent member = 0 are
        // connected in one mesh.
        if (parentID == 0 && m.DisjointMeshCount > 1)
        {
          // revert back to list of meshes instead of the joined one
          foreach (int key in elems.Keys)
          {
            // create new element from api-element, id, mesh (takes care of topology lists etc) and prop2d
            elems.TryGetValue(key, out Element api_elem);
            mList.TryGetValue(key, out Mesh mesh);
            prop2Ds.TryGetValue(key, out GsaProp2d prop);

            GsaElement2d singleelement2D = new GsaElement2d(
                      new List<Element> { api_elem },
                      new List<int> { key },
                      mesh,
                      new List<GsaProp2d> { prop }
                      );

            // add the element to list of goo 2d elements
            elem2dGoos.Add(new GsaElement2dGoo(singleelement2D));
          }
        }
        else
        {
          // lists needed to create GsaElement2d
          List<Element> api_elems = elems.Values.ToList();
          List<int> ids = elems.Keys.ToList();
          List<GsaProp2d> props = prop2Ds.Values.ToList();

          // create new element from api-element, id, mesh (takes care of topology lists etc) and prop2d
          GsaElement2d element2D = new GsaElement2d(
                    api_elems,
                    ids,
                    m,
                    props);

          elem2dGoos.Add(new GsaElement2dGoo(element2D));
        }

      });

      return elem2dGoos;
      //return elem2dGoos.AsParallel().OrderBy(e => e.Value.ID.Max()).ToList();
    }

    public static Mesh ConvertElement3D(Element element, ConcurrentDictionary<int, Node> nodes, LengthUnit unit)
    {
      // get element's topology
      ReadOnlyCollection<int> topo = element.Topology;

      // check if element is 3D
      List<bool> check3d = new List<bool>
                    {
                        element.Type == ElementType.THREE_D,
                        element.Type == ElementType.BRICK8,
                        element.Type == ElementType.WEDGE6,
                        element.Type == ElementType.PYRAMID5,
                        element.Type == ElementType.TETRA4
                    };
      if (!check3d.Contains(true))
        return null;

      Mesh outMesh = new Mesh();

      // Get verticies:
      for (int k = 0; k < topo.Count; k++)
      {
        if (nodes.TryGetValue(topo[k], out Node node))
          outMesh.Vertices.Add(Point3dFromNode(node, unit));
      }

      // Create 3D element
      switch (topo.Count)
      {
        case 4:
          // tetrahedron element
          outMesh.Faces.AddFace(0, 2, 1); //bottom
          outMesh.Faces.AddFace(0, 1, 3); //side 1
          outMesh.Faces.AddFace(1, 2, 3); //side 2
          outMesh.Faces.AddFace(2, 0, 3); //side 3
          List<int> verts4 = new List<int>() { 0, 1, 2, 3 };
          List<int> faces4 = new List<int>() { 0, 1, 2, 3 };
          MeshNgon meshGon4 = MeshNgon.Create(verts4, faces4);
          outMesh.Ngons.AddNgon(meshGon4);
          break;

        case 5:
          // pyramid element
          outMesh.Faces.AddFace(0, 3, 2, 1); //bottom
          outMesh.Faces.AddFace(0, 1, 4); //side 1
          outMesh.Faces.AddFace(1, 2, 4); //side 2
          outMesh.Faces.AddFace(2, 3, 4); //side 3
          outMesh.Faces.AddFace(3, 0, 4); //side 4
          List<int> verts5 = new List<int>() { 0, 1, 2, 3, 4 };
          List<int> faces5 = new List<int>() { 0, 1, 2, 3, 4 };
          MeshNgon meshGon5 = MeshNgon.Create(verts5, faces5);
          outMesh.Ngons.AddNgon(meshGon5);
          break;

        case 6:
          // wedge element
          outMesh.Faces.AddFace(0, 2, 1); //end1
          outMesh.Faces.AddFace(0, 3, 5, 2); //side 1
          outMesh.Faces.AddFace(1, 2, 5, 4); //side 2
          outMesh.Faces.AddFace(0, 1, 4, 3); //side 3
          outMesh.Faces.AddFace(3, 4, 5); //end 2
          List<int> verts6 = new List<int>() { 0, 1, 2, 3, 4, 5 };
          List<int> faces6 = new List<int>() { 0, 1, 2, 3, 4 };
          MeshNgon meshGon6 = MeshNgon.Create(verts6, faces6);
          outMesh.Ngons.AddNgon(meshGon6);
          break;

        case 8:
          // brick element
          outMesh.Faces.AddFace(0, 3, 2, 1); //bottom
          outMesh.Faces.AddFace(0, 1, 5, 4); //side 1
          outMesh.Faces.AddFace(1, 2, 6, 5); //side 2
          outMesh.Faces.AddFace(2, 3, 7, 6); //side 2
          outMesh.Faces.AddFace(3, 0, 4, 7); //side 3
          outMesh.Faces.AddFace(4, 5, 6, 7); //top
          List<int> verts8 = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };
          List<int> faces8 = new List<int>() { 0, 1, 2, 3, 4, 5 };
          MeshNgon meshGon8 = MeshNgon.Create(verts8, faces8);
          outMesh.Ngons.AddNgon(meshGon8);
          break;

        default:
          return null;
      }
      return outMesh;
    }

    public static ConcurrentBag<GsaElement3dGoo> ConvertToElement3Ds(ConcurrentDictionary<int, Element> elements, ConcurrentDictionary<int, Node> nodes,
        ConcurrentDictionary<int, Prop3D> properties, ConcurrentDictionary<int, AnalysisMaterial> materials, LengthUnit unit)
    {
      // main sorted dictionary with 
      // key = parent member
      // value = dictionary of elements belong to that parent:
      //    key = element id
      //    value = element
      ConcurrentDictionary<int, ConcurrentDictionary<int, Element>> sortedElements = new ConcurrentDictionary<int, ConcurrentDictionary<int, Element>>();

      Parallel.ForEach(elements, elem =>
      {
        // get parent member
        int parent = -elem.Value.ParentMember.Member;

        // if no parent member then split by property
        if (parent == 0)
          parent = elem.Value.Property;

        if (!sortedElements.ContainsKey(parent))
        {
          sortedElements.TryAdd(parent, new ConcurrentDictionary<int, Element>());
        }
        // add elements to parent member
        sortedElements[parent][elem.Key] = elem.Value;
      });

      // bag to drop all elements to
      ConcurrentBag<GsaElement3dGoo> elem3dGoos = new ConcurrentBag<GsaElement3dGoo>();

      Parallel.For(0, sortedElements.Count, i =>
      {
        int parentID = sortedElements.Keys.ElementAt(i);

        // list of elements with same parent
        ConcurrentDictionary<int, Element> elems = sortedElements[parentID];

        // create list of Prop3Ds
        ConcurrentDictionary<int, GsaProp3d> prop3Ds = new ConcurrentDictionary<int, GsaProp3d>();

        // create list of meshes
        ConcurrentDictionary<int, Mesh> mList = new ConcurrentDictionary<int, Mesh>();

        Parallel.For(0, elems.Count, j =>
              {
                int elementID = elems.Keys.ElementAt(j);
                Mesh ngonClosedMesh = ConvertElement3D(elems[elementID], nodes, unit);
                if (ngonClosedMesh == null) { return; }
                mList[elementID] = ngonClosedMesh;

                // get prop2d (if it exist)
                int propID = elems[elementID].Property;
                GsaProp3d prop = new GsaProp3d(propID);
                if (properties.TryGetValue(propID, out Prop3D apiprop))
                {
                  prop = new GsaProp3d(propID);
                  prop.API_Prop3d = apiprop;

                  // get material (if analysis material exist)
                  //if (prop.API_Prop3d.MaterialAnalysisProperty > 0)
                  //{
                  //    materials.TryGetValue(apiprop.MaterialAnalysisProperty, out AnalysisMaterial apimaterial);
                  //    prop.Material = new GsaMaterial(prop, apimaterial);
                  //}
                  //else
                  //    prop.Material = new GsaMaterial(prop);
                }

                prop3Ds[elementID] = prop;
              });

        // create one large mesh from single mesh face using
        // append list of meshes (faster than appending each mesh one by one)
        Mesh m = new Mesh(); // new mesh to merge existing into
        m.Append(mList.Values.ToList());

        // if parent member value is 0 then no parent member exist for element
        // we can therefore not be sure all elements with parent member = 0 are
        // connected in one mesh.
        if (parentID == 0 && m.DisjointMeshCount > 1)
        {
          // revert back to list of meshes instead of the joined one
          foreach (int key in elems.Keys)
          {
            // create new element from api-element, id, mesh (takes care of topology lists etc) and prop2d
            elems.TryGetValue(key, out Element api_elem);
            mList.TryGetValue(key, out Mesh mesh);
            prop3Ds.TryGetValue(key, out GsaProp3d prop);

            GsaElement3d singleelement3D = new GsaElement3d(
                      new List<Element> { api_elem },
                      new List<int> { key },
                      mesh,
                      new List<GsaProp3d> { prop }
                      );

            // add the element to list of goo 2d elements
            elem3dGoos.Add(new GsaElement3dGoo(singleelement3D));
          }
        }
        else
        {
          // lists needed to create GsaElement3d
          List<Element> api_elems = elems.Values.ToList();
          List<int> ids = elems.Keys.ToList();
          List<GsaProp3d> props = prop3Ds.Values.ToList();

          // create new element from api-element, id, mesh (takes care of topology lists etc) and prop2d
          GsaElement3d element3D = new GsaElement3d(
                    api_elems,
                    ids,
                    m,
                    props
                    );

          elem3dGoos.Add(new GsaElement3dGoo(element3D));
        }

      });
      return elem3dGoos;
      //return elem3dGoos.AsParallel().OrderBy(e => e.Value.ID.Max()).ToList();

    }
    public static Element DuplicateElement(Element elem)
    {
      Element dup = new Element()
      {
        Group = elem.Group,
        IsDummy = elem.IsDummy,
        Name = elem.Name.ToString(),
        Offset = elem.Offset,
        OrientationAngle = elem.OrientationAngle,
        OrientationNode = elem.OrientationNode,
        ParentMember = elem.ParentMember,
        Property = elem.Property,
        Type = elem.Type //GsaToModel.Element1dType((int)Element.Type)
      };

      dup.Topology = new ReadOnlyCollection<int>(elem.Topology.ToList());

      if ((Color)elem.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        dup.Colour = elem.Colour;

      dup.Offset.X1 = elem.Offset.X1;
      dup.Offset.X2 = elem.Offset.X2;
      dup.Offset.Y = elem.Offset.Y;
      dup.Offset.Z = elem.Offset.Z;

      return dup;
    }


    #endregion
    public static Member DuplicateMember(Member mem)
    {
      Member dup = new Member();
      dup.Group = mem.Group;
      dup.IsDummy = mem.IsDummy;
      dup.MeshSize = mem.MeshSize;
      dup.Name = mem.Name.ToString();
      dup.Offset = mem.Offset;
      dup.OrientationAngle = mem.OrientationAngle;
      dup.OrientationNode = mem.OrientationNode;
      dup.Property = mem.Property;
      dup.Topology = mem.Topology.ToString();
      dup.Type = mem.Type;
      dup.Type1D = mem.Type1D;
      dup.Type2D = mem.Type2D;

      if ((Color)mem.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        dup.Colour = mem.Colour;

      dup.Offset.X1 = mem.Offset.X1;
      dup.Offset.X2 = mem.Offset.X2;
      dup.Offset.Y = mem.Offset.Y;
      dup.Offset.Z = mem.Offset.Z;

      return dup;
    }
    #region members
    /// <summary>
    /// Method to convert Members to Member 1D, 2D and 3D
    /// Will output a tuple containing a:
    /// 1. List of GsaMember1dGoos and 
    /// 2. List of GsaMember2dGoos and
    /// 3. List of GsaMember3dGoos
    /// </summary>
    /// <param name="mDict">Dictionary of Members to import</param>
    /// <param name="nDict">Dictionary of Nodes that elements refers to by topology. Include all Nodes unless you are sure what you are doing</param>
    /// <param name="sDict">Dictionary of Sections (for 1D elements)</param>
    /// <param name="pDict">Dictionary of 2D Properties (for 2D elements)</param>
    /// <returns></returns>
    public static Tuple<ConcurrentBag<GsaMember1dGoo>, ConcurrentBag<GsaMember2dGoo>, ConcurrentBag<GsaMember3dGoo>>
        GetMembers(ConcurrentDictionary<int, Member> mDict, ConcurrentDictionary<int, Node> nDict, LengthUnit unit,
        ConcurrentDictionary<int, Section> sDict, ConcurrentDictionary<int, Prop2D> pDict, ConcurrentDictionary<int, Prop3D> p3Dict, GH_Component owner = null)
    {
      // Create lists for Rhino lines and meshes
      ConcurrentBag<GsaMember1dGoo> mem1ds = new ConcurrentBag<GsaMember1dGoo>();
      ConcurrentBag<GsaMember2dGoo> mem2ds = new ConcurrentBag<GsaMember2dGoo>();
      ConcurrentBag<GsaMember3dGoo> mem3ds = new ConcurrentBag<GsaMember3dGoo>();

      // Loop through all members in Member dictionary 
      //try
      //{
      Parallel.ForEach(mDict.Keys, key =>
      {
        if (mDict.TryGetValue(key, out Member member))
        {
          //Member mem = DuplicateMember(member);
          Member mem = member;

          // Get member topology list
          string toporg = mem.Topology; //original topology list

          // ## Member 3D ##
          // if 3D member we have different method:
          if (mem.Type == MemberType.GENERIC_3D)
          {
            List<List<int>> topints = Topology_detangler_Mem3d(toporg);

            // create list of meshes
            List<Mesh> mList = new List<Mesh>();
            bool invalid_node = false;
            // loop through elements in list
            for (int i = 0; i < topints.Count; i++)
            {
              Mesh tempMesh = new Mesh();
              // Get verticies:
              for (int j = 0; j < topints[i].Count; j++)
              {
                if (nDict.TryGetValue(topints[i][j], out Node node))
                {
                  var p = node.Position;
                  tempMesh.Vertices.Add(Point3dFromNode(node, unit));
                }
                else
                  invalid_node = true; // if node cannot be found continue with next key
              }

              // Create mesh face (Tri- or Quad):
              tempMesh.Faces.AddFace(0, 1, 2);

              mList.Add(tempMesh);
            }
            if (invalid_node)
              return;

            // new mesh to merge existing into
            Mesh m = new Mesh();

            // create one large mesh from single mesh face using
            // append list of meshes (faster than appending each mesh one by one)
            m.Append(mList);

            // create prop
            int propID = mem.Property;
            GsaProp3d prop = new GsaProp3d(propID);
            if (p3Dict.TryGetValue(propID, out Prop3D apiprop))
            {
              prop = new GsaProp3d(propID);
              prop.API_Prop3d = apiprop;

              // get material (if analysis material exist)
              //if (prop.API_Prop3d.MaterialAnalysisProperty > 0)
              //{
              //    materials.TryGetValue(apiprop.MaterialAnalysisProperty, out AnalysisMaterial apimaterial);
              //    prop.Material = new GsaMaterial(prop, apimaterial);
              //}
              //else
              //    prop.Material = new GsaMaterial(prop);
            }

            // create 3D member from mesh
            GsaMember3d mem3d = new GsaMember3d(mem, key, m, prop);
            mem3d.MeshSize = new Length(mem.MeshSize, LengthUnit.Meter).As(unit);

            // add member to list
            mem3ds.Add(new GsaMember3dGoo(mem3d));

            //topints.Clear();
          }
          else // ## Member1D or Member2D ##
          {
            // Build topology lists:

            Tuple<Tuple<List<int>, List<string>>, Tuple<List<List<int>>, List<List<string>>>,
                  Tuple<List<List<int>>, List<List<string>>>, List<int>> topologyTuple = Topology_detangler(toporg);
            // tuple of int and string describing the outline topology
            Tuple<List<int>, List<string>> topoTuple = topologyTuple.Item1;
            // tuple of list of int and strings describing voids topology (one list for each void)
            Tuple<List<List<int>>, List<List<string>>> voidTuple = topologyTuple.Item2;
            // tuple of list of int and strings describing inclusion lines topology (one list for each line)
            Tuple<List<List<int>>, List<List<string>>> lineTuple = topologyTuple.Item3;

            List<int> topo_int = topoTuple.Item1;
            List<string> topoType = topoTuple.Item2; //list of polyline curve type (arch or line) for member1d/2d

            List<List<int>> void_topo_int = voidTuple.Item1;
            List<List<string>> void_topoType = voidTuple.Item2; //list of polyline curve type (arch or line) for void /member2d

            List<List<int>> incLines_topo_int = lineTuple.Item1;
            List<List<string>> inclLines_topoType = lineTuple.Item2; //list of polyline curve type (arch or line) for inclusion /member2d

            List<int> inclpts = topologyTuple.Item4;

            // replace topology integers with actual points
            List<Point3d> topopts = new List<Point3d>(); // list of topology points for visualisation /member1d/member2d
            bool invalid_node = false;
            for (int i = 0; i < topo_int.Count; i++)
            {
              if (nDict.TryGetValue(topo_int[i], out Node node))
                topopts.Add(Point3dFromNode(node, unit));
              else
                invalid_node = true; // if node cannot be found continue with next key
            }
            if (invalid_node)
              return;

            //list of lists of void points /member2d
            List<List<Point3d>> void_topo = new List<List<Point3d>>();
            for (int i = 0; i < void_topo_int.Count; i++)
            {
              void_topo.Add(new List<Point3d>());
              for (int j = 0; j < void_topo_int[i].Count; j++)
              {
                if (nDict.TryGetValue(void_topo_int[i][j], out Node node))
                  void_topo[i].Add(Point3dFromNode(node, unit));
              }
            }

            //list of lists of line inclusion topology points /member2d
            List<List<Point3d>> incLines_topo = new List<List<Point3d>>();
            for (int i = 0; i < incLines_topo_int.Count; i++)
            {
              incLines_topo.Add(new List<Point3d>());
              for (int j = 0; j < incLines_topo_int[i].Count; j++)
              {
                if (nDict.TryGetValue(incLines_topo_int[i][j], out Node node))
                  incLines_topo[i].Add(Point3dFromNode(node, unit));
              }
            }

            //list of points for inclusion /member2d
            List<Point3d> incl_pts = new List<Point3d>();
            for (int i = 0; i < inclpts.Count; i++)
            {
              if (nDict.TryGetValue(inclpts[i], out Node node))
              {
                var p = node.Position;
                incl_pts.Add(Point3dFromNode(node, unit));
              }
            }

            // create Members

            // Member1D:
            if (mem.Type == MemberType.GENERIC_1D | mem.Type == MemberType.BEAM | mem.Type == MemberType.CANTILEVER |
                      mem.Type == MemberType.COLUMN | mem.Type == MemberType.COMPOS | mem.Type == MemberType.PILE)
            {
              // check if Mem1D topology has minimum 2 points
              if (topopts.Count < 2)
              {
                //topopts.Add(topopts[0]);
                //topoType.Add(topoType[0]);
                string error = " Invalid topology Mem1D ID: " + key + ".";
                if (owner != null)
                  owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, error);
                return;
              }

              // orientation node
              GsaNode orient = null;
              if (mem.OrientationNode > 0)
              {
                if (nDict.TryGetValue(mem.OrientationNode, out Node node))
                {
                  orient = new GsaNode(Point3dFromNode(node, unit),
                            mem.OrientationNode);
                }
              }

              // get section (if it exist)
              GsaSection section = new GsaSection(mem.Property);
              if (sDict.TryGetValue(mem.Property, out Section apisection))
              {
                section = new GsaSection(mem.Property);
                section.API_Section = apisection;

                // material to be implemented
              }

              // create the element from list of points and type description
              GsaMember1d mem1d = new GsaMember1d(mem, unit, key, topopts.ToList(), topoType.ToList(), section, orient);
              mem1d.MeshSize = new Length(mem.MeshSize, LengthUnit.Meter).As(unit);

              // add member to output list
              mem1ds.Add(new GsaMember1dGoo(mem1d));
            }
            else // Member2D:
            {

              // create 2d property
              GsaProp2d prop2d = new GsaProp2d(mem.Property);
              if (pDict.TryGetValue(mem.Property, out Prop2D apiProp))
              {
                prop2d = new GsaProp2d(mem.Property);
                prop2d.API_Prop2d = apiProp;

                // material to be implemented
              }

              // create member from topology lists
              GsaMember2d mem2d = new GsaMember2d(mem, unit, key,
                        topopts.ToList(),
                        topoType.ToList(),
                        void_topo.ToList(),
                        void_topoType.ToList(),
                        incLines_topo.ToList(),
                        inclLines_topoType.ToList(),
                        incl_pts.ToList(),
                        prop2d, owner);
              mem2d.MeshSize = new Length(mem.MeshSize, LengthUnit.Meter).As(unit);

              // add member to output list
              mem2ds.Add(new GsaMember2dGoo(mem2d));
            }

            topopts.Clear();
            topoType.Clear();
            void_topo.Clear();
            void_topoType.Clear();
            incLines_topo.Clear();
            inclLines_topoType.Clear();
            incl_pts.Clear();
          }
        }
      });
      //}
      //catch (Exception e)
      //{
      //    if (owner == null)
      //        throw new Exception(e.InnerException.Message);
      //    else
      //        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, e.InnerException.Message);
      //}
      return new Tuple<ConcurrentBag<GsaMember1dGoo>, ConcurrentBag<GsaMember2dGoo>, ConcurrentBag<GsaMember3dGoo>>(
          mem1ds, mem2ds, mem3ds);

      //return new Tuple<List<GsaMember1dGoo>, List<GsaMember2dGoo>, List<GsaMember3dGoo>>(
      //    mem1ds.AsParallel().OrderBy(e => e.Value.ID).ToList(), 
      //    mem2ds.AsParallel().OrderBy(e => e.Value.ID).ToList(), 
      //    mem3ds.AsParallel().OrderBy(e => e.Value.ID).ToList());
    }
    internal static Point3d Point3dFromNode(Node node, LengthUnit unit)
    {
      return (unit == LengthUnit.Meter) ?
          new Point3d(node.Position.X, node.Position.Y, node.Position.Z) : // skip unitsnet conversion, gsa api node always in meters
          new Point3d(new Length(node.Position.X, LengthUnit.Meter).As(unit),
                      new Length(node.Position.Y, LengthUnit.Meter).As(unit),
                      new Length(node.Position.Z, LengthUnit.Meter).As(unit));
    }
    internal static Point3d Point3dFromXYZUnit(double x, double y, double z, LengthUnit modelUnit)
    {
      return (modelUnit == LengthUnit.Meter) ?
          new Point3d(x, y, z) : // skip unitsnet conversion, gsa api node always in meters
          new Point3d(new Length(x, LengthUnit.Meter).As(modelUnit),
                      new Length(y, LengthUnit.Meter).As(modelUnit),
                      new Length(z, LengthUnit.Meter).As(modelUnit));
    }
    internal static Vector3d Vector3dFromXYZUnit(double x, double y, double z, LengthUnit modelUnit)
    {
      return (modelUnit == LengthUnit.Meter) ?
          new Vector3d(x, y, z) : // skip unitsnet conversion, gsa api node always in meters
          new Vector3d(new Length(x, LengthUnit.Meter).As(modelUnit),
                      new Length(y, LengthUnit.Meter).As(modelUnit),
                      new Length(z, LengthUnit.Meter).As(modelUnit));
    }
    internal static Node UpdateNodePositionUnit(Node node, LengthUnit unit)
    {
      if (unit != LengthUnit.Meter) // convert from meter to input unit if not meter
      {
        Vector3 pos = new Vector3();
        pos.X = new Length(node.Position.X, LengthUnit.Meter).As(unit);
        pos.Y = new Length(node.Position.Y, LengthUnit.Meter).As(unit);
        pos.Z = new Length(node.Position.Z, LengthUnit.Meter).As(unit);
        node.Position = pos;
      }
      return node;
    }
    #endregion

    #region topology string manipulation

    /// <summary>
    /// Method to split/untangle a topology list from GSA into separate lists for
    /// Topology, Voids, Inclusion lines and Inclusion points with corrosponding list for topology type.
    /// 
    /// Output tuple with three sub-tubles for:
    /// - Topology: (Topology integers and topology types)
    /// - Voids: (List of integers and list of topology types)
    /// - Lines: (List of integers and list of topology types)
    /// - Points: (Topology integers)
    /// 
    /// Example: gsa_topology = 
    /// "7 8 9 a 10 11 7 V(12 13 a 14 15) L(16 a 18 17) 94 P 20 P(19 21 22) L(23 24) 84"
    /// will results in:
    /// 
    /// Tuple1, Item1: Topology: (7, 8, 9, 10, 11, 7, 94, 84)
    /// Tuple1, Item2: TopoType: ( ,  ,  ,  a,   ,  ,   ,   )
    /// 
    /// Tuple2, Item1: List(Voids): (12, 13, 14, 15)
    /// Tuple2, Item2: List(VType): (  ,   ,  a,   )
    /// 
    /// Tuple3, Item1: List(Lines): (16, 18, 17) (23, 24)
    /// Tuple3, Item2: List(LType): (  ,  a,   ) (  ,   )
    /// 
    /// Points: (20, 19, 21, 22)
    /// 
    /// </summary>
    /// <param name="gsa_topology"></param>
    /// <returns></returns>
    public static Tuple<Tuple<List<int>, List<string>>, Tuple<List<List<int>>, List<List<string>>>,
        Tuple<List<List<int>>, List<List<string>>>, List<int>> Topology_detangler(string gsa_topology)
    {
      List<string> voids = new List<string>();
      List<string> lines = new List<string>();
      List<string> points = new List<string>();
      //string gsa_topology = "7 8 9 a 10 11 7 V(12 13 a 14 15) L(16 a 18 17) 94 P 20 P(19 21 22) L(23 24) 84";
      gsa_topology = gsa_topology.ToUpper();
      char[] spearator = { '(', ')' };

      String[] strlist = gsa_topology.Split(spearator);
      List<String> topos = new List<String>(strlist);

      // first split out anything in brackets and put them into lists for V, L or P
      // also remove those lines so that they dont appear twice in the end
      for (int i = 0; i < topos.Count(); i++)
      {
        if (topos[i].Length > 1)
        {
          if (topos[i].Substring(topos[i].Length - 1, 1) == "V")
          {
            topos[i] = topos[i].Substring(0, topos[i].Length - 1);
            voids.Add(topos[i + 1]);
            topos.RemoveAt(i + 1);
            continue;
          }
        }

        if (topos[i].Length > 1)
        {
          if (topos[i].Substring(topos[i].Length - 1, 1) == "L")
          {
            topos[i] = topos[i].Substring(0, topos[i].Length - 1);
            lines.Add(topos[i + 1]);
            topos.RemoveAt(i + 1);
            continue;
          }
        }

        if (topos[i].Length > 1)
        {
          if (topos[i].Substring(topos[i].Length - 1, 1) == "P")
          {
            topos[i] = topos[i].Substring(0, topos[i].Length - 1);
            points.Add(topos[i + 1]);
            topos.RemoveAt(i + 1);
            continue;
          }
        }
      }

      // then split list with whitespace
      List<String> topolos = new List<String>();
      for (int i = 0; i < topos.Count(); i++)
      {
        List<String> temptopos = new List<String>(topos[i].Split(' '));
        topolos.AddRange(temptopos);
      }

      // also split list of points by whitespace as they go to single list
      List<String> pts = new List<String>();
      for (int i = 0; i < points.Count(); i++)
      {
        List<String> temppts = new List<String>(points[i].Split(' '));
        pts.AddRange(temppts);
      }

      // voids and lines needs to be made into list of lists
      List<List<int>> void_topo = new List<List<int>>();
      List<List<String>> void_topoType = new List<List<String>>();
      for (int i = 0; i < voids.Count(); i++)
      {
        List<String> tempvoids = new List<String>(voids[i].Split(' '));
        List<int> tmpvds = new List<int>();
        List<String> tmpType = new List<String>();
        for (int j = 0; j < tempvoids.Count(); j++)
        {
          if (tempvoids[j] == "A")
          {
            tmpType.Add("A");
            tempvoids.RemoveAt(j);
          }
          else
            tmpType.Add(" ");
          int tpt = Int32.Parse(tempvoids[j]);
          tmpvds.Add(tpt);
        }
        void_topo.Add(tmpvds);
        void_topoType.Add(tmpType);
      }
      List<List<int>> incLines_topo = new List<List<int>>();
      List<List<String>> inclLines_topoType = new List<List<String>>();
      for (int i = 0; i < lines.Count(); i++)
      {
        List<String> templines = new List<String>(lines[i].Split(' '));
        List<int> tmplns = new List<int>();
        List<String> tmpType = new List<String>();
        for (int j = 0; j < templines.Count(); j++)
        {
          if (templines[j] == "A")
          {
            tmpType.Add("A");
            templines.RemoveAt(j);
          }
          else
            tmpType.Add(" ");
          int tpt = Int32.Parse(templines[j]);
          tmplns.Add(tpt);
        }
        incLines_topo.Add(tmplns);
        inclLines_topoType.Add(tmpType);
      }

      // then remove empty entries
      for (int i = 0; i < topolos.Count(); i++)
      {
        if (topolos[i] == null)
        {
          topolos.RemoveAt(i);
          i -= 1;
          continue;
        }
        if (topolos[i].Length < 1)
        {
          topolos.RemoveAt(i);
          i -= 1;
          continue;
        }
      }

      // Find any single inclusion points not in brackets
      for (int i = 0; i < topolos.Count(); i++)
      {
        if (topolos[i] == "P")
        {
          pts.Add(topolos[i + 1]);
          topolos.RemoveAt(i + 1);
          topolos.RemoveAt(i);
          i -= 1;
          continue;
        }
        if (topolos[i].Length < 1)
        {
          topolos.RemoveAt(i);
          i -= 1;
          continue;
        }
      }
      List<int> inclpoint = new List<int>();
      for (int i = 0; i < pts.Count(); i++)
      {
        if (pts[i] != "")
        {
          int tpt = Int32.Parse(pts[i]);
          inclpoint.Add(tpt);
        }
      }

      // write out topology type (A) to list
      List<int> topoint = new List<int>();
      List<String> topoType = new List<String>();
      for (int i = 0; i < topolos.Count(); i++)
      {
        if (topolos[i] == "A")
        {
          topoType.Add("A");
          int tptA = Int32.Parse(topolos[i + 1]);
          topoint.Add(tptA);
          i += 1;
          continue;
        }
        topoType.Add(" ");
        int tpt = Int32.Parse(topolos[i]);
        topoint.Add(tpt);
      }
      Tuple<List<int>, List<string>> topoTuple = new Tuple<List<int>, List<string>>(topoint, topoType);
      Tuple<List<List<int>>, List<List<string>>> voidTuple = new Tuple<List<List<int>>, List<List<string>>>(void_topo, void_topoType);
      Tuple<List<List<int>>, List<List<string>>> lineTuple = new Tuple<List<List<int>>, List<List<string>>>(incLines_topo, inclLines_topoType);

      return new Tuple<Tuple<List<int>, List<string>>, Tuple<List<List<int>>, List<List<string>>>,
      Tuple<List<List<int>>, List<List<string>>>, List<int>>(topoTuple, voidTuple, lineTuple, inclpoint);
    }

    /// <summary>
    /// Method to convert a topology string from a 3D Member
    /// into a list of 3 verticies
    /// </summary>
    /// <param name="gsa_topology">Topology list as string</param>
    /// <returns></returns>
    public static List<List<int>> Topology_detangler_Mem3d(string gsa_topology)
    {
      // Example input string ‘1 2 4 3; 5 6 8 7; 1 5 2 6 3 7 4 8 1 5’ 
      // we want to create a triangular mesh for Member3D SolidMesh
      List<List<int>> topolist = new List<List<int>>();

      // first split the string by ";"
      char spearator = ';';

      String[] strlist = gsa_topology.Split(spearator);

      // loop through all face lists
      foreach (string stripe in strlist)
      {
        // trim and split list by white space
        string trimmedstripe = stripe.Trim();
        List<String> verticiesString = new List<String>(trimmedstripe.Split(' '));

        // convert string to int
        List<int> tempverticies = new List<int>();
        foreach (string vert in verticiesString)
        {
          int tpt = Int32.Parse(vert);
          tempverticies.Add(tpt);
        }

        while (tempverticies.Count > 2)
        {
          // add the first triangle
          List<int> templist1 = new List<int>();
          templist1.Add(tempverticies[0]);
          templist1.Add(tempverticies[1]);
          templist1.Add(tempverticies[2]);

          // add the list to the main list
          topolist.Add(templist1);

          if (tempverticies.Count > 3)
          {
            // add the second triangle the other way round
            List<int> templist2 = new List<int>();
            templist2.Add(tempverticies[1]);
            templist2.Add(tempverticies[3]);
            templist2.Add(tempverticies[2]);

            // add the list to the main list
            topolist.Add(templist2);

            // remove the first two verticies from list
            tempverticies.RemoveAt(0);
          }
          // put the second remove outside the if to also remove if we only 
          // have 3 verticies to bring count below 3 and exit while loop
          tempverticies.RemoveAt(0);
        }
      }
      return topolist;

    }
    #endregion

    #region section and properties
    /// <summary>
    /// Method to import Sections from a GSA model.
    /// Will output a list of GsaSectionsGoo.
    /// </summary>
    /// <param name="sDict">Dictionary of pre-filtered sections to import</param>
    /// <returns></returns>
    public static List<GsaSectionGoo> GetSections(IReadOnlyDictionary<int, Section> sDict, ReadOnlyDictionary<int, AnalysisMaterial> analysisMaterials, IReadOnlyDictionary<int, SectionModifier> modDict)
    {
      List<GsaSectionGoo> sections = new List<GsaSectionGoo>();

      // Loop through all sections in Section dictionary and create new GsaSections
      foreach (int key in sDict.Keys)
      {
        if (sDict.TryGetValue(key, out Section apisection)) //1-base numbering
        {
          GsaSection sect = new GsaSection(key);
          sect.API_Section = apisection;
          if (sect.API_Section.MaterialAnalysisProperty != 0)
          {
            if (analysisMaterials.ContainsKey(sect.API_Section.MaterialAnalysisProperty))
              sect.Material.AnalysisMaterial = analysisMaterials[apisection.MaterialAnalysisProperty];
          }
          if (modDict.Keys.Contains(key))
            sect.Modifier = new GsaSectionModifier(modDict[key]);
          sections.Add(new GsaSectionGoo(sect));
        }
      }
      return sections;
    }
    /// <summary>
    /// Method to import Prop2ds from a GSA model.
    /// Will output a list of GsaProp2dGoo.
    /// </summary>
    /// <param name="pDict">Dictionary of pre-filtered 2D Properties to import</param>
    /// <returns></returns>
    public static List<GsaProp2dGoo> GetProp2ds(IReadOnlyDictionary<int, Prop2D> pDict, ReadOnlyDictionary<int, AnalysisMaterial> analysisMaterials)
    {
      List<GsaProp2dGoo> prop2ds = new List<GsaProp2dGoo>();

      // Loop through all sections in Properties dictionary and create new GsaProp2d
      foreach (int key in pDict.Keys)
      {
        if (pDict.TryGetValue(key, out Prop2D apisection)) //1-base numbering
        {
          GsaProp2d prop = new GsaProp2d(key);
          prop.API_Prop2d = apisection;
          if (prop.API_Prop2d.MaterialAnalysisProperty != 0)
          {
            if (analysisMaterials.ContainsKey(prop.API_Prop2d.MaterialAnalysisProperty))
              prop.Material.AnalysisMaterial = analysisMaterials[apisection.MaterialAnalysisProperty];
          }

          prop2ds.Add(new GsaProp2dGoo(prop));
        }
      }
      return prop2ds;
    }
    public static List<GsaProp3dGoo> GetProp3ds(IReadOnlyDictionary<int, Prop3D> pDict, ReadOnlyDictionary<int, AnalysisMaterial> analysisMaterials)
    {
      List<GsaProp3dGoo> prop2ds = new List<GsaProp3dGoo>();

      // Loop through all sections in Properties dictionary and create new GsaProp2d
      foreach (int key in pDict.Keys)
      {
        if (pDict.TryGetValue(key, out Prop3D apisection)) //1-base numbering
        {
          GsaProp3d prop = new GsaProp3d(key);
          prop.API_Prop3d = apisection;
          if (prop.API_Prop3d.MaterialAnalysisProperty != 0)
          {
            if (analysisMaterials.ContainsKey(prop.API_Prop3d.MaterialAnalysisProperty))
              prop.Material.AnalysisMaterial = analysisMaterials[apisection.MaterialAnalysisProperty];
          }
          prop2ds.Add(new GsaProp3dGoo(prop));
        }
      }
      return prop2ds;
    }
    #endregion

    #region loads
    /// <summary>
    /// Method to import Gravity Loads from a GSA model.
    /// Will output a list of GsaLoadsGoo.
    /// </summary>
    /// <param name="gravityLoads">Collection of gravity loads to import</param>
    /// <returns></returns>
    public static List<GsaLoadGoo> GetGravityLoads(ReadOnlyCollection<GravityLoad> gravityLoads)
    {
      List<GsaLoadGoo> loads = new List<GsaLoadGoo>();

      List<GravityLoad> gloads = gravityLoads.ToList();

      // Loop through all loads in list and create new GsaLoads
      for (int i = 0; i < gloads.Count; i++)
      {
        GsaGravityLoad myload = new GsaGravityLoad();
        myload.GravityLoad = gloads[i];
        GsaLoad load = new GsaLoad(myload);
        loads.Add(new GsaLoadGoo(load));
      }
      return loads;
    }
    /// <summary>
    /// Method to import all Node Loads from a GSA model.
    /// 
    /// GSA Node loads vary by type, to get all node loads easiest
    /// method seems to be toogling through all enum types which
    /// requeres the entire model to be inputted to this method.
    /// 
    /// Will output a list of GsaLoads.
    /// </summary>
    /// <param name="model">GSA model containing node loads</param>
    /// <returns></returns>
    public static List<GsaLoadGoo> GetNodeLoads(Model model)
    {
      List<GsaLoadGoo> loads = new List<GsaLoadGoo>();

      // NodeLoads come in varioys types, depending on GsaAPI.NodeLoadType:
      foreach (NodeLoadType typ in Enum.GetValues(typeof(NodeLoadType)))
      {
        try // some GsaAPI.NodeLoadTypes are currently not supported in the API and throws an error
        {
          List<NodeLoad> gsaloads = model.NodeLoads(typ).ToList();
          GsaNodeLoad.NodeLoadTypes ntyp = GsaNodeLoad.NodeLoadTypes.NODE_LOAD;
          switch (typ)
          {
            case NodeLoadType.APPL_DISP:
              ntyp = GsaNodeLoad.NodeLoadTypes.APPLIED_DISP;
              break;
            case NodeLoadType.GRAVITY:
              ntyp = GsaNodeLoad.NodeLoadTypes.GRAVITY;
              break;
            case NodeLoadType.NODE_LOAD:
              ntyp = GsaNodeLoad.NodeLoadTypes.NODE_LOAD;
              break;
            case NodeLoadType.NUM_TYPES:
              ntyp = GsaNodeLoad.NodeLoadTypes.NUM_TYPES;
              break;
            case NodeLoadType.SETTLEMENT:
              ntyp = GsaNodeLoad.NodeLoadTypes.SETTLEMENT;
              break;
          }

          // Loop through all loads in list and create new GsaLoads
          for (int i = 0; i < gsaloads.Count; i++)
          {
            GsaNodeLoad myload = new GsaNodeLoad();
            myload.NodeLoad = gsaloads[i];
            myload.Type = ntyp;
            GsaLoad load = new GsaLoad(myload);
            loads.Add(new GsaLoadGoo(load));
          }
        }
        catch (Exception)
        {

        }

      }
      return loads;
    }
    /// <summary>
    /// Method to import Beam Loads from a GSA model.
    /// Will output a list of GsaLoads.
    /// </summary>
    /// <param name="beamLoads">Collection of beams loads to be imported</param>
    /// <returns></returns>
    public static List<GsaLoadGoo> GetBeamLoads(ReadOnlyCollection<BeamLoad> beamLoads)
    {
      List<GsaLoadGoo> loads = new List<GsaLoadGoo>();

      List<BeamLoad> gsaloads = beamLoads.ToList();

      // Loop through all loads in list and create new GsaLoads
      for (int i = 0; i < gsaloads.Count; i++)
      {
        GsaBeamLoad myload = new GsaBeamLoad();
        myload.BeamLoad = gsaloads[i];
        GsaLoad load = new GsaLoad(myload);
        loads.Add(new GsaLoadGoo(load));
      }
      return loads;
    }
    /// <summary>
    /// Method to import Face Loads from a GSA model.
    /// Will output a list of GsaLoads.
    /// </summary>
    /// <param name="faceLoads">Collection of Face loads to be imported</param>
    /// <returns></returns>
    public static List<GsaLoadGoo> GetFaceLoads(ReadOnlyCollection<FaceLoad> faceLoads)
    {
      List<GsaLoadGoo> loads = new List<GsaLoadGoo>();

      List<FaceLoad> gsaloads = faceLoads.ToList();

      // Loop through all loads in list and create new GsaLoads
      for (int i = 0; i < gsaloads.Count; i++)
      {
        GsaFaceLoad myload = new GsaFaceLoad();
        myload.FaceLoad = gsaloads[i];
        GsaLoad load = new GsaLoad(myload);
        loads.Add(new GsaLoadGoo(load));
      }
      return loads;
    }
    /// <summary>
    /// Method to import Grid Point Loads from a GSA model.
    /// Will output a list of GsaLoads.
    /// </summary>
    /// <param name="pointLoads">Collection of Grid Point loads to be imported</param>
    /// <param name="srfDict">Grid Surface Dictionary</param>
    /// <param name="plnDict">Grid Plane Dictionary</param>
    /// <param name="axDict">Axes Dictionary</param>
    /// <returns></returns>
    public static List<GsaLoadGoo> GetGridPointLoads(ReadOnlyCollection<GridPointLoad> pointLoads,
        IReadOnlyDictionary<int, GridSurface> srfDict, IReadOnlyDictionary<int, GridPlane> plnDict, IReadOnlyDictionary<int, Axis> axDict, LengthUnit unit)
    {
      List<GsaLoadGoo> loads = new List<GsaLoadGoo>();

      List<GridPointLoad> gsaloads = pointLoads.ToList();

      // Loop through all loads in list and create new GsaLoads
      for (int i = 0; i < gsaloads.Count; i++)
      {
        // Get Grid Point Load
        GsaGridPointLoad myload = new GsaGridPointLoad();
        myload.GridPointLoad = gsaloads[i];

        // Get GridPlaneSurface
        myload.GridPlaneSurface = GetGridPlaneSurface(srfDict, plnDict, axDict, gsaloads[i].GridSurface, unit);

        // Add load to list
        GsaLoad load = new GsaLoad(myload);
        loads.Add(new GsaLoadGoo(load));
      }
      return loads;
    }
    /// <summary>
    /// Method to import Grid Line Loads from a GSA model.
    /// Will output a list of GsaLoads.
    /// </summary>
    /// <param name="lineLoads">Collection of Grid Line loads to be imported</param>
    /// <param name="srfDict">Grid Surface Dictionary</param>
    /// <param name="plnDict">Grid Plane Dictionary</param>
    /// <param name="axDict">Axes Dictionary</param>
    /// <returns></returns>
    public static List<GsaLoadGoo> GetGridLineLoads(ReadOnlyCollection<GridLineLoad> lineLoads,
        IReadOnlyDictionary<int, GridSurface> srfDict, IReadOnlyDictionary<int, GridPlane> plnDict, IReadOnlyDictionary<int, Axis> axDict, LengthUnit unit)
    {
      List<GsaLoadGoo> loads = new List<GsaLoadGoo>();

      List<GridLineLoad> gsaloads = lineLoads.ToList();

      // Loop through all loads in list and create new GsaLoads
      for (int i = 0; i < gsaloads.Count; i++)
      {
        // Get Grid Point Load
        GsaGridLineLoad myload = new GsaGridLineLoad();
        myload.GridLineLoad = gsaloads[i];

        // Get GridPlaneSurface
        myload.GridPlaneSurface = GetGridPlaneSurface(srfDict, plnDict, axDict, gsaloads[i].GridSurface, unit);

        // Add load to list
        GsaLoad load = new GsaLoad(myload);
        loads.Add(new GsaLoadGoo(load));
      }
      return loads;
    }
    /// <summary>
    /// Method to import Grid Area Loads from a GSA model.
    /// Will output a list of GsaLoads.
    /// </summary>
    /// <param name="areaLoads">Collection of Grid Area loads to be imported</param>
    /// <param name="srfDict">Grid Surface Dictionary</param>
    /// <param name="plnDict">Grid Plane Dictionary</param>
    /// <param name="axDict">Axes Dictionary</param>
    /// <returns></returns>
    public static List<GsaLoadGoo> GetGridAreaLoads(ReadOnlyCollection<GridAreaLoad> areaLoads,
        IReadOnlyDictionary<int, GridSurface> srfDict, IReadOnlyDictionary<int, GridPlane> plnDict, IReadOnlyDictionary<int, Axis> axDict, LengthUnit unit)
    {
      List<GsaLoadGoo> loads = new List<GsaLoadGoo>();

      List<GridAreaLoad> gsaloads = areaLoads.ToList();

      // Loop through all loads in list and create new GsaLoads
      for (int i = 0; i < gsaloads.Count; i++)
      {
        // Get Grid Point Load
        GsaGridAreaLoad myload = new GsaGridAreaLoad();
        myload.GridAreaLoad = gsaloads[i];

        // Get GridPlaneSurface
        myload.GridPlaneSurface = GetGridPlaneSurface(srfDict, plnDict, axDict, gsaloads[i].GridSurface, unit);

        // Add load to list
        GsaLoad load = new GsaLoad(myload);
        loads.Add(new GsaLoadGoo(load));
      }
      return loads;
    }

    /// <summary>
    /// Method to create GsaGridPlaneSurface including 
    /// grid surface, grid plane and axis from GSA Model
    /// 
    /// Grid Surface references a Grid Plane
    /// Grid Plane references an Axis
    /// Only Grid Surface ID is required, the others will be found by ref
    /// 
    /// Will output a new GsaGridPlaneSurface.
    /// </summary>
    /// <param name="srfDict">Grid Surface Dictionary</param>
    /// <param name="plnDict">Grid Plane Dictionary</param>
    /// <param name="axDict">Axes Dictionary</param>
    /// <param name="gridSrfId">ID/Key/number of Grid Surface in GSA model to convert</param>
    /// <returns></returns>
    public static GsaGridPlaneSurface GetGridPlaneSurface(IReadOnlyDictionary<int, GridSurface> srfDict,
        IReadOnlyDictionary<int, GridPlane> plnDict, IReadOnlyDictionary<int, Axis> axDict, int gridSrfId, LengthUnit unit)
    {
      // GridPlaneSurface
      GsaGridPlaneSurface gps = new GsaGridPlaneSurface();

      // Get Grid Surface
      if (srfDict.Count > 0)
      {
        srfDict.TryGetValue(gridSrfId, out GridSurface gs);
        gps.GridSurface = gs;
        gps.GridSurfaceId = gridSrfId;

        // Get Grid Plane
        plnDict.TryGetValue(gs.GridPlane, out GridPlane gp);
        gps.GridPlane = gp;
        gps.GridPlaneId = gs.GridPlane;
        gps.Elevation = gp.Elevation;

        // Get Axis
        axDict.TryGetValue(gp.AxisProperty, out Axis ax);

        gps.AxisId = gp.AxisProperty;

        // Construct Plane from Axis
        Plane pln;
        if (ax != null)
        {
          // for new origin Z-coordinate we add axis origin and grid plane elevation
          pln = new Plane(Point3dFromXYZUnit(ax.Origin.X, ax.Origin.Y, ax.Origin.Z + gp.Elevation, unit),
            Vector3dFromXYZUnit(ax.XVector.X, ax.XVector.Y, ax.XVector.Z, unit),
            Vector3dFromXYZUnit(ax.XYPlane.X, ax.XYPlane.Y, ax.XYPlane.Z, unit)
            );
        }
        else
        {
          pln = Plane.WorldXY;
          pln.OriginZ = new Length(gp.Elevation, LengthUnit.Meter).As(unit);
        }
        gps.Plane = pln;
      }
      else
        return null;

      return gps;
    }
    #endregion

    #region analysis
    internal static Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> GetAnalysisTasksAndCombinations(GsaModel gsaModel)
    {
      return GetAnalysisTasksAndCombinations(gsaModel.Model);
    }
    internal static Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> GetAnalysisTasksAndCombinations(Model model)
    {
      ReadOnlyDictionary<int, AnalysisTask> tasks = model.AnalysisTasks();

      List<GsaAnalysisTaskGoo> tasksList = new List<GsaAnalysisTaskGoo>();
      List<GsaAnalysisCaseGoo> caseList = new List<GsaAnalysisCaseGoo>();
      List<int> caseIDs = new List<int>();

      foreach (KeyValuePair<int, AnalysisTask> item in tasks)
      {
        GsaAnalysisTask task = new GsaAnalysisTask(item.Key, item.Value, model);
        tasksList.Add(new GsaAnalysisTaskGoo(task));
        foreach (GsaAnalysisCase acase in task.Cases)
        {
          caseIDs.Add(acase.ID);
        }
      }
      ReadOnlyCollection<GravityLoad> gravities = model.GravityLoads();
      caseIDs.AddRange(gravities.Select(x => x.Case));

      foreach (NodeLoadType typ in Enum.GetValues(typeof(NodeLoadType)))
      {
        ReadOnlyCollection<NodeLoad> nodeLoads;
        try // some GsaAPI.NodeLoadTypes are currently not supported in the API and throws an error
        {
          nodeLoads = model.NodeLoads(typ);
          caseIDs.AddRange(nodeLoads.Select(x => x.Case));
        }
        catch (Exception) { }
      }

      ReadOnlyCollection<BeamLoad> beamLoads = model.BeamLoads();
      caseIDs.AddRange(beamLoads.Select(x => x.Case));

      ReadOnlyCollection<FaceLoad> faceLoads = model.FaceLoads();
      caseIDs.AddRange(faceLoads.Select(x => x.Case));

      ReadOnlyCollection<GridPointLoad> gridPointLoads = model.GridPointLoads();
      caseIDs.AddRange(gridPointLoads.Select(x => x.Case));

      ReadOnlyCollection<GridLineLoad> gridLineLoads = model.GridLineLoads();
      caseIDs.AddRange(gridLineLoads.Select(x => x.Case));

      ReadOnlyCollection<GridAreaLoad> gridAreaLoads = model.GridAreaLoads();
      caseIDs.AddRange(gridAreaLoads.Select(x => x.Case));

      caseIDs = caseIDs.GroupBy(x => x).Select(y => y.First()).ToList();

      foreach (int caseID in caseIDs)
      {
        string caseName = model.AnalysisCaseName(caseID);
        if (caseName == "")
          caseName = "Case " + caseID.ToString();
        string caseDescription = model.AnalysisCaseDescription(caseID);
        if (caseDescription == "")
          caseDescription = "L" + caseID.ToString();
        caseList.Add(new GsaAnalysisCaseGoo(new GsaAnalysisCase(caseID, caseName, caseDescription)));
      }

      return new Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>>(tasksList, caseList);
    }



    #endregion

  }
}
