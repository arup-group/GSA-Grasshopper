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
        public static void Preview1D(PolyCurve crv, double angle, GsaBool6 start, GsaBool6 end,
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
        public static void PreviewRestraint(GsaBool6 restraint, Plane localAxis, Point3d pt, ref Brep support, ref Rhino.Display.Text3d text)
        {
            // pin
            if (restraint.X == true & restraint.Y == true & restraint.Z == true &
                restraint.XX == false & restraint.YY == false & restraint.ZZ == false)
            {
                Plane plane = localAxis.Clone();
                if (!plane.IsValid) { plane = Plane.WorldXY; }
                plane.Origin = pt;
                Cone pin = new Cone(plane, -0.4, 0.4);
                support = pin.ToBrep(true);
            }
            else if (restraint.X == true & restraint.Y == true & restraint.Z == true &
                    restraint.XX == true & restraint.YY == true & restraint.ZZ == true)
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
                if (restraint.X == true)
                    rest += "X";
                if (restraint.Y == true)
                    rest += "Y";
                if (restraint.Z == true)
                    rest += "Z";
                if (restraint.XX == true)
                    rest += "XX";
                if (restraint.YY == true)
                    rest += "YY";
                if (restraint.ZZ == true)
                    rest += "ZZ";
                text = new Text3d(rest, plane, 0.3);
                text.HorizontalAlignment = Rhino.DocObjects.TextHorizontalAlignment.Left;
                text.VerticalAlignment = Rhino.DocObjects.TextVerticalAlignment.Top;
            }
        }
    }
}
