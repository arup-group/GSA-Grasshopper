using System;

using NUnit.Framework;
using GsaGH;
using GsaGH.Parameters;
using Rhino.Geometry;
using GsaAPI;
using System.Collections.Generic;

namespace ParamsIntegrationTests
{
    public class Mem2dTests
    {
        [TestCase]
        public void TestCreateGsaMem2dFromBrep()
        {
            // create a list of corner points
            List<Point3d> pts = new List<Point3d>();
            pts.Add(new Point3d(-3, -4, 0));
            pts.Add(new Point3d(5, -2, 0));
            pts.Add(new Point3d(6, 7, 0));
            pts.Add(new Point3d(-1, 2, 0));
            pts.Add(pts[0]); // add initial point to close curve
            Polyline pol = new Polyline(pts); // create edge-crv from pts
            // create planar brep from polyline
            Brep brep = Brep.CreatePlanarBreps(pol.ToNurbsCurve(), 0.001)[0];

            // empty lists for inclusion points and lines
            List<Point3d> inclpts = new List<Point3d>();
            List<Curve> inclcrvs = new List<Curve>();
            
            // create 2d member from brep
            GsaMember2d mem = new GsaMember2d(brep, inclcrvs, inclpts);
            
            // set some members
            mem.Colour = System.Drawing.Color.White;
            mem.ID = 4;
            mem.MeshSize = 0.56;
            mem.Name = "meminem";
            mem.IsDummy = true;
            mem.Offset = new GsaOffset(0, 0, 0, -0.45);
            mem.Property.ID = 2;
            mem.Type2D = AnalysisOrder.LINEAR;
            mem.Type = MemberType.SLAB;

            Assert.AreEqual(mem.Brep.Vertices[0].Location.X, mem.Topology[0].X);
            Assert.AreEqual(mem.Brep.Vertices[0].Location.Y, mem.Topology[0].Y);
            Assert.AreEqual(mem.Brep.Vertices[1].Location.X, mem.Topology[1].X);
            Assert.AreEqual(mem.Brep.Vertices[1].Location.Y, mem.Topology[1].Y);
            Assert.AreEqual(mem.Brep.Vertices[2].Location.X, mem.Topology[2].X);
            Assert.AreEqual(mem.Brep.Vertices[2].Location.Y, mem.Topology[2].Y);
            Assert.AreEqual(mem.Brep.Vertices[3].Location.X, mem.Topology[3].X);
            Assert.AreEqual(mem.Brep.Vertices[3].Location.Y, mem.Topology[3].Y);
            Assert.AreEqual(mem.Brep.Vertices[0].Location.X, mem.Topology[4].X);
            Assert.AreEqual(mem.Brep.Vertices[0].Location.Y, mem.Topology[4].Y);

            Assert.AreEqual(System.Drawing.Color.FromArgb(255, 255, 255, 255), mem.Colour);
            Assert.AreEqual(4, mem.ID);
            Assert.AreEqual(0.56, mem.MeshSize);
            Assert.AreEqual("meminem", mem.Name);
            Assert.IsTrue(mem.IsDummy);
            Assert.AreEqual(-0.45, mem.Offset.Z);
            Assert.AreEqual(2, mem.Property.ID);
            Assert.AreEqual(AnalysisOrder.LINEAR, mem.Type2D);
            Assert.AreEqual(MemberType.SLAB, mem.Type);
        }
        
