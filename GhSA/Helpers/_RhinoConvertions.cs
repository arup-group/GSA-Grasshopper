﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using GsaAPI;
using Rhino;
using Rhino.Geometry;
using Rhino.Collections;
using Rhino.Geometry.Collections;

namespace GhSA.Util.GH
{
    /// <summary>
    /// Tolerance class
    /// </summary>
    public class Tolerance
    {
        /// <summary>
        /// Method to retrieve active document Rhino units
        /// </summary>
        /// <returns></returns>
        public static double RhinoDocTolerance()
        {
            try
            {
                double tolerance = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
                return tolerance;
            }
            catch (global::System.Exception)
            {
                return 0.001;
            }
        }
    }
    /// <summary>
    /// Helper class to perform some decent geometry approximations from NURBS to poly-geometry
    /// </summary>
    public class Convert
    {
        public static Plane CreateBestFitUnitisedPlaneFromPts(List<Point3d> ctrl_pts)
        {
            Plane pln = Plane.WorldXY;

            // calculate best fit plane:
            Plane.FitPlaneToPoints(ctrl_pts, out pln);

            // change origin to closest point world xyz
            // this will ensure that axes created in same plane will not be duplicated
            pln.Origin = pln.ClosestPoint(new Point3d(0, 0, 0));

            // find significant digits for rounding
            int dig = Units.SignificantDigits;

            // unitise the plane normal so we can evaluate if it is XY-type plane
            pln.Normal.Unitize();
            if (Math.Abs(Math.Round(pln.Normal.Z, dig)) == 1) // if normal's z direction is close to vertical
            {
                // set X and Y axis to unit vectors to ensure no funny rotations
                pln.XAxis = Vector3d.XAxis;
                pln.YAxis = Vector3d.YAxis;
            }

            // round origin coordinates
            pln.OriginX = Util.Gsa.ResultHelper.RoundToSignificantDigits(pln.OriginX, dig);
            pln.OriginY = Util.Gsa.ResultHelper.RoundToSignificantDigits(pln.OriginY, dig);
            pln.OriginZ = Util.Gsa.ResultHelper.RoundToSignificantDigits(pln.OriginZ, dig);

            // unitize and round x-axis
            pln.XAxis.Unitize();
            Vector3d xaxis = pln.XAxis;
            xaxis.X = Util.Gsa.ResultHelper.RoundToSignificantDigits(Math.Abs(xaxis.X), dig);
            xaxis.Y = Util.Gsa.ResultHelper.RoundToSignificantDigits(Math.Abs(xaxis.Y), dig);
            xaxis.Z = Util.Gsa.ResultHelper.RoundToSignificantDigits(Math.Abs(xaxis.Z), dig);
            pln.XAxis = xaxis;

            // unitize and round y-axis
            pln.YAxis.Unitize();
            Vector3d yaxis = pln.YAxis;
            yaxis.X = Util.Gsa.ResultHelper.RoundToSignificantDigits(Math.Abs(yaxis.X), dig);
            yaxis.Y = Util.Gsa.ResultHelper.RoundToSignificantDigits(Math.Abs(yaxis.Y), dig);
            yaxis.Z = Util.Gsa.ResultHelper.RoundToSignificantDigits(Math.Abs(yaxis.Z), dig);
            pln.YAxis = yaxis;

            // unitize and round z-axis
            pln.ZAxis.Unitize();
            Vector3d zaxis = pln.ZAxis;
            zaxis.X = Util.Gsa.ResultHelper.RoundToSignificantDigits(Math.Abs(zaxis.X), dig);
            zaxis.Y = Util.Gsa.ResultHelper.RoundToSignificantDigits(Math.Abs(zaxis.Y), dig);
            zaxis.Z = Util.Gsa.ResultHelper.RoundToSignificantDigits(Math.Abs(zaxis.Z), dig);
            pln.ZAxis = zaxis;

            return pln;
        }
        
        
        /// <summary>
        /// Method to convert a NURBS curve into a PolyCurve made of lines and arcs.
        /// Automatically uses Rhino document tolerance if tolerance is not inputted
        /// </summary>
        /// <param name="crv"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        
        public static Tuple<PolyCurve, List<Point3d>, List<string>> ConvertMem1dCrv(Curve crv, double tolerance = -1)
        {
            PolyCurve m_crv = null;
            List<string> crv_type = new List<string>();
            List<Point3d> m_topo = new List<Point3d>();

            // arc curve
            if (crv.IsArc())
            {
                crv_type.Add("");
                crv_type.Add("A");
                crv_type.Add("");

                m_topo.Add(crv.PointAtStart);
                m_topo.Add(crv.PointAtNormalizedLength(0.5));
                m_topo.Add(crv.PointAtEnd);

                m_crv = new PolyCurve();
                m_crv.Append(crv);
            }
            else
            {
                if (crv.SpanCount > 1) // polyline (or assumed polyline, we will take controlpoints)
                {
                    m_crv = new PolyCurve();

                    if (tolerance < 0)
                        tolerance = Tolerance.RhinoDocTolerance();

                    crv = crv.ToPolyline(tolerance * 20, 5, 0, 0);

                    Curve[] segments = crv.DuplicateSegments();

                    for (int i = 0; i < segments.Length; i++)
                    {
                        crv_type.Add("");
                        m_topo.Add(segments[i].PointAtStart);

                        m_crv.Append(segments[i]);
                    }
                    crv_type.Add("");
                    m_topo.Add(segments[segments.Length - 1].PointAtEnd);
                }
                else // single line segment
                {
                    crv_type.Add("");
                    crv_type.Add("");

                    m_topo.Add(crv.PointAtStart);
                    m_topo.Add(crv.PointAtEnd);

                    m_crv = new PolyCurve();
                    m_crv.Append(crv);
                }
            }

            return new Tuple<PolyCurve, List<Point3d>, List<string>>(m_crv, m_topo, crv_type);
        }

