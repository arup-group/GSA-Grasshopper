using System.Collections.Generic;
using Rhino.Display;
using Rhino.Geometry;
using GsaGH.Parameters;
using System;

namespace GsaGH.Helpers.Graphics
{
  /// <summary>
  /// Colour class holding the main colours used in colour scheme. 
  /// Make calls to this class to be able to easy update colours.
  /// 
  /// </summary>
  internal class Display
  {
    internal static Tuple<Vector3d, Vector3d, Vector3d> GetLocalPlane(PolyCurve crv, double t, double orientationAngle)
    {
      crv.PerpendicularFrameAt(t, out Plane pln);
      pln.Rotate(orientationAngle, pln.Normal);

      double absAngleToZ = Vector3d.VectorAngle(pln.Normal, Vector3d.ZAxis);
      absAngleToZ %= Math.PI;

      Vector3d outX = pln.ZAxis;
      Vector3d outY;
      if (absAngleToZ < 0.25 * Math.PI || absAngleToZ > 0.75 * Math.PI)
      {
        outY = new Vector3d(outX);
        double angle;
        if (outX.Z > 0)
          angle = -0.5 * Math.PI;
        else
          angle = 0.5 * Math.PI;

        if (!outY.Rotate(angle, pln.XAxis))
          throw new Exception();
      }
      else
      {
        outY = pln.YAxis;
      }
      Vector3d outZ = Vector3d.CrossProduct(outX, outY);
      return new Tuple<Vector3d, Vector3d, Vector3d>(outX, outY, outZ);
    }

    public static void Preview1D(PolyCurve crv, double angle_radian, GsaBool6 start, GsaBool6 end,
        ref List<Line> greenLines20, ref List<Line> redLines10)
    {
      int i = 0;

      if (start == null | end == null) { return; }
      #region translation start
      if (start.X == true)
      {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1)
        {
          pt = crv.PointAtNormalizedLength(0.05);
          scale = crv.GetLength();
        }
        else
          pt = crv.PointAtLength(0.05);

        Plane pln = new Plane();
        crv.PerpendicularFrameAt(0.02, out pln);
        pln.Rotate(angle_radian, pln.Normal);
        Vector3d vec1 = new Vector3d(pln.XAxis);
        vec1.Unitize();
        vec1 = new Vector3d(vec1.X * 0.025 * scale, vec1.Y * 0.025 * scale, vec1.Z * 0.025 * scale);
        Vector3d vec2 = new Vector3d(vec1);
        vec2.Reverse();
        Transform xf1 = Rhino.Geometry.Transform.Translation(vec1);
        Transform xf2 = Rhino.Geometry.Transform.Translation(vec2);
        Point3d pt1 = new Point3d(pt);
        pt1.Transform(xf1);
        Point3d pt2 = new Point3d(pt);
        pt2.Transform(xf2);
        Vector3d vec = new Vector3d(pln.Normal);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.25 * scale, vec.Y * 0.25 * scale, vec.Z * 0.25 * scale);
        greenLines20[i++] = new Line(pt1, vec);
        //args.Pipeline.DrawLine(ln1, UI.Colour.Support);
        greenLines20[i++] = new Line(pt2, vec);
        //args.Pipeline.DrawLine(ln2, UI.Colour.Support);
      }
      else
      {
        greenLines20[i++] = Line.Unset;
        greenLines20[i++] = Line.Unset;
      }
      if (start.Y == true)
      {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1)
        {
          pt = crv.PointAtNormalizedLength(0.05);
          scale = crv.GetLength();
        }
        else
          pt = crv.PointAtLength(0.05);

