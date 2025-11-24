using System.Collections.Generic;
using System.Drawing;

using GsaAPI;

using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Rhino.Collections;
using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaMember1dTest {

    [Fact]
    public void CloneApiObjectTest() {
      var member1d = new GsaMember1D();
      member1d.ApiMember.Name = "Name";
      Member original = member1d.ApiMember;
      Member duplicate = member1d.DuplicateApiObject();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void CloneTest() {
      var original = new GsaMember1D();
      original.ApiMember.Name = "Name";

      var duplicate = new GsaMember1D(original);

      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid" });
    }

    [Fact]
    public void DuplicateTest() {
      var original = new GsaMember1D();
      original.ApiMember.Name = "Name";

      GsaMember1D duplicate = original;

      Assert.Equal(original, duplicate);
    }

    [Fact]
    public void TestCreateGsaMem1dFromCrv() {
      var pts = new Point3dList {
        new Point3d(-3, -4, 0),
        new Point3d(5, -2, 0),
        new Point3d(2, 2, 0),
        new Point3d(6, 7, 0),
      };

      var crv = new PolylineCurve(pts);

      var mem = new GsaMember1D(crv) {
        ApiMember = new Member() {
          Colour = Color.Red,
          Name = "gemma",
          IsDummy = true,
          Type1D = ElementType.BEAM,
          Type = MemberType.BEAM,
        },
        Id = 3,
        Offset = new GsaOffset(0, 0, 0, -0.45),
        Section = new GsaSection(2),
      };


      Assert.Equal(mem.PolyCurve.PointAtStart.X, mem.Topology[0].X);
      Assert.Equal(mem.PolyCurve.PointAtStart.Y, mem.Topology[0].Y);
      Assert.Equal(mem.PolyCurve.PointAtEnd.X, mem.Topology[mem.Topology.Count - 1].X);
      Assert.Equal(mem.PolyCurve.PointAtEnd.Y, mem.Topology[mem.Topology.Count - 1].Y);

      for (int i = 0; i < mem.PolyCurve.SegmentCount; i++) {
        Assert.True(mem.PolyCurve.SegmentCurve(i).IsLinear()
          || mem.PolyCurve.SegmentCurve(i).IsArc());
      }

      Assert.Equal(Color.FromArgb(255, 255, 0, 0), (Color)mem.ApiMember.Colour);
      Assert.Equal(3, mem.Id);
      Assert.Equal("gemma", mem.ApiMember.Name);
      Assert.True(mem.ApiMember.IsDummy);
      Assert.Equal(-0.45, mem.Offset.Z.Value);
      Assert.Equal(2, mem.Section.Id);
      Assert.Equal(ElementType.BEAM, mem.ApiMember.Type1D);
      Assert.Equal(MemberType.BEAM, mem.ApiMember.Type);
    }

    [Fact]
    public void TestDuplicateMem1d() {
      var pts = new Point3dList {
        new Point3d(0, 0, 0),
        new Point3d(0, 10, 0),
      };
      var crv = NurbsCurve.Create(false, 1, pts);

      var orig = new GsaMember1D(crv) {
        ApiMember = new Member() {
          Colour = Color.Green,
          Name = "Sally",
          IsDummy = false,
          Group = 99,
          Type1D = ElementType.BAR,
          Type = MemberType.COLUMN,
        },
        Id = 2,
        Offset = new GsaOffset(0, 0.1, 0, 0),
        Section = new GsaSection(4),
      };

      var dup = new GsaMember1D(orig);

      Assert.Equal(Color.FromArgb(255, 0, 128, 0), (Color)dup.ApiMember.Colour);
      Assert.Equal(2, dup.Id);
      Assert.Equal("Sally", dup.ApiMember.Name);
      Assert.False(dup.ApiMember.IsDummy);
      Assert.Equal(0.1, dup.Offset.X2.Value);
      Assert.Equal(4, dup.Section.Id);
      Assert.Equal(99, dup.ApiMember.Group);
      Assert.Equal(ElementType.BAR, dup.ApiMember.Type1D);
      Assert.Equal(MemberType.COLUMN, dup.ApiMember.Type);

      orig.ApiMember.Colour = Color.White;
      orig.Id = 1;
      orig.ApiMember.Name = "Peter Peterson";
      orig.ApiMember.IsDummy = true;
      orig.Offset = new GsaOffset(0, 0.4, 0, 0);
      orig.Section.Id = 1;
      orig.ApiMember.Group = 4;
      orig.ApiMember.Type1D = ElementType.BEAM;
      orig.ApiMember.Type = MemberType.BEAM;

      Assert.Equal(Color.FromArgb(255, 0, 128, 0), (Color)dup.ApiMember.Colour);
      Assert.Equal(2, dup.Id);
      Assert.Equal("Sally", dup.ApiMember.Name);
      Assert.False(dup.ApiMember.IsDummy);
      Assert.Equal(0.1, dup.Offset.X2.Value);
      Assert.Equal(1, dup.Section.Id);
      Assert.Equal(99, dup.ApiMember.Group);
      Assert.Equal(ElementType.BAR, dup.ApiMember.Type1D);
      Assert.Equal(MemberType.COLUMN, dup.ApiMember.Type);

      Assert.Equal(Color.FromArgb(255, 255, 255, 255), (Color)orig.ApiMember.Colour);
      Assert.Equal(1, orig.Id);
      Assert.Equal("Peter Peterson", orig.ApiMember.Name);
      Assert.True(orig.ApiMember.IsDummy);
      Assert.Equal(0.4, orig.Offset.X2.Value);
      Assert.Equal(1, orig.Section.Id);
      Assert.Equal(4, orig.ApiMember.Group);
      Assert.Equal(ElementType.BEAM, orig.ApiMember.Type1D);
      Assert.Equal(MemberType.BEAM, orig.ApiMember.Type);
    }
  }
}
