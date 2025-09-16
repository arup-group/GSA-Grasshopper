using System;
using System.Collections.Generic;

using GsaGH.Helpers.GH;

using Rhino.Collections;
using GsaGHTests.Components.Geometry;
using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Helpers.GH {
  [Collection("GrasshopperFixture collection")]
  public class RhinoConversionsTests {
    [Fact]
    public void BuildArcLineCurveFromPtsAndTopoTypeTestArc1d() {
      var topolist = new Point3dList() {
        new Point3d(0, 0, 0),
        new Point3d(2, 2, 0),
        new Point3d(4, 0, 0)
      };
      var topotype = new List<string>() {
        string.Empty,
        "A",
        string.Empty,
      };

      PolyCurve crv = RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(topolist, topotype);
      Assert.True(crv != null);
      Assert.True(crv.IsArc());
    }

    [Fact]
    public void ConvertMem1dCrvTest() {
      var topolist = new Point3dList() {
        new Point3d(0, 0, 0),
        new Point3d(2, 2, 0),
        new Point3d(4, 0, 0)
      };
      var crvs = new PolyCurve();
      crvs.Append(new Arc(topolist[0], topolist[1], topolist[2]));

      Tuple<PolyCurve, Point3dList, List<string>> mem1d
        = RhinoConversions.ConvertMem1dCrv(crvs);

      Assert.True(mem1d.Item1.IsArc());
      Assert.Equal(3, mem1d.Item2.Count);
      TestPoint(topolist[0], mem1d.Item2[0]);
      TestPoint(topolist[1], mem1d.Item2[1]);
      TestPoint(topolist[2], mem1d.Item2[2]);
      Assert.Equal(string.Empty, mem1d.Item3[0]);
      Assert.Equal("A", mem1d.Item3[1]);
      Assert.Equal(string.Empty, mem1d.Item3[2]);
    }


    [Theory]
    [InlineData(0.03)]
    [InlineData(0.07)]
    [InlineData(0.09)]
    public void ConvertMem2dCrvSmallCircle(double radius) {
      var circle = new Circle(Plane.WorldXY, radius);
      var result = RhinoConversions.ConvertMem2dCrv(circle.ToNurbsCurve());
      Assert.Equal(1, result.Item1.SpanCount);
      Assert.Equal(5, result.Item2.Count);
      Assert.Equal(5, result.Item3.Count);
    }
    
    [Fact]
    public void ConvertMem2dCrvSmallException() {
      var circle = new Circle(Plane.WorldXY, 0.9);
      Assert.Throws<Exception>(
        () => RhinoConversions.ConvertMem2dCrv(circle.ToNurbsCurve(), parameter: 1.2)
      );
    }

    private void TestPoint(Point3d expected, Point3d actual) {
      Assert.Equal(expected.X, actual.X, 10);
      Assert.Equal(expected.Y, actual.Y, 10);
      Assert.Equal(expected.Z, actual.Z, 10);
    }

    [Fact]
    public void ConvertCurveToLoadPanelTopoTest() {
      PolylineCurve curve = CreateElement2dTests.Get2dPolyline();
      Point3dList actualPoints = RhinoConversions.LoadPanelTopo(curve);
      Assert.Equal(curve.PointCount - 1, actualPoints.Count);
      for (int i = 0; i < actualPoints.Count; i++) {
        Assert.Equal(curve.PointAt(i).X, actualPoints[i].X);
      }
    }

    [Fact]
    public void ConvertCurveToLoadPanelIndexTest() {
      List<List<int>> loadPanelIndices = RhinoConversions.LoadPanelTopoIndices(CreateElement2dTests.Get2dPolyline());
      foreach (List<int> Indices in loadPanelIndices) {
        Assert.Equal(4, Indices.Count);
        for (int i = 0; i < Indices.Count; i++) {
          Assert.Equal(i, Indices[i]);
        }
      }
    }
  }
}
