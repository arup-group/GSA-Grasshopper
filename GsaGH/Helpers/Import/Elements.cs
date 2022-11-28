using System;
using System.Linq;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Helpers.Import
{
  /// <summary>
  /// Class containing functions to import various object types from GSA
  /// </summary>
  internal class Elements
  {
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
    internal static Tuple<ConcurrentBag<GsaElement1dGoo>, ConcurrentBag<GsaElement2dGoo>, ConcurrentBag<GsaElement3dGoo>>
        GetElements(ConcurrentDictionary<int, Element> eDict, ConcurrentDictionary<int, Node> nDict,
        ConcurrentDictionary<int, Section> sDict, ConcurrentDictionary<int, Prop2D> pDict, ConcurrentDictionary<int, Prop3D> p3Dict,
        ConcurrentDictionary<int, AnalysisMaterial> mDict, ConcurrentDictionary<int, SectionModifier> modDict, ConcurrentDictionary<int, ReadOnlyCollection<double>> localAxesDict, LengthUnit unit)
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
                ConvertToElement1D(item.Value, item.Key, nDict, sDict, mDict, modDict, localAxesDict, unit))));

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
    internal static GsaElement1d ConvertToElement1D(Element element,
        int ID, ConcurrentDictionary<int, Node> nodes, ConcurrentDictionary<int, Section> sections,
        ConcurrentDictionary<int, AnalysisMaterial> materials, ConcurrentDictionary<int, SectionModifier> sectionModifiers, ConcurrentDictionary<int, ReadOnlyCollection<double>> localAxes, LengthUnit unit)
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
            pts.Add(Nodes.Point3dFromNode(node, unit));
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
            orient = new GsaNode(Nodes.Point3dFromNode(node, unit),
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
        GsaElement1d element1d =  new GsaElement1d(element, ln, ID, section, orient);

        // set local axes
        element1d.LocalAxes = new GsaLocalAxes(localAxes[ID]);

        return element1d;
      }
      return null;
    }

    /// <summary>
    /// Method to get convert an Element to a Mesh with one face (tri, quad or ngon)
    /// </summary>
    /// <param name="element">Element to get mesh face from</param>
    /// <param name="nodes">Dictionary of nodes that includes nodes for the topology which the element.Topology refers to. Typically use all nodes from a GSA model</param>
    /// <returns></returns>
    internal static Mesh ConvertElement2D(Element element, IReadOnlyDictionary<int, Node> nodes, LengthUnit unit)
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
          outMesh.Vertices.Add(Nodes.Point3dFromNode(node, unit));
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
    internal static ConcurrentBag<GsaElement2dGoo> ConvertToElement2Ds(
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

    internal static Mesh ConvertElement3D(Element element, ConcurrentDictionary<int, Node> nodes, LengthUnit unit)
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
          outMesh.Vertices.Add(Nodes.Point3dFromNode(node, unit));
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

    internal static ConcurrentBag<GsaElement3dGoo> ConvertToElement3Ds(ConcurrentDictionary<int, Element> elements, ConcurrentDictionary<int, Node> nodes,
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
    internal static Element DuplicateElement(Element elem)
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
  }
}
