using System.Collections.Generic;

using Grasshopper.Kernel;

using GsaAPI;

using GsaGH.Helpers.Graphics;

using Rhino.Geometry;

using Line = Rhino.Geometry.Line;

namespace GsaGH.Parameters {
  public class ReleasePreview {
    public List<Line> PreviewGreenLines { get; set; } = new List<Line>();
    public List<Line> PreviewRedLines { get; set; } = new List<Line>();
    public List<Line> PreviewXXLines { get; set; } = new List<Line>();

    public ReleasePreview() { }

    internal ReleasePreview(PolyCurve crv, double angleRadian, Bool6 start, Bool6 end) {
      PreviewGreenLines = new List<Line>();
      PreviewRedLines = new List<Line>();

      if (start == null || end == null) {
        return;
      }

      #region translation start
      if (start.X) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(0);
          scale = crv.GetLength();
        } else {
          pt = crv.PointAtLength(0);
        }

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
        PreviewGreenLines.Add(new Line(pt1, vec));
        PreviewGreenLines.Add(new Line(pt2, vec));
      } else {
        PreviewGreenLines.Add(Line.Unset);
        PreviewGreenLines.Add(Line.Unset);
      }

      if (start.Y) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(0.05);
          scale = crv.GetLength();
        } else {
          pt = crv.PointAtLength(0.05);
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
        PreviewGreenLines.Add(new Line(pt3A, vec));
        PreviewGreenLines.Add(new Line(pt3B, vecRev));
        PreviewGreenLines.Add(new Line(pt4A, vec));
        PreviewGreenLines.Add(new Line(pt4B, vecRev));
      } else {
        PreviewGreenLines.Add(Line.Unset);
        PreviewGreenLines.Add(Line.Unset);
        PreviewGreenLines.Add(Line.Unset);
        PreviewGreenLines.Add(Line.Unset);
      }

      if (start.Z) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(0.05);
          scale = crv.GetLength();
        } else {
          pt = crv.PointAtLength(0.05);
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
        PreviewGreenLines.Add(new Line(pt3A, vec));
        PreviewGreenLines.Add(new Line(pt3B, vecRev));
        PreviewGreenLines.Add(new Line(pt4A, vec));
        PreviewGreenLines.Add(new Line(pt4B, vecRev));
      } else {
        PreviewGreenLines.Add(Line.Unset);
        PreviewGreenLines.Add(Line.Unset);
        PreviewGreenLines.Add(Line.Unset);
        PreviewGreenLines.Add(Line.Unset);
      }

      #endregion

      #region translation end
      if (end.X) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(1);
          scale = crv.GetLength();
        } else {
          double len = crv.GetLength();
          pt = crv.PointAtLength(len);
        }

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
        vec.Reverse();
        PreviewGreenLines.Add(new Line(pt1, vec));
        PreviewGreenLines.Add(new Line(pt2, vec));
      }

      if (end.Y) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(0.95);
          scale = crv.GetLength();
        } else {
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
        PreviewGreenLines.Add(new Line(pt3A, vec));
        PreviewGreenLines.Add(new Line(pt3B, vecRev));
        PreviewGreenLines.Add(new Line(pt4A, vec));
        PreviewGreenLines.Add(new Line(pt4B, vecRev));
      }

      if (end.Z) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(0.95);
          scale = crv.GetLength();
        } else {
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
        PreviewGreenLines.Add(new Line(pt3A, vec));
        PreviewGreenLines.Add(new Line(pt3B, vecRev));
        PreviewGreenLines.Add(new Line(pt4A, vec));
        PreviewGreenLines.Add(new Line(pt4B, vecRev));
      }

      #endregion

      #region rotation start
      if (start.XX) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(0);
          scale = crv.GetLength();
        } else {
          pt = crv.PointAtLength(0);
        }

        crv.PerpendicularFrameAt(0.02, out Plane pln);
        var vec = new Vector3d(pln.Normal);
        vec.Unitize();
        vec = new Vector3d(vec.X * 0.25 * scale, vec.Y * 0.25 * scale, vec.Z * 0.25 * scale);
        PreviewXXLines.Add(new Line(pt, vec));
      }

      if (start.YY) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(0.05);
          scale = crv.GetLength();
        } else {
          pt = crv.PointAtLength(0.05);
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
        PreviewRedLines.Add(new Line(pt1, vec));
        PreviewRedLines.Add(new Line(pt2, vecRev));
      }

      if (start.ZZ) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(0.05);
          scale = crv.GetLength();
        } else {
          pt = crv.PointAtLength(0.05);
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
        PreviewRedLines.Add(new Line(pt1, vec));
        PreviewRedLines.Add(new Line(pt2, vecRev));
      }

      #endregion

      #region rotation end
      if (end.XX) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(1);
          scale = crv.GetLength();
        } else {
          double len = crv.GetLength();
          pt = crv.PointAtLength(len);
        }

        crv.PerpendicularFrameAt(0.02, out Plane pln);
        var vec = new Vector3d(pln.Normal);
        vec.Unitize();
        vec.Reverse();
        vec = new Vector3d(vec.X * 0.25 * scale, vec.Y * 0.25 * scale, vec.Z * 0.25 * scale);
        PreviewXXLines.Add(new Line(pt, vec));
      }

      if (end.YY) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(0.95);
          scale = crv.GetLength();
        } else {
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
        PreviewRedLines.Add(new Line(pt1, vec));
        PreviewRedLines.Add(new Line(pt2, vecRev));
      }

      if (end.ZZ) {
        Point3d pt;
        double scale = 1;
        if (crv.GetLength() < 1) {
          pt = crv.PointAtNormalizedLength(0.95);
          scale = crv.GetLength();
        } else {
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
        PreviewRedLines.Add(new Line(pt1, vec));
        PreviewRedLines.Add(new Line(pt2, vecRev));
      }

      #endregion
    }

    public void DrawViewportWires(GH_PreviewWireArgs args) {
      args.Pipeline.DrawLines(PreviewGreenLines, Colours.Support, 2);
      args.Pipeline.DrawLines(PreviewRedLines, Colours.Release, 2);
      args.Pipeline.DrawLines(PreviewXXLines, Colours.Release, 4);
    }
  }
}
