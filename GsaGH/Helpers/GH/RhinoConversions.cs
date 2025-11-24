using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

using GsaAPI;

using GsaGH.Helpers.Assembly;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;

using OasysGH.Units;

using OasysUnits;

using Rhino;
using Rhino.Collections;
using Rhino.Geometry;

using LengthUnit = OasysUnits.Units.LengthUnit;
using Line = Rhino.Geometry.Line;
using Polyline = Rhino.Geometry.Polyline;

namespace GsaGH.Helpers.GH {
  /// <summary>
  ///   Helper class to perform some decent geometry approximations from NURBS to poly-geometry
  /// </summary>
  public class RhinoConversions {

    public static PolyCurve BuildArcLineCurveFromPtsAndTopoType(
      Point3dList topology, List<string> topoType = null) {
      var crvs = new PolyCurve();

      for (int i = 0; i < topology.Count - 1; i++) {
        if (topoType != null && topoType[i + 1] == "A") {
          crvs.Append(new Arc(topology[i], topology[i + 1], topology[i + 2]));
          i++;
        } else {
          crvs.Append(new Line(topology[i], topology[i + 1]));
        }
      }

      return crvs;
    }

    public static Brep BuildBrep(
      PolyCurve externalEdge, List<PolyCurve> voidCurves = null, double tolerance = -1) {
      if (tolerance < 0) {
        tolerance = DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry);
      }

      var curves = new CurveList {
        externalEdge,
      };
      if (voidCurves != null) {
        curves.AddRange(voidCurves);
      }

      Brep[] brep = Brep.CreatePlanarBreps(curves, tolerance);
      if (brep != null) {
        return brep[0];
      }

      tolerance *= 2;
      brep = Brep.CreatePlanarBreps(curves, tolerance);
      if (brep != null && brep.Length > 0 && brep[0].IsValid) {
        return brep[0];
      }

      if (curves.Count < 5) {
        var brep2 = Brep.CreateEdgeSurface(curves);
        if (brep2 != null && brep2.IsValid) {
          return brep2;
        }
      }

      var stopwatch = Stopwatch.StartNew();
      while (brep == null || brep.Length == 0 || !brep[0].IsValid) {
        if (stopwatch.ElapsedMilliseconds > 5000) {
          break;
        }

        tolerance *= 2;
        brep = Brep.CreatePlanarBreps(curves, tolerance);
        if (brep != null && brep.Length > 0 && brep[0].IsValid) {
          return brep[0];
        }
      }
      return null;
    }

