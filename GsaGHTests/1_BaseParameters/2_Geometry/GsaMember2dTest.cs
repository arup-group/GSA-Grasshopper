using System.Collections.Generic;
using System.Drawing;
using GsaAPI;
using GsaGH.Parameters;
using Rhino.Geometry;
using Xunit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaMember2dTest {

    [Fact]
    public void TestCreateGsaMem2dFromBrep() {
      var pts = new List<Point3d> {
        new Point3d(-3, -4, 0),
        new Point3d(5, -2, 0),
        new Point3d(6, 7, 0),
        new Point3d(-1, 2, 0),
      };
      pts.Add(pts[0]);
      var pol = new Polyline(pts);
      Brep brep = Brep.CreatePlanarBreps(pol.ToNurbsCurve(), 0.001)[0];

      var inclpts = new List<Point3d>();
      var inclcrvs = new List<Curve>();

      var mem = new GsaMember2d(brep, inclcrvs, inclpts) {
        Colour = Color.White,
        Id = 4,
        MeshSize = 0.56,
        Name = "meminem",
        IsDummy = true,
        Offset = new GsaOffset(0, 0, 0, -0.45),
        Prop2d = {
          Id = 2,
        },
        Type2D = AnalysisOrder.LINEAR,
        Type = MemberType.SLAB,
      };

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

      Assert.Equal(Color.FromArgb(255, 255, 255, 255), mem.Colour);
      Assert.Equal(4, mem.Id);
      Assert.Equal(0.56, mem.MeshSize);
      Assert.Equal("meminem", mem.Name);
      Assert.True(mem.IsDummy);
      Assert.Equal(-0.45, mem.Offset.Z.Value);
      Assert.Equal(2, mem.Prop2d.Id);
      Assert.Equal(AnalysisOrder.LINEAR, mem.Type2D);
      Assert.Equal(MemberType.SLAB, mem.Type);
    }

    [Fact]
    public void TestDuplicateMem2d() {
      var pts = new List<Point3d> {
        new Point3d(1, 1, 0),
        new Point3d(0, 5, 0),
        new Point3d(6, 7, 0),
        new Point3d(4, 2, 0),
      };
      pts.Add(pts[0]);
      var pol = new Polyline(pts);
      Brep brep = Brep.CreatePlanarBreps(pol.ToNurbsCurve(), 0.001)[0];

      var inclpts = new List<Point3d> {
        new Point3d(1, 1, 0),
      };
      var inclcrvs = new List<Curve>();
      var line = new LineCurve(new Point3d(1, 2, 0), new Point3d(3, 2, 0));
      inclcrvs.Add(line);

      var original = new GsaMember2d(brep, inclcrvs, inclpts) {
        Colour = Color.Blue,
        Id = 13,
        MeshSize = 1.56,
        Name = "ehbaba",
        IsDummy = false,
        Offset = new GsaOffset(0.33, 0, 0, 0, LengthUnit.Meter),
        Prop2d = {
          Id = 3,
        },
        Type2D = AnalysisOrder.RIGID_DIAPHRAGM,
        Type = MemberType.GENERIC_2D,
      };

      GsaMember2d dup = original.Duplicate(true);

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

      Assert.Equal(Color.FromArgb(255, 0, 0, 255), dup.Colour);
      Assert.Equal(13, dup.Id);
      Assert.Equal(1.56, dup.MeshSize);
      Assert.Equal("ehbaba", dup.Name);
      Assert.False(dup.IsDummy);
      Assert.Equal(0.33, dup.Offset.X1.Value);
      Assert.Equal(3, dup.Prop2d.Id);
      Assert.Equal(AnalysisOrder.RIGID_DIAPHRAGM, dup.Type2D);
      Assert.Equal(MemberType.GENERIC_2D, dup.Type);

      var pts2 = new List<Point3d> {
        new Point3d(0, 0, 0),
        new Point3d(5, 0, 0),
        new Point3d(7, 6, 0),
        new Point3d(2, 4, 0),
      };
      pts2.Add(pts2[0]);
      var pol2 = new Polyline(pts2);
      Brep brep2 = Brep.CreatePlanarBreps(pol2.ToNurbsCurve(), 0.001)[0];

      original = original.UpdateGeometry(brep2);
      original.Colour = Color.Black;
      original.Id = 7;
      original.MeshSize = 0;
      original.Name = "Persepolis";
      original.IsDummy = true;
      original.Offset = new GsaOffset(0.12, 0, 0, 0, LengthUnit.Meter);
      original.Prop2d.Id = 44;
      original.Type2D = AnalysisOrder.QUADRATIC;
      original.Type = MemberType.WALL;

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

      Assert.Equal(Color.FromArgb(255, 0, 0, 255), dup.Colour);
      Assert.Equal(13, dup.Id);
      Assert.Equal(1.56, dup.MeshSize);
      Assert.Equal("ehbaba", dup.Name);
      Assert.False(dup.IsDummy);
      Assert.Equal(0.33, dup.Offset.X1.Meters);
      Assert.Equal(44, dup.Prop2d.Id);
      Assert.Equal(AnalysisOrder.RIGID_DIAPHRAGM, dup.Type2D);
      Assert.Equal(MemberType.GENERIC_2D, dup.Type);

      Assert.Equal(Color.FromArgb(255, 0, 0, 0), original.Colour);
      Assert.Equal(7, original.Id);
      Assert.Equal(0, original.MeshSize);
      Assert.Equal("Persepolis", original.Name);
      Assert.True(original.IsDummy);
      Assert.Equal(0.12, original.Offset.X1.Value);
      Assert.Equal(44, original.Prop2d.Id);
      Assert.Equal(AnalysisOrder.QUADRATIC, original.Type2D);
      Assert.Equal(MemberType.WALL, original.Type);
    }
  }
}
