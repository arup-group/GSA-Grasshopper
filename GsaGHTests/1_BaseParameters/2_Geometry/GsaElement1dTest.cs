using System.Drawing;
using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;
using Xunit;
using Line = Rhino.Geometry.Line;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaElement1dTest {

    [Fact]
    public void CloneApiObjectTest() {
      var section = new GsaSection {
        Name = "Name",
      };
      var element1d = new GsaElement1d(new Element(), new LineCurve(), 1, section, new GsaNode()) {
        Name = "Name",
      };
      Element original = element1d.ApiElement;

      Element duplicate = element1d.GetApiElementClone();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void DuplicateTest() {
      var section = new GsaSection {
        Name = "Name",
      };
      var original = new GsaElement1d(new Element(), new LineCurve(), 1, section, new GsaNode()) {
        Name = "Name",
      };

      GsaElement1d duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void TestCreateGsaElem1dFromLn() {
      var ln = new Line(new Point3d(1, 4, 6), new Point3d(-2, 3, -5));

      var elem = new GsaElement1d(new LineCurve(ln)) {
        Id = 66,
        Section = new GsaSection {
          Id = 2,
        },
        Colour = Color.Yellow,
        Group = 4,
        IsDummy = true,
        Name = "EltonJohn",
      };
      var offset = new GsaOffset(0, 0, 14.3, 0);
      elem.Offset = offset;
      elem.OrientationAngle = new Angle(90, AngleUnit.Degree);
      elem.Section.Id = 3;

      Assert.Equal(1, elem.Line.PointAtStart.X);
      Assert.Equal(4, elem.Line.PointAtStart.Y);
      Assert.Equal(6, elem.Line.PointAtStart.Z);
      Assert.Equal(-2, elem.Line.PointAtEnd.X);
      Assert.Equal(3, elem.Line.PointAtEnd.Y);
      Assert.Equal(-5, elem.Line.PointAtEnd.Z);

      Assert.Equal(66, elem.Id);
      Assert.Equal(3, elem.Section.Id);
      Assert.Equal(Color.FromArgb(255, 255, 255, 0), elem.Colour);
      Assert.Equal(4, elem.Group);
      Assert.True(elem.IsDummy);
      Assert.Equal("EltonJohn", elem.Name);
      Assert.Equal(14.3, elem.Offset.Y.Value);
      Assert.Equal(90, elem.OrientationAngle.Degrees);
      Assert.Equal(3, elem.Section.Id);
    }

    [Fact]
    public void TestCreateGsaElem1dGetReleases() {
      var ln = new Line(new Point3d(1, 4, 6), new Point3d(-2, 3, -5));

      var elem = new GsaElement1d(new LineCurve(ln));

      GsaBool6 rel1 = elem.ReleaseStart;
      Assert.False(rel1.X);
      Assert.False(rel1.Y);
      Assert.False(rel1.Z);
      Assert.False(rel1.Xx);
      Assert.False(rel1.Yy);
      Assert.False(rel1.Zz);
    }

    [Fact]
    public void TestDuplicateElem1d() {
      var ln = new Line(new Point3d(2, -1, 0), new Point3d(2, -1, 4));

      var orig = new GsaElement1d(new LineCurve(ln)) {
        Id = 3,
        Section = new GsaSection {
          Id = 7,
        },
        Colour = Color.Aqua,
        Group = 1,
        IsDummy = false,
        Name = "Tilman",
      };
      var offset = new GsaOffset(0, 0, 2.9, 0);
      orig.Offset = offset;
      orig.OrientationAngle = new Angle(-0.14, AngleUnit.Radian);

      GsaElement1d dup = orig.Duplicate();

      orig.Line = new LineCurve(new Line(new Point3d(1, 1, -4), new Point3d(1, 1, 0)));
      orig.Id = 5;
      orig.Section.Id = 9;
      orig.Colour = Color.Red;
      orig.Group = 2;
      orig.IsDummy = true;
      orig.Name = "Hugh";
      var offset2 = new GsaOffset(0, 0, -0.991, 0, LengthUnit.Meter);
      orig.Offset = offset2;
      orig.OrientationAngle = new Angle(0, AngleUnit.Radian);

      // check that values in duplicate are not changed
      Assert.Equal(2, dup.Line.PointAtStart.X, 1E-9);
      Assert.Equal(-1, dup.Line.PointAtStart.Y, 1E-9);
      Assert.Equal(0, dup.Line.PointAtStart.Z, 1E-9);
      Assert.Equal(2, dup.Line.PointAtEnd.X, 1E-9);
      Assert.Equal(-1, dup.Line.PointAtEnd.Y, 1E-9);
      Assert.Equal(4, dup.Line.PointAtEnd.Z, 1E-9);
      Assert.Equal(3, dup.Id);
      Assert.Equal(7, dup.Section.Id);
      Assert.Equal(Color.FromArgb(255, 0, 255, 255), dup.Colour);
      Assert.Equal(1, dup.Group);
      Assert.False(dup.IsDummy);
      Assert.Equal("Tilman", dup.Name);
      Assert.Equal(2.9, dup.Offset.Y.Meters, 1E-9);
      Assert.Equal(-0.14, dup.OrientationAngle.Radians, 1E-9);

      // check that original has changed values
      Assert.Equal(1, orig.Line.PointAtStart.X, 1E-9);
      Assert.Equal(1, orig.Line.PointAtStart.Y, 1E-9);
      Assert.Equal(-4, orig.Line.PointAtStart.Z, 1E-9);
      Assert.Equal(1, orig.Line.PointAtEnd.X, 1E-9);
      Assert.Equal(1, orig.Line.PointAtEnd.Y, 1E-9);
      Assert.Equal(0, orig.Line.PointAtEnd.Z, 1E-9);
      Assert.Equal(5, orig.Id);
      Assert.Equal(9, orig.Section.Id);
      Assert.Equal(Color.FromArgb(255, 255, 0, 0), orig.Colour);
      Assert.Equal(2, orig.Group);
      Assert.True(orig.IsDummy);
      Assert.Equal("Hugh", orig.Name);
      Assert.Equal(-0.991, orig.Offset.Y.Meters, 1E-9);
      Assert.Equal(0, orig.OrientationAngle.Radians, 1E-9);
    }
  }
}
