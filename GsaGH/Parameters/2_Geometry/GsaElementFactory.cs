using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using GsaAPI;

using GsaGH.Helpers.Import;

using Rhino.Geometry;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters {
  public class GsaElementFactory {
    internal static ConcurrentBag<GsaElement1dGoo> CreateElement1dFromApi(
  ConcurrentDictionary<int, GSAElement> elements, GsaModel model) {
      var elem1dGoos = new ConcurrentBag<GsaElement1dGoo>();
      Parallel.ForEach(elements, item => {
        if (item.Value.Type == ElementType.SPRING) {
          GsaSpringProperty springProperty = model.GetSpringProperty(item.Value);
          var elem = new GsaElement1d(
            item, model.ApiNodes, springProperty, model.ApiElementLocalAxes[item.Key], model.ModelUnit);
          elem1dGoos.Add(new GsaElement1dGoo(elem));
        } else {
          GsaSection section = model.GetSection(item.Value);
          var elem = new GsaElement1d(
            item, model.ApiNodes, section, model.ApiElementLocalAxes[item.Key], model.ModelUnit);
          elem1dGoos.Add(new GsaElement1dGoo(elem));
        }
      });
      return elem1dGoos;
    }

    internal static ConcurrentBag<GsaElement2dGoo> CreateLoadPanelElementFromApi(
  ConcurrentDictionary<int, GSAElement> elements, GsaModel model) {
      ReadOnlyDictionary<int, Node> nodes = model.ApiNodes;
      var elem2dGoos = new ConcurrentBag<GsaElement2dGoo>();
      Parallel.ForEach(elements, elem => {
        if(elem.Value.IsLoadPanel) {
          GsaProperty2d prop2d = model.GetProp2d(elem.Value);
          Curve polyline = GetPolylineFromApiElement2d(elem.Value, nodes, model.ModelUnit);
          var element2D = new GsaElement2d(elem.Key, elem.Value, polyline, prop2d, model.ModelUnit);
          elem2dGoos.Add(new GsaElement2dGoo(element2D));
        }
      });

      return elem2dGoos;
    }

    internal static ConcurrentBag<GsaElement2dGoo> CreateElement2dFromApi(
    ConcurrentDictionary<int, GSAElement> elements, GsaModel model) {
      ReadOnlyDictionary<int, Node> nodes = model.ApiNodes;
      var sortedElements = new ConcurrentDictionary<int, ConcurrentDictionary<int, GSAElement>>();
      Parallel.ForEach(elements, elem => {
        if (!elem.Value.IsLoadPanel) {
          int parent = -elem.Value.ParentMember.Member;
          if (parent == 0) {
            parent = elem.Value.Property;
          }

          if (!sortedElements.ContainsKey(parent)) {
            sortedElements.TryAdd(parent, new ConcurrentDictionary<int, GSAElement>());
          }

          sortedElements[parent][elem.Key] = elem.Value;
        }
      });

      var elem2dGoos = new ConcurrentBag<GsaElement2dGoo>();

      Parallel.For(0, sortedElements.Count, i => {
        int parentId = sortedElements.Keys.ElementAt(i);
        ConcurrentDictionary<int, GSAElement> elems = sortedElements[parentId];
        var prop2Ds = new ConcurrentDictionary<int, GsaProperty2d>();
        var mList = new ConcurrentDictionary<int, Mesh>();
        Parallel.For(0, elems.Count, j => {
          int elementId = elems.Keys.ElementAt(j);
          Mesh faceMesh = GetMeshFromApiElement2d(elems[elementId], nodes, model.ModelUnit);
          if (faceMesh == null) {
            return;
          }
          mList[elementId] = faceMesh;
          GsaProperty2d prop2d = model.GetProp2d(elems[elementId]);
          if (prop2d != null) {
            prop2Ds.TryAdd(elementId, model.GetProp2d(elems[elementId]));
          }
        });

        // create one large mesh from single mesh face using
        // append list of meshes (faster than appending each mesh one by one)
        var m = new Mesh();
        m.Append(mList.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value));

        // if parent member value is 0 then no parent member exist for element
        // we can therefore not be sure all elements with parent member = 0 are
        // connected in one mesh.
        if (parentId == 0 && m.DisjointMeshCount > 1) {
          // revert back to list of meshes instead of the joined one
          foreach (int key in elems.Keys) {
            // create new element from api-element, id, mesh (takes care of topology lists etc) and prop2d
            elems.TryGetValue(key, out GSAElement apiElem);
            var apiElems = new ConcurrentDictionary<int, GSAElement>();
            apiElems.TryAdd(key, apiElem);
            mList.TryGetValue(key, out Mesh mesh);
            prop2Ds.TryGetValue(key, out GsaProperty2d prop);
            var propList = new ConcurrentDictionary<int, GsaProperty2d>();
            propList.TryAdd(key, prop);
            var singleelement2D = new GsaElement2d(apiElems, mesh, propList,model.ModelUnit);
            elem2dGoos.Add(new GsaElement2dGoo(singleelement2D));
          }
        } else {
          // create new element from api-element, id, mesh (takes care of topology lists etc) and prop2d
          var element2D = new GsaElement2d(elems, m, prop2Ds, model.ModelUnit);

          elem2dGoos.Add(new GsaElement2dGoo(element2D));
        }
      });
      return new ConcurrentBag<GsaElement2dGoo>(elem2dGoos.Union(CreateLoadPanelElementFromApi(elements, model)));
    }

    internal static ConcurrentBag<GsaElement3dGoo> CreateElement3dFromApi(
      ConcurrentDictionary<int, GSAElement> elements, GsaModel model) {
      ReadOnlyDictionary<int, Node> nodes = model.ApiModel.Nodes();
      ReadOnlyDictionary<int, Axis> axDict = model.ApiModel.Axes();

      var sortedElements = new ConcurrentDictionary<int, ConcurrentDictionary<int, GSAElement>>();
      Parallel.ForEach(elements, elem => {
        int parent = -elem.Value.ParentMember.Member;

        // if no parent member then split by property
        if (parent == 0) {
          parent = elem.Value.Property;
        }

        if (!sortedElements.ContainsKey(parent)) {
          sortedElements.TryAdd(parent, new ConcurrentDictionary<int, GSAElement>());
        }

        // add elements to parent member
        sortedElements[parent][elem.Key] = elem.Value;
      });

      var elem3dGoos = new ConcurrentBag<GsaElement3dGoo>();

      Parallel.For(0, sortedElements.Count, i => {
        int parentId = sortedElements.Keys.ElementAt(i);

        ConcurrentDictionary<int, GSAElement> elems = sortedElements[parentId];
        var prop3Ds = new ConcurrentDictionary<int, GsaProperty3d>();
        var mList = new ConcurrentDictionary<int, Mesh>();

        Parallel.For(0, elems.Count, j => {
          int elementId = elems.Keys.ElementAt(j);
          Mesh ngonClosedMesh = GetMeshFromApiElement3d(elems[elementId], nodes, model.ModelUnit);
          if (ngonClosedMesh == null) {
            return;
          }

          mList[elementId] = ngonClosedMesh;

          GsaProperty3d prop3d = model.GetProp3d(elems[elementId]);
          if (prop3d != null) {
            prop3Ds.TryAdd(elementId, prop3d);
          }
        });

        // create one large mesh from single mesh face using
        // append list of meshes (faster than appending each mesh one by one)
        var m = new Mesh();
        m.Append(mList.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value));

        // if parent member value is 0 then no parent member exist for element
        // we can therefore not be sure all elements with parent member = 0 are
        // connected in one mesh.
        if (parentId == 0 && m.DisjointMeshCount > 1) {
          // revert back to list of meshes instead of the joined one
          foreach (int key in elems.Keys) {
            // create new element from api-element, id, mesh (takes care of topology lists etc) and prop2d
            elems.TryGetValue(key, out GSAElement apiElem);
            var apiElems = new ConcurrentDictionary<int, GSAElement>();
            apiElems.TryAdd(key, apiElem);
            mList.TryGetValue(key, out Mesh mesh);
            prop3Ds.TryGetValue(key, out GsaProperty3d prop);
            var propList = new ConcurrentDictionary<int, GsaProperty3d>();
            propList.TryAdd(key, prop);
            var singleelement3D = new GsaElement3d(apiElems, mesh, propList, model.ModelUnit);
            elem3dGoos.Add(new GsaElement3dGoo(singleelement3D));
          }
        } else {
          // create new element from api-element, id, mesh (takes care of topology lists etc) and prop2d
          var element3D = new GsaElement3d(elems, m, prop3Ds, model.ModelUnit);
          elem3dGoos.Add(new GsaElement3dGoo(element3D));
        }
      });
      return elem3dGoos;
    }

    internal static Mesh GetMeshFromApiElement2d(GSAElement element, ReadOnlyDictionary<int, Node> nodes, LengthUnit unit) {
      ReadOnlyCollection<int> topo = element.Topology;
      if (topo.Count < 3) {
        return null;
      }

      if (!element.IsLoadPanel) {
        if (element.Type == ElementType.BRICK8 || element.Type == ElementType.WEDGE6
        || element.Type == ElementType.PYRAMID5 || element.Type == ElementType.TETRA4) {
          return null;
        }
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
                      0, 3, 1, 4, 2, 5,
                    };
                    var tri6Face = new List<int>() {
                      0, 1, 2, 3,
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
                      0, 4, 1, 5, 2, 6, 3, 7,
                    };
                    var quad8Face = new List<int>() {
                      0, 1, 2, 3, 4, 5, 6, 7,
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

    internal static Curve GetPolylineFromApiElement2d(GSAElement element, ReadOnlyDictionary<int, Node> nodes, LengthUnit unit) {
      ReadOnlyCollection<int> topo = element.Topology;
      if (topo.Count < 3) {
        return null;
      }
      var points = new List<Point3d>();
      foreach (int t in topo) {
        if (nodes.TryGetValue(t, out Node node)) {
          points.Add(Nodes.Point3dFromNode(node, unit));
        }
      }
      points.Add(points[0]);

      return new PolylineCurve(points);
    }

    internal static Mesh GetMeshFromApiElement3d(
      GSAElement element, ReadOnlyDictionary<int, Node> nodes, LengthUnit unit) {
      ReadOnlyCollection<int> topo = element.Topology;
      var check3d = new List<bool> {
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
            0, 1, 2, 3,
          };
          var faces4 = new List<int>() {
            0, 1, 2, 3,
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
            0, 1, 2, 3, 4,
          };
          var faces5 = new List<int>() {
            0, 1, 2, 3, 4,
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
            0, 1, 2, 3, 4, 5,
          };
          var faces6 = new List<int>() {
            0, 1, 2, 3, 4,
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
            0, 1, 2, 3, 4, 5, 6, 7,
          };
          var faces8 = new List<int>() {
            0, 1, 2, 3, 4, 5,
          };
          var meshGon8 = MeshNgon.Create(verts8, faces8);
          outMesh.Ngons.AddNgon(meshGon8);
          break;

        default: return null;
      }

      return outMesh;
    }
  }
}
