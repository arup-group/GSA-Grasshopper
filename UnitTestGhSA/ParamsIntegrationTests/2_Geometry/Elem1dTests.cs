using System;

using NUnit.Framework;
using GsaGH;
using GsaGH.Parameters;
using Rhino.Geometry;
using GsaAPI;
using System.Collections.Generic;

namespace ParamsIntegrationTests
{
    public class Elem1dTests
    {
        [TestCase]
        public void TestCreateGsaElem1dFromLn()
        {
            // create new line
            Line ln = new Line(new Point3d(1, 4, 6), new Point3d(-2, 3, -5));

            // create element
            GsaElement1d elem = new GsaElement1d(new LineCurve(ln));

            // set some element class members
            elem.ID = 66;
            elem.Section = new GsaSection();
            elem.Section.ID = 2;
            elem.Colour = System.Drawing.Color.Yellow;
            elem.Group = 4;
            elem.IsDummy = true;
            elem.Name = "EltonJohn";
            GsaOffset offset = new GsaOffset(0, 0, 14.3, 0);
            elem.Offset = offset;
            elem.OrientationAngle = 90;
            elem.Section.ID = 3;

            // check the line end points are correct
            Assert.AreEqual(1, elem.Line.PointAtStart.X);
            Assert.AreEqual(4, elem.Line.PointAtStart.Y);
            Assert.AreEqual(6, elem.Line.PointAtStart.Z);
            Assert.AreEqual(-2, elem.Line.PointAtEnd.X);
            Assert.AreEqual(3, elem.Line.PointAtEnd.Y);
            Assert.AreEqual(-5, elem.Line.PointAtEnd.Z);

            // check other members are valid
            Assert.AreEqual(66, elem.ID);
            Assert.AreEqual(3, elem.Section.ID);
            Assert.AreEqual(System.Drawing.Color.FromArgb(255, 255, 255, 0), elem.Colour);
            Assert.AreEqual(4, elem.Group);
            Assert.IsTrue(elem.IsDummy);
            Assert.AreEqual("EltonJohn", elem.Name);
            Assert.AreEqual(14.3, elem.Offset.Y);
            Assert.AreEqual(90, elem.OrientationAngle);
            Assert.AreEqual(3, elem.Section.ID);
        }
        
        [TestCase]
        public void TestDuplicateElem1d()
        {
            // create new line
            Line ln = new Line(new Point3d(2, -1, 0), new Point3d(2, -1, 4));

            // create element
            GsaElement1d orig = new GsaElement1d(new LineCurve(ln));

            // set some element class members
            orig.ID = 3;
            orig.Section = new GsaSection();
            orig.Section.ID = 7;
            orig.Colour = System.Drawing.Color.Aqua;
            orig.Group = 1;
            orig.IsDummy = false;
            orig.Name = "Tilman";
            GsaOffset offset = new GsaOffset(0, 0, 2.9, 0);
            orig.Offset = offset;
            orig.OrientationAngle = -0.14;

            // duplicate original
            GsaElement1d dup = orig.Duplicate();

            // make some changes to original
            orig.Line = new LineCurve(new Line(new Point3d(1, 1, -4), new Point3d(1, 1, 0)));
            orig.ID = 5;
            orig.Section.ID = 9;
            orig.Colour = System.Drawing.Color.Red;
            orig.Group = 2;
            orig.IsDummy = true;
            orig.Name = "Hugh";
            GsaOffset offset2 = new GsaOffset(0, 0, -0.991, 0);
            orig.Offset = offset2;
            orig.OrientationAngle = 0;

            // check that values in duplicate are not changed
            Assert.AreEqual(2, dup.Line.PointAtStart.X, 1E-9);
            Assert.AreEqual(-1, dup.Line.PointAtStart.Y, 1E-9);
            Assert.AreEqual(0, dup.Line.PointAtStart.Z, 1E-9);
            Assert.AreEqual(2, dup.Line.PointAtEnd.X, 1E-9);
            Assert.AreEqual(-1, dup.Line.PointAtEnd.Y, 1E-9);
            Assert.AreEqual(4, dup.Line.PointAtEnd.Z, 1E-9);
            Assert.AreEqual(3, dup.ID);
            Assert.AreEqual(7, dup.Section.ID);
            Assert.AreEqual(System.Drawing.Color.FromArgb(255, 0, 255, 255), dup.Colour);
            Assert.AreEqual(1, dup.Group);
            Assert.IsFalse(dup.IsDummy);
            Assert.AreEqual("Tilman", dup.Name);
            Assert.AreEqual(2.9, dup.Offset.Y.Value, 1E-9);
            Assert.AreEqual(-0.14, dup.OrientationAngle, 1E-9);

            // check that original has changed values
            Assert.AreEqual(1, orig.Line.PointAtStart.X, 1E-9);
            Assert.AreEqual(1, orig.Line.PointAtStart.Y, 1E-9);
            Assert.AreEqual(-4, orig.Line.PointAtStart.Z, 1E-9);
            Assert.AreEqual(1, orig.Line.PointAtEnd.X, 1E-9);
            Assert.AreEqual(1, orig.Line.PointAtEnd.Y, 1E-9);
            Assert.AreEqual(0, orig.Line.PointAtEnd.Z, 1E-9);
            Assert.AreEqual(5, orig.ID);
            Assert.AreEqual(9, orig.Section.ID);
            Assert.AreEqual(System.Drawing.Color.FromArgb(255, 255, 0, 0), orig.Colour);
            Assert.AreEqual(2, orig.Group);
            Assert.IsTrue(orig.IsDummy);
            Assert.AreEqual("Hugh", orig.Name);
            Assert.AreEqual(-0.991, orig.Offset.Y.Value, 1E-9);
            Assert.AreEqual(0, orig.OrientationAngle, 1E-9);
        }

        [TestCase]
        public void TestCreateGsaElem1dGetReleases()
        {
            // create new line
            Line ln = new Line(new Point3d(1, 4, 6), new Point3d(-2, 3, -5));

            // create element
            GsaElement1d elem = new GsaElement1d(new LineCurve(ln));

            GsaBool6 rel1 = elem.ReleaseStart;
            Assert.IsFalse(rel1.X);
            Assert.IsFalse(rel1.Y);
            Assert.IsFalse(rel1.Z);
            Assert.IsFalse(rel1.XX);
            Assert.IsFalse(rel1.YY);
            Assert.IsFalse(rel1.ZZ);
        }
    }
}