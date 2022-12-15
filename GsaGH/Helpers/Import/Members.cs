using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits.Units;
using OasysUnits;
using Rhino.Geometry;
using System.Collections.ObjectModel;

namespace GsaGH.Helpers.Import
{
  /// <summary>
  /// Class containing functions to import various object types from GSA
  /// </summary>
  internal class Members
  {
    internal static Tuple<ConcurrentBag<GsaMember1dGoo>, ConcurrentBag<GsaMember2dGoo>, ConcurrentBag<GsaMember3dGoo>>
        GetMembers(ReadOnlyDictionary<int, Member> mDict, ReadOnlyDictionary<int, Node> nDict,
        ReadOnlyDictionary<int, Section> sDict, ReadOnlyDictionary<int, Prop2D> pDict, ReadOnlyDictionary<int, Prop3D> p3Dict,
        ReadOnlyDictionary<int, AnalysisMaterial> matDict, ReadOnlyDictionary<int, SectionModifier> modDict, Dictionary<int, ReadOnlyCollection<double>> localAxesDict, LengthUnit modelUnit, bool duplicateApiObjects, GH_Component owner = null)
    {
      ConcurrentBag<GsaMember1dGoo> mem1ds = new ConcurrentBag<GsaMember1dGoo>();
      ConcurrentBag<GsaMember2dGoo> mem2ds = new ConcurrentBag<GsaMember2dGoo>();
      ConcurrentBag<GsaMember3dGoo> mem3ds = new ConcurrentBag<GsaMember3dGoo>();

      Parallel.ForEach(mDict.Keys, key =>
      {
        if (mDict.TryGetValue(key, out Member member))
        {
          Member mem = member;
          string toporg = mem.Topology;

          // ## Member 3D ##
          // if 3D member we have different method:
          if (mem.Type == MemberType.GENERIC_3D)
          {
            List<List<int>> topints = Topology.Topology_detangler_Mem3d(toporg);
            List<Mesh> mList = new List<Mesh>();
            for (int i = 0; i < topints.Count; i++)
            {
              Mesh tempMesh = new Mesh();
              // Get verticies:
              for (int j = 0; j < topints[i].Count; j++)
              {
                if (nDict.TryGetValue(topints[i][j], out Node node))
                {
                  var p = node.Position;
                  tempMesh.Vertices.Add(Nodes.Point3dFromNode(node, modelUnit));
                }
                else
                  return;
              }
              tempMesh.Faces.AddFace(0, 1, 2); // all faces in Mem3d are Tri
              mList.Add(tempMesh);
            }
            Mesh m = new Mesh();
            // create one large mesh from single mesh face using
            // append list of meshes (faster than appending each mesh one by one)
            m.Append(mList);

            // create prop
            int propID = mem.Property;
            GsaProp3d prop = new GsaProp3d(p3Dict, mem.Property, matDict);

            GsaMember3d mem3d = new GsaMember3d(mem, key, m, prop, new Length(mem.MeshSize, LengthUnit.Meter).As(modelUnit));
            mem3ds.Add(new GsaMember3dGoo(mem3d, duplicateApiObjects));
          }
          else // ## Member1D or Member2D ##
          {
            // Build topology lists:
            Tuple<Tuple<List<int>, List<string>>, Tuple<List<List<int>>, List<List<string>>>,
                  Tuple<List<List<int>>, List<List<string>>>, List<int>> topologyTuple = Topology.Topology_detangler(toporg);
            // tuple of int and string describing the outline topology
            Tuple<List<int>, List<string>> topoTuple = topologyTuple.Item1;
            List<int> topo_int = topoTuple.Item1;
            List<string> topoType = topoTuple.Item2; //list of polyline curve type (arch or line) for member1d/2d
            // replace topology integers with actual points
            List<Point3d> topopts = new List<Point3d>(); // list of topology points for visualisation /member1d/member2d
            bool invalid_node = false;
            for (int i = 0; i < topo_int.Count; i++)
            {
              if (nDict.TryGetValue(topo_int[i], out Node node))
                topopts.Add(Nodes.Point3dFromNode(node, modelUnit));
              else
                invalid_node = true; // if node cannot be found continue with next key
            }
            if (invalid_node)
              return;

            // Member1D:
            if (mem.Type == MemberType.GENERIC_1D | mem.Type == MemberType.BEAM | mem.Type == MemberType.CANTILEVER |
                mem.Type == MemberType.COLUMN | mem.Type == MemberType.COMPOS | mem.Type == MemberType.PILE)
            {

              // check if Mem1D topology has minimum 2 points
              if (topopts.Count < 2)
              {
                string error = " Invalid topology Mem1D ID: " + key + ".";
                if (owner != null)
                  owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, error);
                return;
              }

              GsaMember1d mem1d = new GsaMember1d(mem, key, topopts.ToList(), topoType.ToList(), nDict, sDict, modDict, matDict, localAxesDict, modelUnit);
              mem1ds.Add(new GsaMember1dGoo(mem1d, duplicateApiObjects));
            }
            else
            {
              // check if Mem2D topology has minimum 3 points
              if (topopts.Count < 2)
              {
                string error = " Invalid topology Mem2D ID: " + key + ".";
                if (owner != null)
                  owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, error);
                return;
              }

              // tuple of list of int and strings describing voids topology (one list for each void)
              Tuple<List<List<int>>, List<List<string>>> voidTuple = topologyTuple.Item2;
              // tuple of list of int and strings describing inclusion lines topology (one list for each line)
              Tuple<List<List<int>>, List<List<string>>> lineTuple = topologyTuple.Item3;

              List<List<int>> void_topo_int = voidTuple.Item1;
              List<List<string>> void_topoType = voidTuple.Item2; //list of polyline curve type (arch or line) for void /member2d
              List<List<int>> incLines_topo_int = lineTuple.Item1;
              List<List<string>> inclLines_topoType = lineTuple.Item2; //list of polyline curve type (arch or line) for inclusion /member2d
              List<int> inclpts = topologyTuple.Item4;

              //list of lists of void points /member2d
              List<List<Point3d>> void_topo = new List<List<Point3d>>();
              for (int i = 0; i < void_topo_int.Count; i++)
              {
                void_topo.Add(new List<Point3d>());
                for (int j = 0; j < void_topo_int[i].Count; j++)
                {
                  if (nDict.TryGetValue(void_topo_int[i][j], out Node node))
                    void_topo[i].Add(Nodes.Point3dFromNode(node, modelUnit));
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
                    incLines_topo[i].Add(Nodes.Point3dFromNode(node, modelUnit));
                }
              }

              //list of points for inclusion /member2d
              List<Point3d> incl_pts = new List<Point3d>();
              for (int i = 0; i < inclpts.Count; i++)
              {
                if (nDict.TryGetValue(inclpts[i], out Node node))
                {
                  var p = node.Position;
                  incl_pts.Add(Nodes.Point3dFromNode(node, modelUnit));
                }
              }

              GsaMember2d mem2d = new GsaMember2d(mem, key, topopts, topoType, void_topo, void_topoType, incLines_topo, inclLines_topoType, incl_pts, pDict, matDict, modelUnit);
              mem2ds.Add(new GsaMember2dGoo(mem2d, duplicateApiObjects));
            }
          }
        }
      });
      
      return new Tuple<ConcurrentBag<GsaMember1dGoo>, ConcurrentBag<GsaMember2dGoo>, ConcurrentBag<GsaMember3dGoo>>(
          mem1ds, mem2ds, mem3ds);
    }
  }
}
