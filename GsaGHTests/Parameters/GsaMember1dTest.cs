using System.Collections.Generic;
using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GsaMember1dTest
  {
    [Fact]
    public void DuplicateTest()
    {
      // Arrange
      GsaSection section = new GsaSection();
      section.Name = "Name";
      GsaMember1d original = new GsaMember1d(new Member(), 1, new List<Point3d>(), new List<string>(), section, new GsaNode());
      original.Name = "Name";

      // Act
      GsaMember1d duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
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
      Assert.Equal(mem.PolyCurve.PointAtStart.X, mem.Topology[0].X);
      Assert.Equal(mem.PolyCurve.PointAtStart.Y, mem.Topology[0].Y);
      Assert.Equal(mem.PolyCurve.PointAtEnd.X, mem.Topology[mem.Topology.Count - 1].X);
      Assert.Equal(mem.PolyCurve.PointAtEnd.Y, mem.Topology[mem.Topology.Count - 1].Y);

      // loop through segments and check they are either arc or line
      for (int i = 0; i < mem.PolyCurve.SegmentCount; i++)
        Assert.True(mem.PolyCurve.SegmentCurve(i).IsLinear() || mem.PolyCurve.SegmentCurve(i).IsArc());

      Assert.Equal(System.Drawing.Color.FromArgb(255, 255, 0, 0), mem.Colour);
      Assert.Equal(3, mem.ID);
      Assert.Equal("gemma", mem.Name);
      Assert.True(mem.IsDummy);
      Assert.Equal(-0.45, mem.Offset.Z.Value);
      Assert.Equal(2, mem.Section.ID);
      Assert.Equal(ElementType.BEAM, mem.Type1D);
      Assert.Equal(MemberType.BEAM, mem.Type);
    }

    [Fact]
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
      Assert.Equal(System.Drawing.Color.FromArgb(255, 0, 128, 0), dup.Colour);
      Assert.Equal(2, dup.ID);
      Assert.Equal("Sally", dup.Name);
      Assert.False(dup.IsDummy);
      Assert.Equal(0.1, dup.Offset.X2.Value);
      Assert.Equal(4, dup.Section.ID);
      Assert.Equal(99, dup.Group);
      Assert.Equal(ElementType.BAR, dup.Type1D);
      Assert.Equal(MemberType.COLUMN, dup.Type);

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
      Assert.Equal(System.Drawing.Color.FromArgb(255, 0, 128, 0), dup.Colour);
      Assert.Equal(2, dup.ID);
      Assert.Equal("Sally", dup.Name);
      Assert.False(dup.IsDummy);
      Assert.Equal(0.1, dup.Offset.X2.Value);
      Assert.Equal(4, dup.Section.ID);
      Assert.Equal(99, dup.Group);
      Assert.Equal(ElementType.BAR, dup.Type1D);
      Assert.Equal(MemberType.COLUMN, dup.Type);

      // check that original is changed
      Assert.Equal(System.Drawing.Color.FromArgb(255, 255, 255, 255), orig.Colour);
      Assert.Equal(1, orig.ID);
      Assert.Equal("Peter Peterson", orig.Name);
      Assert.True(orig.IsDummy);
      Assert.Equal(0.4, orig.Offset.X2.Value);
      Assert.Equal(1, orig.Section.ID);
      Assert.Equal(4, orig.Group);
      Assert.Equal(ElementType.BEAM, orig.Type1D);
      Assert.Equal(MemberType.BEAM, orig.Type);
    }
  }
}
