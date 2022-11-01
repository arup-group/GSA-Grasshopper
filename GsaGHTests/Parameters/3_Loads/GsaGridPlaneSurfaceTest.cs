using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Rhino.Geometry;
using Xunit;
using static GsaAPI.GridSurface;

namespace GsaGHTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GsaGridPlaneSurfaceTest
  {
    [Fact]
    public void EmptyConstructorTest()
    {
      // Act
      GsaGridPlaneSurface gps = new GsaGridPlaneSurface();

      // Assert
      Assert.Equal(Plane.WorldXY, gps.Plane);
      Assert.Equal(0, gps.GridPlane.AxisProperty);
      Assert.Equal(0, gps.GridPlane.Elevation);
      Assert.False(gps.GridPlane.IsStoreyType);
      Assert.Equal("", gps.GridPlane.Name);
      Assert.Equal(0, gps.GridPlane.ToleranceAbove);
      Assert.Equal(0, gps.GridPlane.ToleranceBelow);
      Assert.Equal(0, gps.GridSurface.Direction);
      Assert.Equal("all", gps.GridSurface.Elements);
      Assert.Equal(Element_Type.ONE_DIMENSIONAL, gps.GridSurface.ElementType);
      Assert.Equal(GridSurfaceExpansionType.PLANE_CORNER, gps.GridSurface.ExpansionType);
      Assert.Equal("", gps.GridSurface.Name);
      Assert.Equal(Span_Type.TWO_WAY, gps.GridSurface.SpanType);
      Assert.Equal(0.01, gps.GridSurface.Tolerance);
      Assert.Equal("", gps.Axis.Name);
      Assert.Equal(0, gps.Axis.Origin.X);
      Assert.Equal(0, gps.Axis.Origin.Y);
      Assert.Equal(0, gps.Axis.Origin.Z);
      Assert.Equal(1, gps.Axis.XVector.X);
      Assert.Equal(0, gps.Axis.XVector.Y);
      Assert.Equal(0, gps.Axis.XVector.Z);
      Assert.Equal(0, gps.Axis.XYPlane.X);
      Assert.Equal(1, gps.Axis.XYPlane.Y);
      Assert.Equal(0, gps.Axis.XYPlane.Z);
      Assert.Equal(0, gps.GridSurfaceID);
      Assert.Equal(0, gps.GridPlaneID);
    }

    [Theory]
    [InlineData(0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1)]
    [InlineData(1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0)]
    [InlineData(-1, -1, -1, -1, 0, 0, 0, -1, 0, 0, 0, -1)]
    public void ConstructorTest(double originX, double originY, double originZ, double xAxisX, double xAxisY, double xAxisZ, double yAxisX, double yAxisY, double yAxisZ, double zAxisX, double zAxisY, double zAxisZ)
    {
      // Act
      Plane plane = new Plane();
      plane.Origin = new Point3d(originX, originY, originZ);
      plane.XAxis = new Vector3d(xAxisX, xAxisY, xAxisZ);
      plane.YAxis = new Vector3d(yAxisX, yAxisY, yAxisZ);
      plane.ZAxis = new Vector3d(zAxisX, zAxisY, zAxisZ);

      GsaGridPlaneSurface gps = new GsaGridPlaneSurface(plane);

      // Assert
      Assert.Equal(originX, gps.Plane.OriginX);
      Assert.Equal(originY, gps.Plane.OriginY);
      Assert.Equal(originZ, gps.Plane.OriginZ);
      Assert.Equal(xAxisX, gps.Plane.XAxis.X);
      Assert.Equal(xAxisY, gps.Plane.XAxis.Y);
      Assert.Equal(xAxisZ, gps.Plane.XAxis.Z);
      Assert.Equal(yAxisX, gps.Plane.YAxis.X);
      Assert.Equal(yAxisY, gps.Plane.YAxis.Y);
      Assert.Equal(yAxisZ, gps.Plane.YAxis.Z);
      Assert.Equal(zAxisX, gps.Plane.ZAxis.X);
      Assert.Equal(zAxisY, gps.Plane.ZAxis.Y);
      Assert.Equal(zAxisZ, gps.Plane.ZAxis.Z);
      Assert.Equal(0, gps.GridPlane.AxisProperty);
      Assert.Equal(0, gps.GridPlane.Elevation);
      Assert.False(gps.GridPlane.IsStoreyType);
      Assert.Equal("", gps.GridPlane.Name);
      Assert.Equal(0, gps.GridPlane.ToleranceAbove);
      Assert.Equal(0, gps.GridPlane.ToleranceBelow);
      Assert.Equal(0, gps.GridSurface.Direction);
      Assert.Equal("all", gps.GridSurface.Elements);
      Assert.Equal(Element_Type.ONE_DIMENSIONAL, gps.GridSurface.ElementType);
      Assert.Equal(GridSurfaceExpansionType.UNDEF, gps.GridSurface.ExpansionType);
      Assert.Equal("", gps.GridSurface.Name);
      Assert.Equal(Span_Type.ONE_WAY, gps.GridSurface.SpanType);
      Assert.Equal(0.01, gps.GridSurface.Tolerance);
      Assert.Equal("", gps.Axis.Name);
      Assert.Equal(originX, gps.Axis.Origin.X);
      Assert.Equal(originY, gps.Axis.Origin.Y);
      Assert.Equal(originZ, gps.Axis.Origin.Z);
      Assert.Equal(xAxisX, gps.Axis.XVector.X);
      Assert.Equal(xAxisY, gps.Axis.XVector.Y);
      Assert.Equal(xAxisZ, gps.Axis.XVector.Z);
      Assert.Equal(yAxisX, gps.Axis.XYPlane.X);
      Assert.Equal(yAxisY, gps.Axis.XYPlane.Y);
      Assert.Equal(yAxisZ, gps.Axis.XYPlane.Z);
      Assert.Equal(0, gps.GridSurfaceID);
      Assert.Equal(0, gps.GridPlaneID);
    }

    [Fact]
    public void DuplicateTest()
    {
      // Arrange
      GsaGridPlaneSurface original = new GsaGridPlaneSurface();

      // Act
      GsaGridPlaneSurface duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);

      // make some changes to duplicate
      duplicate.Plane = Plane.WorldYZ;
      duplicate.GridPlane.AxisProperty = 1;
      duplicate.GridPlane.Elevation = 1;
      duplicate.GridPlane.IsStoreyType = true;
      duplicate.GridPlane.Name = "name";
      duplicate.GridPlane.ToleranceAbove = 1;
      duplicate.GridPlane.ToleranceBelow = 1;
      duplicate.GridSurface.Direction = 1;
      duplicate.GridSurface.Elements = "";
      duplicate.GridSurface.ElementType = Element_Type.TWO_DIMENSIONAL;
      duplicate.GridSurface.ExpansionType = GridSurfaceExpansionType.LEGACY;
      duplicate.GridSurface.Name = "name";
      duplicate.GridSurface.SpanType = Span_Type.TWO_WAY_SIMPLIFIED_TRIBUTARY_AREAS;
      duplicate.GridSurface.Tolerance = 0.2;
      duplicate.Axis.Name = "name";
      duplicate.Axis.Origin.X = -1;
      duplicate.Axis.Origin.Y = -1;
      duplicate.Axis.Origin.Z = -1;
      duplicate.Axis.XVector.X = -1;
      duplicate.Axis.XVector.Y = -1;
      duplicate.Axis.XVector.Z = -1;
      duplicate.Axis.XYPlane.X = -1;
      duplicate.Axis.XYPlane.Y = -1;
      duplicate.Axis.XYPlane.Z = -1;
      duplicate.GridSurfaceID = 1;
      duplicate.GridPlaneID = 1;

      Assert.Equal(Plane.WorldXY, original.Plane);
      Assert.Equal(0, original.GridPlane.AxisProperty);
      Assert.Equal(0, original.GridPlane.Elevation);
      Assert.False(original.GridPlane.IsStoreyType);
      Assert.Equal("", original.GridPlane.Name);
      Assert.Equal(0, original.GridPlane.ToleranceAbove);
      Assert.Equal(0, original.GridPlane.ToleranceBelow);
      Assert.Equal(0, original.GridSurface.Direction);
      Assert.Equal("all", original.GridSurface.Elements);
      Assert.Equal(Element_Type.ONE_DIMENSIONAL, original.GridSurface.ElementType);
      Assert.Equal(GridSurfaceExpansionType.PLANE_CORNER, original.GridSurface.ExpansionType);
      Assert.Equal("", original.GridSurface.Name);
      Assert.Equal(Span_Type.TWO_WAY, original.GridSurface.SpanType);
      Assert.Equal(0.01, original.GridSurface.Tolerance);
      Assert.Equal("", original.Axis.Name);
      Assert.Equal(0, original.Axis.Origin.X);
      Assert.Equal(0, original.Axis.Origin.Y);
      Assert.Equal(0, original.Axis.Origin.Z);
      Assert.Equal(1, original.Axis.XVector.X);
      Assert.Equal(0, original.Axis.XVector.Y);
      Assert.Equal(0, original.Axis.XVector.Z);
      Assert.Equal(0, original.Axis.XYPlane.X);
      Assert.Equal(1, original.Axis.XYPlane.Y);
      Assert.Equal(0, original.Axis.XYPlane.Z);
      Assert.Equal(0, original.GridSurfaceID);
      Assert.Equal(0, original.GridPlaneID);
    }
  }
}
