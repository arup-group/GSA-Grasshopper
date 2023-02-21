using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using Rhino.Geometry;
using Rhino.Collections;
using Rhino.Geometry.Collections;
using System.Collections.Concurrent;
using GsaGH.Parameters;
using OasysUnits.Units;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using System.Collections.ObjectModel;
using OasysUnits;
using GsaGH.Helpers.Import;

namespace GsaGH.Helpers.GH
{
  /// <summary>
  /// Helper class to perform some decent geometry approximations from NURBS to poly-geometry
  /// </summary>
  public class RhinoConversions
  {
    public static Plane CreateBestFitUnitisedPlaneFromPts(List<Point3d> ctrl_pts, bool round = false)
    {
      Plane pln = Plane.WorldXY;

      // calculate best fit plane:
      Plane.FitPlaneToPoints(ctrl_pts, out pln);

      // change origin to closest point world xyz
      // this will ensure that axes created in same plane will not be duplicated
      pln.Origin = pln.ClosestPoint(new Point3d(0, 0, 0));

      // find significant digits for rounding
      int dig = UnitsHelper.SignificantDigits;

      // unitise the plane normal so we can evaluate if it is XY-type plane
      pln.Normal.Unitize();
      if (Math.Abs(Math.Round(pln.Normal.Z, dig)) == 1) // if normal's z direction is close to vertical
      {
        // set X and Y axis to unit vectors to ensure no funny rotations
        pln.XAxis = Vector3d.XAxis;
        pln.YAxis = Vector3d.YAxis;
      }

      if (round)
      {
        // round origin coordinates
        pln.OriginX = GsaAPI.ResultHelper.RoundToSignificantDigits(pln.OriginX, dig);
        pln.OriginY = GsaAPI.ResultHelper.RoundToSignificantDigits(pln.OriginY, dig);
        pln.OriginZ = GsaAPI.ResultHelper.RoundToSignificantDigits(pln.OriginZ, dig);

        // unitize and round x-axis
        pln.XAxis.Unitize();
        Vector3d xaxis = pln.XAxis;
        xaxis.X = GsaAPI.ResultHelper.RoundToSignificantDigits(Math.Abs(xaxis.X), dig);
        xaxis.Y = GsaAPI.ResultHelper.RoundToSignificantDigits(Math.Abs(xaxis.Y), dig);
        xaxis.Z = GsaAPI.ResultHelper.RoundToSignificantDigits(Math.Abs(xaxis.Z), dig);
        pln.XAxis = xaxis;

        // unitize and round y-axis
        pln.YAxis.Unitize();
        Vector3d yaxis = pln.YAxis;
        yaxis.X = GsaAPI.ResultHelper.RoundToSignificantDigits(Math.Abs(yaxis.X), dig);
        yaxis.Y = GsaAPI.ResultHelper.RoundToSignificantDigits(Math.Abs(yaxis.Y), dig);
        yaxis.Z = GsaAPI.ResultHelper.RoundToSignificantDigits(Math.Abs(yaxis.Z), dig);
        pln.YAxis = yaxis;

        // unitize and round z-axis
        pln.ZAxis.Unitize();
        Vector3d zaxis = pln.ZAxis;
        zaxis.X = GsaAPI.ResultHelper.RoundToSignificantDigits(Math.Abs(zaxis.X), dig);
        zaxis.Y = GsaAPI.ResultHelper.RoundToSignificantDigits(Math.Abs(zaxis.Y), dig);
        zaxis.Z = GsaAPI.ResultHelper.RoundToSignificantDigits(Math.Abs(zaxis.Z), dig);
        pln.ZAxis = zaxis;
      }
      else
      {
        // round origin coordinates
        pln.OriginX = Math.Round(pln.OriginX, dig);
        pln.OriginY = Math.Round(pln.OriginY, dig);
        pln.OriginZ = Math.Round(pln.OriginZ, dig);

        // unitize and round x-axis
        pln.XAxis.Unitize();
        Vector3d xaxis = pln.XAxis;
        xaxis.X = Math.Round(Math.Abs(xaxis.X), dig);
        xaxis.Y = Math.Round(Math.Abs(xaxis.Y), dig);
        xaxis.Z = Math.Round(Math.Abs(xaxis.Z), dig);
        pln.XAxis = xaxis;

        // unitize and round y-axis
        pln.YAxis.Unitize();
        Vector3d yaxis = pln.YAxis;
        yaxis.X = Math.Round(Math.Abs(yaxis.X), dig);
        yaxis.Y = Math.Round(Math.Abs(yaxis.Y), dig);
        yaxis.Z = Math.Round(Math.Abs(yaxis.Z), dig);
        pln.YAxis = yaxis;

        // unitize and round z-axis
        pln.ZAxis.Unitize();
        Vector3d zaxis = pln.ZAxis;
        zaxis.X = Math.Round(Math.Abs(zaxis.X), dig);
        zaxis.Y = Math.Round(Math.Abs(zaxis.Y), dig);
        zaxis.Z = Math.Round(Math.Abs(zaxis.Z), dig);
        pln.ZAxis = zaxis;
      }

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
        if (tolerance < 0)
          tolerance = DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry); // use the user set unit
        if (crv.SpanCount == 1 && crv.Degree > 2)
        {
          crv = crv.ToPolyline(tolerance, 2, 0, 0);
        }
        if (crv.SpanCount > 1) // polyline (or assumed polyline, we will take controlpoints)
        {
          m_crv = new PolyCurve();

          if (!crv.IsPolyline())
            crv = crv.ToPolyline(tolerance, 2, 0, 0);
          if (!crv.IsValid)
            throw new Exception(" Error converting edge or curve to polyline: please verify input geometry is valid and tolerance is set accordingly with your geometry under GSA Plugin Unit Settings or if unset under Rhino unit settings");

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
        tolerance = DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry); // use the user set unit

