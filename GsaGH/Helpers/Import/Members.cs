using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysUnits;
using Rhino.Geometry;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Helpers.Import {
  internal class Members {
    internal ConcurrentBag<GsaMember1dGoo> Member1ds { get; private set; }
    internal ConcurrentBag<GsaMember2dGoo> Member2ds { get; private set; }
    internal ConcurrentBag<GsaMember3dGoo> Member3ds { get; private set; }

    internal Members(GsaModel model, string memberList = "All", GH_Component owner = null) {
      Member1ds = new ConcurrentBag<GsaMember1dGoo>();
      Member2ds = new ConcurrentBag<GsaMember2dGoo>();
      Member3ds = new ConcurrentBag<GsaMember3dGoo>();

      ReadOnlyDictionary<int, Member> mDict = model.Model.Members(memberList);

      Parallel.ForEach(mDict, item => {
        string toporg = item.Value.Topology;

        if (item.Value.Type != MemberType.GENERIC_3D) {
          (Tuple<List<int>, List<string>> item1,
            Tuple<List<List<int>>, List<List<string>>> voidTuple,
            Tuple<List<List<int>>, List<List<string>>> lineTuple,
            List<int> inclpts)
              = Topology.Topology_detangler(toporg);
          (List<int> topoInt, List<string> topoType) = item1;

          var topopts = new List<Point3d>();
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
                string error = " Invalid topology Mem1D ID: " + item.Key + ".";
                owner?.AddRuntimeWarning(error);
                return;
              }

              GsaSection section = model.Properties.GetSection(item.Value);
              var mem1d = new GsaMember1d(
                item, topopts, topoType, model.ApiMemberLocalAxes[item.Key], section, model.ModelUnit);
              Member1ds.Add(new GsaMember1dGoo(mem1d));
              break;

            default: 
              // ### Member 2d ###
              if (topopts.Count < 2) {
                string error = " Invalid topology Mem2D ID: " + item.Key + ".";
                owner?.AddRuntimeWarning(error);
                return;
              }

              List<List<int>> voidTopoInt = voidTuple.Item1;
              //list of polyline curve type (arch or line) for void /member2d
              List<List<string>> voidTopoType = voidTuple.Item2;
              List<List<int>> incLinesTopoInt = lineTuple.Item1;
              //list of polyline curve type (arch or line) for inclusion /member2d
              List<List<string>> inclLinesTopoType = lineTuple.Item2;

              //list of lists of void points /member2d
              var voidTopo = new List<List<Point3d>>();
              for (int i = 0; i < voidTopoInt.Count; i++) {
                voidTopo.Add(new List<Point3d>());
                for (int j = 0; j < voidTopoInt[i].Count; j++) {
                  if (model.ApiNodes.TryGetValue(voidTopoInt[i][j], out Node node)) {
                    voidTopo[i].Add(Nodes.Point3dFromNode(node, model.ModelUnit));
                  }
                }
              }

              //list of lists of line inclusion topology points /member2d
              var incLinesTopo = new List<List<Point3d>>();
              for (int i = 0; i < incLinesTopoInt.Count; i++) {
                incLinesTopo.Add(new List<Point3d>());
                for (int j = 0; j < incLinesTopoInt[i].Count; j++) {
                  if (model.ApiNodes.TryGetValue(incLinesTopoInt[i][j], out Node node)) {
                    incLinesTopo[i].Add(Nodes.Point3dFromNode(node, model.ModelUnit));
                  }
                }
              }

              //list of points for inclusion /member2d
              var inclPts = new List<Point3d>();
              foreach (int point in inclpts) {
                if (!model.ApiNodes.TryGetValue(point, out Node node)) {
                  continue;
                }

                inclPts.Add(Nodes.Point3dFromNode(node, model.ModelUnit));
              }

              GsaProperty2d prop2d = model.Properties.GetProp2d(item.Value);
              var mem2d = new GsaMember2d(item, topopts, topoType, voidTopo, voidTopoType,
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

          GsaProperty3d prop = model.Properties.GetProp3d(item.Value);
          var mem3d = new GsaMember3d(item.Value, item.Key, m, prop,
            new Length(item.Value.MeshSize, LengthUnit.Meter).As(model.ModelUnit));
          Member3ds.Add(new GsaMember3dGoo(mem3d));
        }
      });
    }
  }
}
