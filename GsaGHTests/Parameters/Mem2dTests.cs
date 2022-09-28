using System.Collections.Generic;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;
using Xunit;

namespace ParamsIntegrationTests
{
  public class Mem2dTests
  {
    [Fact]
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
      mem.MeshSize = new Length(0.56, LengthUnit.Meter);
      mem.Name = "meminem";
      mem.IsDummy = true;
      mem.Offset = new GsaOffset(0, 0, 0, -0.45);
      mem.Property.ID = 2;
      mem.Type2D = AnalysisOrder.LINEAR;
      mem.Type = MemberType.SLAB;

      Assert.Equal(mem.Brep.Vertices[0].Location.X, mem.Topology[0].X);
      Assert.Equal(mem.Brep.Vertices[0].Location.Y, mem.Topology[0].Y);
      Assert.Equal(mem.Brep.Vertices[1].Location.X, mem.Topology[1].X);
      Assert.Equal(mem.Brep.Vertices[1].Location.Y, mem.Topology[1].Y);
      Assert.Equal(mem.Brep.Vertices[2].Location.X, mem.Topology[2].X);
      Assert.Equal(mem.Brep.Vertices[2].Location.Y, mem.Topology[2].Y);
      Assert.Equal(mem.Brep.Vertices[3].Location.X, mem.Topology[3].X);
      Assert.Equal(mem.Brep.Vertices[3].Location.Y, mem.Topology[3].Y);
      Assert.Equal(mem.Brep.Vertices[0].Location.X, mem.Topology[4].X);
      Assert.Equal(mem.Brep.Vertices[0].Location.Y, mem.Topology[4].Y);

      Assert.Equal(System.Drawing.Color.FromArgb(255, 255, 255, 255), mem.Colour);
      Assert.Equal(4, mem.ID);
      Assert.Equal(0.56, mem.MeshSize.Value);
      Assert.Equal("meminem", mem.Name);
      Assert.True(mem.IsDummy);
      Assert.Equal(-0.45, mem.Offset.Z.Value);
      Assert.Equal(2, mem.Property.ID);
      Assert.Equal(AnalysisOrder.LINEAR, mem.Type2D);
      Assert.Equal(MemberType.SLAB, mem.Type);
    }

    [Fact]
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
      original.MeshSize = new Length(1.56, LengthUnit.Meter);
      original.Name = "ehbaba";
      original.IsDummy = false;
      original.Offset = new GsaOffset(0.33, 0, 0, 0, LengthUnit.Meter);
      original.Property.ID = 3;
      original.Type2D = AnalysisOrder.RIGID_DIAPHRAGM;
      original.Type = MemberType.GENERIC_2D;

      // create duplicate
      GsaMember2d dup = original.Duplicate();

      Assert.Equal(original.Brep.Vertices[0].Location.X, dup.Topology[0].X);
      Assert.Equal(original.Brep.Vertices[0].Location.Y, dup.Topology[0].Y);
      Assert.Equal(original.Brep.Vertices[1].Location.X, dup.Topology[1].X);
      Assert.Equal(original.Brep.Vertices[1].Location.Y, dup.Topology[1].Y);
      Assert.Equal(original.Brep.Vertices[2].Location.X, dup.Topology[2].X);
      Assert.Equal(original.Brep.Vertices[2].Location.Y, dup.Topology[2].Y);
      Assert.Equal(original.Brep.Vertices[3].Location.X, dup.Topology[3].X);
      Assert.Equal(original.Brep.Vertices[3].Location.Y, dup.Topology[3].Y);
      Assert.Equal(original.Brep.Vertices[0].Location.X, dup.Topology[4].X);
      Assert.Equal(original.Brep.Vertices[0].Location.Y, dup.Topology[4].Y);
      Assert.Equal(1, dup.IncLinesTopology[0][0].X);
      Assert.Equal(2, dup.IncLinesTopology[0][0].Y);
      Assert.Equal(3, dup.IncLinesTopology[0][1].X);
      Assert.Equal(2, dup.IncLinesTopology[0][1].Y);
      Assert.Equal(1, dup.InclusionPoints[0].X);
      Assert.Equal(1, dup.InclusionPoints[0].Y);

      Assert.Equal(System.Drawing.Color.FromArgb(255, 0, 0, 255), dup.Colour);
      Assert.Equal(13, dup.ID);
      Assert.Equal(1.56, dup.MeshSize.Value);
      Assert.Equal("ehbaba", dup.Name);
      Assert.False(dup.IsDummy);
      Assert.Equal(0.33, dup.Offset.X1.Value);
      Assert.Equal(3, dup.Property.ID);
      Assert.Equal(AnalysisOrder.RIGID_DIAPHRAGM, dup.Type2D);
      Assert.Equal(MemberType.GENERIC_2D, dup.Type);

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
      original.MeshSize = Length.Zero;
      original.Name = "Persepolis";
      original.IsDummy = true;
      original.Offset = new GsaOffset(0.12, 0, 0, 0, LengthUnit.Meter);
      original.Property.ID = 44;
      original.Type2D = AnalysisOrder.QUADRATIC;
      original.Type = MemberType.WALL;

      // check that orignal are not equal to duplicate
      Assert.NotEqual(original.Brep.Vertices[0].Location.X, dup.Topology[0].X);
      Assert.NotEqual(original.Brep.Vertices[0].Location.Y, dup.Topology[0].Y);
      Assert.NotEqual(original.Brep.Vertices[1].Location.X, dup.Topology[1].X);
      Assert.NotEqual(original.Brep.Vertices[1].Location.Y, dup.Topology[1].Y);
      Assert.NotEqual(original.Brep.Vertices[2].Location.X, dup.Topology[2].X);
      Assert.NotEqual(original.Brep.Vertices[2].Location.Y, dup.Topology[2].Y);
      Assert.NotEqual(original.Brep.Vertices[3].Location.X, dup.Topology[3].X);
      Assert.NotEqual(original.Brep.Vertices[3].Location.Y, dup.Topology[3].Y);
      Assert.NotEqual(original.Brep.Vertices[0].Location.X, dup.Topology[4].X);
      Assert.NotEqual(original.Brep.Vertices[0].Location.Y, dup.Topology[4].Y);

      // check that duplicate keeps it's member values
      Assert.Equal(System.Drawing.Color.FromArgb(255, 0, 0, 255), dup.Colour);
      Assert.Equal(13, dup.ID);
      Assert.Equal(1.56, dup.MeshSize.Meters);
      Assert.Equal("ehbaba", dup.Name);
      Assert.False(dup.IsDummy);
      Assert.Equal(0.33, dup.Offset.X1.Meters);
      Assert.Equal(3, dup.Property.ID);
      Assert.Equal(AnalysisOrder.RIGID_DIAPHRAGM, dup.Type2D);
      Assert.Equal(MemberType.GENERIC_2D, dup.Type);

      // check that changes are made to original
      Assert.Equal(System.Drawing.Color.FromArgb(255, 0, 0, 0), original.Colour);
      Assert.Equal(7, original.ID);
      Assert.Equal(0, original.MeshSize.Value);
      Assert.Equal("Persepolis", original.Name);
      Assert.True(original.IsDummy);
      Assert.Equal(0.12, original.Offset.X1.Value);
      Assert.Equal(44, original.Property.ID);
      Assert.Equal(AnalysisOrder.QUADRATIC, original.Type2D);
      Assert.Equal(MemberType.WALL, original.Type);
    }
  }
}