      PolyCurve m_crv = crv.ToArcsAndLines(tolerance, 2, 0, 0);
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
            Tuple<PolyCurve, List<Point3d>, List<string>> convertBadSrf = ConvertMem2dCrv(edges[0], tolerance);
            ctrl_pts = convertBadSrf.Item2;
          }
          Plane.FitPlaneToPoints(ctrl_pts, out Plane plane);
          for (int j = 0; j < edges.Count; j++)
            edges[j] = Curve.ProjectToPlane(edges[j], plane);
        }
      }

      Tuple<PolyCurve, List<Point3d>, List<string>> convert = ConvertMem2dCrv(edges[0], tolerance);
      PolyCurve edge_crv = convert.Item1;
      List<Point3d> m_topo = convert.Item2;
      List<string> m_topoType = convert.Item3;

      for (int i = 1; i < edges.Count; i++)
      {
        convert = ConvertMem2dCrv(edges[i], tolerance);
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
          outer = brep.Loops[i].To3dCurve();
        else
          inner.Add(brep.Loops[i].To3dCurve());
      }
      List<Curve> edges = new List<Curve>();
      edges.Add(outer);
      edges.AddRange(inner);

      List<Point3d> ctrl_pts;
      if (edges[0].TryGetPolyline(out Polyline temp_crv))
        ctrl_pts = temp_crv.ToList();
      else
      {
        Tuple<PolyCurve, List<Point3d>, List<string>> convertBadSrf = ConvertMem2dCrv(edges[0], tolerance);
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
      Tuple<PolyCurve, List<Point3d>, List<string>> convert = ConvertMem2dCrv(edges[0], tolerance);
      PolyCurve edge_crv = convert.Item1;
      List<Point3d> m_topo = convert.Item2;
      List<string> m_topoType = convert.Item3;

      for (int i = 1; i < edges.Count; i++)
      {
        convert = ConvertMem2dCrv(edges[i], tolerance);
        void_crvs.Add(convert.Item1);
        void_topo.Add(convert.Item2);
        void_topoType.Add(convert.Item3);
      }

      if (inclCrvs != null)
      {
        for (int i = 0; i < inclCrvs.Count; i++)
        {
          if (inclCrvs[i].IsInPlane(plane, DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry)))
            inclCrvs[i] = Curve.ProjectToPlane(inclCrvs[i], plane);
          else
          {
            //TODO - find intersection overlaps or points btw curve and plane: https://developer.rhino3d.com/api/RhinoCommon/html/T_Rhino_Geometry_Intersect_IntersectionEvent.htm
            break;
          }
          convert = ConvertMem2dCrv(inclCrvs[i], tolerance);
          incl_crvs.Add(convert.Item1);
          incl_topo.Add(convert.Item2);
          incl_topoType.Add(convert.Item3);
        }
      }

      if (inclPts != null)
      {
        List<Point3d> inclPtsWithinTolerance = new List<Point3d>();
        for (int i = 0; i < inclPts.Count; i++)
        {
          Point3d tempPt = plane.ClosestPoint(inclPts[i]);
          if (inclPts[i].DistanceTo(tempPt) <= DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry))
            inclPtsWithinTolerance.Add(tempPt);
        }
        inclPts = inclPtsWithinTolerance;
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
        tolerance = DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry); // use the user set units

      CurveList curves = new CurveList
            {
                externalEdge
            };
      if (voidCurves != null)
        curves.AddRange(voidCurves);

      Brep[] brep = Brep.CreatePlanarBreps(curves, tolerance);
      if (brep == null)
      {
        tolerance = tolerance * 2;
        brep = Brep.CreatePlanarBreps(curves, tolerance);
        if (brep == null)
        {
          Brep brep2 = Brep.CreateEdgeSurface(curves);
          if (brep2 == null)
            return null;
          else
            return brep2;
        }
        else
          return brep[0];
      }
      else
        return brep[0];
    }

    public static PolyCurve ConvertCurveMem1d(Curve curve, double tolerance = -1)
    {
      Tuple<PolyCurve, List<Point3d>, List<string>> convertCrv = ConvertMem1dCrv(curve, tolerance);
      return convertCrv.Item1;
    }
    public static PolyCurve ConvertCurveMem2d(Curve curve, double tolerance = -1)
    {
      Tuple<PolyCurve, List<Point3d>, List<string>> convertCrv = ConvertMem2dCrv(curve, tolerance);
      return convertCrv.Item1;
    }

    public static PolyCurve BuildArcLineCurveFromPtsAndTopoType(List<Point3d> topology, List<string> topo_type = null)
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
          = ConvertPolyBrep(brep, tolerance);
      return BuildBrep(convertBrep.Item1, convertBrep.Item4);
    }
    public static List<List<int>> ConvertMeshToElem2d(Mesh mesh)
    {
      List<List<int>> topoInts = new List<List<int>>();

      // get list of mesh ngons (faces in mesh with both tri/quads and ngons above 4 verticies)
      List<MeshNgon> ngons = mesh.GetNgonAndFacesEnumerable().ToList();

      for (int i = 0; i < ngons.Count; i++)
      {
        Element elem = new Element();
        List<int> topo = ngons[i].BoundaryVertexIndexList().Select(u => (int)u).ToList();

        if (topo.Count == 3)
        {
          topoInts.Add(topo);
        }
        else if (topo.Count == 4)
        {
          topoInts.Add(topo);
        }
        else if (topo.Count == 6)
        {
          List<int> topo6 = new List<int>();
          topo6.Add(topo[0]);
          topo6.Add(topo[2]);
          topo6.Add(topo[4]);
          topo6.Add(topo[1]);
          topo6.Add(topo[3]);
          topo6.Add(topo[5]);
          topoInts.Add(topo6);
        }
        else if (topo.Count == 8)
        {
          List<int> topo8 = new List<int>();
          topo8.Add(topo[0]);
          topo8.Add(topo[2]);
          topo8.Add(topo[4]);
          topo8.Add(topo[6]);
          topo8.Add(topo[1]);
          topo8.Add(topo[3]);
          topo8.Add(topo[5]);
          topo8.Add(topo[7]);
          topoInts.Add(topo8);
        }
      }

      return topoInts;
    }
    public static Tuple<List<Element>, List<Point3d>, List<List<int>>> ConvertMeshToElem2d(Mesh mesh, int prop = 1, bool createQuadraticElements = false)
    {
      // list of elements to output
      List<Element> elems = new List<Element>();
      // list of points from mesh topology
      List<Point3d> topoPts = new List<Point3d>(mesh.Vertices.ToPoint3dArray());
      // list of reference ids between elements and points
      List<List<int>> topoInts = new List<List<int>>();

      // get list of mesh ngons (faces in mesh with both tri/quads and ngons above 4 verticies)
      List<MeshNgon> ngons = mesh.GetNgonAndFacesEnumerable().ToList();

      for (int i = 0; i < ngons.Count; i++)
      {
        Element elem = new Element();
        List<int> topo = ngons[i].BoundaryVertexIndexList().Select(u => (int)u).ToList();

        if (topo.Count == 3)
        {
          if (createQuadraticElements)
          {
            // to create a tri6 we add mid-points
            Point3d pt3 = new Point3d(
                (topoPts[topo[0]].X + topoPts[topo[1]].X) / 2,
                (topoPts[topo[0]].Y + topoPts[topo[1]].Y) / 2,
                (topoPts[topo[0]].Z + topoPts[topo[1]].Z) / 2); // average between verticy 0 and 1
            topo.Add(topoPts.Count);
            topoPts.Add(pt3);
            Point3d pt4 = new Point3d(
                (topoPts[topo[1]].X + topoPts[topo[2]].X) / 2,
                (topoPts[topo[1]].Y + topoPts[topo[2]].Y) / 2,
                (topoPts[topo[1]].Z + topoPts[topo[2]].Z) / 2); // average between verticy 1 and 2
            topo.Add(topoPts.Count);
            topoPts.Add(pt4);
            Point3d pt5 = new Point3d(
                (topoPts[topo[2]].X + topoPts[topo[0]].X) / 2,
                (topoPts[topo[2]].Y + topoPts[topo[0]].Y) / 2,
                (topoPts[topo[2]].Z + topoPts[topo[0]].Z) / 2); // average between verticy 2 and 0
            topo.Add(topoPts.Count);
            topoPts.Add(pt5);

            elem.Type = ElementType.TRI6;
            topoInts.Add(topo);
          }
          else
          {
            elem.Type = ElementType.TRI3;
            topoInts.Add(topo);
          }
        }
        else if (topo.Count == 4)
        {
          if (createQuadraticElements)
          {
            // to create a tri6 we add mid-points
            Point3d pt3 = new Point3d(
                (topoPts[topo[0]].X + topoPts[topo[1]].X) / 2,
                (topoPts[topo[0]].Y + topoPts[topo[1]].Y) / 2,
                (topoPts[topo[0]].Z + topoPts[topo[1]].Z) / 2); // average between verticy 0 and 1
            topo.Add(topoPts.Count);
            topoPts.Add(pt3);
            Point3d pt4 = new Point3d(
                (topoPts[topo[1]].X + topoPts[topo[2]].X) / 2,
                (topoPts[topo[1]].Y + topoPts[topo[2]].Y) / 2,
                (topoPts[topo[1]].Z + topoPts[topo[2]].Z) / 2); // average between verticy 1 and 2
            topo.Add(topoPts.Count);
            topoPts.Add(pt4);
            Point3d pt5 = new Point3d(
                (topoPts[topo[2]].X + topoPts[topo[3]].X) / 2,
                (topoPts[topo[2]].Y + topoPts[topo[3]].Y) / 2,
                (topoPts[topo[2]].Z + topoPts[topo[3]].Z) / 2); // average between verticy 2 and 3
            topo.Add(topoPts.Count);
            topoPts.Add(pt5);
            Point3d pt6 = new Point3d(
                (topoPts[topo[3]].X + topoPts[topo[0]].X) / 2,
                (topoPts[topo[3]].Y + topoPts[topo[0]].Y) / 2,
                (topoPts[topo[3]].Z + topoPts[topo[0]].Z) / 2); // average between verticy 3 and 0
            topo.Add(topoPts.Count);
            topoPts.Add(pt6);

            elem.Type = ElementType.QUAD8;
            topoInts.Add(topo);
          }
          else
          {
            elem.Type = ElementType.QUAD4;
            topoInts.Add(topo);
          }

        }
        else if (topo.Count == 6)
        {
          elem.Type = ElementType.TRI6;
          List<int> topo6 = new List<int>();
          topo6.Add(topo[0]);
          topo6.Add(topo[2]);
          topo6.Add(topo[4]);
          topo6.Add(topo[1]);
          topo6.Add(topo[3]);
          topo6.Add(topo[5]);
          topoInts.Add(topo6);
        }
        else if (topo.Count == 8)
        {
          elem.Type = ElementType.QUAD8;
          List<int> topo8 = new List<int>();
          topo8.Add(topo[0]);
          topo8.Add(topo[2]);
          topo8.Add(topo[4]);
          topo8.Add(topo[6]);
          topo8.Add(topo[1]);
          topo8.Add(topo[3]);
          topo8.Add(topo[5]);
          topo8.Add(topo[7]);
          topoInts.Add(topo8);
        }

        elem.Property = prop;
        elems.Add(elem);
      }

      return new Tuple<List<Element>, List<Point3d>, List<List<int>>>(elems, topoPts, topoInts);
    }

    public static Tuple<List<Element>, List<Point3d>, List<List<int>>, List<List<int>>> ConvertMeshToElem3d(Mesh mesh, int prop = 1)
    {
      // list of elements to output
      List<Element> elems = new List<Element>();
      // list of points from mesh topology
      List<Point3d> topoPts = new List<Point3d>(mesh.Vertices.ToPoint3dArray());
      // list of reference ids between elements and points
      List<List<int>> topoInts = new List<List<int>>();
      // list of reference ids between elements and faces
      List<List<int>> faceInts = new List<List<int>>();

      // get list of mesh ngons (faces in mesh with both tri/quads and ngons above 4 verticies)
      List<MeshNgon> ngons = mesh.GetNgonAndFacesEnumerable().ToList();

      for (int i = 0; i < ngons.Count; i++)
      {
        // create new element
        Element elem = new Element();
        // copy topology list
        List<int> topo = ngons[i].BoundaryVertexIndexList().Select(u => (int)u).ToList();
        topoInts.Add(topo);
        // switch between number of points and set element type
        if (topo.Count == 4)
        {
          elem.Type = ElementType.TETRA4;

        }
        else if (topo.Count == 5)
        {
          elem.Type = ElementType.PYRAMID5;
        }
        else if (topo.Count == 6)
        {
          elem.Type = ElementType.WEDGE6;
        }
        else if (topo.Count == 8)
        {
          elem.Type = ElementType.BRICK8;
        }

        // add the facelist to list of facelists
        List<int> faces = ngons[i].FaceIndexList().Select(u => (int)u).ToList();
        faceInts.Add(faces);

        // set the element property
        elem.Property = prop;

        // add element to list of elements
        elems.Add(elem);
      }

      return new Tuple<List<Element>, List<Point3d>, List<List<int>>, List<List<int>>>(elems, topoPts, topoInts, faceInts);
    }

    public static Tuple<Mesh, List<GsaNode>, List<GsaElement1d>> ConvertBrepToMesh(Brep brep, List<Point3d> points, List<GsaNode> in_nodes, List<Curve> in_curves, List<GsaElement1d> in_elem1ds, List<GsaMember1d> in_mem1ds, double meshSize, LengthUnit unit, Length tolerance)
    {
      Brep in_brep = brep.DuplicateBrep();
      in_brep.Faces.ShrinkFaces();

      // set up unroller
      Unroller unroller = new Unroller(in_brep);

      List<int> types = new List<int>();
      if (in_curves != null)
      {
        foreach (Curve crv in in_curves)
        {
          unroller.AddFollowingGeometry(crv);
          types.Add(0);
        }
      }
      if (in_elem1ds != null)
      {
        foreach (GsaElement1d elem in in_elem1ds)
        {
          unroller.AddFollowingGeometry(elem.Line);
          types.Add(1);
        }
      }
      if (in_mem1ds != null)
      {
        foreach (GsaMember1d mem1d in in_mem1ds)
        {
          unroller.AddFollowingGeometry(mem1d.PolyCurve);
          types.Add(2);
        }
      }

      List<int> nodeIds= new List<int>();
      int nodeid = 0;
      if (in_nodes != null)
      {
        foreach (GsaNode node in in_nodes)
        {
          in_brep.Surfaces[0].ClosestPoint(node.Point, out double u, out double v);
          TextDot dot = new TextDot(node.Name, in_brep.Surfaces[0].PointAt(u, v));
          unroller.AddFollowingGeometry(dot);
          nodeIds.Add(nodeid++);
        }
      }
      if (points != null)
        unroller.AddFollowingGeometry(points);

      unroller.RelativeTolerance = tolerance.As(unit) * 2;
      unroller.AbsoluteTolerance = tolerance.As(unit);

      // create list of flattened geometry
      Point3d[] inclPts;
      Curve[] inclCrvs;
      TextDot[] inclNodes;
      // perform unroll
      Brep[] flattened = unroller.PerformUnroll(out inclCrvs, out inclPts, out inclNodes);
      if (flattened.Length == 0)
      {
        throw new Exception(" Unable to unroll surface for re-meshing, the curvature is likely too high! Try with a less 'dramatic' curvature.");
      }

      List<Curve> curves = new List<Curve>();
      List<GsaElement1d> elem1ds = new List<GsaElement1d>();
      List<GsaMember1d> mem1ds = new List<GsaMember1d>();
      int nCrvs = in_curves == null ? 0 : in_curves.Count;
      int nElem1ds = in_elem1ds == null ? 0 : in_elem1ds.Count;
      Dictionary<int, GsaSection> elemSections = new Dictionary<int, GsaSection>();
      Dictionary<int, GsaSection> memSections = new Dictionary<int, GsaSection>();
      int elemid = 1;
      int memid = 1;
      foreach (Curve crv in inclCrvs)
      {
        int id = unroller.FollowingGeometryIndex(crv);
        int type = types[id];
        switch (type)
        {
          case 0:
            curves.Add(crv);
            break;
          case 1:
            in_elem1ds[id - nCrvs].Line = new LineCurve(crv.PointAtStart, crv.PointAtEnd);
            in_elem1ds[id - nCrvs].Id = elemid;
            elemSections.Add(elemid++, in_elem1ds[id - nCrvs].Section);
            elem1ds.Add(in_elem1ds[id - nCrvs]);
            break;
          case 2:
            GsaMember1d mem1d = new GsaMember1d(crv);
            mem1d.ApiMember = in_mem1ds[id - nCrvs - nElem1ds].GetAPI_MemberClone();
            mem1d.MeshSize = in_mem1ds[id - nCrvs - nElem1ds].MeshSize;
            mem1d.Id = memid;
            memSections.Add(memid++, in_mem1ds[id - nCrvs - nElem1ds].Section);
            mem1ds.Add(mem1d);
            break;
        }
      }

      List<GsaNode> nodes = new List<GsaNode>();
      List<Point3d> inclusionPoints = inclPts.ToList();
      foreach (TextDot dot in inclNodes)
      {
        int id = unroller.FollowingGeometryIndex(dot);
        nodes.Add(in_nodes[id]);
        inclusionPoints.Add(dot.Point);
        nodeIds[id] = -1;
      }
      for(int i = 0; i < nodeIds.Count; i++)
      {
        if (nodeIds[i] >= 0)
        {
          in_brep.Surfaces[0].ClosestPoint(in_nodes[i].Point, out double u, out double v);
          Point3d pt = flattened[0].Surfaces[0].PointAt(u, v);
          inclusionPoints.Add(pt);
          in_nodes[i].Point = pt;
          nodes.Add(in_nodes[i]);
        }
      }

      // create 2d member from flattened geometry
      GsaMember2d mem = new GsaMember2d(flattened[0], curves, inclusionPoints);
      mem.MeshSize = meshSize;
      mem.Type = MemberType.GENERIC_2D;

      // assemble temp model
      Model model = Export.AssembleModel.Assemble(null, nodes, elem1ds, null, null, mem1ds, new List<GsaMember2d> { mem }, null, null, null, null, null, null, null, null, unit, tolerance.Meters, true, null);

      ReadOnlyDictionary<int, Element> elementDict = model.Elements();
      // populate local axes dictionary
      Dictionary<int, ReadOnlyCollection<double>> elementLocalAxesDict = new Dictionary<int, ReadOnlyCollection<double>>();
      foreach (int id in elementDict.Keys)
        elementLocalAxesDict.Add(id, model.ElementDirectionCosine(id));
      // extract elements from model
      ReadOnlyDictionary<int, Node> nodeDict = model.Nodes();
      Tuple<ConcurrentBag<GsaElement1dGoo>, ConcurrentBag<GsaElement2dGoo>, ConcurrentBag<GsaElement3dGoo>> elementTuple
                = Import.Elements.GetElements(elementDict, nodeDict, model.Sections(), model.Prop2Ds(), model.Prop3Ds(), model.AnalysisMaterials(), model.SectionModifiers(),
                    elementLocalAxesDict, model.Axes(), unit, false);

      List<GsaElement2dGoo> elem2dgoo = elementTuple.Item2.OrderBy(item => item.Value.Ids).ToList();
      Mesh mesh = elem2dgoo[0].Value.Mesh;

      Surface flat = flattened[0].Surfaces[0];
      Surface orig = in_brep.Surfaces[0];

      MeshVertexList vertices = mesh.Vertices;
      for (int i = 0; i < vertices.Count; i++)
      {
        flat.ClosestPoint(vertices.Point3dAt(i), out double u, out double v);
        Point3d mapVertex = orig.PointAt(u, v);
        vertices.SetVertex(i, mapVertex);
      }

      mesh.Faces.ConvertNonPlanarQuadsToTriangles(
          tolerance.As(unit), Rhino.RhinoMath.DefaultAngleTolerance, 0);

      List<GsaNode> out_nodes = null;
      if (nodes != null && nodes.Count > 0)
      {
        Member mem2d = model.Members()[elem2dgoo[0].Value.API_Elements[0].ParentMember.Member];
        List<int> topoInts = Topology.Topology_detangler(mem2d.Topology).Item4;
        int add = points == null ? 0 : points.Count;
        out_nodes = new List<GsaNode>();
        for (int i = 0; i < nodes.Count; i++)
        {
          Vector3 pos = nodeDict[topoInts[i + add]].Position;
          Point3d pt = new Point3d(pos.X, pos.Y, pos.Z);
          flat.ClosestPoint(pt, out double u, out double v);
          Point3d mapPt = orig.PointAt(u, v);
          nodes[i].Point = mapPt;
          out_nodes.Add(nodes[i]);
        }
      }

      List<GsaElement1d> out_elem1ds = null;
      if (inclCrvs != null && inclCrvs.Length > 0)
      {
        out_elem1ds = new List<GsaElement1d>();
        ReadOnlyDictionary<int, Element> elemDict = model.Elements();
        ReadOnlyDictionary<int, Section> sDict = model.Sections();
        ReadOnlyDictionary<int, SectionModifier> modDict = model.SectionModifiers();
        ReadOnlyDictionary<int, AnalysisMaterial> aDict = model.AnalysisMaterials();
        elementLocalAxesDict = new Dictionary<int, ReadOnlyCollection<double>>();
        foreach (int id in elemDict.Keys)
          elementLocalAxesDict.Add(id, model.ElementDirectionCosine(id));
        foreach (KeyValuePair<int, Element> kvp in elemDict)
        {
          Element elem = kvp.Value;
          if (elem.Topology.Count != 2)
            continue;
          Vector3 posS = nodeDict[elem.Topology[0]].Position;
          Point3d start = new Point3d(posS.X, posS.Y, posS.Z);
          flat.ClosestPoint(start, out double us, out double vs);
          Point3d mapPts = orig.PointAt(us, vs);
          Vector3 posE = nodeDict[elem.Topology[1]].Position;
          Point3d end = new Point3d(posE.X, posE.Y, posE.Z);
          flat.ClosestPoint(end, out double ue, out double ve);
          Point3d mapPte = orig.PointAt(ue, ve);
          GsaElement1d elem1d = new GsaElement1d(elemDict, kvp.Key, nodeDict, sDict, modDict, aDict, elementLocalAxesDict, unit);
          elem1d.Line = new LineCurve(mapPts, mapPte);
          if (elem1d.ApiElement.ParentMember.Member > 0)
            elem1d.Section = memSections[elem1d.ApiElement.ParentMember.Member];
          else
            elem1d.Section = elemSections[kvp.Key];
          out_elem1ds.Add(elem1d);
        }
      }

      return new Tuple<Mesh, List<GsaNode>, List<GsaElement1d>>(mesh, out_nodes, out_elem1ds);
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

      return m;
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

      m.CollapseFacesByEdgeLength(false, DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry));

      return m;
    }
  }
}
