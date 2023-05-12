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
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Helpers.Import {
  /// <summary>
  ///   Class containing functions to import various object types from GSA
  /// </summary>
  internal class Members {

    internal static
      (ConcurrentBag<GsaMember1dGoo> m1d, ConcurrentBag<GsaMember2dGoo> m2d,
      ConcurrentBag<GsaMember3dGoo> m3d) GetMembers(
        ReadOnlyDictionary<int, Member> mDict, ReadOnlyDictionary<int, Node> nDict,
        ReadOnlyDictionary<int, Section> sDict, ReadOnlyDictionary<int, Prop2D> pDict,
        ReadOnlyDictionary<int, Prop3D> p3Dict, ReadOnlyDictionary<int, AnalysisMaterial> matDict,
        ReadOnlyDictionary<int, SectionModifier> modDict,
        Dictionary<int, ReadOnlyCollection<double>> localAxesDict,
        ReadOnlyDictionary<int, Axis> axDict, LengthUnit modelUnit, bool duplicateApiObjects,
        GH_Component owner = null) {
      var mem1ds = new ConcurrentBag<GsaMember1dGoo>();
      var mem2ds = new ConcurrentBag<GsaMember2dGoo>();
      var mem3ds = new ConcurrentBag<GsaMember3dGoo>();

      Parallel.ForEach(mDict.Keys, key => {
        if (!mDict.TryGetValue(key, out Member member)) {
          return;
        }

        Member mem = member;
        string toporg = mem.Topology;

        if (mem.Type == MemberType.GENERIC_3D) {
          List<List<int>> topints = Topology.Topology_detangler_Mem3d(toporg);
          var mList = new List<Mesh>();
          foreach (List<int> ints in topints) {
            var tempMesh = new Mesh();
            foreach (int t in ints) {
              if (nDict.TryGetValue(t, out Node node)) {
                Vector3 p = node.Position;
                tempMesh.Vertices.Add(Nodes.Point3dFromNode(node, modelUnit));
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

          int propId = mem.Property;
          var prop = new GsaProp3d(p3Dict, mem.Property, matDict);

          var mem3d = new GsaMember3d(mem, key, m, prop,
            new Length(mem.MeshSize, LengthUnit.Meter).As(modelUnit));
          mem3ds.Add(new GsaMember3dGoo(mem3d, duplicateApiObjects));
        } else // ## Member1D or Member2D ##
        {
          (Tuple<List<int>, List<string>> item1,
              Tuple<List<List<int>>, List<List<string>>> voidTuple,
              Tuple<List<List<int>>, List<List<string>>> lineTuple, List<int> inclpts)
            = Topology.Topology_detangler(toporg);
          (List<int> topoInt, List<string> topoType) = item1;
          var topopts = new List<Point3d>();
          bool invalidNode = false;
          foreach (int t in topoInt) {
            if (nDict.TryGetValue(t, out Node node)) {
              topopts.Add(Nodes.Point3dFromNode(node, modelUnit));
            } else {
              invalidNode = true; // if node cannot be found continue with next key
            }
          }

          if (invalidNode) {
            return;
          }

          if ((mem.Type == MemberType.GENERIC_1D) | (mem.Type == MemberType.BEAM)
            | (mem.Type == MemberType.CANTILEVER) | (mem.Type == MemberType.COLUMN)
            | (mem.Type == MemberType.COMPOS) | (mem.Type == MemberType.PILE)) {
            if (topopts.Count < 2) {
              string error = " Invalid topology Mem1D ID: " + key + ".";
              owner?.AddRuntimeWarning(error);
              return;
            }

            var mem1d = new GsaMember1d(mem, key, topopts.ToList(), topoType.ToList(), sDict,
              modDict, matDict, localAxesDict, modelUnit);
            mem1ds.Add(new GsaMember1dGoo(mem1d, duplicateApiObjects));
          } else {
            if (topopts.Count < 2) {
              string error = " Invalid topology Mem2D ID: " + key + ".";
              owner?.AddRuntimeWarning(error);
              return;
            }

            List<List<int>> voidTopoInt = voidTuple.Item1;
            List<List<string>> voidTopoType
              = voidTuple.Item2; //list of polyline curve type (arch or line) for void /member2d
            List<List<int>> incLinesTopoInt = lineTuple.Item1;
            List<List<string>> inclLinesTopoType
              = lineTuple
               .Item2; //list of polyline curve type (arch or line) for inclusion /member2d

            //list of lists of void points /member2d
            var voidTopo = new List<List<Point3d>>();
            for (int i = 0; i < voidTopoInt.Count; i++) {
              voidTopo.Add(new List<Point3d>());
              for (int j = 0; j < voidTopoInt[i].Count; j++) {
                if (nDict.TryGetValue(voidTopoInt[i][j], out Node node)) {
                  voidTopo[i].Add(Nodes.Point3dFromNode(node, modelUnit));
                }
              }
            }

            //list of lists of line inclusion topology points /member2d
            var incLinesTopo = new List<List<Point3d>>();
            for (int i = 0; i < incLinesTopoInt.Count; i++) {
              incLinesTopo.Add(new List<Point3d>());
              for (int j = 0; j < incLinesTopoInt[i].Count; j++) {
                if (nDict.TryGetValue(incLinesTopoInt[i][j], out Node node)) {
                  incLinesTopo[i].Add(Nodes.Point3dFromNode(node, modelUnit));
                }
              }
            }

            //list of points for inclusion /member2d
            var inclPts = new List<Point3d>();
            foreach (int point in inclpts) {
              if (!nDict.TryGetValue(point, out Node node)) {
                continue;
              }

              inclPts.Add(Nodes.Point3dFromNode(node, modelUnit));
            }

            var mem2d = new GsaMember2d(mem, key, topopts, topoType, voidTopo, voidTopoType,
              incLinesTopo, inclLinesTopoType, inclPts, pDict, matDict, axDict, modelUnit);
            mem2ds.Add(new GsaMember2dGoo(mem2d, duplicateApiObjects));
          }
        }
      });

      return (mem1ds, mem2ds, mem3ds);
    }
  }
}
