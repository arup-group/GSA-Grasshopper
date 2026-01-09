using GsaGH.Parameters;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Model {
  [Collection("GrasshopperFixture collection")]
  public class GsaGridLineTests {

    [Fact]
    public void ShouldReturnZeroForParallelLine() {
      var line = new Line(Point3d.Origin, new Point3d(1, 0, 0));
      var gsaGridLine = new GsaGridLine(line, string.Empty);

      Assert.Equal(0, gsaGridLine.GridLine.Theta1, precision: 5);
    }

    [Fact]
    public void ShouldCalculateAngleBetweenXAxisAndLine() {
      var line = new Line(Point3d.Origin, new Point3d(1, 1, 0));
      var gsaGridLine = new GsaGridLine(line, string.Empty);

      Assert.Equal(45, gsaGridLine.GridLine.Theta1, precision: 5);
    }

    [Fact]
    public void ShouldProvideTheClockwiseAngle() {
      var line = new Line(Point3d.Origin, new Point3d(1, -1, 0));
      var gsaGridLine = new GsaGridLine(line, string.Empty);

      Assert.Equal(315, gsaGridLine.GridLine.Theta1, precision: 5);
    }

    [Fact]
    public void ShouldProvideTheClockwiseAngle2() {
      var line = new Line(Point3d.Origin, new Point3d(-1, -1, 0));
      var gsaGridLine = new GsaGridLine(line, string.Empty);

      Assert.Equal(225, gsaGridLine.GridLine.Theta1, precision: 5);
    }

  }
}