    internal static Tuple<Mesh, List<GsaNode>, List<GsaElement1D>> ConvertBrepToMesh(
      Brep brep, Point3dList points, List<GsaNode> inNodes, List<Curve> inCurves,
      List<GsaElement1D> inElem1ds, List<GsaMember1D> inMem1ds, double meshSize, LengthUnit unit,
      Length tolerance, MeshMode2d meshMode) {
      bool convertNonPlanarQuads = meshMode == MeshMode2d.Mixed;
      Brep inBrep = brep.DuplicateBrep();
      inBrep.Faces.ShrinkFaces();
      var unroller = new Unroller(inBrep);
      var types = new List<int>();
      var finalNodes = new Point3dList(points);
      if (inCurves != null) {
        foreach (Curve crv in inCurves) {
          finalNodes.Add(crv.PointAtStart);
          finalNodes.Add(crv.PointAtEnd);
          unroller.AddFollowingGeometry(crv);
          types.Add(0);
        }
      }

      if (inElem1ds != null) {
        foreach (GsaElement1D elem in inElem1ds) {
          finalNodes.Add(elem.Line.PointAtEnd);
          finalNodes.Add(elem.Line.PointAtEnd);
          unroller.AddFollowingGeometry(elem.Line);
          types.Add(1);
        }
      }

      if (inMem1ds != null) {
        foreach (GsaMember1D mem1d in inMem1ds) {
          foreach (Point3d ctrlPt in mem1d.Topology) {
            finalNodes.Add(ctrlPt);
          }
          unroller.AddFollowingGeometry(mem1d.PolyCurve);
          types.Add(2);
        }
      }

      var nodeIds = new List<int>();
      int nodeid = 0;
      if (inNodes != null) {
        foreach (GsaNode node in inNodes) {
          finalNodes.Add(node.Point);
          inBrep.Surfaces[0].ClosestPoint(node.Point, out double u, out double v);
          var dot = new TextDot(node.ApiNode.Name, inBrep.Surfaces[0].PointAt(u, v));
          unroller.AddFollowingGeometry(dot);
          nodeIds.Add(nodeid++);
        }
      }

      if (points != null) {
        unroller.AddFollowingGeometry(points);
      }

      unroller.RelativeTolerance = tolerance.As(unit) * 2;
      unroller.AbsoluteTolerance = tolerance.As(unit);

      Brep[] flattened = unroller.PerformUnroll(out Curve[] inclCrvs, out Point3d[] inclPts,
        out TextDot[] inclNodes);
      if (flattened.Length == 0) {
        throw new Exception(
          " Unable to unroll surface for re-meshing, the curvature is likely too high! Try with a less 'dramatic' curvature.");
      }

      var curves = new List<Curve>();
      var elem1ds = new List<GsaElement1D>();
      var mem1ds = new List<GsaMember1D>();
      int nCrvs = inCurves?.Count ?? 0;
      int nElem1ds = inElem1ds?.Count ?? 0;
      var elemSections = new Dictionary<int, GsaSection>();
      var memSections = new Dictionary<int, GsaSection>();
      int elemid = 1;
      int memid = 1;
      foreach (Curve crv in inclCrvs) {
        int id = unroller.FollowingGeometryIndex(crv);
        int type = types[id];
        switch (type) {
          case 0:
            curves.Add(crv);
            break;

          case 1:
            inElem1ds[id - nCrvs].Line = new LineCurve(crv.PointAtStart, crv.PointAtEnd);
            inElem1ds[id - nCrvs].Id = elemid;
            elemSections.Add(elemid++, inElem1ds[id - nCrvs].Section);
            elem1ds.Add(inElem1ds[id - nCrvs]);
            break;

          case 2:
            var mem1d = new GsaMember1D(crv) {
              ApiMember = inMem1ds[id - nCrvs - nElem1ds].DuplicateApiObject(),
              Id = memid,
            };
            memSections.Add(memid++, inMem1ds[id - nCrvs - nElem1ds].Section);
            mem1ds.Add(mem1d);
            break;
        }
      }

      var nodes = new List<GsaNode>();
      var inclusionPoints = new Point3dList(inclPts);
      foreach (TextDot dot in inclNodes) {
        int id = unroller.FollowingGeometryIndex(dot);
        nodes.Add(inNodes[id]);
        inclusionPoints.Add(dot.Point);
        nodeIds[id] = -1;
      }

      for (int i = 0; i < nodeIds.Count; i++) {
        if (nodeIds[i] < 0) {
          continue;
        }

        inBrep.Surfaces[0].ClosestPoint(inNodes[i].Point, out double u, out double v);
        Point3d pt = flattened[0].Surfaces[0].PointAt(u, v);
        inclusionPoints.Add(pt);
        inNodes[i].Point = pt;
        nodes.Add(inNodes[i]);
      }

      var mem = new GsaMember2D(flattened[0], curves, inclusionPoints);
      mem.ApiMember.MeshSize = new Length(meshSize, unit).Meters;
      mem.ApiMember.MeshMode2d = meshMode;

      var geometry = new GsaGeometry {
        Nodes = nodes,
        Element1ds = elem1ds,
        Member1ds = mem1ds,
        Member2ds = new List<GsaMember2D> { mem }
      };
      var assembly = new ModelAssembly(null, null, null, geometry, null, null, null, unit,
        tolerance, true, null);
      Model model = assembly.GetModel();

      var tempModel = new GsaModel(model);
      ReadOnlyDictionary<int, Node> nodeDict = model.Nodes();
      var elements = new Elements(tempModel);

      var elem2dgoo = elements.Element2ds.OrderBy(item => item.Value.Ids).ToList();
      Mesh mesh = elem2dgoo[0].Value.Mesh;

      Surface flat = flattened[0].Surfaces[0];
      Surface orig = inBrep.Surfaces[0];

      // map flat mesh onto original surface
      mesh.Vertices.CombineIdentical(true, true);
      var vertices = new Point3dList(mesh.Vertices.ToPoint3dArray());
      for (int i = 0; i < vertices.Count; i++) {
        flat.ClosestPoint(vertices[i], out double u, out double v);
        vertices[i] = orig.PointAt(u, v);
      }

      // swap closest points in mapped mesh with original inclusion points
      if (!finalNodes.IsNullOrEmpty()) {
        foreach (Point3d finalNode in finalNodes) {
          int index = Point3dList.ClosestIndexInList(vertices, finalNode);
          vertices[index] = finalNode;
        }
      }

      for (int i = 0; i < mesh.Vertices.Count; i++) {
        mesh.Vertices.SetVertex(i, vertices[i]);
      }

      if (convertNonPlanarQuads) {
        mesh.Faces.ConvertNonPlanarQuadsToTriangles(tolerance.As(unit),
          RhinoMath.DefaultAngleTolerance, 0);
      }

      List<GsaNode> outNodes = null;
      if (!nodes.IsNullOrEmpty()) {
        Member mem2d = model.Members()[elem2dgoo[0].Value.ApiElements[0].ParentMember.Member];
        List<int> topoInts = Topology.Topology_detangler(mem2d.Topology).Item4;
        int add = points?.Count ?? 0;
        outNodes = new List<GsaNode>();
        for (int i = 0; i < nodes.Count; i++) {
          Vector3 pos = nodeDict[topoInts[i + add]].Position;
          var pt = new Point3d(pos.X, pos.Y, pos.Z);
          flat.ClosestPoint(pt, out double u, out double v);
          pt = orig.PointAt(u, v);
          nodes[i].Point = Point3dList.ClosestPointInList(vertices, pt);
          outNodes.Add(nodes[i]);
        }
      }

      List<GsaElement1D> outElem1ds = null;
      if (inclCrvs == null || inclCrvs.Length <= 0) {
        return new Tuple<Mesh, List<GsaNode>, List<GsaElement1D>>(mesh, outNodes, outElem1ds);
      }

      outElem1ds = new List<GsaElement1D>();
      var element1ds = elements.Element1ds.ToDictionary(x => x.Value.Id, x => x.Value);
      foreach (KeyValuePair<int, Element> kvp in model.Elements()) {
        Element elem = kvp.Value;
        if (elem.Topology.Count != 2) {
          continue;
        }

        Vector3 posS = nodeDict[elem.Topology[0]].Position;
        var start = new Point3d(posS.X, posS.Y, posS.Z);
        flat.ClosestPoint(start, out double u1, out double v1);
        start = orig.PointAt(u1, v1);
        Point3d mapPts = Point3dList.ClosestPointInList(vertices, start);

        Vector3 posE = nodeDict[elem.Topology[1]].Position;
        var end = new Point3d(posE.X, posE.Y, posE.Z);
        flat.ClosestPoint(end, out double u2, out double v2);
        end = orig.PointAt(u2, v2);
        Point3d mapPte = Point3dList.ClosestPointInList(vertices, end);

        element1ds[kvp.Key].Line = new LineCurve(mapPts, mapPte);
        outElem1ds.Add(element1ds[kvp.Key]);
      }

      return new Tuple<Mesh, List<GsaNode>, List<GsaElement1D>>(mesh, outNodes, outElem1ds);
    }