        [TestCase]
        public void TestDuplicateMem2d()
        {
            // create a list of corner points
            List<Point3d> pts = new List<Point3d>();
            pts.Add(new Point3d(1, 1, 0));
            pts.Add(new Point3d(0, 5, 0));
            pts.Add(new Point3d(6, 7, 0));
            pts.Add(new Point3d(4, 2, 0));
            pts.Add(pts[0]); // add initial point to close curve
            Polyline pol = new Polyline(pts); // create edge-crv from pts
            // create planar brep from polyline
            Brep brep = Brep.CreatePlanarBreps(pol.ToNurbsCurve(), 0.001)[0];

            // lists for inclusion points and lines
            List<Point3d> inclpts = new List<Point3d>();
            inclpts.Add(new Point3d(1, 1, 0));
            List<Curve> inclcrvs = new List<Curve>();
            LineCurve line = new LineCurve(new Point3d(1, 2, 0), new Point3d(3, 2, 0));
            inclcrvs.Add(line);

            // create 2d member from brep
            GsaMember2d original = new GsaMember2d(brep, inclcrvs, inclpts);

            // set some members
            original.Colour = System.Drawing.Color.Blue;
            original.ID = 13;
            original.MeshSize = 1.56;
            original.Name = "ehbaba";
            original.IsDummy = false;
            original.Offset = new GsaOffset(0.33, 0, 0, 0);
            original.Property.ID = 3;
            original.Type2D = AnalysisOrder.RIGID_DIAPHRAGM;
            original.Type = MemberType.GENERIC_2D;

            // create duplicate
            GsaMember2d dup = original.Duplicate();

            Assert.AreEqual(original.Brep.Vertices[0].Location.X, dup.Topology[0].X);
            Assert.AreEqual(original.Brep.Vertices[0].Location.Y, dup.Topology[0].Y);
            Assert.AreEqual(original.Brep.Vertices[1].Location.X, dup.Topology[1].X);
            Assert.AreEqual(original.Brep.Vertices[1].Location.Y, dup.Topology[1].Y);
            Assert.AreEqual(original.Brep.Vertices[2].Location.X, dup.Topology[2].X);
            Assert.AreEqual(original.Brep.Vertices[2].Location.Y, dup.Topology[2].Y);
            Assert.AreEqual(original.Brep.Vertices[3].Location.X, dup.Topology[3].X);
            Assert.AreEqual(original.Brep.Vertices[3].Location.Y, dup.Topology[3].Y);
            Assert.AreEqual(original.Brep.Vertices[0].Location.X, dup.Topology[4].X);
            Assert.AreEqual(original.Brep.Vertices[0].Location.Y, dup.Topology[4].Y);
            Assert.AreEqual(1, dup.IncLinesTopology[0][0].X);
            Assert.AreEqual(2, dup.IncLinesTopology[0][0].Y);
            Assert.AreEqual(3, dup.IncLinesTopology[0][1].X);
            Assert.AreEqual(2, dup.IncLinesTopology[0][1].Y);
            Assert.AreEqual(1, dup.InclusionPoints[0].X);
            Assert.AreEqual(1, dup.InclusionPoints[0].Y);

            Assert.AreEqual(System.Drawing.Color.FromArgb(255, 0, 0, 255), dup.Colour);
            Assert.AreEqual(13, dup.ID);
            Assert.AreEqual(1.56, dup.MeshSize);
            Assert.AreEqual("ehbaba", dup.Name);
            Assert.IsFalse(dup.IsDummy);
            Assert.AreEqual(0.33, dup.Offset.X1);
            Assert.AreEqual(3, dup.Property.ID);
            Assert.AreEqual(AnalysisOrder.RIGID_DIAPHRAGM, dup.Type2D);
            Assert.AreEqual(MemberType.GENERIC_2D, dup.Type);

            // make some changes to original
            // create a list of corner points
            List<Point3d> pts2 = new List<Point3d>();
            pts2.Add(new Point3d(0, 0, 0));
            pts2.Add(new Point3d(5, 0, 0));
            pts2.Add(new Point3d(7, 6, 0));
            pts2.Add(new Point3d(2, 4, 0));
            pts2.Add(pts2[0]); // add initial point to close curve
            Polyline pol2 = new Polyline(pts2); // create edge-crv from pts
            // create planar brep from polyline
            Brep brep2 = Brep.CreatePlanarBreps(pol2.ToNurbsCurve(), 0.001)[0];

            // set new brep
            original = original.UpdateGeometry(brep2);
            // changes to class members
            original.Colour = System.Drawing.Color.Black;
            original.ID = 7;
            original.MeshSize = 0;
            original.Name = "Persepolis";
            original.IsDummy = true;
            original.Offset = new GsaOffset(0.12, 0, 0, 0);
            original.Property.ID = 44;
            original.Type2D = AnalysisOrder.QUADRATIC;
            original.Type = MemberType.WALL;

            // check that orignal are not equal to duplicate
            Assert.AreNotEqual(original.Brep.Vertices[0].Location.X, dup.Topology[0].X);
            Assert.AreNotEqual(original.Brep.Vertices[0].Location.Y, dup.Topology[0].Y);
            Assert.AreNotEqual(original.Brep.Vertices[1].Location.X, dup.Topology[1].X);
            Assert.AreNotEqual(original.Brep.Vertices[1].Location.Y, dup.Topology[1].Y);
            Assert.AreNotEqual(original.Brep.Vertices[2].Location.X, dup.Topology[2].X);
            Assert.AreNotEqual(original.Brep.Vertices[2].Location.Y, dup.Topology[2].Y);
            Assert.AreNotEqual(original.Brep.Vertices[3].Location.X, dup.Topology[3].X);
            Assert.AreNotEqual(original.Brep.Vertices[3].Location.Y, dup.Topology[3].Y);
            Assert.AreNotEqual(original.Brep.Vertices[0].Location.X, dup.Topology[4].X);
            Assert.AreNotEqual(original.Brep.Vertices[0].Location.Y, dup.Topology[4].Y);

            // check that duplicate keeps it's member values
            Assert.AreEqual(System.Drawing.Color.FromArgb(255, 0, 0, 255), dup.Colour);
            Assert.AreEqual(13, dup.ID);
            Assert.AreEqual(1.56, dup.MeshSize);
            Assert.AreEqual("ehbaba", dup.Name);
            Assert.IsFalse(dup.IsDummy);
            Assert.AreEqual(0.33, dup.Offset.X1);
            Assert.AreEqual(3, dup.Property.ID);
            Assert.AreEqual(AnalysisOrder.RIGID_DIAPHRAGM, dup.Type2D);
            Assert.AreEqual(MemberType.GENERIC_2D, dup.Type);

            // check that changes are made to original
            Assert.AreEqual(System.Drawing.Color.FromArgb(255, 0, 0, 0), original.Colour);
            Assert.AreEqual(7, original.ID);
            Assert.AreEqual(0, original.MeshSize);
            Assert.AreEqual("Persepolis", original.Name);
            Assert.IsTrue(original.IsDummy);
            Assert.AreEqual(0.12, original.Offset.X1);
            Assert.AreEqual(44, original.Property.ID);
            Assert.AreEqual(AnalysisOrder.QUADRATIC, original.Type2D);
            Assert.AreEqual(MemberType.WALL, original.Type);
        }
    }
}