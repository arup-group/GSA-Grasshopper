using System;

using GsaAPI;

using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Rhino.Geometry;

using Xunit;

using Line = Rhino.Geometry.Line;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaGridLineTest {
    [Fact]
    public void ConstructorTest() {
      var gridLine = new GsaGridLine(new GridLine("label"), new PolyCurve());

      Assert.NotNull(gridLine.GridLine);
      Assert.Equal("label", gridLine.GridLine.Label);
    }

    [Fact]
    public void DuplicateTest() {
      var original = new GsaGridLine(new GridLine("label1"), new PolyCurve());

      var duplicate = new GsaGridLine(original);

      Duplicates.AreEqual(original, duplicate);

      duplicate.GridLine = new GridLine("label2");
      duplicate.Curve = new PolyCurve();

      Assert.NotNull(original.GridLine);
      Assert.Equal("label1", original.GridLine.Label);
    }

    [Fact]
    public void ArcGridLineTest() {
      var arc = new Arc(new Point3d(0, 0, 0), 0.5, Math.PI / 4);
      var gridline = new GsaGridLine(arc, "Arc");

      Assert.True(gridline.Curve.GetLength(0) > 0);
      Assert.Equal(arc.Length, gridline.Curve.GetLength(0));
      Assert.Equal("Arc", gridline.GridLine.Label);
      Assert.Equal(GridLineShape.Arc, gridline.GridLine.Shape);
      Assert.Equal(0, gridline.GridLine.X);
      Assert.Equal(0, gridline.GridLine.Y);
      Assert.Equal(0.5, gridline.GridLine.Length);
      Assert.Equal(0, gridline.GridLine.Theta1);
      Assert.Equal(45, gridline.GridLine.Theta2);
    }

    [Fact]
    public void LineGridLineTest() {
      var line = new Line(new Point3d(10, 15, 0), new Point3d(20, 15, 0));
      var gridline = new GsaGridLine(line, "Line");

      Assert.True(gridline.Curve.GetLength(0) > 0);
      Assert.Equal(10, gridline.Curve.GetLength(0));
      Assert.Equal("Line", gridline.GridLine.Label);
      Assert.Equal(GridLineShape.Line, gridline.GridLine.Shape);
      Assert.Equal(10, gridline.GridLine.X);
      Assert.Equal(15, gridline.GridLine.Y);
      Assert.Equal(10, gridline.GridLine.Length);
      Assert.Equal(0, gridline.GridLine.Theta1);
      Assert.Equal(0, gridline.GridLine.Theta2);
    }
  }
}