        Plane pln = new Plane();
        crv.PerpendicularFrameAt(0.02, out pln);
        pln.Rotate(angle_radian, pln.Normal);
        Vector3d vec1 = new Vector3d(pln.XAxis);
        vec1.Unitize();
        vec1 = new Vector3d(vec1.X * 0.05 * scale, vec1.Y * 0.05 * scale, vec1.Z * 0.05 * scale);
        Vector3d vec2 = new Vector3d(vec1);
        vec2.Reverse();
        Transform xf1 = Rhino.Geometry.Transform.Translation(vec1);
        Transform xf2 = Rhino.Geometry.Transform.Translation(vec2);
        Point3d pt1 = new Point3d(pt);
        pt1.Transform(xf1);
        Point3d pt2 = new Point3d(pt);
        pt2.Transform(xf2);
        Vector3d vec3 = new Vector3d(pln.Normal);
        vec3.Unitize();
        vec3 = new Vector3d(vec3.X * 0.025 * scale, vec3.Y * 0.025 * scale, vec3.Z * 0.025 * scale);
        Vector3d vec4 = new Vector3d(vec3);
        vec4.Reverse();
        Transform xf3 = Rhino.Geometry.Transform.Translation(vec3);
        Transform xf4 = Rhino.Geometry.Transform.Translation(vec4);
        Point3d pt3A = new Point3d(pt1);
        pt3A.Transform(xf3);
        Point3d pt3B = new Point3d(pt2);
        pt3B.Transform(xf3);
        Point3d pt4A = new Point3d(pt1);
        pt4A.Transform(xf4);
        Point3d pt4B = new Point3d(pt2);
        pt4B.Transform(xf4);