        public static Tuple<PolyCurve, List<Point3d>, List<string>> ConvertMem2dCrv(Curve crv, double tolerance = -1)
        {
            if (tolerance < 0)
                tolerance = Tolerance.RhinoDocTolerance();

            PolyCurve m_crv = crv.ToArcsAndLines(tolerance * 20, 5, 0, 0);
            Curve[] segments;
            if (m_crv != null)
                segments = m_crv.DuplicateSegments();
            else
                segments = new Curve[] { crv };

            if (segments.Length == 1)
            {
                if (segments[0].IsClosed)
                {
                    segments = segments[0].Split(0.5);
                }
            }

            List<string> crv_type = new List<string>();
            List<Point3d> m_topo = new List<Point3d>();

            for (int i = 0; i < segments.Length; i++)
            {
                m_topo.Add(segments[i].PointAtStart);
                crv_type.Add("");
                if (segments[i].IsArc())
                {
                    m_topo.Add(segments[i].PointAtNormalizedLength(0.5));
                    crv_type.Add("A");
                }
            }
            m_topo.Add(segments[segments.Length - 1].PointAtEnd);
            crv_type.Add("");

            return new Tuple<PolyCurve, List<Point3d>, List<string>>(m_crv, m_topo, crv_type);
        }

        public static Brep ConvertBrep(Brep brep)
        {
            return new Brep();
        }
        
        public static Tuple<PolyCurve, List<Point3d>, List<string>> _notUsedConvertMem2dCrv(Curve crv, double tolerance = -1)
        {
            PolyCurve m_crv = null;
            List<string> crv_type = new List<string>();
            List<Point3d> m_topo = new List<Point3d>();

            if (crv.Degree > 1)
            {
                if (!crv.IsArc() | crv.IsClosed)
                {
                    if (tolerance < 0)
                        tolerance = Tolerance.RhinoDocTolerance();

                    m_crv = crv.ToArcsAndLines(tolerance * 20, 5, 0, 0);
                    Curve[] segments;
                    if (m_crv != null)
                        segments = m_crv.DuplicateSegments();
                    else
                        segments = new Curve[] { crv };

                    for (int i = 0; i < segments.Length; i++)
                    {
                        m_topo.Add(segments[i].PointAtStart);
                        crv_type.Add("");
                        if (segments[i].IsArc())
                        {
                            m_topo.Add(segments[i].PointAtNormalizedLength(0.5));
                            crv_type.Add("A");
                        }
                    }
                    m_topo.Add(segments[segments.Length - 1].PointAtEnd);
                    crv_type.Add("");
                }
                else
                {
                    crv_type.Add("");
                    crv_type.Add("A");
                    crv_type.Add("");

                    m_topo.Add(crv.PointAtStart);
                    m_topo.Add(crv.PointAtNormalizedLength(0.5));
                    m_topo.Add(crv.PointAtEnd);

                    m_crv = new PolyCurve();
                    m_crv.Append(crv);
                }
            }
            else if (crv.Degree == 1)
            {
                if (crv.SpanCount > 1)
                {
                    m_crv = new PolyCurve();
                    Curve[] segments = crv.DuplicateSegments();
                    for (int i = 0; i < segments.Length; i++)
                    {
                        crv_type.Add("");
                        m_topo.Add(segments[i].PointAtStart);

                        m_crv.Append(segments[i]);
                    }
                    crv_type.Add("");
                    m_topo.Add(segments[segments.Length - 1].PointAtEnd);
                }
                else
                {
                    crv_type.Add("");
                    crv_type.Add("");

                    m_topo.Add(crv.PointAtStart);
                    m_topo.Add(crv.PointAtEnd);

                    m_crv = new PolyCurve();
                    m_crv.Append(crv);
                }
            }

            return new Tuple<PolyCurve, List<Point3d>, List<string>>(m_crv, m_topo, crv_type);
        }

