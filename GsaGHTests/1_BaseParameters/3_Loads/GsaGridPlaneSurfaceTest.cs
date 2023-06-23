﻿using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysUnits.Units;
using Rhino.Geometry;
using Xunit;
using static GsaAPI.GridSurface;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaGridPlaneSurfaceTest {

    [Theory]
    [InlineData(0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1)]
    [InlineData(1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0)]
    [InlineData(-1, -1, -1, -1, 0, 0, 0, -1, 0, 0, 0, -1)]
    public void ConstructorTest(
      double originX, double originY, double originZ, double xAxisX, double xAxisY, double xAxisZ,
      double yAxisX, double yAxisY, double yAxisZ, double zAxisX, double zAxisY, double zAxisZ) {
      var plane = new Plane {
        Origin = new Point3d(originX, originY, originZ),
        XAxis = new Vector3d(xAxisX, xAxisY, xAxisZ),
        YAxis = new Vector3d(yAxisX, yAxisY, yAxisZ),
        ZAxis = new Vector3d(zAxisX, zAxisY, zAxisZ),
      };

      var gps = new GsaGridPlaneSurface(plane);

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
      Axis axis = gps.GetAxis(LengthUnit.Meter);
      Assert.Equal(originX, axis.Origin.X);
      Assert.Equal(originY, axis.Origin.Y);
      Assert.Equal(originZ, axis.Origin.Z);
      Assert.Equal(xAxisX, axis.XVector.X);
      Assert.Equal(xAxisY, axis.XVector.Y);
      Assert.Equal(xAxisZ, axis.XVector.Z);
      Assert.Equal(yAxisX, axis.XYPlane.X);
      Assert.Equal(yAxisY, axis.XYPlane.Y);
      Assert.Equal(yAxisZ, axis.XYPlane.Z);
      Assert.Equal(0, gps.GridSurfaceId);
      Assert.Equal(0, gps.GridPlaneId);
    }

    [Fact]
    public void DuplicateTest() {
      var original = new GsaGridPlaneSurface();

      GsaGridPlaneSurface duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

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
      duplicate.GridSurfaceId = 1;
      duplicate.GridPlaneId = 1;

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
      Assert.Equal(0, original.GridSurfaceId);
      Assert.Equal(0, original.GridPlaneId);
    }

    [Fact]
    public void EmptyConstructorTest() {
      var gps = new GsaGridPlaneSurface();

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
      Axis axis = gps.GetAxis(LengthUnit.Meter);
      Assert.Equal(0, axis.Origin.X);
      Assert.Equal(0, axis.Origin.Y);
      Assert.Equal(0, axis.Origin.Z);
      Assert.Equal(1, axis.XVector.X);
      Assert.Equal(0, axis.XVector.Y);
      Assert.Equal(0, axis.XVector.Z);
      Assert.Equal(0, axis.XYPlane.X);
      Assert.Equal(1, axis.XYPlane.Y);
      Assert.Equal(0, axis.XYPlane.Z);
      Assert.Equal(0, gps.GridSurfaceId);
      Assert.Equal(0, gps.GridPlaneId);
    }
  }
}
