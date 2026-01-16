using System;
using System.Collections.Generic;
using System.Drawing;

using GsaAPI;

using GsaGH.Parameters;

using Rhino.Collections;
using Rhino.Geometry;

using Xunit;

using LengthUnit = OasysUnits.Units.LengthUnit;
using Polyline = Rhino.Geometry.Polyline;
using GsaGH.Helpers;
namespace GsaGHTests.Parameters {

  [Collection("GrasshopperFixture collection")]
  public class GsaMember2dTest {

    [Fact]
    public void TestCreateGsaMem2dFromBrep() {
      var pts = new Point3dList {
        new Point3d(-3, -4, 0),
        new Point3d(5, -2, 0),
        new Point3d(6, 7, 0),
        new Point3d(-1, 2, 0),
      };
      pts.Add(pts[0]);
      var pol = new Polyline(pts);
      Brep brep = Brep.CreatePlanarBreps(pol.ToNurbsCurve(), 0.001)[0];

      var inclpts = new Point3dList();
      var inclcrvs = new List<Curve>();

      var mem = new GsaMember2d(brep, inclcrvs, inclpts) {
        Id = 4,
        Offset = new GsaOffset(0, 0, 0, -0.45),
        Prop2d = new GsaProperty2d(2)
      };
      mem.ApiMember.Colour = Color.White;
      mem.ApiMember.MeshSize = 0.56;
      mem.ApiMember.Name = "meminem";
      mem.ApiMember.IsDummy = true;
      mem.ApiMember.Type2D
        = AnalysisOrder.LINEAR; // if prop2d == Load Panel -> AnalysisOrder.LoadPanel
      mem.ApiMember.Type = MemberType.SLAB;

      Assert.Equal(mem.Brep.Vertices[0].Location.X, mem.Topology[0].X, DoubleComparer.Default);
      Assert.Equal(mem.Brep.Vertices[0].Location.Y, mem.Topology[0].Y, DoubleComparer.Default);
      Assert.Equal(mem.Brep.Vertices[1].Location.X, mem.Topology[1].X, DoubleComparer.Default);
      Assert.Equal(mem.Brep.Vertices[1].Location.Y, mem.Topology[1].Y, DoubleComparer.Default);
      Assert.Equal(mem.Brep.Vertices[2].Location.X, mem.Topology[2].X, DoubleComparer.Default);
      Assert.Equal(mem.Brep.Vertices[2].Location.Y, mem.Topology[2].Y, DoubleComparer.Default);
      Assert.Equal(mem.Brep.Vertices[3].Location.X, mem.Topology[3].X, DoubleComparer.Default);
      Assert.Equal(mem.Brep.Vertices[3].Location.Y, mem.Topology[3].Y, DoubleComparer.Default);
      Assert.Equal(mem.Brep.Vertices[0].Location.X, mem.Topology[4].X, DoubleComparer.Default);
      Assert.Equal(mem.Brep.Vertices[0].Location.Y, mem.Topology[4].Y, DoubleComparer.Default);

      Assert.Equal(Color.FromArgb(255, 255, 255, 255), mem.ApiMember.Colour);
      Assert.Equal(4, mem.Id);
      Assert.Equal(0.56, mem.ApiMember.MeshSize);
      Assert.Equal("meminem", mem.ApiMember.Name);
      Assert.True(mem.ApiMember.IsDummy);
      Assert.Equal(-0.45, mem.Offset.Z.Value);
      Assert.Equal(2, mem.Prop2d.Id);
      Assert.Equal(AnalysisOrder.LINEAR, mem.ApiMember.Type2D);
      Assert.Equal(MemberType.SLAB, mem.ApiMember.Type);
    }