        /// <summary>
        /// Method to convert a NURBS Brep into a planar trimmed surface with PolyCurve
        /// internal and external edges of lines and arcs
        /// 
        /// BRep conversion to planar routine first converts the external edge to a PolyCurve
        /// of lines and arcs and uses these controlpoints to fit a plane through points.
        /// 
        /// Will output a Tuple containing:
        /// - PolyCurve
        /// - TopologyList of control points
        /// - TopoTypeList (" " or "a") corrosponding to control points
        /// - List of PolyCurves for internal (void) curves
        /// - Corrosponding list of topology points
        /// - Corrosponding list of topologytypes
        /// </summary>
        /// <param name="brep"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static Tuple<PolyCurve, List<Point3d>, List<string>, List<PolyCurve>, List<List<Point3d>>, List<List<string>>>
            ConvertPolyBrep(Brep brep, double tolerance = -1)
        {
            List<PolyCurve> void_crvs = new List<PolyCurve>();
            List<List<Point3d>> void_topo = new List<List<Point3d>>();
            List<List<string>> void_topoType = new List<List<string>>();

            Curve outer = null;
            List<Curve> inner = new List<Curve>();
            for (int i = 0; i < brep.Loops.Count; i++)
            {
                if (brep.Loops[i].LoopType == BrepLoopType.Outer)
                {
                    outer = brep.Loops[i].To3dCurve();
                }
                else
                {
                    inner.Add(brep.Loops[i].To3dCurve());
                }
            }
            List<Curve> edges = new List<Curve>();
            edges.Add(outer);
            edges.AddRange(inner);

            for (int i = 0; i < edges.Count; i++)
            {
                if (!edges[i].IsPlanar())
                {
                    List<Point3d> ctrl_pts;
                    if (edges[0].TryGetPolyline(out Polyline temp_crv))
                        ctrl_pts = temp_crv.ToList();
                    else
                    {
                        Tuple<PolyCurve, List<Point3d>, List<string>> convertBadSrf = GH.Convert.ConvertMem2dCrv(edges[0], tolerance);
                        ctrl_pts = convertBadSrf.Item2;
                    }
                    Plane.FitPlaneToPoints(ctrl_pts, out Plane plane);
                    for (int j = 0; j < edges.Count; j++)
                        edges[j] = Curve.ProjectToPlane(edges[j], plane);
                }
            }

            Tuple<PolyCurve, List<Point3d>, List<string>> convert = GH.Convert.ConvertMem2dCrv(edges[0], tolerance);
            PolyCurve edge_crv = convert.Item1;
            List<Point3d>  m_topo = convert.Item2;
            List<string> m_topoType = convert.Item3;

            for (int i = 1; i < edges.Count; i++)
            {
                convert = GH.Convert.ConvertMem2dCrv(edges[i], tolerance);
                void_crvs.Add(convert.Item1);
                void_topo.Add(convert.Item2);
                void_topoType.Add(convert.Item3);
            }

            return new Tuple<PolyCurve, List<Point3d>, List<string>, List<PolyCurve>, List<List<Point3d>>, List<List<string>>>
                (edge_crv, m_topo, m_topoType, void_crvs, void_topo, void_topoType);
        }

