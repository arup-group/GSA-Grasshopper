using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using GsaAPI;
using Rhino;
using Rhino.Geometry;
using Rhino.Collections;

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
        /// <summary>
        /// Method to convert a NURBS curve into a PolyCurve made of lines and arcs.
        /// Automatically uses Rhino document tolerance if tolerance is not inputted
        /// </summary>
        /// <param name="crv"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        
        public static Tuple<PolyCurve, List<Point3d>, List<string>> ConvertPolyCrv(Curve crv, double tolerance = -1)
        {
            if (tolerance < 0)
                tolerance = Tolerance.RhinoDocTolerance();
            
            PolyCurve m_crv = crv.ToArcsAndLines(tolerance * 20, 5, 0, 0);
            Curve[] segments;
            if (m_crv != null)
                segments = m_crv.DuplicateSegments();
            else
                segments = new Curve[] { crv };
                
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
            
            Curve[] edgeSegments = brep.DuplicateEdgeCurves();
            Curve[] edges = Curve.JoinCurves(edgeSegments);

            for (int i = 0; i < edges.Length; i++)
            {
                if (!edges[i].IsPlanar())
                {
                    List<Point3d> ctrl_pts;
                    if (edges[0].TryGetPolyline(out Polyline temp_crv))
                        ctrl_pts = temp_crv.ToList();
                    else
                    {
                        Tuple<PolyCurve, List<Point3d>, List<string>> convertBadSrf = GH.Convert.ConvertPolyCrv(edges[0], tolerance);
                        ctrl_pts = convertBadSrf.Item2;
                    }
                    Plane.FitPlaneToPoints(ctrl_pts, out Plane plane);
                    for (int j = 0; j < edges.Length; j++)
                        edges[j] = Curve.ProjectToPlane(edges[j], plane);
                }
            }

            Tuple<PolyCurve, List<Point3d>, List<string>> convert = GH.Convert.ConvertPolyCrv(edges[0], tolerance);
            PolyCurve edge_crv = convert.Item1;
            List<Point3d>  m_topo = convert.Item2;
            List<string> m_topoType = convert.Item3;

            for (int i = 1; i < edges.Length; i++)
            {
                convert = GH.Convert.ConvertPolyCrv(edges[i], tolerance);
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

            Curve[] edgeSegments = brep.DuplicateEdgeCurves();
            Curve[] edges = Curve.JoinCurves(edgeSegments);

            List<Point3d> ctrl_pts;
            if (edges[0].TryGetPolyline(out Polyline temp_crv))
                ctrl_pts = temp_crv.ToList();
            else
            {
                Tuple<PolyCurve, List<Point3d>, List<string>> convertBadSrf = GH.Convert.ConvertPolyCrv(edges[0], tolerance);
                ctrl_pts = convertBadSrf.Item2;
            }
            Plane.FitPlaneToPoints(ctrl_pts, out Plane plane);


            for (int i = 0; i < edges.Length; i++)
            {
                if (!edges[i].IsPlanar())
                {
                    for (int j = 0; j < edges.Length; j++)
                        edges[j] = Curve.ProjectToPlane(edges[j], plane);
                }
            }
            Tuple<PolyCurve, List<Point3d>, List<string>> convert = GH.Convert.ConvertPolyCrv(edges[0], tolerance);
            PolyCurve edge_crv = convert.Item1;
            List<Point3d> m_topo = convert.Item2;
            List<string> m_topoType = convert.Item3;

            for (int i = 1; i < edges.Length; i++)
            {
                convert = GH.Convert.ConvertPolyCrv(edges[i], tolerance);
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
                    convert = GH.Convert.ConvertPolyCrv(inclCrvs[i], tolerance);
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

        public static PolyCurve ConvertCurve(Curve curve, double tolerance = -1)
        {
            Tuple<PolyCurve, List<Point3d>, List<string>> convertCrv = GH.Convert.ConvertPolyCrv(curve, tolerance);
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
