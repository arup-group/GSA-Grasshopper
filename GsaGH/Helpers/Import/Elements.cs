using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Helpers.Import {
  /// <summary>
  ///   Class containing functions to import various object types from GSA
  /// </summary>
  internal class Elements {

    internal static Mesh ConvertElement2D(
      Element element, ReadOnlyDictionary<int, Node> nodes, LengthUnit unit) {
      ReadOnlyCollection<int> topo = element.Topology;

      if (topo.Count < 3 || element.Type == ElementType.THREE_D
        || element.Type == ElementType.BRICK8 || element.Type == ElementType.WEDGE6
        || element.Type == ElementType.PYRAMID5 || element.Type == ElementType.TETRA4) {
        return null;
      }

      var outMesh = new Mesh();

      foreach (int t in topo) {
        if (nodes.TryGetValue(t, out Node node)) {
          outMesh.Vertices.Add(Nodes.Point3dFromNode(node, unit));
        }
      }

      switch (topo.Count) {
        case 3:
          outMesh.Faces.AddFace(0, 1, 2);
          break;

        case 4:
          outMesh.Faces.AddFace(0, 1, 2, 3);
          break;

        default: {
          if (topo.Count > 4) {
            // so we introduce the average middle point and create more faces

            switch (topo.Count) {
              case 6: {
                outMesh.Faces.AddFace(0, 3, 5);
                outMesh.Faces.AddFace(1, 4, 3);
                outMesh.Faces.AddFace(2, 5, 4);
                outMesh.Faces.AddFace(3, 4, 5);
                var tri6Vert = new List<int>() {
                  0,
                  3,
                  1,
                  4,
                  2,
                  5,
                };
                var tri6Face = new List<int>() {
                  0,
                  1,
                  2,
                  3,
                };
                var meshGon = MeshNgon.Create(tri6Vert, tri6Face);

                outMesh.Ngons.AddNgon(meshGon);
                break;
              }
              case 8: {
                var ave = new Point3d {
                  X = 0,
                  Y = 0,
                  Z = 0,
                };
                for (int k = 0; k < topo.Count; k++) {
                  ave.X += outMesh.Vertices[k].X;
                  ave.Y += outMesh.Vertices[k].Y;
                  ave.Z += outMesh.Vertices[k].Z;
                }

                ave.X /= topo.Count;
                ave.Y /= topo.Count;
                ave.Z /= topo.Count;

                outMesh.Vertices.Add(ave);

                outMesh.Faces.AddFace(0, 4, 8);
                outMesh.Faces.AddFace(1, 8, 4);
                outMesh.Faces.AddFace(1, 5, 8);
                outMesh.Faces.AddFace(2, 8, 5);
                outMesh.Faces.AddFace(2, 6, 8);
                outMesh.Faces.AddFace(3, 8, 6);
                outMesh.Faces.AddFace(3, 7, 8);
                outMesh.Faces.AddFace(0, 8, 7);
                var quad8Vert = new List<int>() {
                  0,
                  4,
                  1,
                  5,
                  2,
                  6,
                  3,
                  7,
                };
                var quad8Face = new List<int>() {
                  0,
                  1,
                  2,
                  3,
                  4,
                  5,
                  6,
                  7,
                };

                var meshGon = MeshNgon.Create(quad8Vert, quad8Face);

                outMesh.Ngons.AddNgon(meshGon);
                break;
              }
            }
          }

          break;
        }
      }

      return outMesh;
    }

    internal static Mesh ConvertElement3D(
      Element element, ReadOnlyDictionary<int, Node> nodes, LengthUnit unit) {
      ReadOnlyCollection<int> topo = element.Topology;
      var check3d = new List<bool> {
        element.Type == ElementType.THREE_D,
        element.Type == ElementType.BRICK8,
        element.Type == ElementType.WEDGE6,
        element.Type == ElementType.PYRAMID5,
        element.Type == ElementType.TETRA4,
      };
      if (!check3d.Contains(true)) {
        return null;
      }

      var outMesh = new Mesh();

      foreach (int t in topo) {
        if (nodes.TryGetValue(t, out Node node)) {
          outMesh.Vertices.Add(Nodes.Point3dFromNode(node, unit));
        }
      }

      switch (topo.Count) {
        case 4:
          outMesh.Faces.AddFace(0, 2, 1); //bottom
          outMesh.Faces.AddFace(0, 1, 3); //side 1
          outMesh.Faces.AddFace(1, 2, 3); //side 2
          outMesh.Faces.AddFace(2, 0, 3); //side 3
          var verts4 = new List<int>() {
            0,
            1,
            2,
            3,
          };
          var faces4 = new List<int>() {
            0,
            1,
            2,
            3,
          };
          var meshGon4 = MeshNgon.Create(verts4, faces4);
          outMesh.Ngons.AddNgon(meshGon4);
          break;

        case 5:
          outMesh.Faces.AddFace(0, 3, 2, 1); //bottom
          outMesh.Faces.AddFace(0, 1, 4); //side 1
          outMesh.Faces.AddFace(1, 2, 4); //side 2
          outMesh.Faces.AddFace(2, 3, 4); //side 3
          outMesh.Faces.AddFace(3, 0, 4); //side 4
          var verts5 = new List<int>() {
            0,
            1,
            2,
            3,
            4,
          };
          var faces5 = new List<int>() {
            0,
            1,
            2,
            3,
            4,
          };
          var meshGon5 = MeshNgon.Create(verts5, faces5);
          outMesh.Ngons.AddNgon(meshGon5);
          break;

        case 6:
          outMesh.Faces.AddFace(0, 2, 1); //end1
          outMesh.Faces.AddFace(0, 3, 5, 2); //side 1
          outMesh.Faces.AddFace(1, 2, 5, 4); //side 2
          outMesh.Faces.AddFace(0, 1, 4, 3); //side 3
          outMesh.Faces.AddFace(3, 4, 5); //end 2
          var verts6 = new List<int>() {
            0,
            1,
            2,
            3,
            4,
            5,
          };
          var faces6 = new List<int>() {
            0,
            1,
            2,
            3,
            4,
          };
          var meshGon6 = MeshNgon.Create(verts6, faces6);
          outMesh.Ngons.AddNgon(meshGon6);
          break;

        case 8:
          outMesh.Faces.AddFace(0, 3, 2, 1); //bottom
          outMesh.Faces.AddFace(0, 1, 5, 4); //side 1
          outMesh.Faces.AddFace(1, 2, 6, 5); //side 2
          outMesh.Faces.AddFace(2, 3, 7, 6); //side 2
          outMesh.Faces.AddFace(3, 0, 4, 7); //side 3
          outMesh.Faces.AddFace(4, 5, 6, 7); //top
          var verts8 = new List<int>() {
            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,
          };
          var faces8 = new List<int>() {
            0,
            1,
            2,
            3,
            4,
            5,
          };
          var meshGon8 = MeshNgon.Create(verts8, faces8);
          outMesh.Ngons.AddNgon(meshGon8);
          break;

        default: return null;
      }

      return outMesh;
    }

    internal static ConcurrentBag<GsaElement2dGoo> ConvertToElement2Ds(
      ConcurrentDictionary<int, Element> elements, ReadOnlyDictionary<int, Node> nodes,
      ReadOnlyDictionary<int, Prop2D> properties,
      ReadOnlyDictionary<int, AnalysisMaterial> materials, ReadOnlyDictionary<int, Axis> axDict,
      LengthUnit unit, bool duplicateApiObjects) {
      var sortedElements = new ConcurrentDictionary<int, ConcurrentDictionary<int, Element>>();

      Parallel.ForEach(elements, elem => {
        int parent = -elem.Value.ParentMember.Member;
        if (parent == 0) {
          parent = elem.Value.Property;
        }

        if (!sortedElements.ContainsKey(parent)) {
          sortedElements.TryAdd(parent, new ConcurrentDictionary<int, Element>());
        }

        sortedElements[parent][elem.Key] = elem.Value;
      });

      var elem2dGoos = new ConcurrentBag<GsaElement2dGoo>();

      Parallel.For(0, sortedElements.Count, i => {
        int parentId = sortedElements.Keys.ElementAt(i);
        ConcurrentDictionary<int, Element> elems = sortedElements[parentId];
        var prop2Ds = new ConcurrentDictionary<int, GsaProp2d>();
        var mList = new ConcurrentDictionary<int, Mesh>();

        Parallel.For(0, elems.Count, j => {
          int elementId = elems.Keys.ElementAt(j);
          Mesh faceMesh = ConvertElement2D(elems[elementId], nodes, unit);
          if (faceMesh == null) {
            return;
          }

          mList[elementId] = faceMesh;

          var prop = new GsaProp2d(properties, elems[elementId].Property, materials, axDict, unit);
          prop2Ds.TryAdd(elementId, prop);
        });

        // create one large mesh from single mesh face using
        // append list of meshes (faster than appending each mesh one by one)
        var m = new Mesh();
        m.Append(mList.Values);

        // if parent member value is 0 then no parent member exist for element
        // we can therefore not be sure all elements with parent member = 0 are
        // connected in one mesh.
        if (parentId == 0 && m.DisjointMeshCount > 1) {
          // revert back to list of meshes instead of the joined one
          foreach (int key in elems.Keys) {
            // create new element from api-element, id, mesh (takes care of topology lists etc) and prop2d
            elems.TryGetValue(key, out Element apiElem);
            var apiElems = new ConcurrentDictionary<int, Element>();
            apiElems.TryAdd(key, apiElem);
            mList.TryGetValue(key, out Mesh mesh);
            prop2Ds.TryGetValue(key, out GsaProp2d prop);
            var propList = new List<GsaProp2d>() { 
              prop 
            };

            var singleelement2D = new GsaElement2d(apiElems, mesh, propList);
            elem2dGoos.Add(new GsaElement2dGoo(singleelement2D, duplicateApiObjects));
          }
        } else {
          // create new element from api-element, id, mesh (takes care of topology lists etc) and prop2d
          var element2D = new GsaElement2d(elems, m, prop2Ds.Values.ToList());

          elem2dGoos.Add(new GsaElement2dGoo(element2D, duplicateApiObjects));
        }
      });

      return elem2dGoos;
    }

    internal static ConcurrentBag<GsaElement3dGoo> ConvertToElement3Ds(
      ConcurrentDictionary<int, Element> elements, ReadOnlyDictionary<int, Node> nodes,
      ReadOnlyDictionary<int, Prop3D> properties,
      ReadOnlyDictionary<int, AnalysisMaterial> materials, LengthUnit unit,
      bool duplicateApiObjects) {
      // main sorted dictionary with
      // key = parent member
      // value = dictionary of elements belong to that parent:
      //    key = element id
      //    value = element
      var sortedElements = new ConcurrentDictionary<int, ConcurrentDictionary<int, Element>>();

      Parallel.ForEach(elements, elem => {
        int parent = -elem.Value.ParentMember.Member;

        // if no parent member then split by property
        if (parent == 0) {
          parent = elem.Value.Property;
        }

        if (!sortedElements.ContainsKey(parent)) {
          sortedElements.TryAdd(parent, new ConcurrentDictionary<int, Element>());
        }

        // add elements to parent member
        sortedElements[parent][elem.Key] = elem.Value;
      });

      var elem3dGoos = new ConcurrentBag<GsaElement3dGoo>();

      Parallel.For(0, sortedElements.Count, i => {
        int parentId = sortedElements.Keys.ElementAt(i);

        ConcurrentDictionary<int, Element> elems = sortedElements[parentId];
        var prop3Ds = new ConcurrentDictionary<int, GsaProp3d>();
        var mList = new ConcurrentDictionary<int, Mesh>();

        Parallel.For(0, elems.Count, j => {
          int elementId = elems.Keys.ElementAt(j);
          Mesh ngonClosedMesh = ConvertElement3D(elems[elementId], nodes, unit);
          if (ngonClosedMesh == null) {
            return;
          }

          mList[elementId] = ngonClosedMesh;

          var prop = new GsaProp3d(properties, elems[elementId].Property, materials);
          prop3Ds.TryAdd(elementId, prop);
        });

        // create one large mesh from single mesh face using
        // append list of meshes (faster than appending each mesh one by one)
        var m = new Mesh();
        m.Append(mList.Values.ToList());

        // if parent member value is 0 then no parent member exist for element
        // we can therefore not be sure all elements with parent member = 0 are
        // connected in one mesh.
        if (parentId == 0 && m.DisjointMeshCount > 1) {
          // revert back to list of meshes instead of the joined one
          foreach (int key in elems.Keys) {
            // create new element from api-element, id, mesh (takes care of topology lists etc) and prop2d
            elems.TryGetValue(key, out Element apiElem);
            mList.TryGetValue(key, out Mesh mesh);
            prop3Ds.TryGetValue(key, out GsaProp3d prop);

            var singleelement3D = new GsaElement3d(apiElem, key, mesh, prop);
            elem3dGoos.Add(new GsaElement3dGoo(singleelement3D, duplicateApiObjects));
          }
        } else {
          // create new element from api-element, id, mesh (takes care of topology lists etc) and prop2d
          var element3D = new GsaElement3d(elems, m, prop3Ds.Values.ToList());
          elem3dGoos.Add(new GsaElement3dGoo(element3D, duplicateApiObjects));
        }
      });
      return elem3dGoos;
    }

    internal static
      (ConcurrentBag<GsaElement1dGoo> e1d, ConcurrentBag<GsaElement2dGoo> e2d,
      ConcurrentBag<GsaElement3dGoo> e3d) GetElements(
        ReadOnlyDictionary<int, Element> eDict, ReadOnlyDictionary<int, Node> nDict,
        ReadOnlyDictionary<int, Section> sDict, ReadOnlyDictionary<int, Prop2D> pDict,
        ReadOnlyDictionary<int, Prop3D> p3Dict, ReadOnlyDictionary<int, AnalysisMaterial> mDict,
        ReadOnlyDictionary<int, SectionModifier> modDict,
        Dictionary<int, ReadOnlyCollection<double>> localAxesDict,
        ReadOnlyDictionary<int, Axis> axDict, LengthUnit modelUnit, bool duplicateApiObjects) {
      var elem1ds = new ConcurrentBag<GsaElement1dGoo>();
      var elem2dDict = new ConcurrentDictionary<int, Element>();
      var elem3dDict = new ConcurrentDictionary<int, Element>();

      Parallel.ForEach(eDict, item => {
        int elemDimension = 1; // default assume 1D element
        ElementType type = item.Value.Type;
        switch (type) {
          case ElementType.TRI3:
          case ElementType.TRI6:
          case ElementType.QUAD4:
          case ElementType.QUAD8:
          case ElementType.TWO_D:
          case ElementType.TWO_D_FE:
          case ElementType.TWO_D_LOAD:
            elemDimension = 2;
            break;

          case ElementType.BRICK8:
          case ElementType.WEDGE6:
          case ElementType.PYRAMID5:
          case ElementType.TETRA4:
          case ElementType.THREE_D:
            elemDimension = 3;
            break;
        }

        switch (elemDimension) {
          case 1:
            elem1ds.Add(new GsaElement1dGoo(
              new GsaElement1d(eDict, item.Key, nDict, sDict, modDict, mDict, localAxesDict,
                modelUnit), duplicateApiObjects));
            break;

          case 2:
            elem2dDict.TryAdd(item.Key, item.Value);
            break;

          case 3:
            elem3dDict.TryAdd(item.Key, item.Value);
            break;
        }
      });

      var elem2ds = new ConcurrentBag<GsaElement2dGoo>();
      if (elem2dDict.Count > 0) {
        elem2ds = ConvertToElement2Ds(elem2dDict, nDict, pDict, mDict, axDict, modelUnit,
          duplicateApiObjects);
      }

      var elem3ds = new ConcurrentBag<GsaElement3dGoo>();
      if (elem3dDict.Count > 0) {
        elem3ds = ConvertToElement3Ds(elem3dDict, nDict, p3Dict, mDict, modelUnit,
          duplicateApiObjects);
      }

      return (elem1ds, elem2ds, elem3ds);
    }
  }
}