        /// <summary>
        /// Method to convert a NURBS Brep into a planar trimmed surface with PolyCurve
        /// internal and external edges of lines and arcs. 
        /// 
        /// BRep conversion to planar routine first converts the external edge to a PolyCurve
        /// of lines and arcs and uses these controlpoints to fit a plane through points.
        /// 
        /// Input list of curves and list of points to be included in 2D Member;
        /// lines and curves will automatically be projected onto planar Brep plane
        /// 
        /// Will output 3 Tuples:
        /// (edgeTuple, voidTuple, inclTuple)
        /// 
        /// edgeTuple:
        /// (edge_crv, m_topo, m_topoType)
        /// - PolyCurve
        /// - TopologyList of control points
        /// - TopoTypeList (" " or "a") corrosponding to control points
        /// 
        /// voidTuple:
        /// (void_crvs, void_topo, void_topoType)
        /// - List of PolyCurves for internal (void) curves
        /// - Corrosponding list of topology points
        /// - Corrosponding list of topologytypes
        /// 
        /// inclTuple:
        /// (incl_crvs, incl_topo, incl_topoType, inclPts)
        /// - List of PolyCurves for internal (void) curves
        /// - Corrosponding list of topology points
        /// - Corrosponding list of topologytypes
        /// - List of inclusion points
        /// 
        /// </summary>
        /// <param name="brep"></param>
        /// <param name="inclCrvs"></param>
        /// <param name="inclPts"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static Tuple<Tuple<PolyCurve, List<Point3d>, List<string>>, Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>>, Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>, List<Point3d>>>
            ConvertPolyBrepInclusion(Brep brep, List<Curve> inclCrvs = null, List<Point3d> inclPts = null, double tolerance = -1)
        {
            List<PolyCurve> void_crvs = new List<PolyCurve>();
            List<List<Point3d>> void_topo = new List<List<Point3d>>();
            List<List<string>> void_topoType = new List<List<string>>();

            List<PolyCurve> incl_crvs = new List<PolyCurve>();
            List<List<Point3d>> incl_topo = new List<List<Point3d>>();
            List<List<string>> incl_topoType = new List<List<string>>();

            Curve outer = null;
            List<Curve> inner = new List<Curve>();
            for (int i = 0; i < brep.Loops.Count; i++)
            {
                if (brep.Loops[i].LoopType == BrepLoopType.Outer)
                {
                    outer = brep.Loops[i].To3dCurve();
                }
                else
                {
                    inner.Add(brep.Loops[i].To3dCurve());
                }
            }
            List<Curve> edges = new List<Curve>();
            edges.Add(outer);
            edges.AddRange(inner);

            List<Point3d> ctrl_pts;
            if (edges[0].TryGetPolyline(out Polyline temp_crv))
                ctrl_pts = temp_crv.ToList();
            else
            {
                Tuple<PolyCurve, List<Point3d>, List<string>> convertBadSrf = GH.Convert.ConvertMem2dCrv(edges[0], tolerance);
                ctrl_pts = convertBadSrf.Item2;
            }
            Plane.FitPlaneToPoints(ctrl_pts, out Plane plane);

            for (int i = 0; i < edges.Count; i++)
            {
                if (!edges[i].IsPlanar())
                {
                    for (int j = 0; j < edges.Count; j++)
                        edges[j] = Curve.ProjectToPlane(edges[j], plane);
                }
            }
            Tuple<PolyCurve, List<Point3d>, List<string>> convert = GH.Convert.ConvertMem2dCrv(edges[0], tolerance);
            PolyCurve edge_crv = convert.Item1;
            List<Point3d> m_topo = convert.Item2;
            List<string> m_topoType = convert.Item3;

            for (int i = 1; i < edges.Count; i++)
            {
                convert = GH.Convert.ConvertMem2dCrv(edges[i], tolerance);
                void_crvs.Add(convert.Item1);
                void_topo.Add(convert.Item2);
                void_topoType.Add(convert.Item3);
            }

            if (inclCrvs != null)
            {
                for (int i = 0; i < inclCrvs.Count; i++)
                {
                    if (!inclCrvs[i].IsInPlane(plane))
                        inclCrvs[i] = Curve.ProjectToPlane(inclCrvs[i], plane);
                    convert = GH.Convert.ConvertMem2dCrv(inclCrvs[i], tolerance);
                    incl_crvs.Add(convert.Item1);
                    incl_topo.Add(convert.Item2);
                    incl_topoType.Add(convert.Item3);
                }
            }

            if (inclPts != null)
            {
                for (int i = 0; i < inclPts.Count; i++)
                    inclPts[i] = plane.ClosestPoint(inclPts[i]);
            }

            Tuple<PolyCurve, List<Point3d>, List<string>> edgeTuple = new Tuple<PolyCurve, List<Point3d>, List<string>>(edge_crv, m_topo, m_topoType);
            Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>> voidTuple = new Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>>(void_crvs, void_topo, void_topoType);
            Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>, List<Point3d>> inclTuple = new Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>, List<Point3d>>(incl_crvs, incl_topo, incl_topoType, inclPts);

            return new Tuple<Tuple<PolyCurve, List<Point3d>, List<string>>, Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>>, Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>, List<Point3d>>>
                (edgeTuple, voidTuple, inclTuple);
        }