        Vector3d vec = new Vector3d(pln.XAxis);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.15 * scale, vec.Y * 0.15 * scale, vec.Z * 0.15 * scale);
        Vector3d vecRev = new Vector3d(vec);
        vecRev.Reverse();
        greenLines20[i++] = new Line(pt3A, vec);
        //args.Pipeline.DrawLine(ln1A, UI.Colour.Support);
        greenLines20[i++] = new Line(pt3B, vecRev);
        //args.Pipeline.DrawLine(ln1B, UI.Colour.Support);
        greenLines20[i++] = new Line(pt4A, vec);
        //args.Pipeline.DrawLine(ln2A, UI.Colour.Support);
        greenLines20[i++] = new Line(pt4B, vecRev);
        //args.Pipeline.DrawLine(ln2B, UI.Colour.Support);
      }
      else
      {
        greenLines20[i++] = Line.Unset;
        greenLines20[i++] = Line.Unset;
        greenLines20[i++] = Line.Unset;
        greenLines20[i++] = Line.Unset;
      }
      if (start.Z == true)
      {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1)
        {
          pt = crv.PointAtNormalizedLength(0.05);
          scale = crv.GetLength();
        }
        else
          pt = crv.PointAtLength(0.05);

        Plane pln = new Plane();
        crv.PerpendicularFrameAt(0.02, out pln);
        pln.Rotate(angle_radian, pln.Normal);
        Vector3d vec1 = new Vector3d(pln.YAxis);
        vec1.Unitize();
        vec1 = new Vector3d(vec1.X * 0.05 * scale, vec1.Y * 0.05 * scale, vec1.Z * 0.05 * scale);
        Vector3d vec2 = new Vector3d(vec1);
        vec2.Reverse();
        Transform xf1 = Rhino.Geometry.Transform.Translation(vec1);
        Transform xf2 = Rhino.Geometry.Transform.Translation(vec2);
        Point3d pt1 = new Point3d(pt);
        pt1.Transform(xf1);
        Point3d pt2 = new Point3d(pt);
        pt2.Transform(xf2);
        Vector3d vec3 = new Vector3d(pln.Normal);
        vec3.Unitize();
        vec3 = new Vector3d(vec3.X * 0.025 * scale, vec3.Y * 0.025 * scale, vec3.Z * 0.025 * scale);
        Vector3d vec4 = new Vector3d(vec3);
        vec4.Reverse();
        Transform xf3 = Rhino.Geometry.Transform.Translation(vec3);
        Transform xf4 = Rhino.Geometry.Transform.Translation(vec4);
        Point3d pt3A = new Point3d(pt1);
        pt3A.Transform(xf3);
        Point3d pt3B = new Point3d(pt2);
        pt3B.Transform(xf3);
        Point3d pt4A = new Point3d(pt1);
        pt4A.Transform(xf4);
        Point3d pt4B = new Point3d(pt2);
        pt4B.Transform(xf4);

        Vector3d vec = new Vector3d(pln.YAxis);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.15 * scale, vec.Y * 0.15 * scale, vec.Z * 0.15 * scale);
        Vector3d vecRev = new Vector3d(vec);
        vecRev.Reverse();
        greenLines20[i++] = new Line(pt3A, vec);
        //args.Pipeline.DrawLine(ln1A, UI.Colour.Support);
        greenLines20[i++] = new Line(pt3B, vecRev);
        //args.Pipeline.DrawLine(ln1B, UI.Colour.Support);
        greenLines20[i++] = new Line(pt4A, vec);
        //args.Pipeline.DrawLine(ln2A, UI.Colour.Support);
        greenLines20[i++] = new Line(pt4B, vecRev);
        //args.Pipeline.DrawLine(ln2B, UI.Colour.Support);
      }
      else
      {
        greenLines20[i++] = Line.Unset;
        greenLines20[i++] = Line.Unset;
        greenLines20[i++] = Line.Unset;
        greenLines20[i++] = Line.Unset;
      }
      #endregion
      #region translation end
      if (end.X == true)
      {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1)
        {
          pt = crv.PointAtNormalizedLength(0.95);
          scale = crv.GetLength();
        }
        else
        {
          double len = crv.GetLength();
          pt = crv.PointAtLength(len - 0.05);
        }

        Plane pln = new Plane();
        crv.PerpendicularFrameAt(0.98, out pln);
        pln.Rotate(angle_radian, pln.Normal);
        Vector3d vec1 = new Vector3d(pln.XAxis);
        vec1.Unitize();
        vec1 = new Vector3d(vec1.X * 0.025 * scale, vec1.Y * 0.025 * scale, vec1.Z * 0.025 * scale);
        Vector3d vec2 = new Vector3d(vec1);
        vec2.Reverse();
        Transform xf1 = Rhino.Geometry.Transform.Translation(vec1);
        Transform xf2 = Rhino.Geometry.Transform.Translation(vec2);
        Point3d pt1 = new Point3d(pt);
        pt1.Transform(xf1);
        Point3d pt2 = new Point3d(pt);
        pt2.Transform(xf2);
        Vector3d vec = new Vector3d(pln.Normal);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.25 * scale, vec.Y * 0.25 * scale, vec.Z * 0.25 * scale);
        vec.Reverse();
        greenLines20[i++] = new Line(pt1, vec);
        //args.Pipeline.DrawLine(ln1, UI.Colour.Support);
        greenLines20[i++] = new Line(pt2, vec);
        //args.Pipeline.DrawLine(ln2, UI.Colour.Support);
      }
      else
      {
        greenLines20[i++] = Line.Unset;
        greenLines20[i++] = Line.Unset;
      }
      if (end.Y == true)
      {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1)
        {
          pt = crv.PointAtNormalizedLength(0.95);
          scale = crv.GetLength();
        }
        else
        {
          double len = crv.GetLength();
          pt = crv.PointAtLength(len - 0.05);
        }

        Plane pln = new Plane();
        crv.PerpendicularFrameAt(0.02, out pln);
        pln.Rotate(angle_radian, pln.Normal);
        Vector3d vec1 = new Vector3d(pln.XAxis);
        vec1.Unitize();
        vec1 = new Vector3d(vec1.X * 0.05 * scale, vec1.Y * 0.05 * scale, vec1.Z * 0.05 * scale);
        Vector3d vec2 = new Vector3d(vec1);
        vec2.Reverse();
        Transform xf1 = Rhino.Geometry.Transform.Translation(vec1);
        Transform xf2 = Rhino.Geometry.Transform.Translation(vec2);
        Point3d pt1 = new Point3d(pt);
        pt1.Transform(xf1);
        Point3d pt2 = new Point3d(pt);
        pt2.Transform(xf2);
        Vector3d vec3 = new Vector3d(pln.Normal);
        vec3.Unitize();
        vec3 = new Vector3d(vec3.X * 0.025 * scale, vec3.Y * 0.025 * scale, vec3.Z * 0.025 * scale);
        Vector3d vec4 = new Vector3d(vec3);
        vec4.Reverse();
        Transform xf3 = Rhino.Geometry.Transform.Translation(vec3);
        Transform xf4 = Rhino.Geometry.Transform.Translation(vec4);
        Point3d pt3A = new Point3d(pt1);
        pt3A.Transform(xf3);
        Point3d pt3B = new Point3d(pt2);
        pt3B.Transform(xf3);
        Point3d pt4A = new Point3d(pt1);
        pt4A.Transform(xf4);
        Point3d pt4B = new Point3d(pt2);
        pt4B.Transform(xf4);

        Vector3d vec = new Vector3d(pln.XAxis);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.15 * scale, vec.Y * 0.15 * scale, vec.Z * 0.15 * scale);
        Vector3d vecRev = new Vector3d(vec);
        vecRev.Reverse();
        greenLines20[i++] = new Line(pt3A, vec);
        //args.Pipeline.DrawLine(ln1A, UI.Colour.Support);
        greenLines20[i++] = new Line(pt3B, vecRev);
        //args.Pipeline.DrawLine(ln1B, UI.Colour.Support);
        greenLines20[i++] = new Line(pt4A, vec);
        //args.Pipeline.DrawLine(ln2A, UI.Colour.Support);
        greenLines20[i++] = new Line(pt4B, vecRev);
        //args.Pipeline.DrawLine(ln2B, UI.Colour.Support);
      }
      else
      {
        greenLines20[i++] = Line.Unset;
        greenLines20[i++] = Line.Unset;
        greenLines20[i++] = Line.Unset;
        greenLines20[i++] = Line.Unset;
      }
      if (end.Z == true)
      {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1)
        {
          pt = crv.PointAtNormalizedLength(0.95);
          scale = crv.GetLength();
        }
        else
        {
          double len = crv.GetLength();
          pt = crv.PointAtLength(len - 0.05);
        }

        Plane pln = new Plane();
        crv.PerpendicularFrameAt(0.02, out pln);
        pln.Rotate(angle_radian, pln.Normal);
        Vector3d vec1 = new Vector3d(pln.YAxis);
        vec1.Unitize();
        vec1 = new Vector3d(vec1.X * 0.05 * scale, vec1.Y * 0.05 * scale, vec1.Z * 0.05 * scale);
        Vector3d vec2 = new Vector3d(vec1);
        vec2.Reverse();
        Transform xf1 = Rhino.Geometry.Transform.Translation(vec1);
        Transform xf2 = Rhino.Geometry.Transform.Translation(vec2);
        Point3d pt1 = new Point3d(pt);
        pt1.Transform(xf1);
        Point3d pt2 = new Point3d(pt);
        pt2.Transform(xf2);
        Vector3d vec3 = new Vector3d(pln.Normal);
        vec3.Unitize();
        vec3 = new Vector3d(vec3.X * 0.025 * scale, vec3.Y * 0.025 * scale, vec3.Z * 0.025 * scale);
        Vector3d vec4 = new Vector3d(vec3);
        vec4.Reverse();
        Transform xf3 = Rhino.Geometry.Transform.Translation(vec3);
        Transform xf4 = Rhino.Geometry.Transform.Translation(vec4);
        Point3d pt3A = new Point3d(pt1);
        pt3A.Transform(xf3);
        Point3d pt3B = new Point3d(pt2);
        pt3B.Transform(xf3);
        Point3d pt4A = new Point3d(pt1);
        pt4A.Transform(xf4);
        Point3d pt4B = new Point3d(pt2);
        pt4B.Transform(xf4);

        Vector3d vec = new Vector3d(pln.YAxis);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.15 * scale, vec.Y * 0.15 * scale, vec.Z * 0.15 * scale);
        Vector3d vecRev = new Vector3d(vec);
        vecRev.Reverse();
        greenLines20[i++] = new Line(pt3A, vec);
        //args.Pipeline.DrawLine(ln1A, UI.Colour.Support);
        greenLines20[i++] = new Line(pt3B, vecRev);
        //args.Pipeline.DrawLine(ln1B, UI.Colour.Support);
        greenLines20[i++] = new Line(pt4A, vec);
        //args.Pipeline.DrawLine(ln2A, UI.Colour.Support);
        greenLines20[i++] = new Line(pt4B, vecRev);
        //args.Pipeline.DrawLine(ln2B, UI.Colour.Support);
      }
      else
      {
        greenLines20[i++] = Line.Unset;
        greenLines20[i++] = Line.Unset;
        greenLines20[i++] = Line.Unset;
        greenLines20[i++] = Line.Unset;
      }
      #endregion
      #region rotation start
      i = 0;
      if (start.XX == true)
      {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1)
        {
          pt = crv.PointAtNormalizedLength(0.05);
          scale = crv.GetLength();
        }
        else
          pt = crv.PointAtLength(0.05);

        Plane pln = new Plane();
        crv.PerpendicularFrameAt(0.02, out pln);
        Vector3d vec = new Vector3d(pln.Normal);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.25 * scale, vec.Y * 0.25 * scale, vec.Z * 0.25 * scale);
        redLines10[i++] = new Line(pt, vec);
        //args.Pipeline.DrawLine(ln1, UI.Colour.Release, 3);
      }
      else
      {
        redLines10[i++] = Line.Unset;
      }
      if (start.YY == true)
      {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1)
        {
          pt = crv.PointAtNormalizedLength(0.05);
          scale = crv.GetLength();
        }
        else
          pt = crv.PointAtLength(0.05);

        Plane pln = new Plane();
        crv.PerpendicularFrameAt(0.02, out pln);
        pln.Rotate(angle_radian, pln.Normal);
        Vector3d vec1 = new Vector3d(pln.XAxis);
        vec1.Unitize();
        vec1 = new Vector3d(vec1.X * 0.05 * scale, vec1.Y * 0.05 * scale, vec1.Z * 0.05 * scale);
        Vector3d vec2 = new Vector3d(vec1);
        vec2.Reverse();
        Transform xf1 = Rhino.Geometry.Transform.Translation(vec1);
        Transform xf2 = Rhino.Geometry.Transform.Translation(vec2);
        Point3d pt1 = new Point3d(pt);
        pt1.Transform(xf1);
        Point3d pt2 = new Point3d(pt);
        pt2.Transform(xf2);

        Vector3d vec = new Vector3d(pln.XAxis);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.15 * scale, vec.Y * 0.15 * scale, vec.Z * 0.15 * scale);
        Vector3d vecRev = new Vector3d(vec);
        vecRev.Reverse();
        redLines10[i++] = new Line(pt1, vec);
        //args.Pipeline.DrawLine(ln1A, UI.Colour.Release);
        redLines10[i++] = new Line(pt2, vecRev);
        //args.Pipeline.DrawLine(ln1B, UI.Colour.Release);
      }
      else
      {
        redLines10[i++] = Line.Unset;
        redLines10[i++] = Line.Unset;
      }
      if (start.ZZ == true)
      {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1)
        {
          pt = crv.PointAtNormalizedLength(0.05);
          scale = crv.GetLength();
        }
        else
          pt = crv.PointAtLength(0.05);

        Plane pln = new Plane();
        crv.PerpendicularFrameAt(0.02, out pln);
        pln.Rotate(angle_radian, pln.Normal);
        Vector3d vec1 = new Vector3d(pln.YAxis);
        vec1.Unitize();
        vec1 = new Vector3d(vec1.X * 0.05 * scale, vec1.Y * 0.05 * scale, vec1.Z * 0.05 * scale);
        Vector3d vec2 = new Vector3d(vec1);
        vec2.Reverse();
        Transform xf1 = Rhino.Geometry.Transform.Translation(vec1);
        Transform xf2 = Rhino.Geometry.Transform.Translation(vec2);
        Point3d pt1 = new Point3d(pt);
        pt1.Transform(xf1);
        Point3d pt2 = new Point3d(pt);
        pt2.Transform(xf2);

        Vector3d vec = new Vector3d(pln.YAxis);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.15 * scale, vec.Y * 0.15 * scale, vec.Z * 0.15 * scale);
        Vector3d vecRev = new Vector3d(vec);
        vecRev.Reverse();
        redLines10[i++] = new Line(pt1, vec);
        //args.Pipeline.DrawLine(ln1A, UI.Colour.Release);
        redLines10[i++] = new Line(pt2, vecRev);
        //args.Pipeline.DrawLine(ln1B, UI.Colour.Release);
      }
      else
      {
        redLines10[i++] = Line.Unset;
        redLines10[i++] = Line.Unset;
      }
      #endregion
      #region rotation end
      if (end.XX == true)
      {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1)
        {
          pt = crv.PointAtNormalizedLength(0.95);
          scale = crv.GetLength();
        }
        else
        {
          double len = crv.GetLength();
          pt = crv.PointAtLength(len - 0.05);
        }

        Plane pln = new Plane();
        crv.PerpendicularFrameAt(0.02, out pln);
        Vector3d vec = new Vector3d(pln.Normal);
        vec.Unitize();
        vec.Reverse();
        vec = new Vector3d(vec.X * 0.25 * scale, vec.Y * 0.25 * scale, vec.Z * 0.25 * scale);
        redLines10[i++] = new Line(pt, vec);
        //args.Pipeline.DrawLine(ln1, UI.Colour.Release, 3);
      }
      else
      {
        redLines10[i++] = Line.Unset;
      }
      if (end.YY == true)
      {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1)
        {
          pt = crv.PointAtNormalizedLength(0.95);
          scale = crv.GetLength();
        }
        else
        {
          double len = crv.GetLength();
          pt = crv.PointAtLength(len - 0.05);
        }

        Plane pln = new Plane();
        crv.PerpendicularFrameAt(0.02, out pln);
        pln.Rotate(angle_radian, pln.Normal);
        Vector3d vec1 = new Vector3d(pln.XAxis);
        vec1.Unitize();
        vec1 = new Vector3d(vec1.X * 0.05 * scale, vec1.Y * 0.05 * scale, vec1.Z * 0.05 * scale);
        Vector3d vec2 = new Vector3d(vec1);
        vec2.Reverse();
        Transform xf1 = Rhino.Geometry.Transform.Translation(vec1);
        Transform xf2 = Rhino.Geometry.Transform.Translation(vec2);
        Point3d pt1 = new Point3d(pt);
        pt1.Transform(xf1);
        Point3d pt2 = new Point3d(pt);
        pt2.Transform(xf2);

        Vector3d vec = new Vector3d(pln.XAxis);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.15 * scale, vec.Y * 0.15 * scale, vec.Z * 0.15 * scale);
        Vector3d vecRev = new Vector3d(vec);
        vecRev.Reverse();
        redLines10[i++] = new Line(pt1, vec);
        //args.Pipeline.DrawLine(ln1A, UI.Colour.Release);
        redLines10[i++] = new Line(pt2, vecRev);
        //args.Pipeline.DrawLine(ln1B, UI.Colour.Release);
      }
      else
      {
        redLines10[i++] = Line.Unset;
        redLines10[i++] = Line.Unset;
      }
      if (end.ZZ == true)
      {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1)
        {
          pt = crv.PointAtNormalizedLength(0.95);
          scale = crv.GetLength();
        }
        else
        {
          double len = crv.GetLength();
          pt = crv.PointAtLength(len - 0.05);
        }

        Plane pln = new Plane();
        crv.PerpendicularFrameAt(0.02, out pln);
        pln.Rotate(angle_radian, pln.Normal);
        Vector3d vec1 = new Vector3d(pln.YAxis);
        vec1.Unitize();
        vec1 = new Vector3d(vec1.X * 0.05 * scale, vec1.Y * 0.05 * scale, vec1.Z * 0.05 * scale);
        Vector3d vec2 = new Vector3d(vec1);
        vec2.Reverse();
        Transform xf1 = Rhino.Geometry.Transform.Translation(vec1);
        Transform xf2 = Rhino.Geometry.Transform.Translation(vec2);
        Point3d pt1 = new Point3d(pt);
        pt1.Transform(xf1);
        Point3d pt2 = new Point3d(pt);
        pt2.Transform(xf2);

        Vector3d vec = new Vector3d(pln.YAxis);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.15 * scale, vec.Y * 0.15 * scale, vec.Z * 0.15 * scale);
        Vector3d vecRev = new Vector3d(vec);
        vecRev.Reverse();
        redLines10[i++] = new Line(pt1, vec);
        //args.Pipeline.DrawLine(ln1A, UI.Colour.Release);
        redLines10[i++] = new Line(pt2, vecRev);
        //args.Pipeline.DrawLine(ln1B, UI.Colour.Release);
      }
      else
      {
        redLines10[i++] = Line.Unset;
        redLines10[i++] = Line.Unset;
      }
      #endregion
    }

    public static void PreviewRestraint(GsaBool6 restraint, Plane localAxis, Point3d pt, ref Brep support, ref Text3d text)
    {
      // pin
      if (restraint.X & restraint.Y & restraint.Z &
          !restraint.XX & !restraint.YY & !restraint.ZZ)
      {
        Plane plane = localAxis.Clone();
        if (!plane.IsValid) { plane = Plane.WorldXY; }
        plane.Origin = pt;
        Cone pin = new Cone(plane, -0.4, 0.4);
        support = pin.ToBrep(true);
      }
      else if (restraint.X & restraint.Y & restraint.Z &
              restraint.XX & restraint.YY & restraint.ZZ)
      {
        Plane plane = localAxis.Clone();
        if (!plane.IsValid) { plane = Plane.WorldXY; }
        plane.Origin = pt;
        Box fix = new Box(plane, new Interval(-0.3, 0.3), new Interval(-0.3, 0.3), new Interval(-0.2, 0));
        support = fix.ToBrep();
      }
      else
      {
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
        text = new Text3d(rest, plane, 0.3);
        text.HorizontalAlignment = Rhino.DocObjects.TextHorizontalAlignment.Left;
        text.VerticalAlignment = Rhino.DocObjects.TextVerticalAlignment.Top;
      }
    }

    public static void PreviewMem3d(ref Mesh solidMesh, ref List<Polyline> hiddenLines, ref List<Line> edgeLines, ref List<Point3d> pts)
    {
      Rhino.Geometry.Collections.MeshTopologyEdgeList alledges = solidMesh.TopologyEdges;
      if (solidMesh.FaceNormals.Count < solidMesh.Faces.Count)
        solidMesh.FaceNormals.ComputeFaceNormals();

      hiddenLines = new List<Polyline>();
      edgeLines = new List<Line>();
      // curves
      for (int i = 0; i < alledges.Count; i++)
      {
        int[] faceID = alledges.GetConnectedFaces(i);
        Vector3d vec1 = solidMesh.FaceNormals[faceID[0]];
        Vector3d vec2 = solidMesh.FaceNormals[faceID[1]];
        vec1.Unitize(); vec2.Unitize();
        if (!vec1.Equals(vec2) || faceID.Length > 2)
        {
          edgeLines.Add(alledges.EdgeLine(i));
        }
        else
        {
          Polyline hidden = new Polyline();
          hidden.Add(alledges.EdgeLine(i).PointAt(0));
          hidden.Add(alledges.EdgeLine(i).PointAt(1));
          hiddenLines.Add(hidden);
        }
      }

      //  points
      pts = new List<Point3d>(solidMesh.Vertices.ToPoint3dArray());

    }
  }
}
