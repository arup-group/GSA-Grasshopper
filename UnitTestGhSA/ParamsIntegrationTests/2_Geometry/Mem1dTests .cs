using System;

using NUnit.Framework;
using GhSA;
using GhSA.Parameters;
using Rhino.Geometry;
using GsaAPI;
using System.Collections.Generic;

namespace ParamsIntegrationTests
{
    public class Mem1dTests
    {
        [TestCase]
        public void TestCreateGsaMem1dFromCrv()
        {
            // create a list of control points
            List<Point3d> pts = new List<Point3d>();
            pts.Add(new Point3d(-3, -4, 0));
            pts.Add(new Point3d(5, -2, 0));
            pts.Add(new Point3d(2, 2, 0));
            pts.Add(new Point3d(6, 7, 0));

            // create nurbscurve from pts
            NurbsCurve crv = NurbsCurve.Create(false, 3, pts);

            // create 1d member from crv
            GsaMember1d mem = new GsaMember1d(crv);
            
            // set some members
            mem.Colour = System.Drawing.Color.Red;
            mem.ID = 3;
            mem.Member.Name = "gemma";
            mem.Member.IsDummy = true;
            mem.Member.Offset.Z = -0.45;
            mem.Member.Property = 2;
            mem.Member.Type1D = ElementType.BEAM;
            mem.Member.Type = MemberType.BEAM;

            // check that end-points are similar between converted curve and topology list
            Assert.AreEqual(mem.PolyCurve.PointAtStart.X, mem.Topology[0].X);
            Assert.AreEqual(mem.PolyCurve.PointAtStart.Y, mem.Topology[0].Y);
            Assert.AreEqual(mem.PolyCurve.PointAtEnd.X, mem.Topology[mem.Topology.Count - 1].X);
            Assert.AreEqual(mem.PolyCurve.PointAtEnd.Y, mem.Topology[mem.Topology.Count - 1].Y);

            // loop through segments and check they are either arc or line
            for (int i = 0; i < mem.PolyCurve.SegmentCount; i++)
                Assert.IsTrue(mem.PolyCurve.SegmentCurve(i).IsArc() ^ mem.PolyCurve.SegmentCurve(i).IsLinear());
            
            Assert.AreEqual(System.Drawing.Color.FromArgb(255, 255, 0, 0), mem.Member.Colour);
            Assert.AreEqual(3, mem.ID);
            Assert.AreEqual("gemma", mem.Member.Name);
            Assert.IsTrue(mem.Member.IsDummy);
            Assert.AreEqual(-0.45, mem.Member.Offset.Z);
            Assert.AreEqual(2, mem.Member.Property);
            Assert.AreEqual(ElementType.BEAM, mem.Member.Type1D);
            Assert.AreEqual(MemberType.BEAM, mem.Member.Type);
        }
        
        [TestCase]
        public void TestDuplicateMem1d()
        {
            // create a list of control points
            List<Point3d> pts = new List<Point3d>();
            pts.Add(new Point3d(0, 0, 0));
            pts.Add(new Point3d(0, 10, 0));

            // create nurbscurve from pts
            NurbsCurve crv = NurbsCurve.Create(false, 1, pts);

            // create 1d member from crv
            GsaMember1d orig = new GsaMember1d(crv);

            // set some members
            orig.Colour = System.Drawing.Color.Green;
            orig.ID = 2;
            orig.Member.Name = "Sally";
            orig.Member.IsDummy = false;
            orig.Member.Offset.X2 = 0.1;
            orig.Member.Property = 3;
            orig.Section.ID = 4;
            orig.Member.Group = 99;
            orig.Member.Type1D = ElementType.BAR;
            orig.Member.Type = MemberType.COLUMN;

            // duplicate member
            GsaMember1d dup = orig.Duplicate();

            // check that member is duplicated
            Assert.AreEqual(System.Drawing.Color.FromArgb(255, 0, 128, 0), dup.Member.Colour);
            Assert.AreEqual(2, dup.ID);
            Assert.AreEqual("Sally", dup.Member.Name);
            Assert.IsFalse(dup.Member.IsDummy);
            Assert.AreEqual(0.1, dup.Member.Offset.X2);
            Assert.AreEqual(3, dup.Member.Property);
            Assert.AreEqual(4, dup.Section.ID);
            Assert.AreEqual(99, dup.Member.Group);
            Assert.AreEqual(ElementType.BAR, dup.Member.Type1D);
            Assert.AreEqual(MemberType.COLUMN, dup.Member.Type);

            // make changes to original
            orig.Colour = System.Drawing.Color.White;
            orig.ID = 1;
            orig.Member.Name = "Peter Peterson";
            orig.Member.IsDummy = true;
            orig.Member.Offset.X2 = 0.4;
            orig.Member.Property = 2;
            orig.Section.ID = 1;
            orig.Member.Group = 4;
            orig.Member.Type1D = ElementType.BEAM;
            orig.Member.Type = MemberType.BEAM;

            // check that duplicate keeps its values
            Assert.AreEqual(System.Drawing.Color.FromArgb(255, 0, 128, 0), dup.Member.Colour);
            Assert.AreEqual(2, dup.ID);
            Assert.AreEqual("Sally", dup.Member.Name);
            Assert.IsFalse(dup.Member.IsDummy);
            Assert.AreEqual(0.1, dup.Member.Offset.X2);
            Assert.AreEqual(3, dup.Member.Property);
            Assert.AreEqual(4, dup.Section.ID);
            Assert.AreEqual(99, dup.Member.Group);
            Assert.AreEqual(ElementType.BAR, dup.Member.Type1D);
            Assert.AreEqual(MemberType.COLUMN, dup.Member.Type);

            // check that original is changed
            Assert.AreEqual(System.Drawing.Color.FromArgb(255, 255, 255, 255), orig.Member.Colour);
            Assert.AreEqual(1, orig.ID);
            Assert.AreEqual("Peter Peterson", orig.Member.Name);
            Assert.IsTrue(orig.Member.IsDummy);
            Assert.AreEqual(0.4, orig.Member.Offset.X2);
            Assert.AreEqual(2, orig.Member.Property);
            Assert.AreEqual(1, orig.Section.ID);
            Assert.AreEqual(4, orig.Member.Group);
            Assert.AreEqual(ElementType.BEAM, orig.Member.Type1D);
            Assert.AreEqual(MemberType.BEAM, orig.Member.Type);
        }
    }
}