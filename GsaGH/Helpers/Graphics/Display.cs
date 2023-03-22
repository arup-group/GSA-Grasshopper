using System;
using System.Collections.Generic;
using GsaGH.Parameters;
using Rhino.Display;
using Rhino.Geometry;
using Rhino.Geometry.Collections;

namespace GsaGH.Helpers.Graphics {
  /// <summary>
  /// Colour class holding the main colours used in colour scheme. 
  /// Make calls to this class to be able to easy update colours.
  /// </summary>
  internal class Display {
    public static Tuple<List<Line>, List<Line>> Preview1D(PolyCurve crv, double angleRadian, GsaBool6 start, GsaBool6 end) {
      var greenLines20 = new List<Line>();
      var redLines10 = new List<Line>();

      if (start == null | end == null) {
        return null;
      }
      #region translation start
      if (start.X) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(0.05);
          scale = crv.GetLength();
        }
        else
          pt = crv.PointAtLength(0.05);

        crv.PerpendicularFrameAt(0.02, out Plane pln);
        pln.Rotate(angleRadian, pln.Normal);
        var vec1 = new Vector3d(pln.XAxis);
        vec1.Unitize();
        vec1 = new Vector3d(vec1.X * 0.025 * scale, vec1.Y * 0.025 * scale, vec1.Z * 0.025 * scale);
        var vec2 = new Vector3d(vec1);
        vec2.Reverse();
        var xf1 = Transform.Translation(vec1);
        var xf2 = Transform.Translation(vec2);
        var pt1 = new Point3d(pt);
        pt1.Transform(xf1);
        var pt2 = new Point3d(pt);
        pt2.Transform(xf2);
        var vec = new Vector3d(pln.Normal);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.25 * scale, vec.Y * 0.25 * scale, vec.Z * 0.25 * scale);
        greenLines20.Add(new Line(pt1, vec));
        greenLines20.Add(new Line(pt2, vec));
      }
      else {
        greenLines20.Add(Line.Unset);
        greenLines20.Add(Line.Unset);
      }
      if (start.Y) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(0.05);
          scale = crv.GetLength();
        }
        else
          pt = crv.PointAtLength(0.05);

        crv.PerpendicularFrameAt(0.02, out Plane pln);
        pln.Rotate(angleRadian, pln.Normal);
        var vec1 = new Vector3d(pln.XAxis);
        vec1.Unitize();
        vec1 = new Vector3d(vec1.X * 0.05 * scale, vec1.Y * 0.05 * scale, vec1.Z * 0.05 * scale);
        var vec2 = new Vector3d(vec1);
        vec2.Reverse();
        var xf1 = Transform.Translation(vec1);
        var xf2 = Transform.Translation(vec2);
        var pt1 = new Point3d(pt);
        pt1.Transform(xf1);
        var pt2 = new Point3d(pt);
        pt2.Transform(xf2);
        var vec3 = new Vector3d(pln.Normal);
        vec3.Unitize();
        vec3 = new Vector3d(vec3.X * 0.025 * scale, vec3.Y * 0.025 * scale, vec3.Z * 0.025 * scale);
        var vec4 = new Vector3d(vec3);
        vec4.Reverse();
        var xf3 = Transform.Translation(vec3);
        var xf4 = Transform.Translation(vec4);
        var pt3A = new Point3d(pt1);
        pt3A.Transform(xf3);
        var pt3B = new Point3d(pt2);
        pt3B.Transform(xf3);
        var pt4A = new Point3d(pt1);
        pt4A.Transform(xf4);
        var pt4B = new Point3d(pt2);
        pt4B.Transform(xf4);

        var vec = new Vector3d(pln.XAxis);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.15 * scale, vec.Y * 0.15 * scale, vec.Z * 0.15 * scale);
        var vecRev = new Vector3d(vec);
        vecRev.Reverse();
        greenLines20.Add(new Line(pt3A, vec));
        greenLines20.Add(new Line(pt3B, vecRev));
        greenLines20.Add(new Line(pt4A, vec));
        greenLines20.Add(new Line(pt4B, vecRev));
      }
      else {
        greenLines20.Add(Line.Unset);
        greenLines20.Add(Line.Unset);
        greenLines20.Add(Line.Unset);
        greenLines20.Add(Line.Unset);
      }
      if (start.Z) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(0.05);
          scale = crv.GetLength();
        }
        else
          pt = crv.PointAtLength(0.05);

        crv.PerpendicularFrameAt(0.02, out Plane pln);
        pln.Rotate(angleRadian, pln.Normal);
        var vec1 = new Vector3d(pln.YAxis);
        vec1.Unitize();
        vec1 = new Vector3d(vec1.X * 0.05 * scale, vec1.Y * 0.05 * scale, vec1.Z * 0.05 * scale);
        var vec2 = new Vector3d(vec1);
        vec2.Reverse();
        var xf1 = Transform.Translation(vec1);
        var xf2 = Transform.Translation(vec2);
        var pt1 = new Point3d(pt);
        pt1.Transform(xf1);
        var pt2 = new Point3d(pt);
        pt2.Transform(xf2);
        var vec3 = new Vector3d(pln.Normal);
        vec3.Unitize();
        vec3 = new Vector3d(vec3.X * 0.025 * scale, vec3.Y * 0.025 * scale, vec3.Z * 0.025 * scale);
        var vec4 = new Vector3d(vec3);
        vec4.Reverse();
        var xf3 = Transform.Translation(vec3);
        var xf4 = Transform.Translation(vec4);
        var pt3A = new Point3d(pt1);
        pt3A.Transform(xf3);
        var pt3B = new Point3d(pt2);
        pt3B.Transform(xf3);
        var pt4A = new Point3d(pt1);
        pt4A.Transform(xf4);
        var pt4B = new Point3d(pt2);
        pt4B.Transform(xf4);

        var vec = new Vector3d(pln.YAxis);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.15 * scale, vec.Y * 0.15 * scale, vec.Z * 0.15 * scale);
        var vecRev = new Vector3d(vec);
        vecRev.Reverse();
        greenLines20.Add(new Line(pt3A, vec));
        greenLines20.Add(new Line(pt3B, vecRev));
        greenLines20.Add(new Line(pt4A, vec));
        greenLines20.Add(new Line(pt4B, vecRev));
      }
      else {
        greenLines20.Add(Line.Unset);
        greenLines20.Add(Line.Unset);
        greenLines20.Add(Line.Unset);
        greenLines20.Add(Line.Unset);
      }
      #endregion
      #region translation end
      if (end.X) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(0.95);
          scale = crv.GetLength();
        }
        else {
          double len = crv.GetLength();
          pt = crv.PointAtLength(len - 0.05);
        }

        crv.PerpendicularFrameAt(0.98, out Plane pln);
        pln.Rotate(angleRadian, pln.Normal);
        var vec1 = new Vector3d(pln.XAxis);
        vec1.Unitize();
        vec1 = new Vector3d(vec1.X * 0.025 * scale, vec1.Y * 0.025 * scale, vec1.Z * 0.025 * scale);
        var vec2 = new Vector3d(vec1);
        vec2.Reverse();
        var xf1 = Transform.Translation(vec1);
        var xf2 = Transform.Translation(vec2);
        var pt1 = new Point3d(pt);
        pt1.Transform(xf1);
        var pt2 = new Point3d(pt);
        pt2.Transform(xf2);
        var vec = new Vector3d(pln.Normal);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.25 * scale, vec.Y * 0.25 * scale, vec.Z * 0.25 * scale);
        vec.Reverse();
        greenLines20.Add(new Line(pt1, vec));
        greenLines20.Add(new Line(pt2, vec));
      }
      else {
        greenLines20.Add(Line.Unset);
        greenLines20.Add(Line.Unset);
      }
      if (end.Y) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(0.95);
          scale = crv.GetLength();
        }
        else {
          double len = crv.GetLength();
          pt = crv.PointAtLength(len - 0.05);
        }

        crv.PerpendicularFrameAt(0.02, out Plane pln);
        pln.Rotate(angleRadian, pln.Normal);
        var vec1 = new Vector3d(pln.XAxis);
        vec1.Unitize();
        vec1 = new Vector3d(vec1.X * 0.05 * scale, vec1.Y * 0.05 * scale, vec1.Z * 0.05 * scale);
        var vec2 = new Vector3d(vec1);
        vec2.Reverse();
        var xf1 = Transform.Translation(vec1);
        var xf2 = Transform.Translation(vec2);
        var pt1 = new Point3d(pt);
        pt1.Transform(xf1);
        var pt2 = new Point3d(pt);
        pt2.Transform(xf2);
        var vec3 = new Vector3d(pln.Normal);
        vec3.Unitize();
        vec3 = new Vector3d(vec3.X * 0.025 * scale, vec3.Y * 0.025 * scale, vec3.Z * 0.025 * scale);
        var vec4 = new Vector3d(vec3);
        vec4.Reverse();
        var xf3 = Transform.Translation(vec3);
        var xf4 = Transform.Translation(vec4);
        var pt3A = new Point3d(pt1);
        pt3A.Transform(xf3);
        var pt3B = new Point3d(pt2);
        pt3B.Transform(xf3);
        var pt4A = new Point3d(pt1);
        pt4A.Transform(xf4);
        var pt4B = new Point3d(pt2);
        pt4B.Transform(xf4);

        var vec = new Vector3d(pln.XAxis);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.15 * scale, vec.Y * 0.15 * scale, vec.Z * 0.15 * scale);
        var vecRev = new Vector3d(vec);
        vecRev.Reverse();
        greenLines20.Add(new Line(pt3A, vec));
        greenLines20.Add(new Line(pt3B, vecRev));
        greenLines20.Add(new Line(pt4A, vec));
        greenLines20.Add(new Line(pt4B, vecRev));
      }
      else {
        greenLines20.Add(Line.Unset);
        greenLines20.Add(Line.Unset);
        greenLines20.Add(Line.Unset);
        greenLines20.Add(Line.Unset);
      }
      if (end.Z) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(0.95);
          scale = crv.GetLength();
        }
        else {
          double len = crv.GetLength();
          pt = crv.PointAtLength(len - 0.05);
        }

        crv.PerpendicularFrameAt(0.02, out Plane pln);
        pln.Rotate(angleRadian, pln.Normal);
        var vec1 = new Vector3d(pln.YAxis);
        vec1.Unitize();
        vec1 = new Vector3d(vec1.X * 0.05 * scale, vec1.Y * 0.05 * scale, vec1.Z * 0.05 * scale);
        var vec2 = new Vector3d(vec1);
        vec2.Reverse();
        var xf1 = Transform.Translation(vec1);
        var xf2 = Transform.Translation(vec2);
        var pt1 = new Point3d(pt);
        pt1.Transform(xf1);
        var pt2 = new Point3d(pt);
        pt2.Transform(xf2);
        var vec3 = new Vector3d(pln.Normal);
        vec3.Unitize();
        vec3 = new Vector3d(vec3.X * 0.025 * scale, vec3.Y * 0.025 * scale, vec3.Z * 0.025 * scale);
        var vec4 = new Vector3d(vec3);
        vec4.Reverse();
        var xf3 = Transform.Translation(vec3);
        var xf4 = Transform.Translation(vec4);
        var pt3A = new Point3d(pt1);
        pt3A.Transform(xf3);
        var pt3B = new Point3d(pt2);
        pt3B.Transform(xf3);
        var pt4A = new Point3d(pt1);
        pt4A.Transform(xf4);
        var pt4B = new Point3d(pt2);
        pt4B.Transform(xf4);

        var vec = new Vector3d(pln.YAxis);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.15 * scale, vec.Y * 0.15 * scale, vec.Z * 0.15 * scale);
        var vecRev = new Vector3d(vec);
        vecRev.Reverse();
        greenLines20.Add(new Line(pt3A, vec));
        greenLines20.Add(new Line(pt3B, vecRev));
        greenLines20.Add(new Line(pt4A, vec));
        greenLines20.Add(new Line(pt4B, vecRev));
      }
      else {
        greenLines20.Add(Line.Unset);
        greenLines20.Add(Line.Unset);
        greenLines20.Add(Line.Unset);
        greenLines20.Add(Line.Unset);
      }
      #endregion
      #region rotation start
      if (start.XX) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(0.05);
          scale = crv.GetLength();
        }
        else
          pt = crv.PointAtLength(0.05);

        crv.PerpendicularFrameAt(0.02, out Plane pln);
        var vec = new Vector3d(pln.Normal);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.25 * scale, vec.Y * 0.25 * scale, vec.Z * 0.25 * scale);
        redLines10.Add(new Line(pt, vec));
      }
      else
        redLines10.Add(Line.Unset);

      if (start.YY) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(0.05);
          scale = crv.GetLength();
        }
        else
          pt = crv.PointAtLength(0.05);

        crv.PerpendicularFrameAt(0.02, out Plane pln);
        pln.Rotate(angleRadian, pln.Normal);
        var vec1 = new Vector3d(pln.XAxis);
        vec1.Unitize();
        vec1 = new Vector3d(vec1.X * 0.05 * scale, vec1.Y * 0.05 * scale, vec1.Z * 0.05 * scale);
        var vec2 = new Vector3d(vec1);
        vec2.Reverse();
        var xf1 = Transform.Translation(vec1);
        var xf2 = Transform.Translation(vec2);
        var pt1 = new Point3d(pt);
        pt1.Transform(xf1);
        var pt2 = new Point3d(pt);
        pt2.Transform(xf2);

        var vec = new Vector3d(pln.XAxis);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.15 * scale, vec.Y * 0.15 * scale, vec.Z * 0.15 * scale);
        var vecRev = new Vector3d(vec);
        vecRev.Reverse();
        redLines10.Add(new Line(pt1, vec));
        redLines10.Add(new Line(pt2, vecRev));
      }
      else {
        redLines10.Add(Line.Unset);
        redLines10.Add(Line.Unset);
      }

      if (start.ZZ) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(0.05);
          scale = crv.GetLength();
        }
        else
          pt = crv.PointAtLength(0.05);

        crv.PerpendicularFrameAt(0.02, out Plane pln);
        pln.Rotate(angleRadian, pln.Normal);
        var vec1 = new Vector3d(pln.YAxis);
        vec1.Unitize();
        vec1 = new Vector3d(vec1.X * 0.05 * scale, vec1.Y * 0.05 * scale, vec1.Z * 0.05 * scale);
        var vec2 = new Vector3d(vec1);
        vec2.Reverse();
        var xf1 = Transform.Translation(vec1);
        var xf2 = Transform.Translation(vec2);
        var pt1 = new Point3d(pt);
        pt1.Transform(xf1);
        var pt2 = new Point3d(pt);
        pt2.Transform(xf2);

        var vec = new Vector3d(pln.YAxis);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.15 * scale, vec.Y * 0.15 * scale, vec.Z * 0.15 * scale);
        var vecRev = new Vector3d(vec);
        vecRev.Reverse();
        redLines10.Add(new Line(pt1, vec));
        redLines10.Add(new Line(pt2, vecRev));
      }
      else {
        redLines10.Add(Line.Unset);
        redLines10.Add(Line.Unset);
      }
      #endregion
      #region rotation end
      if (end.XX) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(0.95);
          scale = crv.GetLength();
        }
        else {
          double len = crv.GetLength();
          pt = crv.PointAtLength(len - 0.05);
        }

        crv.PerpendicularFrameAt(0.02, out Plane pln);
        var vec = new Vector3d(pln.Normal);
        vec.Unitize();
        vec.Reverse();
        vec = new Vector3d(vec.X * 0.25 * scale, vec.Y * 0.25 * scale, vec.Z * 0.25 * scale);
        redLines10.Add(new Line(pt, vec));
      }
      else
        redLines10.Add(Line.Unset);

      if (end.YY) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(0.95);
          scale = crv.GetLength();
        }
        else {
          double len = crv.GetLength();
          pt = crv.PointAtLength(len - 0.05);
        }

        crv.PerpendicularFrameAt(0.02, out Plane pln);
        pln.Rotate(angleRadian, pln.Normal);
        var vec1 = new Vector3d(pln.XAxis);
        vec1.Unitize();
        vec1 = new Vector3d(vec1.X * 0.05 * scale, vec1.Y * 0.05 * scale, vec1.Z * 0.05 * scale);
        var vec2 = new Vector3d(vec1);
        vec2.Reverse();
        var xf1 = Transform.Translation(vec1);
        var xf2 = Transform.Translation(vec2);
        var pt1 = new Point3d(pt);
        pt1.Transform(xf1);
        var pt2 = new Point3d(pt);
        pt2.Transform(xf2);

        var vec = new Vector3d(pln.XAxis);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.15 * scale, vec.Y * 0.15 * scale, vec.Z * 0.15 * scale);
        var vecRev = new Vector3d(vec);
        vecRev.Reverse();
        redLines10.Add(new Line(pt1, vec));
        redLines10.Add(new Line(pt2, vecRev));
      }
      else {
        redLines10.Add(Line.Unset);
        redLines10.Add(Line.Unset);
      }

      if (end.ZZ) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(0.95);
          scale = crv.GetLength();
        }
        else {
          double len = crv.GetLength();
          pt = crv.PointAtLength(len - 0.05);
        }

        crv.PerpendicularFrameAt(0.02, out Plane pln);
        pln.Rotate(angleRadian, pln.Normal);
        var vec1 = new Vector3d(pln.YAxis);
        vec1.Unitize();
        vec1 = new Vector3d(vec1.X * 0.05 * scale, vec1.Y * 0.05 * scale, vec1.Z * 0.05 * scale);
        var vec2 = new Vector3d(vec1);
        vec2.Reverse();
        var xf1 = Transform.Translation(vec1);
        var xf2 = Transform.Translation(vec2);
        var pt1 = new Point3d(pt);
        pt1.Transform(xf1);
        var pt2 = new Point3d(pt);
        pt2.Transform(xf2);

        var vec = new Vector3d(pln.YAxis);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.15 * scale, vec.Y * 0.15 * scale, vec.Z * 0.15 * scale);
        var vecRev = new Vector3d(vec);
        vecRev.Reverse();
        redLines10.Add(new Line(pt1, vec));
        redLines10.Add(new Line(pt2, vecRev));
      }
      else {
        redLines10.Add(Line.Unset);
        redLines10.Add(Line.Unset);
      }
      #endregion
      return new Tuple<List<Line>, List<Line>>(greenLines20, redLines10);
    }

    public static void PreviewRestraint(GsaBool6 restraint, Plane localAxis, Point3d pt, ref Brep support, ref Text3d text) {
      if (restraint.X & restraint.Y & restraint.Z &
          !restraint.XX & !restraint.YY & !restraint.ZZ) {
        Plane plane = localAxis.Clone();
        if (!plane.IsValid) { plane = Plane.WorldXY; }
        plane.Origin = pt;
        var pin = new Cone(plane, -0.4, 0.4);
        support = pin.ToBrep(true);
      }
      else if (restraint.X & restraint.Y & restraint.Z &
              restraint.XX & restraint.YY & restraint.ZZ) {
        Plane plane = localAxis.Clone();
        if (!plane.IsValid) { plane = Plane.WorldXY; }
        plane.Origin = pt;
        var fix = new Box(plane, new Interval(-0.3, 0.3), new Interval(-0.3, 0.3), new Interval(-0.2, 0));
        support = fix.ToBrep();
      }
      else {
        Plane plane = localAxis.Clone();
        if (!plane.IsValid) { plane = Plane.WorldXY; }
        plane.Origin = pt;
        string rest = "";
        if (restraint.X)
          rest += "X";
        if (restraint.Y)
          rest += "Y";
        if (restraint.Z)
          rest += "Z";
        if (restraint.XX)
          rest += "XX";
        if (restraint.YY)
          rest += "YY";
        if (restraint.ZZ)
          rest += "ZZ";
        text = new Text3d(rest, plane, 0.3) {
          HorizontalAlignment = Rhino.DocObjects.TextHorizontalAlignment.Left,
          VerticalAlignment = Rhino.DocObjects.TextVerticalAlignment.Top,
        };
      }
    }

    public static void PreviewMem3d(ref Mesh solidMesh, ref List<Polyline> hiddenLines, ref List<Line> edgeLines, ref List<Point3d> pts) {
      MeshTopologyEdgeList alledges = solidMesh.TopologyEdges;
      if (solidMesh.FaceNormals.Count < solidMesh.Faces.Count)
        solidMesh.FaceNormals.ComputeFaceNormals();

      hiddenLines = new List<Polyline>();
      edgeLines = new List<Line>();
      for (int i = 0; i < alledges.Count; i++) {
        int[] faceId = alledges.GetConnectedFaces(i);
        Vector3d vec1 = solidMesh.FaceNormals[faceId[0]];
        Vector3d vec2 = solidMesh.FaceNormals[faceId[1]];
        vec1.Unitize();
        vec2.Unitize();
        if (!vec1.Equals(vec2) || faceId.Length > 2) {
          edgeLines.Add(alledges.EdgeLine(i));
        }
        else {
          var hidden = new Polyline {
            alledges.EdgeLine(i).PointAt(0),
            alledges.EdgeLine(i).PointAt(1)
          };
          hiddenLines.Add(hidden);
        }
      }

      pts = new List<Point3d>(solidMesh.Vertices.ToPoint3dArray());
    }
  }
}
