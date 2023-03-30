﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Collections;
using Rhino.Geometry;
using Rhino.Geometry.Collections;

namespace GsaGH.Helpers.GH {

  /// <summary>
  /// Helper class to perform some decent geometry approximations from NURBS to poly-geometry
  /// </summary>
  public class RhinoConversions {

    #region Public Methods
    public static PolyCurve BuildArcLineCurveFromPtsAndTopoType(List<Point3d> topology, List<string> topoType = null) {
      var crvs = new PolyCurve();

      for (int i = 0; i < topology.Count - 1; i++) {
        if (topoType != null & topoType[i + 1] == "A") {
          crvs.Append(new Arc(topology[i], topology[i + 1], topology[i + 2]));
          i++;
        }
        else
          crvs.Append(new Line(topology[i], topology[i + 1]));
      }
      return crvs;
    }

    public static Brep BuildBrep(PolyCurve externalEdge, List<PolyCurve> voidCurves = null, double tolerance = -1) {
      if (tolerance < 0)
        tolerance = DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry);

      var curves = new CurveList { externalEdge, };
      if (voidCurves != null)
        curves.AddRange(voidCurves);

      Brep[] brep = Brep.CreatePlanarBreps(curves, tolerance);
      if (brep != null) {
        return brep[0];
      }

      tolerance *= 2;
      brep = Brep.CreatePlanarBreps(curves, tolerance);
      if (brep != null) {
        return brep[0];
      }

      var brep2 = Brep.CreateEdgeSurface(curves);
      return brep2;
    }