    public static Mesh ConvertBrepToTriMeshSolid(Brep brep) {
      MeshingParameters mparams = MeshingParameters.Minimal;
      mparams.JaggedSeams = false;
      mparams.SimplePlanes = true;

      Mesh[] ms = Mesh.CreateFromBrep(brep, mparams);
      var m = new Mesh();
      m.Append(ms);

      if (!m.IsClosed) {
        m.FillHoles();
        if (!m.IsClosed) {
          return null;
        }
      }

      m.Weld(Math.PI);
      m.Faces.ConvertQuadsToTriangles();
      m.CollapseFacesByEdgeLength(false,
        DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry));

      return m;
    }

    /// <summary>
    ///   Method to convert a NURBS curve into a PolyCurve made of lines and arcs.
    ///   Automatically uses Rhino document tolerance if tolerance is not inputted
    /// </summary>
    /// <param name="curve"></param>
    /// <param name="tolerance"></param>
    /// <returns></returns>
    public static Tuple<PolyCurve, Point3dList, List<string>> ConvertMem1dCrv(
      Curve curve, double tolerance = -1) {
      PolyCurve polyCurve = null;
      var crvType = new List<string>();
      var point3ds = new Point3dList();

      if (curve.IsArc()) {
        crvType.Add("");
        crvType.Add("A");
        crvType.Add("");

        point3ds.Add(curve.PointAtStart);
        point3ds.Add(curve.PointAtNormalizedLength(0.5));
        point3ds.Add(curve.PointAtEnd);

        polyCurve = new PolyCurve();
        polyCurve.Append(curve);
      } else {
        if (tolerance < 0) {
          tolerance = DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry);
        }

        if (curve.SpanCount == 1 && curve.Degree > 2) {
          curve = curve.ToPolyline(tolerance, 2, 0, 0);
        }

        if (curve.SpanCount > 1) {
          polyCurve = new PolyCurve();

          if (!curve.IsPolyline()) {
            curve = curve.ToPolyline(tolerance, 2, 0, 0);
          }

          if (!curve.IsValid) {
            throw new Exception(
              " Error converting edge or curve to polyline: please verify input geometry is valid and tolerance is set accordingly with your geometry under GSA Plugin Unit Settings or if unset under Rhino unit settings");
          }

          Curve[] segments = curve.DuplicateSegments();

          foreach (Curve segment in segments) {
            crvType.Add("");
            point3ds.Add(segment.PointAtStart);
            polyCurve.Append(segment);
          }

          crvType.Add("");
          point3ds.Add(segments[segments.Length - 1].PointAtEnd);
        } else {
          crvType.Add("");
          crvType.Add("");

          point3ds.Add(curve.PointAtStart);
          point3ds.Add(curve.PointAtEnd);

          polyCurve = new PolyCurve();
          polyCurve.Append(curve);
        }
      }

      return new Tuple<PolyCurve, Point3dList, List<string>>(polyCurve, point3ds, crvType);
    }

    public static Tuple<PolyCurve, Point3dList, List<string>> ConvertMem2dCrv(
      Curve curve, double tolerance = -1, double parameter = 0.5) {
      if (tolerance < 0) {
        tolerance = DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry);
      }

      PolyCurve polyCurve = curve.ToArcsAndLines(tolerance, 2, 0, 0);
      Curve[] segments = polyCurve != null ? polyCurve.DuplicateSegments() : new[] {
        curve,
      };

      if (segments.Length == 1) {
        if (segments[0].IsClosed) {
          double midPoint = segments[0].GetLength() * parameter;
          bool success = segments[0].LengthParameter(midPoint, out double t);
          if (success) {
            segments = segments[0].Split(t);
          }
          if (! success || segments == null) {
            throw new FailedToSplitVoidException(midPoint);
          }
        }
      }


      var crvType = new List<string>();
      var point3ds = new Point3dList();

      foreach (Curve segment in segments) {
        point3ds.Add(segment.PointAtStart);
        crvType.Add(string.Empty);
        if (!segment.IsArc()) {
          continue;
        }

        point3ds.Add(segment.PointAtNormalizedLength(0.5));
        crvType.Add("A");
      }

      point3ds.Add(segments[segments.Length - 1].PointAtEnd);
      crvType.Add(string.Empty);

      return new Tuple<PolyCurve, Point3dList, List<string>>(polyCurve, point3ds, crvType);
    }

    public static List<List<int>> ConvertMeshToElem2d(Mesh mesh) {
      var topoInts = new List<List<int>>();
      var ngons = mesh.GetNgonAndFacesEnumerable().ToList();

      foreach (List<int> topo in ngons.Select(ngon
        => ngon.BoundaryVertexIndexList().Select(u => (int)u).ToList())) {
        switch (topo.Count) {
          case 3:
          case 4:
            topoInts.Add(topo);
            break;

          case 6: {
              var topo6 = new List<int> {
              topo[0],
              topo[2],
              topo[4],
              topo[1],
              topo[3],
              topo[5],
            };
              topoInts.Add(topo6);
              break;
            }
          case 8: {
              var topo8 = new List<int> {
              topo[0],
              topo[2],
              topo[4],
              topo[6],
              topo[1],
              topo[3],
              topo[5],
              topo[7],
            };
              topoInts.Add(topo8);
              break;
            }
        }
      }

      return topoInts;
    }

    public static Tuple<List<GSAElement>, Point3dList, List<List<int>>> ConvertMeshToElem2d(
      Mesh mesh, bool createQuadraticElements = false) {
      var elems = new List<GSAElement>();
      var topoPts = new Point3dList(mesh.Vertices.ToPoint3dArray());
      var topoInts = new List<List<int>>();
      var ngons = mesh.GetNgonAndFacesEnumerable().ToList();

      foreach (MeshNgon ngon in ngons) {
        var elem = new GSAElement(new Element());
        var topo = ngon.BoundaryVertexIndexList().Select(u => (int)u).ToList();

        switch (topo.Count) {
          case 3 when createQuadraticElements: {
              var pt3 = new Point3d((topoPts[topo[0]].X + topoPts[topo[1]].X) / 2,
                (topoPts[topo[0]].Y + topoPts[topo[1]].Y) / 2,
                (topoPts[topo[0]].Z + topoPts[topo[1]].Z) / 2); // average between verticy 0 and 1
              topo.Add(topoPts.Count);
              topoPts.Add(pt3);
              var pt4 = new Point3d((topoPts[topo[1]].X + topoPts[topo[2]].X) / 2,
                (topoPts[topo[1]].Y + topoPts[topo[2]].Y) / 2,
                (topoPts[topo[1]].Z + topoPts[topo[2]].Z) / 2); // average between verticy 1 and 2
              topo.Add(topoPts.Count);
              topoPts.Add(pt4);
              var pt5 = new Point3d((topoPts[topo[2]].X + topoPts[topo[0]].X) / 2,
                (topoPts[topo[2]].Y + topoPts[topo[0]].Y) / 2,
                (topoPts[topo[2]].Z + topoPts[topo[0]].Z) / 2); // average between verticy 2 and 0
              topo.Add(topoPts.Count);
              topoPts.Add(pt5);

              elem.Type = ElementType.TRI6;
              topoInts.Add(topo);
              break;
            }
          case 3:
            elem.Type = ElementType.TRI3;
            topoInts.Add(topo);
            break;

          case 4 when createQuadraticElements: {
              var pt3 = new Point3d((topoPts[topo[0]].X + topoPts[topo[1]].X) / 2,
                (topoPts[topo[0]].Y + topoPts[topo[1]].Y) / 2,
                (topoPts[topo[0]].Z + topoPts[topo[1]].Z) / 2); // average between verticy 0 and 1
              topo.Add(topoPts.Count);
              topoPts.Add(pt3);
              var pt4 = new Point3d((topoPts[topo[1]].X + topoPts[topo[2]].X) / 2,
                (topoPts[topo[1]].Y + topoPts[topo[2]].Y) / 2,
                (topoPts[topo[1]].Z + topoPts[topo[2]].Z) / 2); // average between verticy 1 and 2
              topo.Add(topoPts.Count);
              topoPts.Add(pt4);
              var pt5 = new Point3d((topoPts[topo[2]].X + topoPts[topo[3]].X) / 2,
                (topoPts[topo[2]].Y + topoPts[topo[3]].Y) / 2,
                (topoPts[topo[2]].Z + topoPts[topo[3]].Z) / 2); // average between verticy 2 and 3
              topo.Add(topoPts.Count);
              topoPts.Add(pt5);
              var pt6 = new Point3d((topoPts[topo[3]].X + topoPts[topo[0]].X) / 2,
                (topoPts[topo[3]].Y + topoPts[topo[0]].Y) / 2,
                (topoPts[topo[3]].Z + topoPts[topo[0]].Z) / 2); // average between verticy 3 and 0
              topo.Add(topoPts.Count);
              topoPts.Add(pt6);

              elem.Type = ElementType.QUAD8;
              topoInts.Add(topo);
              break;
            }
          case 4:
            elem.Type = ElementType.QUAD4;
            topoInts.Add(topo);
            break;

          case 6: {
              elem.Type = ElementType.TRI6;
              var topo6 = new List<int> {
              topo[0],
              topo[2],
              topo[4],
              topo[1],
              topo[3],
              topo[5],
            };
              topoInts.Add(topo6);
              break;
            }
          case 8: {
              elem.Type = ElementType.QUAD8;
              var topo8 = new List<int> {
              topo[0],
              topo[2],
              topo[4],
              topo[6],
              topo[1],
              topo[3],
              topo[5],
              topo[7],
            };
              topoInts.Add(topo8);
              break;
            }

          default: {
              throw new Exception($" Unable to create 2D element from mesh face with {topo.Count} verticies");
            }
        }
        elems.Add(elem);
      }

      return new Tuple<List<GSAElement>, Point3dList, List<List<int>>>(elems, topoPts, topoInts);
    }

    public static Tuple<List<GSAElement>, Point3dList, List<List<int>>, List<List<int>>>
      ConvertMeshToElem3d(Mesh mesh) {
      var elems = new List<GSAElement>();
      var topoPts = new List<Point3d>(mesh.Vertices.ToPoint3dArray());
      var topoInts = new List<List<int>>();
      var faceInts = new List<List<int>>();
      var ngons = mesh.GetNgonAndFacesEnumerable().ToList();

      foreach (MeshNgon ngon in ngons) {
        var elem = new GSAElement(new Element());
        var topo = ngon.BoundaryVertexIndexList().Select(u => (int)u).ToList();
        var faces = ngon.FaceIndexList().Select(u => (int)u).ToList();
        topoInts.Add(topo);
        elem.Type = (topo.Count, faces.Count) switch {
          (4, 4) => ElementType.TETRA4,
          (5, 5) => ElementType.PYRAMID5,
          (6, 5) => ElementType.WEDGE6,
          (8, 6) => ElementType.BRICK8,
          _ => throw new ArgumentException("Mesh Ngon verticy and face count does match any known " +
                        "3D Element type"),
        };
        faceInts.Add(faces);
        elems.Add(elem);
      }

      return new Tuple<List<GSAElement>, Point3dList, List<List<int>>, List<List<int>>>(elems,
        new Point3dList(topoPts), topoInts, faceInts);
    }

    public static Mesh ConvertMeshToTriMeshSolid(Mesh mesh) {
      var m = (Mesh)mesh.Duplicate();
      if (!m.IsClosed) {
        m.FillHoles();
        if (!m.IsClosed) {
          return null;
        }
      }

      m.Weld(Math.PI);
      m.Faces.ConvertQuadsToTriangles();

      return m;
    }

    /// <summary>
    ///   Method to convert a NURBS Brep into a planar trimmed surface with PolyCurve
    ///   internal and external edges of lines and arcs.
    ///   BRep conversion to planar routine first converts the external edge to a PolyCurve
    ///   of lines and arcs and uses these controlpoints to fit a plane through points.
    ///   Input list of curves and list of points to be included in 2D Member;
    ///   lines and curves will automatically be projected onto planar Brep plane
    ///   Will output 3 Tuples:
    ///   (edgeTuple, voidTuple, inclTuple)
    ///   edgeTuple:
    ///   (edge_crv, m_topo, m_topoType)
    ///   - PolyCurve
    ///   - TopologyList of control points
    ///   - TopoTypeList (" " or "a") corrosponding to control points
    ///   voidTuple:
    ///   (void_crvs, void_topo, void_topoType)
    ///   - List of PolyCurves for internal (void) curves
    ///   - Corrosponding list of topology points
    ///   - Corrosponding list of topologytypes
    ///   inclTuple:
    ///   (incl_crvs, incl_topo, incl_topoType, inclPts)
    ///   - List of PolyCurves for internal (void) curves
    ///   - Corrosponding list of topology points
    ///   - Corrosponding list of topologytypes
    ///   - List of inclusion points
    /// </summary>
    /// <param name="brep"></param>
    /// <param name="inclCrvs"></param>
    /// <param name="inclPts"></param>
    /// <param name="tolerance"></param>
    /// <returns></returns>
    public static
      Tuple<Tuple<PolyCurve, Point3dList, List<string>>,
        Tuple<List<PolyCurve>, List<Point3dList>, List<List<string>>>,
        Tuple<List<PolyCurve>, List<Point3dList>, List<List<string>>, Point3dList>>
      ConvertPolyBrepInclusion(
        Brep brep, List<Curve> inclCrvs = null, Point3dList inclPts = null,
        double tolerance = -1) {
      var voidCrvs = new List<PolyCurve>();
      var voidTopo = new List<Point3dList>();
      var voidTopoType = new List<List<string>>();

      var inclPolyCrvs = new List<PolyCurve>();
      var inclTopo = new List<Point3dList>();
      var inclTopoType = new List<List<string>>();

      Curve outer = null;
      var inner = new List<Curve>();
      foreach (BrepLoop brepLoop in brep.Loops) {
        if (brepLoop.LoopType == BrepLoopType.Outer) {
          outer = brepLoop.To3dCurve();
        } else {
          inner.Add(brepLoop.To3dCurve());
        }
      }

      var edges = new List<Curve> {
        outer,
      };
      edges.AddRange(inner);

      Point3dList ctrlPts;
      if (edges[0].TryGetPolyline(out Polyline tempCrv)) {
        ctrlPts = new Point3dList(tempCrv);
      } else {
        Tuple<PolyCurve, Point3dList, List<string>> convertBadSrf
          = ConvertMem2dCrv(edges[0], tolerance);
        ctrlPts = convertBadSrf.Item2;
      }

      Plane.FitPlaneToPoints(ctrlPts, out Plane plane);

      for (int i = 0; i < edges.Count; i++) {
        if (edges[i].IsPlanar()) {
          continue;
        }

        for (int j = 0; j < edges.Count; j++) {
          edges[j] = Curve.ProjectToPlane(edges[j], plane);
        }
      }

      Tuple<PolyCurve, Point3dList, List<string>> convert = ConvertMem2dCrv(edges[0], tolerance);
      PolyCurve edgeCrv = convert.Item1;
      Point3dList topo = convert.Item2;
      List<string> topoType = convert.Item3;

      for (int i = 1; i < edges.Count; i++) {
        convert = ConvertMem2dCrv(edges[i], tolerance);
        voidCrvs.Add(convert.Item1);
        voidTopo.Add(convert.Item2);
        voidTopoType.Add(convert.Item3);
      }

      if (inclCrvs != null) {
        for (int i = 0; i < inclCrvs.Count; i++) {
          if (inclCrvs[i].IsInPlane(plane,
            DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry))) {
            inclCrvs[i] = Curve.ProjectToPlane(inclCrvs[i], plane);
          } else {
            //TODO - find intersection overlaps or points btw curve and plane: https://developer.rhino3d.com/api/RhinoCommon/html/T_Rhino_Geometry_Intersect_IntersectionEvent.htm
            break;
          }

          convert = ConvertMem2dCrv(inclCrvs[i], tolerance);
          inclPolyCrvs.Add(convert.Item1);
          inclTopo.Add(convert.Item2);
          inclTopoType.Add(convert.Item3);
        }
      }

      if (inclPts != null) {
        var inclPtsWithinTolerance = new Point3dList();
        for (int i = 0; i < inclPts.Count; i++) {
          Point3d tempPt = plane.ClosestPoint(inclPts[i]);
          if (inclPts[i].DistanceTo(tempPt)
            <= DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry)) {
            inclPtsWithinTolerance.Add(tempPt);
          }
        }

        inclPts = inclPtsWithinTolerance;
      }

      var edgeTuple = new Tuple<PolyCurve, Point3dList, List<string>>(edgeCrv, topo, topoType);
      var voidTuple
        = new Tuple<List<PolyCurve>, List<Point3dList>, List<List<string>>>(voidCrvs, voidTopo,
          voidTopoType);
      var inclTuple
        = new Tuple<List<PolyCurve>, List<Point3dList>, List<List<string>>, Point3dList>(
          inclPolyCrvs, inclTopo, inclTopoType, inclPts);

      return new Tuple<Tuple<PolyCurve, Point3dList, List<string>>,
        Tuple<List<PolyCurve>, List<Point3dList>, List<List<string>>>,
        Tuple<List<PolyCurve>, List<Point3dList>, List<List<string>>, Point3dList>>(edgeTuple,
        voidTuple, inclTuple);
    }

    public static Plane CreateBestFitUnitisedPlaneFromPts(
      Point3dList ctrlPts) {
      Plane.FitPlaneToPoints(ctrlPts, out Plane plane);
      plane.Normal.Unitize();
      return new Plane(plane.Origin, plane.Normal);
    }

    internal static Point3dList LoadPanelTopo(Curve curve) {
      curve.TryGetPolyline(out Polyline polyline);
      var topology = new Point3dList();
      foreach (Point3d item in polyline.ToArray()) {
        if (!topology.Contains(item)) {
          topology.Add(item);
        }
      }
      return topology;
    }

    internal static List<List<int>> LoadPanelTopoIndices(Curve curve) {
      curve.TryGetPolyline(out Polyline polyline);
      var topo = new List<int>();
      foreach (Point3d p in polyline.ToList()) {
        int index = polyline.ToList().IndexOf(p);
        if (!topo.Contains(index)) {
          topo.Add(index);
        }
      }
      return new List<List<int>> { topo };
    }
  }

    public class FailedToSplitVoidException : Exception {
      public FailedToSplitVoidException(double midPoint) : base($"Failed to Split Void, using Mid Point at: {midPoint}") { }

    }
}
