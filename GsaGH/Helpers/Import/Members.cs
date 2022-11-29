using System;
using System.Linq;
using System.Drawing;
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
    internal static Member DuplicateMember(Member mem)
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
    internal static Tuple<ConcurrentBag<GsaMember1dGoo>, ConcurrentBag<GsaMember2dGoo>, ConcurrentBag<GsaMember3dGoo>>
        GetMembers(ConcurrentDictionary<int, Member> mDict, ConcurrentDictionary<int, Node> nDict, LengthUnit unit,
        ConcurrentDictionary<int, Section> sDict, ConcurrentDictionary<int, Prop2D> pDict, ConcurrentDictionary<int, Prop3D> p3Dict, ConcurrentDictionary<int, ReadOnlyCollection<double>> localAxesDict, GH_Component owner = null)
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
            List<List<int>> topints = Topology.Topology_detangler_Mem3d(toporg);

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
                  tempMesh.Vertices.Add(Nodes.Point3dFromNode(node, unit));
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
                  Tuple<List<List<int>>, List<List<string>>>, List<int>> topologyTuple = Topology.Topology_detangler(toporg);
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
                topopts.Add(Nodes.Point3dFromNode(node, unit));
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
                  void_topo[i].Add(Nodes.Point3dFromNode(node, unit));
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
                  incLines_topo[i].Add(Nodes.Point3dFromNode(node, unit));
              }
            }

            //list of points for inclusion /member2d
            List<Point3d> incl_pts = new List<Point3d>();
            for (int i = 0; i < inclpts.Count; i++)
            {
              if (nDict.TryGetValue(inclpts[i], out Node node))
              {
                var p = node.Position;
                incl_pts.Add(Nodes.Point3dFromNode(node, unit));
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
                  orient = new GsaNode(Nodes.Point3dFromNode(node, unit),
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
  }
}