    public static Tuple<Mesh, List<GsaNode>, List<GsaElement1d>> ConvertBrepToMesh(Brep brep, List<Point3d> points, List<GsaNode> inNodes, List<Curve> inCurves, List<GsaElement1d> inElem1ds, List<GsaMember1d> inMem1ds, double meshSize, LengthUnit unit, Length tolerance) {
      Brep inBrep = brep.DuplicateBrep();
      inBrep.Faces.ShrinkFaces();
      var unroller = new Unroller(inBrep);
      var types = new List<int>();
      if (inCurves != null) {
        foreach (Curve crv in inCurves) {
          unroller.AddFollowingGeometry(crv);
          types.Add(0);
        }
      }
      if (inElem1ds != null) {
        foreach (GsaElement1d elem in inElem1ds) {
          unroller.AddFollowingGeometry(elem.Line);
          types.Add(1);
        }
      }
      if (inMem1ds != null) {
        foreach (GsaMember1d mem1d in inMem1ds) {
          unroller.AddFollowingGeometry(mem1d.PolyCurve);
          types.Add(2);
        }
      }

      var nodeIds = new List<int>();
      int nodeid = 0;
      if (inNodes != null) {
        foreach (GsaNode node in inNodes) {
          inBrep.Surfaces[0].ClosestPoint(node.Point, out double u, out double v);
          var dot = new TextDot(node.Name, inBrep.Surfaces[0].PointAt(u, v));
          unroller.AddFollowingGeometry(dot);
          nodeIds.Add(nodeid++);
        }
      }
      if (points != null)
        unroller.AddFollowingGeometry(points);

      unroller.RelativeTolerance = tolerance.As(unit) * 2;
      unroller.AbsoluteTolerance = tolerance.As(unit);

      Brep[] flattened = unroller.PerformUnroll(out Curve[] inclCrvs, out Point3d[] inclPts, out TextDot[] inclNodes);
      if (flattened.Length == 0) {
        throw new Exception(" Unable to unroll surface for re-meshing, the curvature is likely too high! Try with a less 'dramatic' curvature.");
      }

      var curves = new List<Curve>();
      var elem1ds = new List<GsaElement1d>();
      var mem1ds = new List<GsaMember1d>();
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
            var mem1d = new GsaMember1d(crv) {
              ApiMember = inMem1ds[id - nCrvs - nElem1ds].GetAPI_MemberClone(),
              MeshSize = inMem1ds[id - nCrvs - nElem1ds].MeshSize,
              Id = memid,
            };
            memSections.Add(memid++, inMem1ds[id - nCrvs - nElem1ds].Section);
            mem1ds.Add(mem1d);
            break;
        }
      }

      var nodes = new List<GsaNode>();
      var inclusionPoints = inclPts.ToList();
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

      var mem = new GsaMember2d(flattened[0], curves, inclusionPoints) {
        MeshSize = meshSize,
        Type = MemberType.GENERIC_2D,
      };

      Model model = Export.AssembleModel.Assemble(null, nodes, elem1ds, null, null, mem1ds, new List<GsaMember2d> { mem }, null, null, null, null, null, null, null, null, unit, tolerance, true, null);

      ReadOnlyDictionary<int, Element> elementDict = model.Elements();
      var elementLocalAxesDict = elementDict.Keys.ToDictionary(id => id, id => model.ElementDirectionCosine(id));
      ReadOnlyDictionary<int, Node> nodeDict = model.Nodes();
      Tuple<ConcurrentBag<GsaElement1dGoo>, ConcurrentBag<GsaElement2dGoo>, ConcurrentBag<GsaElement3dGoo>> elementTuple
                = Elements.GetElements(elementDict, nodeDict, model.Sections(), model.Prop2Ds(), model.Prop3Ds(), model.AnalysisMaterials(), model.SectionModifiers(),
                    elementLocalAxesDict, model.Axes(), unit, false);

      var elem2dgoo = elementTuple.Item2.OrderBy(item => item.Value.Ids).ToList();
      Mesh mesh = elem2dgoo[0].Value.Mesh;

      Surface flat = flattened[0].Surfaces[0];
      Surface orig = inBrep.Surfaces[0];

      MeshVertexList vertices = mesh.Vertices;
      for (int i = 0; i < vertices.Count; i++) {
        flat.ClosestPoint(vertices.Point3dAt(i), out double u, out double v);
        Point3d mapVertex = orig.PointAt(u, v);
        vertices.SetVertex(i, mapVertex);
      }

      mesh.Faces.ConvertNonPlanarQuadsToTriangles(
          tolerance.As(unit), Rhino.RhinoMath.DefaultAngleTolerance, 0);

      List<GsaNode> outNodes = null;
      if (nodes != null && nodes.Count > 0) {
        Member mem2d = model.Members()[elem2dgoo[0].Value.ApiElements[0].ParentMember.Member];
        List<int> topoInts = Topology.Topology_detangler(mem2d.Topology).Item4;
        int add = points?.Count ?? 0;
        outNodes = new List<GsaNode>();
        for (int i = 0; i < nodes.Count; i++) {
          Vector3 pos = nodeDict[topoInts[i + add]].Position;
          var pt = new Point3d(pos.X, pos.Y, pos.Z);
          flat.ClosestPoint(pt, out double u, out double v);
          Point3d mapPt = orig.PointAt(u, v);
          nodes[i].Point = mapPt;
          outNodes.Add(nodes[i]);
        }
      }

      List<GsaElement1d> outElem1ds = null;
      if (inclCrvs == null || inclCrvs.Length <= 0) {
        return new Tuple<Mesh, List<GsaNode>, List<GsaElement1d>>(mesh, outNodes, outElem1ds);
      }

      {
        outElem1ds = new List<GsaElement1d>();
        ReadOnlyDictionary<int, Element> elemDict = model.Elements();
        ReadOnlyDictionary<int, Section> sDict = model.Sections();
        ReadOnlyDictionary<int, SectionModifier> modDict = model.SectionModifiers();
        ReadOnlyDictionary<int, AnalysisMaterial> aDict = model.AnalysisMaterials();
        elementLocalAxesDict = elemDict.Keys.ToDictionary(id => id, id => model.ElementDirectionCosine(id));
        foreach (KeyValuePair<int, Element> kvp in elemDict) {
          Element elem = kvp.Value;
          if (elem.Topology.Count != 2)
            continue;
          Vector3 posS = nodeDict[elem.Topology[0]].Position;
          var start = new Point3d(posS.X, posS.Y, posS.Z);
          flat.ClosestPoint(start, out double us, out double vs);
          Point3d mapPts = orig.PointAt(us, vs);
          Vector3 posE = nodeDict[elem.Topology[1]].Position;
          var end = new Point3d(posE.X, posE.Y, posE.Z);
          flat.ClosestPoint(end, out double ue, out double ve);
          Point3d mapPte = orig.PointAt(ue, ve);
          var elem1d = new GsaElement1d(elemDict, kvp.Key, nodeDict, sDict, modDict, aDict, elementLocalAxesDict, unit) {
            Line = new LineCurve(mapPts, mapPte),
          };
          elem1d.Section = elem1d.ApiElement.ParentMember.Member > 0
            ? memSections[elem1d.ApiElement.ParentMember.Member]
            : elemSections[kvp.Key];
          outElem1ds.Add(elem1d);
        }
      }

      return new Tuple<Mesh, List<GsaNode>, List<GsaElement1d>>(mesh, outNodes, outElem1ds);
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
        if (!m.IsClosed)
          return null;
      }
      m.Weld(Math.PI);
      m.Faces.ConvertQuadsToTriangles();
      m.CollapseFacesByEdgeLength(false, DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry));

      return m;
    }

    /// <summary>
    /// Method to convert a NURBS curve into a PolyCurve made of lines and arcs.
    /// Automatically uses Rhino document tolerance if tolerance is not inputted
    /// </summary>
    /// <param name="curve"></param>
    /// <param name="tolerance"></param>
    /// <returns></returns>
    public static Tuple<PolyCurve, List<Point3d>, List<string>> ConvertMem1dCrv(Curve curve, double tolerance = -1) {
      PolyCurve polyCurve = null;
      var crvType = new List<string>();
      var point3ds = new List<Point3d>();

      if (curve.IsArc()) {
        crvType.Add("");
        crvType.Add("A");
        crvType.Add("");

        point3ds.Add(curve.PointAtStart);
        point3ds.Add(curve.PointAtNormalizedLength(0.5));
        point3ds.Add(curve.PointAtEnd);

        polyCurve = new PolyCurve();
        polyCurve.Append(curve);
      }
      else {
        if (tolerance < 0)
          tolerance = DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry);
        if (curve.SpanCount == 1 && curve.Degree > 2) {
          curve = curve.ToPolyline(tolerance, 2, 0, 0);
        }
        if (curve.SpanCount > 1) {
          polyCurve = new PolyCurve();

          if (!curve.IsPolyline())
            curve = curve.ToPolyline(tolerance, 2, 0, 0);
          if (!curve.IsValid)
            throw new Exception(" Error converting edge or curve to polyline: please verify input geometry is valid and tolerance is set accordingly with your geometry under GSA Plugin Unit Settings or if unset under Rhino unit settings");

          Curve[] segments = curve.DuplicateSegments();

          foreach (Curve segment in segments) {
            crvType.Add("");
            point3ds.Add(segment.PointAtStart);
            polyCurve.Append(segment);
          }
          crvType.Add("");
          point3ds.Add(segments[segments.Length - 1].PointAtEnd);
        }
        else {
          crvType.Add("");
          crvType.Add("");

          point3ds.Add(curve.PointAtStart);
          point3ds.Add(curve.PointAtEnd);

          polyCurve = new PolyCurve();
          polyCurve.Append(curve);
        }
      }

      return new Tuple<PolyCurve, List<Point3d>, List<string>>(polyCurve, point3ds, crvType);
    }

    public static Tuple<PolyCurve, List<Point3d>, List<string>> ConvertMem2dCrv(Curve curve, double tolerance = -1) {
      if (tolerance < 0)
        tolerance = DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry);

      PolyCurve polyCurve = curve.ToArcsAndLines(tolerance, 2, 0, 0);
      Curve[] segments = polyCurve != null
        ? polyCurve.DuplicateSegments()
        : new[] { curve };

      if (segments.Length == 1) {
        if (segments[0].IsClosed) {
          segments = segments[0].Split(0.5);
        }
      }

      var crvType = new List<string>();
      var point3ds = new List<Point3d>();

      foreach (Curve segment in segments) {
        point3ds.Add(segment.PointAtStart);
        crvType.Add("");
        if (!segment.IsArc()) {
          continue;
        }

        point3ds.Add(segment.PointAtNormalizedLength(0.5));
        crvType.Add("A");
      }
      point3ds.Add(segments[segments.Length - 1].PointAtEnd);
      crvType.Add("");

      return new Tuple<PolyCurve, List<Point3d>, List<string>>(polyCurve, point3ds, crvType);
    }

    public static List<List<int>> ConvertMeshToElem2d(Mesh mesh) {
      var topoInts = new List<List<int>>();
      var ngons = mesh.GetNgonAndFacesEnumerable().ToList();

      foreach (List<int> topo in ngons.Select(ngon => ngon.BoundaryVertexIndexList().Select(u => (int)u).ToList())) {
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

    public static Tuple<List<Element>, List<Point3d>, List<List<int>>> ConvertMeshToElem2d(Mesh mesh, int prop = 1, bool createQuadraticElements = false) {
      var elems = new List<Element>();
      var topoPts = new List<Point3d>(mesh.Vertices.ToPoint3dArray());
      var topoInts = new List<List<int>>();
      var ngons = mesh.GetNgonAndFacesEnumerable().ToList();

      foreach (MeshNgon ngon in ngons) {
        var elem = new Element();
        var topo = ngon.BoundaryVertexIndexList().Select(u => (int)u).ToList();

        switch (topo.Count) {
          case 3 when createQuadraticElements: {
              var pt3 = new Point3d(
                (topoPts[topo[0]].X + topoPts[topo[1]].X) / 2,
                (topoPts[topo[0]].Y + topoPts[topo[1]].Y) / 2,
                (topoPts[topo[0]].Z + topoPts[topo[1]].Z) / 2); // average between verticy 0 and 1
              topo.Add(topoPts.Count);
              topoPts.Add(pt3);
              var pt4 = new Point3d(
                (topoPts[topo[1]].X + topoPts[topo[2]].X) / 2,
                (topoPts[topo[1]].Y + topoPts[topo[2]].Y) / 2,
                (topoPts[topo[1]].Z + topoPts[topo[2]].Z) / 2); // average between verticy 1 and 2
              topo.Add(topoPts.Count);
              topoPts.Add(pt4);
              var pt5 = new Point3d(
                (topoPts[topo[2]].X + topoPts[topo[0]].X) / 2,
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
              var pt3 = new Point3d(
                (topoPts[topo[0]].X + topoPts[topo[1]].X) / 2,
                (topoPts[topo[0]].Y + topoPts[topo[1]].Y) / 2,
                (topoPts[topo[0]].Z + topoPts[topo[1]].Z) / 2); // average between verticy 0 and 1
              topo.Add(topoPts.Count);
              topoPts.Add(pt3);
              var pt4 = new Point3d(
                (topoPts[topo[1]].X + topoPts[topo[2]].X) / 2,
                (topoPts[topo[1]].Y + topoPts[topo[2]].Y) / 2,
                (topoPts[topo[1]].Z + topoPts[topo[2]].Z) / 2); // average between verticy 1 and 2
              topo.Add(topoPts.Count);
              topoPts.Add(pt4);
              var pt5 = new Point3d(
                (topoPts[topo[2]].X + topoPts[topo[3]].X) / 2,
                (topoPts[topo[2]].Y + topoPts[topo[3]].Y) / 2,
                (topoPts[topo[2]].Z + topoPts[topo[3]].Z) / 2); // average between verticy 2 and 3
              topo.Add(topoPts.Count);
              topoPts.Add(pt5);
              var pt6 = new Point3d(
                (topoPts[topo[3]].X + topoPts[topo[0]].X) / 2,
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
        }

        elem.Property = prop;
        elems.Add(elem);
      }

      return new Tuple<List<Element>, List<Point3d>, List<List<int>>>(elems, topoPts, topoInts);
    }

    public static Tuple<List<Element>, List<Point3d>, List<List<int>>, List<List<int>>> ConvertMeshToElem3d(Mesh mesh, int prop = 1) {
      var elems = new List<Element>();
      var topoPts = new List<Point3d>(mesh.Vertices.ToPoint3dArray());
      var topoInts = new List<List<int>>();
      var faceInts = new List<List<int>>();
      var ngons = mesh.GetNgonAndFacesEnumerable().ToList();

      foreach (MeshNgon ngon in ngons) {
        var elem = new Element();
        var topo = ngon.BoundaryVertexIndexList().Select(u => (int)u).ToList();
        topoInts.Add(topo);
        switch (topo.Count) {
          case 4:
            elem.Type = ElementType.TETRA4;
            break;

          case 5:
            elem.Type = ElementType.PYRAMID5;
            break;

          case 6:
            elem.Type = ElementType.WEDGE6;
            break;

          case 8:
            elem.Type = ElementType.BRICK8;
            break;
        }

        var faces = ngon.FaceIndexList().Select(u => (int)u).ToList();
        faceInts.Add(faces);
        elem.Property = prop;
        elems.Add(elem);
      }

      return new Tuple<List<Element>, List<Point3d>, List<List<int>>, List<List<int>>>(elems, topoPts, topoInts, faceInts);
    }

    public static Mesh ConvertMeshToTriMeshSolid(Mesh mesh) {
      var m = (Mesh)mesh.Duplicate();
      if (!m.IsClosed) {
        m.FillHoles();
        if (!m.IsClosed)
          return null;
      }
      m.Weld(Math.PI);
      m.Faces.ConvertQuadsToTriangles();

      return m;
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
        ConvertPolyBrep(Brep brep, double tolerance = -1) {
      var voidCrvs = new List<PolyCurve>();
      var voidTopo = new List<List<Point3d>>();
      var voidTopoType = new List<List<string>>();

      Curve outer = null;
      var inner = new List<Curve>();
      foreach (BrepLoop brepLoop in brep.Loops) {
        if (brepLoop.LoopType == BrepLoopType.Outer) {
          outer = brepLoop.To3dCurve();
        }
        else {
          inner.Add(brepLoop.To3dCurve());
        }
      }
      var edges = new List<Curve> { outer };
      edges.AddRange(inner);

      for (int i = 0; i < edges.Count; i++) {
        if (edges[i].IsPlanar()) {
          continue;
        }

        List<Point3d> ctrlPts;
        if (edges[0].TryGetPolyline(out Polyline polyline))
          ctrlPts = polyline.ToList();
        else {
          Tuple<PolyCurve, List<Point3d>, List<string>> convertBadSrf = ConvertMem2dCrv(edges[0], tolerance);
          ctrlPts = convertBadSrf.Item2;
        }
        Plane.FitPlaneToPoints(ctrlPts, out Plane plane);
        for (int j = 0; j < edges.Count; j++)
          edges[j] = Curve.ProjectToPlane(edges[j], plane);
      }

      Tuple<PolyCurve, List<Point3d>, List<string>> convert = ConvertMem2dCrv(edges[0], tolerance);
      PolyCurve edgeCrv = convert.Item1;
      List<Point3d> point3ds = convert.Item2;
      List<string> topoType = convert.Item3;

      for (int i = 1; i < edges.Count; i++) {
        convert = ConvertMem2dCrv(edges[i], tolerance);
        voidCrvs.Add(convert.Item1);
        voidTopo.Add(convert.Item2);
        voidTopoType.Add(convert.Item3);
      }

      return new Tuple<PolyCurve, List<Point3d>, List<string>, List<PolyCurve>, List<List<Point3d>>, List<List<string>>>
          (edgeCrv, point3ds, topoType, voidCrvs, voidTopo, voidTopoType);
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
        ConvertPolyBrepInclusion(Brep brep, List<Curve> inclCrvs = null, List<Point3d> inclPts = null, double tolerance = -1) {
      var voidCrvs = new List<PolyCurve>();
      var voidTopo = new List<List<Point3d>>();
      var voidTopoType = new List<List<string>>();

      var inclPolyCrvs = new List<PolyCurve>();
      var inclTopo = new List<List<Point3d>>();
      var inclTopoType = new List<List<string>>();

      Curve outer = null;
      var inner = new List<Curve>();
      foreach (BrepLoop brepLoop in brep.Loops) {
        if (brepLoop.LoopType == BrepLoopType.Outer)
          outer = brepLoop.To3dCurve();
        else
          inner.Add(brepLoop.To3dCurve());
      }
      var edges = new List<Curve> { outer };
      edges.AddRange(inner);

      List<Point3d> ctrlPts;
      if (edges[0].TryGetPolyline(out Polyline tempCrv))
        ctrlPts = tempCrv.ToList();
      else {
        Tuple<PolyCurve, List<Point3d>, List<string>> convertBadSrf = ConvertMem2dCrv(edges[0], tolerance);
        ctrlPts = convertBadSrf.Item2;
      }
      Plane.FitPlaneToPoints(ctrlPts, out Plane plane);

      for (int i = 0; i < edges.Count; i++) {
        if (edges[i].IsPlanar()) {
          continue;
        }

        for (int j = 0; j < edges.Count; j++)
          edges[j] = Curve.ProjectToPlane(edges[j], plane);
      }
      Tuple<PolyCurve, List<Point3d>, List<string>> convert = ConvertMem2dCrv(edges[0], tolerance);
      PolyCurve edgeCrv = convert.Item1;
      List<Point3d> topo = convert.Item2;
      List<string> topoType = convert.Item3;

      for (int i = 1; i < edges.Count; i++) {
        convert = ConvertMem2dCrv(edges[i], tolerance);
        voidCrvs.Add(convert.Item1);
        voidTopo.Add(convert.Item2);
        voidTopoType.Add(convert.Item3);
      }

      if (inclCrvs != null) {
        for (int i = 0; i < inclCrvs.Count; i++) {
          if (inclCrvs[i].IsInPlane(plane, DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry)))
            inclCrvs[i] = Curve.ProjectToPlane(inclCrvs[i], plane);
          else {
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
        var inclPtsWithinTolerance = new List<Point3d>();
        for (int i = 0; i < inclPts.Count; i++) {
          Point3d tempPt = plane.ClosestPoint(inclPts[i]);
          if (inclPts[i].DistanceTo(tempPt) <= DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry))
            inclPtsWithinTolerance.Add(tempPt);
        }
        inclPts = inclPtsWithinTolerance;
      }

      var edgeTuple = new Tuple<PolyCurve, List<Point3d>, List<string>>(edgeCrv, topo, topoType);
      var voidTuple = new Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>>(voidCrvs, voidTopo, voidTopoType);
      var inclTuple = new Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>, List<Point3d>>(inclPolyCrvs, inclTopo, inclTopoType, inclPts);

      return new Tuple<Tuple<PolyCurve, List<Point3d>, List<string>>, Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>>, Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>, List<Point3d>>>
          (edgeTuple, voidTuple, inclTuple);
    }

    public static Plane CreateBestFitUnitisedPlaneFromPts(List<Point3d> ctrlPts, bool round = false) {
      Plane.FitPlaneToPoints(ctrlPts, out Plane plane);
      plane.Origin = plane.ClosestPoint(new Point3d(0, 0, 0));
      int dig = UnitsHelper.SignificantDigits;
      plane.Normal.Unitize();
      if (Math.Abs(Math.Round(plane.Normal.Z, dig)) == 1) {
        plane.XAxis = Vector3d.XAxis;
        plane.YAxis = Vector3d.YAxis;
      }

      if (round) {
        plane.OriginX = GsaAPI.ResultHelper.RoundToSignificantDigits(plane.OriginX, dig);
        plane.OriginY = GsaAPI.ResultHelper.RoundToSignificantDigits(plane.OriginY, dig);
        plane.OriginZ = GsaAPI.ResultHelper.RoundToSignificantDigits(plane.OriginZ, dig);

        plane.XAxis.Unitize();
        Vector3d xaxis = plane.XAxis;
        xaxis.X = GsaAPI.ResultHelper.RoundToSignificantDigits(Math.Abs(xaxis.X), dig);
        xaxis.Y = GsaAPI.ResultHelper.RoundToSignificantDigits(Math.Abs(xaxis.Y), dig);
        xaxis.Z = GsaAPI.ResultHelper.RoundToSignificantDigits(Math.Abs(xaxis.Z), dig);
        plane.XAxis = xaxis;

        plane.YAxis.Unitize();
        Vector3d yaxis = plane.YAxis;
        yaxis.X = GsaAPI.ResultHelper.RoundToSignificantDigits(Math.Abs(yaxis.X), dig);
        yaxis.Y = GsaAPI.ResultHelper.RoundToSignificantDigits(Math.Abs(yaxis.Y), dig);
        yaxis.Z = GsaAPI.ResultHelper.RoundToSignificantDigits(Math.Abs(yaxis.Z), dig);
        plane.YAxis = yaxis;

        plane.ZAxis.Unitize();
        Vector3d zaxis = plane.ZAxis;
        zaxis.X = GsaAPI.ResultHelper.RoundToSignificantDigits(Math.Abs(zaxis.X), dig);
        zaxis.Y = GsaAPI.ResultHelper.RoundToSignificantDigits(Math.Abs(zaxis.Y), dig);
        zaxis.Z = GsaAPI.ResultHelper.RoundToSignificantDigits(Math.Abs(zaxis.Z), dig);
        plane.ZAxis = zaxis;
      }
      else {
        plane.OriginX = Math.Round(plane.OriginX, dig);
        plane.OriginY = Math.Round(plane.OriginY, dig);
        plane.OriginZ = Math.Round(plane.OriginZ, dig);

        plane.XAxis.Unitize();
        Vector3d xaxis = plane.XAxis;
        xaxis.X = Math.Round(Math.Abs(xaxis.X), dig);
        xaxis.Y = Math.Round(Math.Abs(xaxis.Y), dig);
        xaxis.Z = Math.Round(Math.Abs(xaxis.Z), dig);
        plane.XAxis = xaxis;

        plane.YAxis.Unitize();
        Vector3d yaxis = plane.YAxis;
        yaxis.X = Math.Round(Math.Abs(yaxis.X), dig);
        yaxis.Y = Math.Round(Math.Abs(yaxis.Y), dig);
        yaxis.Z = Math.Round(Math.Abs(yaxis.Z), dig);
        plane.YAxis = yaxis;

        plane.ZAxis.Unitize();
        Vector3d zaxis = plane.ZAxis;
        zaxis.X = Math.Round(Math.Abs(zaxis.X), dig);
        zaxis.Y = Math.Round(Math.Abs(zaxis.Y), dig);
        zaxis.Z = Math.Round(Math.Abs(zaxis.Z), dig);
        plane.ZAxis = zaxis;
      }

      return plane;
    }

    #endregion Public Methods
  }
}