        public static Brep BuildBrep(PolyCurve externalEdge, List<PolyCurve> voidCurves = null, double tolerance = -1)
        {
            if (tolerance < 0)
                tolerance = Tolerance.RhinoDocTolerance();

            Rhino.Collections.CurveList curves = new Rhino.Collections.CurveList
            {
                externalEdge
            };
            if (voidCurves != null)
                curves.AddRange(voidCurves);

            Brep[] brep = Brep.CreatePlanarBreps(curves, tolerance);
            if (brep == null)
                return null;
            else
                return brep[0];
        }

        public static PolyCurve ConvertCurveMem1d(Curve curve, double tolerance = -1)
        {
            Tuple<PolyCurve, List<Point3d>, List<string>> convertCrv = GH.Convert.ConvertMem1dCrv(curve, tolerance);
            return convertCrv.Item1;
        }
        public static PolyCurve ConvertCurveMem2d(Curve curve, double tolerance = -1)
        {
            Tuple<PolyCurve, List<Point3d>, List<string>> convertCrv = GH.Convert.ConvertMem2dCrv(curve, tolerance);
            return convertCrv.Item1;
        }

        public static PolyCurve BuildArcLineCurveFromPtsAndTopoType(List<Point3d> topology, List<string> topo_type=null)
        {
            PolyCurve crvs = new PolyCurve();

            for (int i = 0; i < topology.Count - 1; i++)
            {
                if (topo_type != null & topo_type[i + 1] == "A")
                {
                    crvs.Append(new Arc(topology[i], topology[i + 1], topology[i + 2]));
                    i++;
                }
                else
                    crvs.Append(new Line(topology[i], topology[i + 1]));
            }
            return crvs;
        }

        public static Brep ConvertBrepToArcLineEdges(Brep brep, double tolerance = -1)
        {
            Tuple<PolyCurve, List<Point3d>, List<string>, List<PolyCurve>, List<List<Point3d>>, List<List<string>>> convertBrep
                = GH.Convert.ConvertPolyBrep(brep, tolerance);
            return BuildBrep(convertBrep.Item1, convertBrep.Item4);
        }

        public static Tuple<List<Element>, List<Point3d>, List<List<int>>> ConvertMeshToElem2d(Mesh mesh, int prop = 1)
        {
            List<Element> elems = new List<Element>();
            List<Point3d> topoPts = new List<Point3d>(mesh.Vertices.ToPoint3dArray());
            List<List<int>> topoInts = new List<List<int>>();

            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                Element elem = new Element();
                List<int> topo = new List<int>();
                topo.Add(mesh.Faces[i].A);
                topo.Add(mesh.Faces[i].B);
                topo.Add(mesh.Faces[i].C);
                if (mesh.Faces[i].IsQuad)
                {
                    topo.Add(mesh.Faces[i].D);
                    elem.Type = ElementType.QUAD4;
                }
                else
                    elem.Type = ElementType.TRI3;
                topoInts.Add(topo);
                elem.Property = prop;
                elems.Add(elem);
            }
            return new Tuple<List<Element>, List<Point3d>, List<List<int>>>(elems, topoPts, topoInts);
        }

