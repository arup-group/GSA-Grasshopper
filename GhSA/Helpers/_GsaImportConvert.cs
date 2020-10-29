using System;
using System.Linq;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using GsaAPI;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GhSA.Parameters;

namespace GhSA.Util.Gsa
{
    /// <summary>
    /// Class containing functions to import various object types from GSA
    /// </summary>
    public class GsaImport
    {
        /// <summary>
        /// Method to import Nodes from a GSA model.
        /// Will output a list of GhSA GsaNodes.
        /// Filter nodes to import using nodeList input;
        /// "all" or empty string ("") will import all nodes
        /// </summary>
        /// <param name="model"></param>
        /// <param name="nodeList"></param>
        /// <returns></returns>
        public static List<GsaNode> GsaGetPoint(Model model, string nodeList, bool removeEmptyIDs = false)
        {
            // Create empty Gsa Node to work on:
            //Node node = new Node();
            //GsaNode n = new GsaNode();
            List<GsaNode> nodes = new List<GsaNode>();

            // Create dictionary to read list of nodes:
            IReadOnlyDictionary<int, Node> nDict;
            nDict = model.Nodes(nodeList);

            if (!removeEmptyIDs)
            {
                if (nDict.Count > 0)
                {
                    // Loop through all nodes in Node dictionary and add points to Rhino point list
                    for (int i = 0; i < nDict.Keys.Max(); i++)
                    {
                        if (nDict.TryGetValue(i + 1, out Node apinode)) //1-base numbering
                        {
                            var p = apinode.Position;
                            GsaNode n = new GsaNode(new Point3d(p.X, p.Y, p.Z), i + 1)
                            {
                                Node = apinode
                            };
                            if (apinode.SpringProperty > 0)
                            {
                                // to be implement. GsaAPI spring missing.
                                // get spring property from model
                            }
                            nodes.Add(n);
                        }
                        else
                            nodes.Add(null);
                    }
                }
            }
            else
            {
                if (nDict.Count > 0)
                {
                    // Loop through all nodes in Node dictionary and add points to Rhino point list
                    foreach (int key in nDict.Keys)
                    {
                        if (nDict.TryGetValue(key, out Node apinode)) //1-base numbering
                        {
                            var p = apinode.Position;
                            GsaNode n = new GsaNode(new Point3d(p.X, p.Y, p.Z), key)
                            {
                                Node = apinode
                            };
                            if (apinode.SpringProperty > 0)
                            {
                                // to be implement. GsaAPI spring missing.
                                // get spring property from model
                            }
                            nodes.Add(n.Duplicate());
                        }
                    }
                }
            }
            
            return nodes;
        }
        /// <summary>
        /// Method to import 1D and 2D Elements from a GSA model.
        /// Will output a tuple of GhSA GsaElement1d and GsaElement2d.
        /// Filter elements to import using elemList input;
        /// "all" or empty string ("") will import all elements. Default is "all"
        /// "Join" bool = true; will try to join 2D element mesh faces into a joined meshes.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="elemList"></param>
        /// <param name="join"></param>
        /// <returns></returns>
        public static Tuple<DataTree<GsaElement1dGoo>, DataTree<GsaElement2dGoo>> GsaGetElem(Model model, string elemList = "all", bool join = true)
        {
            // Create dictionaries to read list of elements and nodes:
            IReadOnlyDictionary<int, Element> eDict;
            eDict = model.Elements(elemList);
            // Create lists for Rhino lines and meshes
            DataTree<GsaElement1dGoo> elem1ds = new DataTree<GsaElement1dGoo>();
            DataTree<GsaElement2dGoo> elem2ds = new DataTree<GsaElement2dGoo>();

            if (eDict.Count > 0)
            {
                IReadOnlyDictionary<int, Node> nDict;
                nDict = model.Nodes("all");
                IReadOnlyDictionary<int, Section> sDict;
                sDict = model.Sections();
                IReadOnlyDictionary<int, Prop2D> pDict;
                pDict = model.Prop2Ds();

                GsaElement2d elem2d = new GsaElement2d();

                
                DataTree<Element> elements = new DataTree<Element>();
                DataTree<Mesh> meshes = new DataTree<Mesh>();
                List<Point3d> pts = new List<Point3d>();

                GH_Path path = new GH_Path();

                if (!join)
                {
                    elem1ds.EnsurePath(0);
                    elem2ds.EnsurePath(0);
                }

                // Loop through all nodes in Node dictionary and add points to Rhino point list
                foreach (var key in eDict.Keys)
                {
                    if (eDict.TryGetValue(key, out Element elem))
                    {
                        List<int> topo = elem.Topology.ToList();
                        int prop = 0;
                        if (join)
                            prop = elem.Property - 1; // actually branch not property


                        // Beams (1D elements):
                        if (topo.Count == 2)
                        {
                            for (int i = 0; i <= 1; i++)
                            {
                                if (nDict.TryGetValue(topo[i], out Node node))
                                {
                                    {
                                        var p = node.Position;
                                        pts.Add(new Point3d(p.X, p.Y, p.Z));
                                    }
                                }
                            }
                            Line line = new Line(pts[0], pts[1]);
                            LineCurve ln = new LineCurve(line);
                            GsaElement1d elem1d = new GsaElement1d(ln)
                            {
                                Element = elem
                            };
                            elem1d.ReleaseStart = new GsaBool6()
                            {
                                X = elem.Release(0).X,
                                Y = elem.Release(0).Y,
                                Z = elem.Release(0).Z,
                                XX = elem.Release(0).XX,
                                YY = elem.Release(0).YY,
                                ZZ = elem.Release(0).ZZ
                            };

                            elem1d.ReleaseEnd = new GsaBool6()
                            {
                                X = elem.Release(1).X,
                                Y = elem.Release(1).Y,
                                Z = elem.Release(1).Z,
                                XX = elem.Release(1).XX,
                                YY = elem.Release(1).YY,
                                ZZ = elem.Release(1).ZZ
                            };

                            GsaSection section = new GsaSection
                            {
                                ID = elem.Property
                            };
                            Section tempSection = new Section();
                            if (sDict.TryGetValue(section.ID, out tempSection))
                                section.Section = tempSection;
                            elem1d.Section = section;
                            elem1d.ID = key;

                            pts.Clear();
                            elem1ds.EnsurePath(prop);
                            path = new GH_Path(prop);
                            if (join)
                                elem1ds.Add(new GsaElement1dGoo(elem1d), path);
                            else
                                elem1ds.Insert(new GsaElement1dGoo(elem1d), path, key - 1);
                            //elem1ds[path, key - 1] = new GsaElement1dGoo(elem1d.Duplicate());
                        }

                        // Shells (2D elements)
                        if (topo.Count > 2) // & topo.Count < 5)
                        {
                            Mesh tempMesh = new Mesh();
                            // Get verticies:
                            for (int i = 0; i < topo.Count; i++)
                            {
                                if (nDict.TryGetValue(topo[i], out Node node))
                                {
                                    {
                                        var p = node.Position;
                                        tempMesh.Vertices.Add(new Point3d(p.X, p.Y, p.Z));
                                    }
                                }
                            }

                            // Create mesh face (Tri- or Quad):
                            if (topo.Count == 3)
                                tempMesh.Faces.AddFace(0, 1, 2);
                            if (topo.Count == 4)
                                tempMesh.Faces.AddFace(0, 1, 2, 3);
                            else
                            {
                                //it must be a TRI6 or a QUAD8
                                List<Point3f> tempPts = tempMesh.Vertices.ToList();
                                double x = 0; double y = 0; double z = 0;
                                for (int i = 0; i < tempPts.Count; i++)
                                {
                                    x += tempPts[i].X; y += tempPts[i].Y; z += tempPts[i].Z;
                                }
                                x /= tempPts.Count; y /= tempPts.Count; z /= tempPts.Count;
                                tempMesh.Vertices.Add(new Point3d(x, y, z));

                                if (topo.Count == 6)
                                {
                                    tempMesh.Faces.AddFace(0, 3, 6);
                                    tempMesh.Faces.AddFace(3, 1, 6);
                                    tempMesh.Faces.AddFace(1, 4, 6);
                                    tempMesh.Faces.AddFace(4, 2, 6);
                                    tempMesh.Faces.AddFace(2, 5, 6);
                                    tempMesh.Faces.AddFace(5, 0, 6);
                                }

                                if (topo.Count == 8)
                                {
                                    tempMesh.Faces.AddFace(0, 4, 8, 7);
                                    tempMesh.Faces.AddFace(1, 5, 8, 4);
                                    tempMesh.Faces.AddFace(2, 6, 8, 5);
                                    tempMesh.Faces.AddFace(3, 7, 8, 6);
                                }
                            }
                            List<int> ids = new List<int>
                        {
                            key
                        };

                            elem2d.ID = ids;

                            List<GsaProp2d> prop2Ds = new List<GsaProp2d>();
                            GsaProp2d prop2d = new GsaProp2d
                            {
                                ID = elem.Property
                            };
                            Prop2D tempProp = new Prop2D();
                            if (pDict.TryGetValue(prop2d.ID, out tempProp))
                                prop2d.Prop2d = tempProp;
                            prop2Ds.Add(prop2d);
                            elem2d.Properties = prop2Ds;


                            if (join)
                            {
                                meshes.EnsurePath(prop);
                                elements.EnsurePath(prop);
                                path = new GH_Path(prop);

                                meshes.Add(tempMesh.DuplicateMesh(), path);
                                elements.Add(elem, path);
                            }
                            else
                            {
                                elem2d = new GsaElement2d(tempMesh);
                                List<Element> elemProps = new List<Element>
                            {
                                elem
                            };
                                elem2d.Elements = elemProps;
                                elem2d.Properties = prop2Ds;
                                elem2d.ID = ids;
                                elem2ds.Insert(new GsaElement2dGoo(elem2d), path, key - 1);
                            }
                        }
                    }
                }

                if (join)
                {
                    foreach (GH_Path ipath in meshes.Paths)
                    {
                        //##### Join meshes #####

                        //List of meshes in each branch
                        List<Mesh> mList = meshes.Branch(ipath);

                        //new temp mesh
                        Mesh m = new Mesh();
                        //Append list of meshes (faster than appending each mesh one by one)
                        m.Append(mList);

                        //split mesh into connected pieces
                        Mesh[] meshy = m.SplitDisjointPieces();

                        //clear whatever is in the current branch (the list in mList)
                        meshes.Branch(ipath).Clear();
                        //rewrite new joined and split meshes to new list in same path:
                        for (int j = 0; j < meshy.Count(); j++)
                            meshes.Add(meshy[j], ipath);
                    }
                    foreach (GH_Path ipath in meshes.Paths)
                    {
                        List<Mesh> mList = meshes.Branch(ipath);
                        foreach (Mesh mesh in mList)
                        {
                            elem2d = new GsaElement2d(mesh);
                            List<Element> elemProps = new List<Element>();
                            for (int i = 0; i < mesh.Faces.Count(); i++)
                                elemProps.Add(elements[ipath, 0]);
                            elem2d.Elements = elemProps;
                            List<GsaProp2d> prop2Ds = new List<GsaProp2d>();
                            GsaProp2d prop2d = new GsaProp2d
                            {
                                ID = ipath.Indices[0] + 1
                            };
                            Prop2D tempProp = new Prop2D();
                            if (pDict.TryGetValue(prop2d.ID, out tempProp))
                                prop2d.Prop2d = tempProp;
                            prop2Ds.Add(prop2d);
                            elem2d.Properties = prop2Ds;

                            elem2ds.Add(new GsaElement2dGoo(elem2d));
                        }
                    }
                }
            }
            
            
            return new Tuple<DataTree<GsaElement1dGoo>, DataTree<GsaElement2dGoo>>(elem1ds, elem2ds);
        }
        /// <summary>
        /// Method to import 1D and 2D Members from a GSA model.
        /// Will output a tuple of GhSA GsaMember1d and GsaMember2d.
        /// Filter members to import using memList input;
        /// "all" or empty string ("") will import all elements. Default is "all"
        /// "propGraft" bool = true; will put members in Grasshopper branch corrosponding to its property
        /// </summary>
        /// <param name="model"></param>
        /// <param name="memList"></param>
        /// <param name="propGraft"></param>
        /// <returns></returns>
        public static Tuple<DataTree<GsaMember1dGoo>, DataTree<GsaMember2dGoo>> GsaGetMemb(Model model, string memList = "all", bool propGraft = true)
        {
            // Create dictionaries to read list of elements and nodes:
            IReadOnlyDictionary<int, Member> mDict;
            mDict = model.Members(memList);
            // Create lists for Rhino lines and meshes
            DataTree<GsaMember1dGoo> mem1ds = new DataTree<GsaMember1dGoo>();
            DataTree<GsaMember2dGoo> mem2ds = new DataTree<GsaMember2dGoo>();
            if (mDict.Count > 0)
            {
                IReadOnlyDictionary<int, Node> nDict;
                nDict = model.Nodes("all");
                IReadOnlyDictionary<int, Section> sDict;
                sDict = model.Sections();
                IReadOnlyDictionary<int, Prop2D> pDict;
                pDict = model.Prop2Ds();

                if (!propGraft)
                {
                    mem1ds.EnsurePath(0);
                    mem2ds.EnsurePath(0);
                    int max = mDict.Count;
                    if (max > 0)
                    {
                        for (int i = 0; i < mDict.Keys.ElementAt(max - 1); i++)
                        {
                            mem1ds.Branches[0].Add(null);
                            mem2ds.Branches[0].Add(null);
                        }
                    }
                }

                // Loop through all members in Member dictionary 
                foreach (var key in mDict.Keys)
                {
                    if (mDict.TryGetValue(key, out Member mem))
                    {
                        int prop = 0;
                        if (propGraft)
                            prop = mem.Property - 1;

                        // Build topology lists
                        string toporg = mem.Topology; //original topology list

                        Tuple<Tuple<List<int>, List<string>>, Tuple<List<List<int>>, List<List<string>>>,
                            Tuple<List<List<int>>, List<List<string>>>, List<int>> topologyTuple = Topology_detangler(toporg);
                        Tuple<List<int>, List<string>> topoTuple = topologyTuple.Item1;
                        Tuple<List<List<int>>, List<List<string>>> voidTuple = topologyTuple.Item2;
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
                        for (int i = 0; i < topo_int.Count; i++)
                        {
                            if (nDict.TryGetValue(topo_int[i], out Node node))
                            {
                                var p = node.Position;
                                topopts.Add(new Point3d(p.X, p.Y, p.Z));
                            }
                        }

                        //list of lists of void points /member2d
                        List<List<Point3d>> void_topo = new List<List<Point3d>>();
                        for (int i = 0; i < void_topo_int.Count; i++)
                        {
                            void_topo.Add(new List<Point3d>());
                            for (int j = 0; j < void_topo_int[i].Count; j++)
                            {
                                if (nDict.TryGetValue(void_topo_int[i][j], out Node node))
                                {
                                    var p = node.Position;
                                    void_topo[i].Add(new Point3d(p.X, p.Y, p.Z));
                                }
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
                                {
                                    var p = node.Position;
                                    incLines_topo[i].Add(new Point3d(p.X, p.Y, p.Z));
                                }
                            }
                        }

                        //list of points for inclusion /member2d
                        List<Point3d> incl_pts = new List<Point3d>();
                        for (int i = 0; i < inclpts.Count; i++)
                        {
                            if (nDict.TryGetValue(inclpts[i], out Node node))
                            {
                                var p = node.Position;
                                incl_pts.Add(new Point3d(p.X, p.Y, p.Z));
                            }
                        }

                        if (mem.Type == MemberType.GENERIC_1D | mem.Type == MemberType.BEAM | mem.Type == MemberType.CANTILEVER |
                            mem.Type == MemberType.COLUMN | mem.Type == MemberType.COMPOS | mem.Type == MemberType.PILE)
                        {
                            GsaMember1d mem1d = new GsaMember1d(topopts, topoType)
                            {
                                ID = key,
                                Member = mem
                            };
                            GsaSection section = new GsaSection
                            {
                                ID = mem.Property
                            };
                            if (sDict.TryGetValue(section.ID, out Section tempSection))
                                section.Section = tempSection;
                            mem1d.Section = section;
                            mem1ds.EnsurePath(prop);
                            GH_Path path = new GH_Path(prop);
                            if (propGraft)
                                mem1ds.Add(new GsaMember1dGoo(mem1d.Duplicate()), path);
                            else
                                mem1ds[path, key - 1] = new GsaMember1dGoo(mem1d.Duplicate());
                        }
                        else
                        {
                            GsaMember2d mem2d = new GsaMember2d(topopts, topoType, void_topo, void_topoType, incLines_topo, inclLines_topoType, incl_pts)
                            {
                                Member = mem,
                                ID = key
                            };
                            GsaProp2d prop2d = new GsaProp2d
                            {
                                ID = mem.Property
                            };
                            if (pDict.TryGetValue(prop2d.ID, out Prop2D tempProp))
                                prop2d.Prop2d = tempProp;
                            mem2d.Property = prop2d;
                            mem2ds.EnsurePath(prop);
                            GH_Path path = new GH_Path(prop);
                            if (propGraft)
                                mem2ds.Add(new GsaMember2dGoo(mem2d.Duplicate()), path);
                            else
                                mem2ds[path, key - 1] = new GsaMember2dGoo(mem2d.Duplicate());
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
            }

            return new Tuple<DataTree<GsaMember1dGoo>, DataTree<GsaMember2dGoo>>(mem1ds, mem2ds);
        }

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
                int tpt = Int32.Parse(pts[i]);
                inclpoint.Add(tpt);
            }

            // write out topology type (A) to list
            List<int> topoint = new List<int>();
            List<String> topoType = new List<String>();
            for (int i = 0; i < topolos.Count(); i++)
            {
                if (topolos[i] == "A")
                {
                    topoType.Add("A");
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
        /// Method to import Sections from a GSA model.
        /// Will output a list of GhSA GsaSectionsGoo.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static List<GsaSectionGoo> GsaGetSections(Model model)
        {
            List<GsaSectionGoo> sections = new List<GsaSectionGoo>();

            // Create dictionary to read list of sections:
            IReadOnlyDictionary<int, Section> sDict;
            sDict = model.Sections();

            // Loop through all sections in Section dictionary and create new GsaSections
            foreach (int key in sDict.Keys)
            {
                if (sDict.TryGetValue(key, out Section apisection)) //1-base numbering
                {
                    GsaSection sect = new GsaSection();
                    sect.Section = apisection;
                    sect.ID = key;
                    sections.Add(new GsaSectionGoo(sect));
                }
                else
                    sections.Add(null);
            }
            return sections;
        }
        /// <summary>
        /// Method to import Prop2ds from a GSA model.
        /// Will output a list of GhSA GsaProp2dGoo.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static List<GsaProp2dGoo> GsaGetProp2ds(Model model)
        {
            List<GsaProp2dGoo> prop2ds = new List<GsaProp2dGoo>();

            // Create dictionary to read list of Properties:
            IReadOnlyDictionary<int, Prop2D> sDict;
            sDict = model.Prop2Ds();

            // Loop through all sections in Properties dictionary and create new GsaProp2d
            foreach (int key in sDict.Keys)
            {
                if (sDict.TryGetValue(key, out Prop2D apisection)) //1-base numbering
                {
                    GsaProp2d prop = new GsaProp2d();
                    prop.Prop2d = apisection;
                    prop.ID = key;
                    prop2ds.Add(new GsaProp2dGoo(prop));
                }
                else
                    prop2ds.Add(null);
            }
            return prop2ds;
        }
        /// <summary>
        /// Method to import Beam Loads from a GSA model.
        /// Will output a list of GhSA GsaLoadsGoo.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static List<GsaLoadGoo> GsaGetGravityLoads(Model model)
        {
            List<GsaLoadGoo> loads = new List<GsaLoadGoo>();

            List<GravityLoad> gloads = model.GravityLoads().ToList();

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
        /// Method to import Node Loads from a GSA model.
        /// Will output a list of GhSA GsaLoads.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static List<GsaLoadGoo> GsaGetNodeLoads(Model model)
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
                        case NodeLoadType.APPLIED_DISP:
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
                        myload.NodeLoadType = ntyp;
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
        /// Will output a list of GhSA GsaLoads.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static List<GsaLoadGoo> GsaGetBeamLoads(Model model)
        {
            List<GsaLoadGoo> loads = new List<GsaLoadGoo>();

            List<BeamLoad> gsaloads = model.BeamLoads().ToList();

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
        /// Will output a list of GhSA GsaLoads.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static List<GsaLoadGoo> GsaGetFaceLoads(Model model)
        {
            List<GsaLoadGoo> loads = new List<GsaLoadGoo>();

            List<FaceLoad> gsaloads = model.FaceLoads().ToList();

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
        /// Will output a list of GhSA GsaLoads.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static List<GsaLoadGoo> GsaGetGridPointLoads(Model model)
        {
            List<GsaLoadGoo> loads = new List<GsaLoadGoo>();

            List<GridPointLoad> gsaloads = model.GridPointLoads().ToList();

            // Loop through all loads in list and create new GsaLoads
            for (int i = 0; i < gsaloads.Count; i++)
            {
                // Get Grid Point Load
                GsaGridPointLoad myload = new GsaGridPointLoad();
                myload.GridPointLoad = gsaloads[i];

                // Get GridPlaneSurface
                myload.GridPlaneSurface = GsaGetGridPlaneSurface(model, gsaloads[i].GridSurface);

                // Add load to list
                GsaLoad load = new GsaLoad(myload);
                loads.Add(new GsaLoadGoo(load));
            }
            return loads;
        }
        /// <summary>
        /// Method to import Grid Line Loads from a GSA model.
        /// Will output a list of GhSA GsaLoads.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static List<GsaLoadGoo> GsaGetGridLineLoads(Model model)
        {
            List<GsaLoadGoo> loads = new List<GsaLoadGoo>();

            List<GridLineLoad> gsaloads = model.GridLineLoads().ToList();

            // Loop through all loads in list and create new GsaLoads
            for (int i = 0; i < gsaloads.Count; i++)
            {
                // Get Grid Point Load
                GsaGridLineLoad myload = new GsaGridLineLoad();
                myload.GridLineLoad = gsaloads[i];

                // Get GridPlaneSurface
                myload.GridPlaneSurface = GsaGetGridPlaneSurface(model, gsaloads[i].GridSurface);

                // Add load to list
                GsaLoad load = new GsaLoad(myload);
                loads.Add(new GsaLoadGoo(load));
            }
            return loads;
        }
        /// <summary>
        /// Method to import Grid Area Loads from a GSA model.
        /// Will output a list of GhSA GsaLoads.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static List<GsaLoadGoo> GsaGetGridAreaLoads(Model model)
        {
            List<GsaLoadGoo> loads = new List<GsaLoadGoo>();

            List<GridAreaLoad> gsaloads = model.GridAreaLoads().ToList();

            // Loop through all loads in list and create new GsaLoads
            for (int i = 0; i < gsaloads.Count; i++)
            {
                // Get Grid Point Load
                GsaGridAreaLoad myload = new GsaGridAreaLoad();
                myload.GridAreaLoad = gsaloads[i];

                // Get GridPlaneSurface
                myload.GridPlaneSurface = GsaGetGridPlaneSurface(model, gsaloads[i].GridSurface);

                // Add load to list
                GsaLoad load = new GsaLoad(myload);
                loads.Add(new GsaLoadGoo(load));
            }
            return loads;
        }
        /// <summary>
        /// Method to get GsaGridPlaneSurface including grid surface, grid plane and axis from GSA Model
        /// Will output a new GsaGridPlaneSurface.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static GsaGridPlaneSurface GsaGetGridPlaneSurface(Model model, int gridsrf_ID)
        {
            // GridPlaneSurface
            GsaGridPlaneSurface gps = new GsaGridPlaneSurface();

            // Get Grid Surface
            IReadOnlyDictionary<int, GridSurface> sDict;
            sDict = model.GridSurfaces();
            sDict.TryGetValue(gridsrf_ID, out GridSurface gs);
            gps.GridSurface = gs;

            // Get Grid Plane
            IReadOnlyDictionary<int, GridPlane> pDict;
            pDict = model.GridPlanes();
            pDict.TryGetValue(gs.GridPlane, out GridPlane gp);
            gps.GridPlane = gp;

            // Get Axis
            IReadOnlyDictionary<int, Axis> aDict;
            aDict = model.Axes();
            aDict.TryGetValue(gp.AxisProperty, out Axis ax);
            gps.Axis = ax;

            // Construct Plane from Axis
            Plane pln = new Plane();
            if (ax != null )
            {
                pln = new Plane(
                new Point3d(ax.Origin.X, ax.Origin.Y, ax.Origin.Z + gp.Elevation), // for new origin Z-coordinate we add axis origin and grid plane elevation
                new Vector3d(ax.XVector.X, ax.XVector.Y, ax.XVector.Z),
                new Vector3d(ax.XYPlane.X, ax.XYPlane.Y, ax.XYPlane.Z));
            }
            else
            {
                pln = Plane.WorldXY;
                pln.OriginZ = gp.Elevation;  
            }
            gps.Plane = pln;
            
            return gps;
        }
    }
}
