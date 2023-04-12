using System.Collections.Generic;
using System.Drawing;
using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaMember1dTest {

    [Fact]
    public void CloneApiObjectTest() {
      var member1d = new GsaMember1d {
        Name = "Name",
      };
      Member original = member1d.ApiMember;
      Member duplicate = member1d.GetAPI_MemberClone();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void DuplicateTest() {
      var original = new GsaMember1d {
        Name = "Name",
      };

      GsaMember1d duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void TestCreateGsaMem1dFromCrv() {
      var pts = new List<Point3d> {
        new Point3d(-3, -4, 0),
        new Point3d(5, -2, 0),
        new Point3d(2, 2, 0),
        new Point3d(6, 7, 0),
      };

      var crv = new PolylineCurve(pts);

      var mem = new GsaMember1d(crv) {
        Colour = Color.Red,
        Id = 3,
        Name = "gemma",
        IsDummy = true,
        Offset = new GsaOffset(0, 0, 0, -0.45),
        Section = {
          Id = 2,
        },
        Type1D = ElementType.BEAM,
        Type = MemberType.BEAM,
      };

      Assert.Equal(mem.PolyCurve.PointAtStart.X,
        mem.Topology[0]
          .X);
      Assert.Equal(mem.PolyCurve.PointAtStart.Y,
        mem.Topology[0]
          .Y);
      Assert.Equal(mem.PolyCurve.PointAtEnd.X,
        mem.Topology[mem.Topology.Count - 1]
          .X);
      Assert.Equal(mem.PolyCurve.PointAtEnd.Y,
        mem.Topology[mem.Topology.Count - 1]
          .Y);

      for (int i = 0; i < mem.PolyCurve.SegmentCount; i++) {
        Assert.True(mem.PolyCurve.SegmentCurve(i)
            .IsLinear()
          || mem.PolyCurve.SegmentCurve(i)
            .IsArc());
      }

      Assert.Equal(Color.FromArgb(255, 255, 0, 0), mem.Colour);
      Assert.Equal(3, mem.Id);
      Assert.Equal("gemma", mem.Name);
      Assert.True(mem.IsDummy);
      Assert.Equal(-0.45, mem.Offset.Z.Value);
      Assert.Equal(2, mem.Section.Id);
      Assert.Equal(ElementType.BEAM, mem.Type1D);
      Assert.Equal(MemberType.BEAM, mem.Type);
    }

    [Fact]
    public void TestDuplicateMem1d() {
      var pts = new List<Point3d> {
        new Point3d(0, 0, 0),
        new Point3d(0, 10, 0),
      };
      var crv = NurbsCurve.Create(false, 1, pts);

      var orig = new GsaMember1d(crv) {
        Colour = Color.Green,
        Id = 2,
        Name = "Sally",
        IsDummy = false,
        Offset = new GsaOffset(0, 0.1, 0, 0),
        Section = new GsaSection {
          Id = 4,
        },
        Group = 99,
        Type1D = ElementType.BAR,
        Type = MemberType.COLUMN,
      };

      GsaMember1d dup = orig.Duplicate();

      Assert.Equal(Color.FromArgb(255, 0, 128, 0), dup.Colour);
      Assert.Equal(2, dup.Id);
      Assert.Equal("Sally", dup.Name);
      Assert.False(dup.IsDummy);
      Assert.Equal(0.1, dup.Offset.X2.Value);
      Assert.Equal(4, dup.Section.Id);
      Assert.Equal(99, dup.Group);
      Assert.Equal(ElementType.BAR, dup.Type1D);
      Assert.Equal(MemberType.COLUMN, dup.Type);

      orig.Colour = Color.White;
      orig.Id = 1;
      orig.Name = "Peter Peterson";
      orig.IsDummy = true;
      orig.Offset = new GsaOffset(0, 0.4, 0, 0);
      orig.Section.Id = 1;
      orig.Group = 4;
      orig.Type1D = ElementType.BEAM;
      orig.Type = MemberType.BEAM;

      Assert.Equal(Color.FromArgb(255, 0, 128, 0), dup.Colour);
      Assert.Equal(2, dup.Id);
      Assert.Equal("Sally", dup.Name);
      Assert.False(dup.IsDummy);
      Assert.Equal(0.1, dup.Offset.X2.Value);
      Assert.Equal(4, dup.Section.Id);
      Assert.Equal(99, dup.Group);
      Assert.Equal(ElementType.BAR, dup.Type1D);
      Assert.Equal(MemberType.COLUMN, dup.Type);

      Assert.Equal(Color.FromArgb(255, 255, 255, 255), orig.Colour);
      Assert.Equal(1, orig.Id);
      Assert.Equal("Peter Peterson", orig.Name);
      Assert.True(orig.IsDummy);
      Assert.Equal(0.4, orig.Offset.X2.Value);
      Assert.Equal(1, orig.Section.Id);
      Assert.Equal(4, orig.Group);
      Assert.Equal(ElementType.BEAM, orig.Type1D);
      Assert.Equal(MemberType.BEAM, orig.Type);
    }
  }
}
