using GsaGH.Parameters;

using OasysGH.Units;

using OasysUnits;

using Rhino.Geometry;

using Xunit;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaGeometryBaseTests {
    [Fact]
    public void GsaElement1dDefaultConstructorInitializesLengthUnit() {
      var elem = new GsaElement1d();

      Assert.Equal(DefaultUnits.LengthUnitGeometry, elem.LengthUnit);
    }

    [Fact]
    public void GsaElement1dConstructorWithLineInitializesLengthUnit() {
      var line = new Line(new Point3d(0, 0, 0), new Point3d(1, 1, 1));
      var elem = new GsaElement1d(new LineCurve(line));

      Assert.Equal(DefaultUnits.LengthUnitGeometry, elem.LengthUnit);
    }

    [Fact]
    public void GsaElement1dCopyConstructorPreservesLengthUnit() {
      var line = new Line(new Point3d(0, 0, 0), new Point3d(1, 1, 1));
      var original = new GsaElement1d(new LineCurve(line)) {
        LengthUnit = LengthUnit.Centimeter,
      };

      var copy = new GsaElement1d(original);

      Assert.Equal(LengthUnit.Centimeter, copy.LengthUnit);
    }

    [Fact]
    public void GsaElement1dCanSetLengthUnit() {
      var elem = new GsaElement1d {
        LengthUnit = LengthUnit.Millimeter,
      };

      Assert.Equal(LengthUnit.Millimeter, elem.LengthUnit);
    }

    [Fact]
    public void GsaElement2dDefaultConstructorInitializesLengthUnit() {
      var elem = new GsaElement2d();

      Assert.Equal(DefaultUnits.LengthUnitGeometry, elem.LengthUnit);
    }

    [Fact]
    public void GsaElement2dCanSetLengthUnit() {
      var elem = new GsaElement2d {
        LengthUnit = LengthUnit.Foot,
      };

      Assert.Equal(LengthUnit.Foot, elem.LengthUnit);
    }

    [Fact]
    public void GsaMember1dDefaultConstructorInitializesLengthUnit() {
      var member = new GsaMember1d();

      Assert.Equal(DefaultUnits.LengthUnitGeometry, member.LengthUnit);
    }

    [Fact]
    public void GsaMember1dConstructorWithCurveInitializesLengthUnit() {
      var pts = new Rhino.Collections.Point3dList {
        new Point3d(0, 0, 0),
        new Point3d(1, 1, 1),
      };
      var curve = new PolylineCurve(pts);
      var member = new GsaMember1d(curve);

      Assert.Equal(DefaultUnits.LengthUnitGeometry, member.LengthUnit);
    }

    [Fact]
    public void GsaMember1dCopyConstructorPreservesLengthUnit() {
      var pts = new Rhino.Collections.Point3dList {
        new Point3d(0, 0, 0),
        new Point3d(1, 1, 1),
      };
      var original = new GsaMember1d(new PolylineCurve(pts)) {
        LengthUnit = LengthUnit.Decimeter,
      };

      var copy = new GsaMember1d(original);

      Assert.Equal(LengthUnit.Decimeter, copy.LengthUnit);
    }

    [Fact]
    public void GsaMember1dCanSetLengthUnit() {
      var member = new GsaMember1d {
        LengthUnit = LengthUnit.Kilometer,
      };

      Assert.Equal(LengthUnit.Kilometer, member.LengthUnit);
    }

    [Fact]
    public void GsaMember2dDefaultConstructorInitializesLengthUnit() {
      var member = new GsaMember2d();

      Assert.Equal(DefaultUnits.LengthUnitGeometry, member.LengthUnit);
    }

    [Fact]
    public void GsaMember2dCanSetLengthUnit() {
      var member = new GsaMember2d {
        LengthUnit = LengthUnit.Inch,
      };

      Assert.Equal(LengthUnit.Inch, member.LengthUnit);
    }

    [Fact]
    public void MultipleInstancesHaveIndependentLengthUnits() {
      var elem1 = new GsaElement1d {
        LengthUnit = LengthUnit.Meter,
      };
      var elem2 = new GsaElement1d {
        LengthUnit = LengthUnit.Centimeter,
      };

      Assert.Equal(LengthUnit.Meter, elem1.LengthUnit);
      Assert.Equal(LengthUnit.Centimeter, elem2.LengthUnit);
    }
  }
}
