using System;
using GsaGH.Graphics;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Helpers
{
  [Collection("GrasshopperFixture collection")]
  public class _DisplayTest
  {
    [Theory]
    [InlineData(0, 0, 1, 0, 0, 0, 1, 0, 1, 0)]
    [InlineData(0, 0, 1, Math.PI / 2.0, 0, 0, 1, -1, 0, 0)]
    [InlineData(0, 0, 1, Math.PI, 0, 0, 1, 0, -1, 0)]
    [InlineData(0, 0, 1, Math.PI * 3 / 2.0, 0, 0, 1, 1, 0, 0)]
    [InlineData(0, 0, 1, 2 * Math.PI, 0, 0, 1, 0, 1, 0)]
    [InlineData(0, 0, 1, 5 * Math.PI / 2.0, 0, 0, 1, -1, 0, 0)]
    [InlineData(0, 0, -1, 0, 0, 0, -1, 0, 1, 0)]
    [InlineData(0, 0, -1, Math.PI / 2.0, 0, 0, -1, 1, 0, 0)]
    [InlineData(0, 0, -1, Math.PI, 0, 0, -1, 0, -1, 0)]
    [InlineData(0, 1, 0, 0, 0, 1, 0, 0, 0, 1)]
    [InlineData(0, 1, 0, Math.PI / 2.0, 0, 1, 0, 1, 0, 0)]
    [InlineData(0, 1, 0, Math.PI, 0, 1, 0, 0, 0, -1)]
    [InlineData(0, 1, 0, Math.PI * 3 / 2.0, 0, 1, 0, -1, 0, 0)]
    [InlineData(0, 1, 0, 2 * Math.PI, 0, 1, 0, 0, 0, 1)]
    [InlineData(0, 1, 0, 5 * Math.PI / 2.0, 0, 1, 0, 1, 0, 0)]
    [InlineData(0, -1, 0, 0, 0, -1, 0, 0, 0, 1)]
    [InlineData(0, -1, 0, Math.PI / 2.0, 0, -1, 0, -1, 0, 0)]
    [InlineData(0, -1, 0, Math.PI, 0, -1, 0, 0, 0, -1)]
    [InlineData(1, 0, 0, 0, 1, 0, 0, 0, 0, 1)]
    [InlineData(1, 0, 0, Math.PI / 2.0, 1, 0, 0, 0, -1, 0)]
    [InlineData(1, 0, 0, Math.PI, 1, 0, 0, 0, 0, -1)]
    [InlineData(-1, 0, 0, 0, -1, 0, 0, 0, 0, 1)]
    [InlineData(-1, 0, 0, Math.PI / 2.0, -1, 0, 0, 0, 1, 0)]
    [InlineData(-1, 0, 0, Math.PI, -1, 0, 0, 0, 0, -1)]
    public void GetLocalPlaneTest(double dx, double dy, double dz, double angle, double expextedLocalX1, double expextedLocalY1, double expextedLocalZ1, double expextedLocalX2, double expextedLocalY2, double expextedLocalZ2)
    {
      // Arrange
      Random random = new Random();
      double x = random.NextDouble();
      double y = random.NextDouble();
      double z = random.NextDouble();
      Point3d start = new Point3d(x, y, z);
      Point3d end = new Point3d(x + dx, y + dy, z + dz);
      Line line = new Line(start, end);
      PolyCurve curve = new PolyCurve();
      curve.Append(line);

      double t = curve.GetLength() / 2.0;

      // Act
      Tuple<Vector3d, Vector3d, Vector3d> localAxis = Display.GetLocalPlane(curve, t, angle);

      // Assert
      Vector3d expectedLocalX = new Vector3d(expextedLocalX1, expextedLocalY1, expextedLocalZ1);
      Vector3d expectedLocalY = new Vector3d(expextedLocalX2, expextedLocalY2, expextedLocalZ2);
      Vector3d expectedLocalZ = Vector3d.CrossProduct(expectedLocalX, expectedLocalY);

      int precision = 10;

      Assert.Equal(expectedLocalX.X, localAxis.Item1.X, precision);
      Assert.Equal(expectedLocalX.Y, localAxis.Item1.Y, precision);
      Assert.Equal(expectedLocalX.Z, localAxis.Item1.Z, precision);
      Assert.Equal(expectedLocalY.X, localAxis.Item2.X, precision);
      Assert.Equal(expectedLocalY.Y, localAxis.Item2.Y, precision);
      Assert.Equal(expectedLocalY.Z, localAxis.Item2.Z, precision);
      Assert.Equal(expectedLocalZ.X, localAxis.Item3.X, precision);
      Assert.Equal(expectedLocalZ.Y, localAxis.Item3.Y, precision);
      Assert.Equal(expectedLocalZ.Z, localAxis.Item3.Z, precision);
    }
  }
}
