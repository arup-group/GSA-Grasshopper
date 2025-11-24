using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Grasshopper.Kernel;

using GsaAPI;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;

using Rhino.Collections;
using Rhino.Geometry;

namespace GsaGH.Helpers.Import {
  internal class Members {
    internal ConcurrentBag<GsaMember1dGoo> Member1ds { get; private set; }
    internal ConcurrentBag<GsaMember2dGoo> Member2ds { get; private set; }
    internal ConcurrentBag<GsaMember3dGoo> Member3ds { get; private set; }

    internal Members(GsaModel model, GH_Component owner, string memberList = "All") {
      Member1ds = new ConcurrentBag<GsaMember1dGoo>();
      Member2ds = new ConcurrentBag<GsaMember2dGoo>();
      Member3ds = new ConcurrentBag<GsaMember3dGoo>();

      ReadOnlyDictionary<int, Member> mDict = model.ApiModel.Members(memberList);

      var errors1d = new ConcurrentBag<int>();
      var errors2d = new ConcurrentBag<int>();

      Parallel.ForEach(mDict, item => {
        string toporg = item.Value.Topology;

        if (item.Value.Type != MemberType.GENERIC_3D) {
          (Tuple<List<int>, List<string>> item1,
            Tuple<List<List<int>>, List<List<string>>> voidTuple,
            Tuple<List<List<int>>, List<List<string>>> lineTuple,
            List<int> inclpts) = Topology.Topology_detangler(toporg);
          (List<int> topoInt, List<string> topoType) = item1;

          var topopts = new Point3dList();
          foreach (int t in topoInt) {
            if (model.ApiNodes.TryGetValue(t, out Node node)) {
              topopts.Add(Nodes.Point3dFromNode(node, model.ModelUnit));
            } else {
              return; ; // if node cannot be found continue with next key
            }
          }

          switch (item.Value.Type) {
            // ### Member 1d ###
            case MemberType.GENERIC_1D:
            case MemberType.BEAM:
            case MemberType.CANTILEVER:
            case MemberType.COLUMN:
            case MemberType.COMPOS:
            case MemberType.PILE:

              if (topopts.Count < 2) {
                errors1d.Add(item.Key);
                return;
              }

              GsaNode orientationNode = null;
              if (model.ApiNodes.Keys.Contains(item.Value.OrientationNode)) {
                orientationNode = new GsaNode(Nodes.Point3dFromNode(
                  model.ApiNodes[item.Value.OrientationNode], model.ModelUnit));
              }

              var mem1d = new GsaMember1D(
                item, topopts, topoType, model.ApiMemberLocalAxes[item.Key], orientationNode, model.ModelUnit) {
                Section = model.GetSection(item.Value)
              };
              Member1ds.Add(new GsaMember1dGoo(mem1d));
              break;

            default:
              // ### Member 2d ###
              if (topopts.Count < 2) {
                errors2d.Add(item.Key);
                return;
              }

              List<List<int>> voidTopoInt = voidTuple.Item1;
              //list of polyline curve type (arch or line) for void /member2d
              List<List<string>> voidTopoType = voidTuple.Item2;
              List<List<int>> incLinesTopoInt = lineTuple.Item1;
              //list of polyline curve type (arch or line) for inclusion /member2d
              List<List<string>> inclLinesTopoType = lineTuple.Item2;

              //list of lists of void points /member2d
              var voidTopo = new List<Point3dList>();
              for (int i = 0; i < voidTopoInt.Count; i++) {
                voidTopo.Add(new Point3dList());
                for (int j = 0; j < voidTopoInt[i].Count; j++) {
                  if (model.ApiNodes.TryGetValue(voidTopoInt[i][j], out Node node)) {
                    voidTopo[i].Add(Nodes.Point3dFromNode(node, model.ModelUnit));
                  }
                }
              }

              //list of lists of line inclusion topology points /member2d
              var incLinesTopo = new List<Point3dList>();
              for (int i = 0; i < incLinesTopoInt.Count; i++) {
                incLinesTopo.Add(new Point3dList());
                for (int j = 0; j < incLinesTopoInt[i].Count; j++) {
                  if (model.ApiNodes.TryGetValue(incLinesTopoInt[i][j], out Node node)) {
                    incLinesTopo[i].Add(Nodes.Point3dFromNode(node, model.ModelUnit));
                  }
                }
              }

              //list of points for inclusion /member2d
              var inclPts = new Point3dList();
              foreach (int point in inclpts) {
                if (!model.ApiNodes.TryGetValue(point, out Node node)) {
                  continue;
                }

                inclPts.Add(Nodes.Point3dFromNode(node, model.ModelUnit));
              }

              GsaProperty2d prop2d = model.GetProp2d(item.Value);
              var mem2d = new GsaMember2D(item, topopts, topoType, voidTopo, voidTopoType,
                incLinesTopo, inclLinesTopoType, inclPts, prop2d, model.ModelUnit);
              Member2ds.Add(new GsaMember2dGoo(mem2d));
              break;
          }
        } else {
          // ### Member 3d ###
          List<List<int>> topints = Topology.Topology_detangler_Mem3d(toporg);
          var mList = new List<Mesh>();
          foreach (List<int> ints in topints) {
            var tempMesh = new Mesh();
            foreach (int t in ints) {
              if (model.ApiNodes.TryGetValue(t, out Node node)) {
                Vector3 p = node.Position;
                tempMesh.Vertices.Add(Nodes.Point3dFromNode(node, model.ModelUnit));
              } else {
                return;
              }
            }

            tempMesh.Faces.AddFace(0, 1, 2);
            mList.Add(tempMesh);
          }

          var m = new Mesh();
          // create one large mesh from single mesh face using
          // append list of meshes (faster than appending each mesh one by one)
          m.Append(mList);

          GsaProperty3d prop = model.GetProp3d(item.Value);
          var mem3d = new GsaMember3D(item.Value, item.Key, m, prop, model.ModelUnit);
          Member3ds.Add(new GsaMember3dGoo(mem3d));
        }
      });

      if (errors1d.Count > 0) {
        string ids = string.Join(Environment.NewLine, errors1d.OrderBy(x => x));
        string err = $" Invalid definition for 1D Member ID(s):{Environment.NewLine}{ids}";
        owner.AddRuntimeWarning(err);
      }

      if (errors2d.Count > 0) {
        string ids = string.Join(Environment.NewLine, errors2d.OrderBy(x => x));
        string err = $" Invalid definition for 2D Member ID(s):{Environment.NewLine}{ids}";
        owner.AddRuntimeWarning(err);
      }
    }
  }
}