        public static Mesh ConvertBrepToMesh(Brep brep, List<Curve> curves, List<Point3d> points, double meshSize, List<Parameters.GsaMember1d> mem1ds = null, List<Parameters.GsaNode> nodes = null)
        {
            // set up unroller
            Unroller unroller = new Unroller(brep);

            List<Curve> memcrvs = new List<Curve>();
            if (mem1ds != null)
            {
                memcrvs = mem1ds.ConvertAll(x => (Curve)x.PolyCurve);
                unroller.AddFollowingGeometry(memcrvs);
            }
            List<Point3d> nodepts = new List<Point3d>();
            if (nodes != null)
            {
                nodepts = nodes.ConvertAll(x => x.Point);
                unroller.AddFollowingGeometry(nodepts);
            }

            unroller.AddFollowingGeometry(points);
            unroller.AddFollowingGeometry(curves);
            unroller.RelativeTolerance = 10000;
            unroller.AbsoluteTolerance = 10000;

            // create list of flattened geometry
            Point3d[] inclPts;
            Curve[] inclCrvs;
            TextDot[] unused;
            // perform unroll
            Brep[] flattened = unroller.PerformUnroll(out inclCrvs, out inclPts, out unused);

            // create 2d member from flattened geometry
            Parameters.GsaMember2d mem = new Parameters.GsaMember2d(flattened[0], inclCrvs.ToList(), inclPts.ToList());
            mem.Member.MeshSize = meshSize;
            // add to temp list for input in assemble function
            List<Parameters.GsaMember2d> mem2ds = new List<Parameters.GsaMember2d>();
            mem2ds.Add(mem);

            if (mem1ds != null)
            {
                for (int i = 0; i < mem1ds.Count; i++)
                {
                    Parameters.GsaMember1d mem1d = new Parameters.GsaMember1d(inclCrvs[i]);
                    mem1d.Member.MeshSize = mem1ds[i].Member.MeshSize;
                    mem1ds[i] = mem1d;
                }
            }
            if (nodes != null)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].Point = inclPts[i];
                }
            }

            // assemble temp model
            Model model = Util.Gsa.ToGSA.Assemble.AssembleModel(null, mem2ds, mem1ds, nodes);

            // call the meshing algorithm
            model.CreateElementsFromMembers();

            // extract elements from model
            Tuple<List<Parameters.GsaElement1dGoo>, List<Parameters.GsaElement2dGoo>> elementTuple
                = Util.Gsa.FromGSA.GetElements(model.Elements(), model.Nodes(), model.Sections(), model.Prop2Ds());

            List<Parameters.GsaElement2dGoo> elem2dgoo = elementTuple.Item2;
            Mesh mesh = elem2dgoo[0].Value.Mesh;

            Surface flat = flattened[0].Surfaces[0];
            Surface orig = brep.Surfaces[0];

            MeshVertexList vertices = mesh.Vertices;
            for (int i = 0; i < vertices.Count; i++)
            {
                flat.ClosestPoint(vertices.Point3dAt(i), out double u, out double v);
                Point3d mapVertex = orig.PointAt(u, v);
                vertices.SetVertex(i, mapVertex);
            }

            mesh.Faces.ConvertNonPlanarQuadsToTriangles(GhSA.Units.Tolerance, Rhino.RhinoMath.DefaultAngleTolerance, 0);

            return mesh;
        }
        public static Mesh ConvertMeshToTriMeshSolid(Mesh mesh)
        {
            // duplicate incoming mesh
            Mesh m = (Mesh)mesh.Duplicate();

            // test if mesh is closed
            if (!m.IsClosed)
            {
                // try fill holes
                m.FillHoles();

                // if not succesfull return null
                if (!m.IsClosed)
                    return null;
            }

            // weld mesh (collapse verticies)
            m.Weld(Math.PI);

            // triangulate all faces
            m.Faces.ConvertQuadsToTriangles();

            return mesh;   
        }
        public static Mesh ConvertBrepToTriMeshSolid(Brep brep)
        {
            // convert to mesh
            MeshingParameters mparams = MeshingParameters.Minimal;
            mparams.JaggedSeams = false;
            mparams.SimplePlanes = true;
            
            Mesh[] ms = Mesh.CreateFromBrep(brep, mparams);
            Mesh m = new Mesh();
            m.Append(ms);

            // test if mesh is closed
            if (!m.IsClosed)
            {
                // try fill holes
                m.FillHoles();

                // if not succesfull return null
                if (!m.IsClosed)
                    return null;
            }

            // weld mesh (collapse verticies)
            m.Weld(Math.PI);

            // triangulate all faces
            m.Faces.ConvertQuadsToTriangles();

            return m;
        }
    }
}
