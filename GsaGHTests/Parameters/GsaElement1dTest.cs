using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysUnits.Units;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GsaElement1dTest
  {
    [Fact]
    public void CloneApiObjectTest()
    {
      // Arrange
      GsaSection section = new GsaSection();
      section.Name = "Name";
      GsaElement1d element1d = new GsaElement1d(new Element(), new LineCurve(), 1, section, new GsaNode());
      element1d.Name = "Name";
      Element original = element1d.API_Element;

      // Act
      Element duplicate = element1d.GetAPI_ElementClone();

      // Assert
      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void DuplicateTest()
    {
      // Arrange
      GsaSection section = new GsaSection();
      section.Name = "Name";
      GsaElement1d original = new GsaElement1d(new Element(), new LineCurve(), 1, section, new GsaNode());
      original.Name = "Name";

      // Act
      GsaElement1d duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void TestCreateGsaElem1dFromLn()
    {
      // create new line
      Line ln = new Line(new Point3d(1, 4, 6), new Point3d(-2, 3, -5));

      // create element
      GsaElement1d elem = new GsaElement1d(new LineCurve(ln));

      // set some element class members
      elem.ID = 66;
      elem.Section = new GsaSection();
      elem.Section.ID = 2;
      elem.Colour = System.Drawing.Color.Yellow;
      elem.Group = 4;
      elem.IsDummy = true;
      elem.Name = "EltonJohn";
      GsaOffset offset = new GsaOffset(0, 0, 14.3, 0);
      elem.Offset = offset;
      elem.OrientationAngle = 90;
      elem.Section.ID = 3;

      // check the line end points are correct
      Assert.Equal(1, elem.Line.PointAtStart.X);
      Assert.Equal(4, elem.Line.PointAtStart.Y);
      Assert.Equal(6, elem.Line.PointAtStart.Z);
      Assert.Equal(-2, elem.Line.PointAtEnd.X);
      Assert.Equal(3, elem.Line.PointAtEnd.Y);
      Assert.Equal(-5, elem.Line.PointAtEnd.Z);

      // check other members are valid
      Assert.Equal(66, elem.ID);
      Assert.Equal(3, elem.Section.ID);
      Assert.Equal(System.Drawing.Color.FromArgb(255, 255, 255, 0), elem.Colour);
      Assert.Equal(4, elem.Group);
      Assert.True(elem.IsDummy);
      Assert.Equal("EltonJohn", elem.Name);
      Assert.Equal(14.3, elem.Offset.Y.Value);
      Assert.Equal(90, elem.OrientationAngle);
      Assert.Equal(3, elem.Section.ID);
    }

    [Fact]
    public void TestDuplicateElem1d()
    {
      // create new line
      Line ln = new Line(new Point3d(2, -1, 0), new Point3d(2, -1, 4));

      // create element
      GsaElement1d orig = new GsaElement1d(new LineCurve(ln));

      // set some element class members
      orig.ID = 3;
      orig.Section = new GsaSection();
      orig.Section.ID = 7;
      orig.Colour = System.Drawing.Color.Aqua;
      orig.Group = 1;
      orig.IsDummy = false;
      orig.Name = "Tilman";
      GsaOffset offset = new GsaOffset(0, 0, 2.9, 0);
      orig.Offset = offset;
      orig.OrientationAngle = -0.14;

      // duplicate original
      GsaElement1d dup = orig.Duplicate();

      // make some changes to original
      orig.Line = new LineCurve(new Line(new Point3d(1, 1, -4), new Point3d(1, 1, 0)));
      orig.ID = 5;
      orig.Section.ID = 9;
      orig.Colour = System.Drawing.Color.Red;
      orig.Group = 2;
      orig.IsDummy = true;
      orig.Name = "Hugh";
      GsaOffset offset2 = new GsaOffset(0, 0, -0.991, 0, LengthUnit.Meter);
      orig.Offset = offset2;
      orig.OrientationAngle = 0;

      // check that values in duplicate are not changed
      Assert.Equal(2, dup.Line.PointAtStart.X, 1E-9);
      Assert.Equal(-1, dup.Line.PointAtStart.Y, 1E-9);
      Assert.Equal(0, dup.Line.PointAtStart.Z, 1E-9);
      Assert.Equal(2, dup.Line.PointAtEnd.X, 1E-9);
      Assert.Equal(-1, dup.Line.PointAtEnd.Y, 1E-9);
      Assert.Equal(4, dup.Line.PointAtEnd.Z, 1E-9);
      Assert.Equal(3, dup.ID);
      Assert.Equal(7, dup.Section.ID);
      Assert.Equal(System.Drawing.Color.FromArgb(255, 0, 255, 255), dup.Colour);
      Assert.Equal(1, dup.Group);
      Assert.False(dup.IsDummy);
      Assert.Equal("Tilman", dup.Name);
      Assert.Equal(2.9, dup.Offset.Y.Meters, 1E-9);
      Assert.Equal(-0.14, dup.OrientationAngle, 1E-9);

      // check that original has changed values
      Assert.Equal(1, orig.Line.PointAtStart.X, 1E-9);
      Assert.Equal(1, orig.Line.PointAtStart.Y, 1E-9);
      Assert.Equal(-4, orig.Line.PointAtStart.Z, 1E-9);
      Assert.Equal(1, orig.Line.PointAtEnd.X, 1E-9);
      Assert.Equal(1, orig.Line.PointAtEnd.Y, 1E-9);
      Assert.Equal(0, orig.Line.PointAtEnd.Z, 1E-9);
      Assert.Equal(5, orig.ID);
      Assert.Equal(9, orig.Section.ID);
      Assert.Equal(System.Drawing.Color.FromArgb(255, 255, 0, 0), orig.Colour);
      Assert.Equal(2, orig.Group);
      Assert.True(orig.IsDummy);
      Assert.Equal("Hugh", orig.Name);
      Assert.Equal(-0.991, orig.Offset.Y.Meters, 1E-9);
      Assert.Equal(0, orig.OrientationAngle, 1E-9);
    }

    [Fact]
    public void TestCreateGsaElem1dGetReleases()
    {
      // create new line
      Line ln = new Line(new Point3d(1, 4, 6), new Point3d(-2, 3, -5));

      // create element
      GsaElement1d elem = new GsaElement1d(new LineCurve(ln));

      GsaBool6 rel1 = elem.ReleaseStart;
      Assert.False(rel1.X);
      Assert.False(rel1.Y);
      Assert.False(rel1.Z);
      Assert.False(rel1.XX);
      Assert.False(rel1.YY);
      Assert.False(rel1.ZZ);
    }
  }
}
