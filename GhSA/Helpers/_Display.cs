using System.Drawing;
using System.Collections.Generic;
using Rhino.Display;
using Grasshopper.Kernel;
using Rhino.Geometry;
using GhSA.Parameters;

namespace GhSA.UI
{
    /// <summary>
    /// Colour class holding the main colours used in colour scheme. 
    /// Make calls to this class to be able to easy update colours.
    /// 
    /// </summary>
    public class Display
    {
        public static void DrawReleases(GH_PreviewWireArgs args, PolyCurve crv, double angle, GsaBool6 start, GsaBool6 end)
        {
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
                pln.Rotate(angle, pln.Normal);
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
                Line ln1 = new Line(pt1, vec);
                args.Pipeline.DrawLine(ln1, UI.Colour.Support);
                Line ln2 = new Line(pt2, vec);
                args.Pipeline.DrawLine(ln2, UI.Colour.Support);
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
                pln.Rotate(angle, pln.Normal);
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
                Line ln1A = new Line(pt3A, vec);
                args.Pipeline.DrawLine(ln1A, UI.Colour.Support);
                Line ln1B = new Line(pt3B, vecRev);
                args.Pipeline.DrawLine(ln1B, UI.Colour.Support);
                Line ln2A = new Line(pt4A, vec);
                args.Pipeline.DrawLine(ln2A, UI.Colour.Support);
                Line ln2B = new Line(pt4B, vecRev);
                args.Pipeline.DrawLine(ln2B, UI.Colour.Support);
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
                pln.Rotate(angle, pln.Normal);
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
                Line ln1A = new Line(pt3A, vec);
                args.Pipeline.DrawLine(ln1A, UI.Colour.Support);
                Line ln1B = new Line(pt3B, vecRev);
                args.Pipeline.DrawLine(ln1B, UI.Colour.Support);
                Line ln2A = new Line(pt4A, vec);
                args.Pipeline.DrawLine(ln2A, UI.Colour.Support);
                Line ln2B = new Line(pt4B, vecRev);
                args.Pipeline.DrawLine(ln2B, UI.Colour.Support);
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
                pln.Rotate(angle, pln.Normal);
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
                Line ln1 = new Line(pt1, vec);
                args.Pipeline.DrawLine(ln1, UI.Colour.Support);
                Line ln2 = new Line(pt2, vec);
                args.Pipeline.DrawLine(ln2, UI.Colour.Support);
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
                pln.Rotate(angle, pln.Normal);
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
                Line ln1A = new Line(pt3A, vec);
                args.Pipeline.DrawLine(ln1A, UI.Colour.Support);
                Line ln1B = new Line(pt3B, vecRev);
                args.Pipeline.DrawLine(ln1B, UI.Colour.Support);
                Line ln2A = new Line(pt4A, vec);
                args.Pipeline.DrawLine(ln2A, UI.Colour.Support);
                Line ln2B = new Line(pt4B, vecRev);
                args.Pipeline.DrawLine(ln2B, UI.Colour.Support);
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
                pln.Rotate(angle, pln.Normal);
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
                Line ln1A = new Line(pt3A, vec);
                args.Pipeline.DrawLine(ln1A, UI.Colour.Support);
                Line ln1B = new Line(pt3B, vecRev);
                args.Pipeline.DrawLine(ln1B, UI.Colour.Support);
                Line ln2A = new Line(pt4A, vec);
                args.Pipeline.DrawLine(ln2A, UI.Colour.Support);
                Line ln2B = new Line(pt4B, vecRev);
                args.Pipeline.DrawLine(ln2B, UI.Colour.Support);
            }
            #endregion
            #region rotation start
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
                Line ln1 = new Line(pt, vec);
                args.Pipeline.DrawLine(ln1, UI.Colour.Release, 3);
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
                pln.Rotate(angle, pln.Normal);
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
                Line ln1A = new Line(pt1, vec);
                args.Pipeline.DrawLine(ln1A, UI.Colour.Release);
                Line ln1B = new Line(pt2, vecRev);
                args.Pipeline.DrawLine(ln1B, UI.Colour.Release);
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
                pln.Rotate(angle, pln.Normal);
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
                Line ln1A = new Line(pt1, vec);
                args.Pipeline.DrawLine(ln1A, UI.Colour.Release);
                Line ln1B = new Line(pt2, vecRev);
                args.Pipeline.DrawLine(ln1B, UI.Colour.Release);
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
                Line ln1 = new Line(pt, vec);
                args.Pipeline.DrawLine(ln1, UI.Colour.Release, 3);
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
                pln.Rotate(angle, pln.Normal);
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
                Line ln1A = new Line(pt1, vec);
                args.Pipeline.DrawLine(ln1A, UI.Colour.Release);
                Line ln1B = new Line(pt2, vecRev);
                args.Pipeline.DrawLine(ln1B, UI.Colour.Release);
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
                pln.Rotate(angle, pln.Normal);
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
                Line ln1A = new Line(pt1, vec);
                args.Pipeline.DrawLine(ln1A, UI.Colour.Release);
                Line ln1B = new Line(pt2, vecRev);
                args.Pipeline.DrawLine(ln1B, UI.Colour.Release);
            }
            #endregion
        }
    }
}
