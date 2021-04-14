using System;

using NUnit.Framework;
using GhSA;
using GhSA.Parameters;
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
            elem.Element.Group = 4;
            elem.Element.IsDummy = true;
            elem.Element.Name = "EltonJohn";
            elem.Element.Offset.Y = 14.3;
            elem.Element.OrientationAngle = 90;
            elem.Element.OrientationNode = 666;
            elem.Element.Property = 3;

            // check the line end points are correct
            Assert.AreEqual(1, elem.Line.PointAtStart.X);
            Assert.AreEqual(4, elem.Line.PointAtStart.Y);
            Assert.AreEqual(6, elem.Line.PointAtStart.Z);
            Assert.AreEqual(-2, elem.Line.PointAtEnd.X);
            Assert.AreEqual(3, elem.Line.PointAtEnd.Y);
            Assert.AreEqual(-5, elem.Line.PointAtEnd.Z);

            // check other members are valid
            Assert.AreEqual(66, elem.ID);
            Assert.AreEqual(2, elem.Section.ID);
            Assert.AreEqual(System.Drawing.Color.FromArgb(255, 255, 255, 0), elem.Element.Colour);
            Assert.AreEqual(4, elem.Element.Group);
            Assert.IsTrue(elem.Element.IsDummy);
            Assert.AreEqual("EltonJohn", elem.Element.Name);
            Assert.AreEqual(14.3, elem.Element.Offset.Y);
            Assert.AreEqual(90, elem.Element.OrientationAngle);
            Assert.AreEqual(666, elem.Element.OrientationNode);
            Assert.AreEqual(3, elem.Element.Property);
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
            orig.Element.Group = 1;
            orig.Element.IsDummy = false;
            orig.Element.Name = "Tilman";
            orig.Element.Offset.Y = -0.9991;
            orig.Element.OrientationAngle = -0.14;
            orig.Element.OrientationNode = 2;
            orig.Element.Property = 1;

            // duplicate original
            GsaElement1d dup = orig.Duplicate();

            // make some changes to original
            orig.Line = new LineCurve(new Line(new Point3d(1, 1, -4), new Point3d(1, 1, 0)));
            orig.ID = 5;
            orig.Section.ID = 9;
            orig.Colour = System.Drawing.Color.Red;
            orig.Element.Group = 2;
            orig.Element.IsDummy = true;
            orig.Element.Name = "Hugh";
            orig.Element.Offset.Y = -0.99991;
            orig.Element.OrientationAngle = 0;
            orig.Element.OrientationNode = 3;
            orig.Element.Property = 2;

            // check that values in duplicate are not changed
            Assert.AreEqual(2, dup.Line.PointAtStart.X);
            Assert.AreEqual(-1, dup.Line.PointAtStart.Y);
            Assert.AreEqual(0, dup.Line.PointAtStart.Z);
            Assert.AreEqual(2, dup.Line.PointAtEnd.X);
            Assert.AreEqual(-1, dup.Line.PointAtEnd.Y);
            Assert.AreEqual(4, dup.Line.PointAtEnd.Z);
            Assert.AreEqual(3, dup.ID);
            Assert.AreEqual(7, dup.Section.ID);
            Assert.AreEqual(System.Drawing.Color.FromArgb(255, 0, 255, 255), dup.Element.Colour);
            Assert.AreEqual(1, dup.Element.Group);
            Assert.IsFalse(dup.Element.IsDummy);
            Assert.AreEqual("Tilman", dup.Element.Name);
            Assert.AreEqual(-0.9991, dup.Element.Offset.Y);
            Assert.AreEqual(-0.14, dup.Element.OrientationAngle);
            Assert.AreEqual(2, dup.Element.OrientationNode);
            Assert.AreEqual(1, dup.Element.Property);

            // check that original has changed values
            Assert.AreEqual(1, orig.Line.PointAtStart.X);
            Assert.AreEqual(1, orig.Line.PointAtStart.Y);
            Assert.AreEqual(-4, orig.Line.PointAtStart.Z);
            Assert.AreEqual(1, orig.Line.PointAtEnd.X);
            Assert.AreEqual(1, orig.Line.PointAtEnd.Y);
            Assert.AreEqual(0, orig.Line.PointAtEnd.Z);
            Assert.AreEqual(5, orig.ID);
            Assert.AreEqual(9, orig.Section.ID);
            Assert.AreEqual(System.Drawing.Color.FromArgb(255, 255, 0, 0), orig.Element.Colour);
            Assert.AreEqual(2, orig.Element.Group);
            Assert.IsTrue(orig.Element.IsDummy);
            Assert.AreEqual("Hugh", orig.Element.Name);
            Assert.AreEqual(-0.99991, orig.Element.Offset.Y);
            Assert.AreEqual(0, orig.Element.OrientationAngle);
            Assert.AreEqual(3, orig.Element.OrientationNode);
            Assert.AreEqual(2, orig.Element.Property);
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