    [Fact]
    public void TestDuplicateMem2d() {
      var pts = new Point3dList {
        new Point3d(1, 1, 0),
        new Point3d(0, 5, 0),
        new Point3d(6, 7, 0),
        new Point3d(4, 2, 0),
      };
      pts.Add(pts[0]);
      var pol = new Polyline(pts);
      Brep brep = Brep.CreatePlanarBreps(pol.ToNurbsCurve(), 0.001)[0];

      var inclpts = new Point3dList {
        new Point3d(1, 1, 0),
      };
      var inclcrvs = new List<Curve>();
      var line = new LineCurve(new Point3d(1, 2, 0), new Point3d(3, 2, 0));
      inclcrvs.Add(line);

      var original = new GsaMember2d(brep, inclcrvs, inclpts) {
        Id = 13,
        Offset = new GsaOffset(0.33, 0, 0, 0, LengthUnit.Meter),
        Prop2d = new GsaProperty2d(3)
      };
      original.ApiMember.Colour = Color.Blue;
      original.ApiMember.MeshSize = 1.56;
      original.ApiMember.Name = "ehbaba";
      original.ApiMember.IsDummy = false;
      original.ApiMember.Type2D = AnalysisOrder.RIGID_DIAPHRAGM;
      original.ApiMember.Type = MemberType.GENERIC_2D;

      var dup = new GsaMember2d(original);

      Assert.Equal(original.Brep.Vertices[0].Location.X, dup.Topology[0].X, DoubleComparer.Default);
      Assert.Equal(original.Brep.Vertices[0].Location.Y, dup.Topology[0].Y, DoubleComparer.Default);
      Assert.Equal(original.Brep.Vertices[1].Location.X, dup.Topology[1].X, DoubleComparer.Default);
      Assert.Equal(original.Brep.Vertices[1].Location.Y, dup.Topology[1].Y, DoubleComparer.Default);
      Assert.Equal(original.Brep.Vertices[2].Location.X, dup.Topology[2].X, DoubleComparer.Default);
      Assert.Equal(original.Brep.Vertices[2].Location.Y, dup.Topology[2].Y, DoubleComparer.Default);
      Assert.Equal(original.Brep.Vertices[3].Location.X, dup.Topology[3].X, DoubleComparer.Default);
      Assert.Equal(original.Brep.Vertices[3].Location.Y, dup.Topology[3].Y, DoubleComparer.Default);
      Assert.Equal(original.Brep.Vertices[0].Location.X, dup.Topology[4].X, DoubleComparer.Default);
      Assert.Equal(original.Brep.Vertices[0].Location.Y, dup.Topology[4].Y, DoubleComparer.Default);
      Assert.Equal(1, dup.InclusionLinesTopology[0][0].X, DoubleComparer.Default);
      Assert.Equal(2, dup.InclusionLinesTopology[0][0].Y, DoubleComparer.Default);
      Assert.Equal(3, dup.InclusionLinesTopology[0][1].X, DoubleComparer.Default);
      Assert.Equal(2, dup.InclusionLinesTopology[0][1].Y, DoubleComparer.Default);
      Assert.Equal(1, dup.InclusionPoints[0].X, DoubleComparer.Default);
      Assert.Equal(1, dup.InclusionPoints[0].Y, DoubleComparer.Default);

      Assert.Equal(Color.FromArgb(255, 0, 0, 255), dup.ApiMember.Colour);
      Assert.Equal(13, dup.Id);
      Assert.Equal(1.56, dup.ApiMember.MeshSize);
      Assert.Equal("ehbaba", dup.ApiMember.Name);
      Assert.False(dup.ApiMember.IsDummy);
      Assert.Equal(0.33, dup.Offset.X1.Value, DoubleComparer.Default);
      Assert.Equal(3, dup.Prop2d.Id);
      Assert.Equal(AnalysisOrder.RIGID_DIAPHRAGM, dup.ApiMember.Type2D);
      Assert.Equal(MemberType.GENERIC_2D, dup.ApiMember.Type);

      var pts2 = new Point3dList {
        new Point3d(0, 0, 0),
        new Point3d(5, 0, 0),
        new Point3d(7, 6, 0),
        new Point3d(2, 4, 0),
      };
      pts2.Add(pts2[0]);
      var pol2 = new Polyline(pts2);
      Brep brep2 = Brep.CreatePlanarBreps(pol2.ToNurbsCurve(), 0.001)[0];

      original.UpdateGeometry(brep2);
      original.ApiMember.Colour = Color.Black;
      original.Id = 7;
      original.ApiMember.MeshSize = 0;
      original.ApiMember.Name = "Persepolis";
      original.ApiMember.IsDummy = true;
      original.Offset = new GsaOffset(0.12, 0, 0, 0, LengthUnit.Meter);
      original.Prop2d.Id = 44;
      original.ApiMember.Type2D = AnalysisOrder.QUADRATIC;
      original.ApiMember.Type = MemberType.WALL;

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

      Assert.Equal(Color.FromArgb(255, 0, 0, 255), dup.ApiMember.Colour);
      Assert.Equal(13, dup.Id);
      Assert.Equal(1.56, dup.ApiMember.MeshSize);
      Assert.Equal("ehbaba", dup.ApiMember.Name);
      Assert.False(dup.ApiMember.IsDummy);
      Assert.Equal(0.33, dup.Offset.X1.Meters, DoubleComparer.Default);
      Assert.Equal(44, dup.Prop2d.Id);
      Assert.Equal(AnalysisOrder.RIGID_DIAPHRAGM, dup.ApiMember.Type2D);
      Assert.Equal(MemberType.GENERIC_2D, dup.ApiMember.Type);

      Assert.Equal(Color.FromArgb(255, 0, 0, 0), original.ApiMember.Colour);
      Assert.Equal(7, original.Id);
      Assert.Equal(0, original.ApiMember.MeshSize);
      Assert.Equal("Persepolis", original.ApiMember.Name);
      Assert.True(original.ApiMember.IsDummy);
      Assert.Equal(0.12, original.Offset.X1.Value, DoubleComparer.Default);
      Assert.Equal(44, original.Prop2d.Id);
      Assert.Equal(AnalysisOrder.QUADRATIC, original.ApiMember.Type2D);
      Assert.Equal(MemberType.WALL, original.ApiMember.Type);
    }

    [Fact]
    public void CheckBrepShouldBeCalledWhenInputBrepIsInvalid() {
      Assert.Throws<NullReferenceException>(() => new GsaMember2d(null));
    }
  }
}
