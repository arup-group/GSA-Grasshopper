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
            PolylineCurve crv = new PolylineCurve(pts);
            //NurbsCurve crv = NurbsCurve.Create(false, 3, pts);

            // create 1d member from crv
            GsaMember1d mem = new GsaMember1d(crv);
            
            // set some members
            mem.Colour = System.Drawing.Color.Red;
            mem.ID = 3;
            mem.Name = "gemma";
            mem.IsDummy = true;
            mem.Offset = new GsaOffset(0, 0, 0, -0.45);
            mem.Section.ID = 2;
            mem.Type1D = ElementType.BEAM;
            mem.Type = MemberType.BEAM;

            // check that end-points are similar between converted curve and topology list
            Assert.AreEqual(mem.PolyCurve.PointAtStart.X, mem.Topology[0].X);
            Assert.AreEqual(mem.PolyCurve.PointAtStart.Y, mem.Topology[0].Y);
            Assert.AreEqual(mem.PolyCurve.PointAtEnd.X, mem.Topology[mem.Topology.Count - 1].X);
            Assert.AreEqual(mem.PolyCurve.PointAtEnd.Y, mem.Topology[mem.Topology.Count - 1].Y);

            // loop through segments and check they are either arc or line
            for (int i = 0; i < mem.PolyCurve.SegmentCount; i++)
                Assert.IsTrue(mem.PolyCurve.SegmentCurve(i).IsLinear() || mem.PolyCurve.SegmentCurve(i).IsArc());
            
            Assert.AreEqual(System.Drawing.Color.FromArgb(255, 255, 0, 0), mem.API_Member.Colour);
            Assert.AreEqual(3, mem.ID);
            Assert.AreEqual("gemma", mem.Name);
            Assert.IsTrue(mem.IsDummy);
            Assert.AreEqual(-0.45, mem.Offset.Z);
            Assert.AreEqual(2, mem.Section.ID);
            Assert.AreEqual(ElementType.BEAM, mem.Type1D);
            Assert.AreEqual(MemberType.BEAM, mem.Type);
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
            orig.Name = "Sally";
            orig.IsDummy = false;
            orig.Offset = new GsaOffset(0, 0.1, 0, 0);
            orig.Section = new GsaSection();
            orig.Section.ID = 4;
            orig.Group = 99;
            orig.Type1D = ElementType.BAR;
            orig.Type = MemberType.COLUMN;

            // duplicate member
            GsaMember1d dup = orig.Duplicate();

            // check that member is duplicated
            Assert.AreEqual(System.Drawing.Color.FromArgb(255, 0, 128, 0), dup.API_Member.Colour);
            Assert.AreEqual(2, dup.ID);
            Assert.AreEqual("Sally", dup.Name);
            Assert.IsFalse(dup.IsDummy);
            Assert.AreEqual(0.1, dup.Offset.X2);
            Assert.AreEqual(4, dup.Section.ID);
            Assert.AreEqual(99, dup.Group);
            Assert.AreEqual(ElementType.BAR, dup.Type1D);
            Assert.AreEqual(MemberType.COLUMN, dup.Type);

            // make changes to original
            orig.Colour = System.Drawing.Color.White;
            orig.ID = 1;
            orig.Name = "Peter Peterson";
            orig.IsDummy = true;
            orig.Offset = new GsaOffset(0, 0.4, 0, 0);
            orig.Section.ID = 1;
            orig.Group = 4;
            orig.Type1D = ElementType.BEAM;
            orig.Type = MemberType.BEAM;

            // check that duplicate keeps its values
            Assert.AreEqual(System.Drawing.Color.FromArgb(255, 0, 128, 0), dup.API_Member.Colour);
            Assert.AreEqual(2, dup.ID);
            Assert.AreEqual("Sally", dup.Name);
            Assert.IsFalse(dup.IsDummy);
            Assert.AreEqual(0.1, dup.Offset.X2);
            Assert.AreEqual(4, dup.Section.ID);
            Assert.AreEqual(99, dup.Group);
            Assert.AreEqual(ElementType.BAR, dup.Type1D);
            Assert.AreEqual(MemberType.COLUMN, dup.Type);

            // check that original is changed
            Assert.AreEqual(System.Drawing.Color.FromArgb(255, 255, 255, 255), orig.API_Member.Colour);
            Assert.AreEqual(1, orig.ID);
            Assert.AreEqual("Peter Peterson", orig.Name);
            Assert.IsTrue(orig.IsDummy);
            Assert.AreEqual(0.4, orig.Offset.X2);
            Assert.AreEqual(1, orig.Section.ID);
            Assert.AreEqual(4, orig.Group);
            Assert.AreEqual(ElementType.BEAM, orig.Type1D);
            Assert.AreEqual(MemberType.BEAM, orig.Type);
        }
    }
}