using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

using GsaAPI;

using GsaGH.Parameters;

using OasysUnits;

using Rhino.Geometry;

using Xunit;

using AngleUnit = OasysUnits.Units.AngleUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;
using Line = Rhino.Geometry.Line;
using GsaGH.Helpers;
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

      Assert.Equal(1, elem.Line.PointAtStart.X, DoubleComparer.Default);
      Assert.Equal(4, elem.Line.PointAtStart.Y, DoubleComparer.Default);
      Assert.Equal(6, elem.Line.PointAtStart.Z, DoubleComparer.Default);
      Assert.Equal(-2, elem.Line.PointAtEnd.X, DoubleComparer.Default);
      Assert.Equal(3, elem.Line.PointAtEnd.Y, DoubleComparer.Default);
      Assert.Equal(-5, elem.Line.PointAtEnd.Z, DoubleComparer.Default);

      Assert.Equal(66, elem.Id);
      Assert.Equal(3, elem.Section.Id);
      Assert.Equal(Color.FromArgb(255, 255, 255, 0), (Color)elem.ApiElement.Colour);
      Assert.Equal(4, elem.ApiElement.Group);
      Assert.True(elem.ApiElement.IsDummy);
      Assert.Equal("EltonJohn", elem.ApiElement.Name);
      Assert.Equal(14.3, elem.Offset.Y.Value, DoubleComparer.Default);
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
      Assert.Equal(2, dup.Line.PointAtStart.X, DoubleComparer.Default);
      Assert.Equal(-1, dup.Line.PointAtStart.Y, DoubleComparer.Default);
      Assert.Equal(0, dup.Line.PointAtStart.Z, DoubleComparer.Default);
      Assert.Equal(2, dup.Line.PointAtEnd.X, DoubleComparer.Default);
      Assert.Equal(-1, dup.Line.PointAtEnd.Y, DoubleComparer.Default);
      Assert.Equal(4, dup.Line.PointAtEnd.Z, DoubleComparer.Default);
      Assert.Equal(3, dup.Id);
      Assert.Equal(9, dup.Section.Id);
      Assert.Equal(Color.FromArgb(255, 0, 255, 255), (Color)dup.ApiElement.Colour);
      Assert.Equal(1, dup.ApiElement.Group);
      Assert.False(dup.ApiElement.IsDummy);
      Assert.Equal("Tilman", dup.ApiElement.Name);
      Assert.Equal(2.9, dup.Offset.Y.Meters, 1E-9);
      Assert.Equal(-0.14, dup.OrientationAngle.Radians, 1E-9);

      // check that original has changed values
      Assert.Equal(1, orig.Line.PointAtStart.X, DoubleComparer.Default);
      Assert.Equal(1, orig.Line.PointAtStart.Y, DoubleComparer.Default);
      Assert.Equal(-4, orig.Line.PointAtStart.Z, DoubleComparer.Default);
      Assert.Equal(1, orig.Line.PointAtEnd.X, DoubleComparer.Default);
      Assert.Equal(1, orig.Line.PointAtEnd.Y, DoubleComparer.Default);
      Assert.Equal(0, orig.Line.PointAtEnd.Z, DoubleComparer.Default);
      Assert.Equal(5, orig.Id);
      Assert.Equal(9, orig.Section.Id);
      Assert.Equal(Color.FromArgb(255, 255, 0, 0), (Color)orig.ApiElement.Colour);
      Assert.Equal(2, orig.ApiElement.Group);
      Assert.True(orig.ApiElement.IsDummy);
      Assert.Equal("Hugh", orig.ApiElement.Name);
      Assert.Equal(-0.991, orig.Offset.Y.Meters, DoubleComparer.Default);
      Assert.Equal(0, orig.OrientationAngle.Radians, DoubleComparer.Default);
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


    public GsaElement1d GetElement(List<int> topo, out Dictionary<int, Node> nodes) {
      var gsaElement = new GSAElement(new Element()) {
        OrientationNode = 0,
        Topology = new ReadOnlyCollection<int>(topo)
      };
      var element = new KeyValuePair<int, GSAElement>(0, gsaElement);
      nodes = new Dictionary<int, Node>();
      var p1 = new Node {
        Position = new Vector3() { X = 1, Y = 2, Z = 3 }
      };
      nodes.Add(0, p1);
      var p2 = new Node {
        Position = new Vector3() { X = 4, Y = 5, Z = 6 }
      };
      nodes.Add(1, p2);

      var localaxes = new ReadOnlyCollection<double>(new List<double> { 1, 0, 0, 0, 1, 0, 0, 0, 1 });
      var section = new GsaSection();
      var elem = new GsaElement1d(element, nodes, section, localaxes, LengthUnit.Meter);
      return elem;
    }

    [Fact]
    public void ShouldCreateElementWithNoValidLine() {
      GsaElement1d elem = GetElement(new List<int> { 2, 3 }, out Dictionary<int, Node> _);
      Assert.Equal(elem.Line.PointAtStart, elem.Line.PointAtEnd);
    }

    [Fact]
    public void ShouldCreateElementWithAValidLine() {

      GsaElement1d elem = GetElement(new List<int> { 0, 1 }, out Dictionary<int, Node> nodes);
      Vector3 pos1 = nodes[0].Position;
      Assert.Equal(elem.Line.PointAtStart,
          new Point3d() { X = pos1.X, Y = pos1.Y, Z = pos1.Z });
      Vector3 pos2 = nodes[1].Position;
      Assert.Equal(elem.Line.PointAtEnd,
          new Point3d() { X = pos2.X, Y = pos2.Y, Z = pos2.Z });
    }

    private GsaElement1d GetBasicElement() {
      return new GsaElement1d(new LineCurve(new Line(new Point3d(0, 0, 0), new Point3d(0, 0, 10))));
    }

    [Fact]
    public void ShouldCreateElementWithSection() {
      GsaElement1d elem = GetBasicElement();
      elem.Id = 1;
      elem.Section = new GsaSection {
        Id = 2
      };

      Assert.Contains("PB", elem.ToString());
    }

    [Fact]
    public void ShouldCreateElementWithSectionNoId() {
      GsaElement1d elem = GetBasicElement();
      elem.Id = 1;
      elem.Section = new GsaSection();
      string asd = elem.Section.ApiSection.Profile = "TESTSECTION";
      Assert.Contains("TESTSECTION", elem.ToString());
    }

    [Fact]
    public void ShouldCreateElementWithSpring() {
      GsaElement1d elem = GetBasicElement();
      elem.Id = 1;
      elem.SpringProperty = new GsaSpringProperty(2);

      Assert.Contains("SP", elem.ToString());
    }

    [Fact]
    public void ShouldCreateElementWithSpringNoId() {
      GsaElement1d elem = GetBasicElement();
      elem.Id = 1;
      elem.SpringProperty = new GsaSpringProperty {
        ApiProperty = new AxialSpringProperty() { Stiffness = 1000 }
      };
      elem.SpringProperty.ApiProperty.Name = "Test";

      Assert.Contains("Test", elem.ToString());
    }
  }
}
