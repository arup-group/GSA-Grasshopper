﻿using System.Drawing;

using GsaGH.Parameters;

using OasysUnits;

using Rhino.Geometry;

using Xunit;

using AngleUnit = OasysUnits.Units.AngleUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;
using Line = Rhino.Geometry.Line;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaElement1dTest {
    [Fact]
    public void TestCreateGsaElem1dFromLn() {
      var ln = new Line(new Point3d(1, 4, 6), new Point3d(-2, 3, -5));

      var elem = new GsaElement1d(new LineCurve(ln)) {
        Id = 66,
        Section = new GsaSection(3),
      };
      elem.ApiElement.Colour = Color.Yellow;
      elem.ApiElement.Group = 4;
      elem.ApiElement.IsDummy = true;
      elem.ApiElement.Name = "EltonJohn";
      var offset = new GsaOffset(0, 0, 14.3, 0);
      elem.Offset = offset;
      elem.OrientationAngle = new Angle(90, AngleUnit.Degree);

      Assert.Equal(1, elem.Line.PointAtStart.X);
      Assert.Equal(4, elem.Line.PointAtStart.Y);
      Assert.Equal(6, elem.Line.PointAtStart.Z);
      Assert.Equal(-2, elem.Line.PointAtEnd.X);
      Assert.Equal(3, elem.Line.PointAtEnd.Y);
      Assert.Equal(-5, elem.Line.PointAtEnd.Z);

      Assert.Equal(66, elem.Id);
      Assert.Equal(3, elem.Section.Id);
      Assert.Equal(Color.FromArgb(255, 255, 255, 0), (Color)elem.ApiElement.Colour);
      Assert.Equal(4, elem.ApiElement.Group);
      Assert.True(elem.ApiElement.IsDummy);
      Assert.Equal("EltonJohn", elem.ApiElement.Name);
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
        Section = new GsaSection(7)
      };
      orig.ApiElement.Colour = Color.Aqua;
      orig.ApiElement.Group = 1;
      orig.ApiElement.IsDummy = false;
      orig.ApiElement.Name = "Tilman";

      var offset = new GsaOffset(0, 0, 2.9, 0);
      orig.Offset = offset;
      orig.OrientationAngle = new Angle(-0.14, AngleUnit.Radian);

      var dup = new GsaElement1d(orig);

      orig.Line = new LineCurve(new Line(new Point3d(1, 1, -4), new Point3d(1, 1, 0)));
      orig.Id = 5;
      orig.Section.Id = 9;
      orig.ApiElement.Colour = Color.Red;
      orig.ApiElement.Group = 2;
      orig.ApiElement.IsDummy = true;
      orig.ApiElement.Name = "Hugh";
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
      Assert.Equal(9, dup.Section.Id);
      Assert.Equal(Color.FromArgb(255, 0, 255, 255), (Color)dup.ApiElement.Colour);
      Assert.Equal(1, dup.ApiElement.Group);
      Assert.False(dup.ApiElement.IsDummy);
      Assert.Equal("Tilman", dup.ApiElement.Name);
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
      Assert.Equal(Color.FromArgb(255, 255, 0, 0), (Color)orig.ApiElement.Colour);
      Assert.Equal(2, orig.ApiElement.Group);
      Assert.True(orig.ApiElement.IsDummy);
      Assert.Equal("Hugh", orig.ApiElement.Name);
      Assert.Equal(-0.991, orig.Offset.Y.Meters, 1E-9);
      Assert.Equal(0, orig.OrientationAngle.Radians, 1E-9);
    }

    [Fact]
    public void TestDuplicateElem1dChangeDuplicateSection() {
      var ln = new Line(new Point3d(0, 0, 0), new Point3d(0, 0, 10));
      var orig = new GsaElement1d(new LineCurve(ln)) {
        Section = new GsaSection("STD R 200 100")
      };

      var dup = new GsaElement1d(orig) {
        Section = new GsaSection("STD I 1000 500 10 5")
      };

      Assert.Equal("STD R 200 100", orig.Section.ApiSection.Profile);
      Assert.Equal("STD I 1000 500 10 5", dup.Section.ApiSection.Profile);
    }

    [Fact]
    public void TestDuplicateElem1dChangeApiSection() {
      var ln = new Line(new Point3d(0, 0, 0), new Point3d(0, 0, 10));
      var orig = new GsaElement1d(new LineCurve(ln)) {
        Section = new GsaSection("STD R 200 100")
      };

      var dup = new GsaElement1d(orig);
      dup.Section.ApiSection.Profile = "STD I 1000 500 10 5";

      Assert.Equal("STD I 1000 500 10 5", orig.Section.ApiSection.Profile);
      Assert.Equal("STD I 1000 500 10 5", dup.Section.ApiSection.Profile);
    }

    [Fact]
    public void TestDuplicateElem1dChangeOriginalSection() {
      var ln = new Line(new Point3d(0, 0, 0), new Point3d(0, 0, 10));
      var orig = new GsaElement1d(new LineCurve(ln)) {
        Section = new GsaSection("STD R 200 100")
      };

      var dup = new GsaElement1d(orig);
      orig.Section = new GsaSection("STD I 1000 500 10 5");

      Assert.Equal("STD R 200 100", dup.Section.ApiSection.Profile);
      Assert.Equal("STD I 1000 500 10 5", orig.Section.ApiSection.Profile);
    }

    [Fact]
    public void TestDuplicateElem1dSection() {
      var ln = new Line(new Point3d(0, 0, 0), new Point3d(0, 0, 10));
      var orig = new GsaElement1d(new LineCurve(ln)) {
        Section = new GsaSection("STD R 200 100")
      };

      var dup = new GsaElement1d(orig);
      Assert.Equal(dup.Section.Guid, orig.Section.Guid);
    }
  }
}
