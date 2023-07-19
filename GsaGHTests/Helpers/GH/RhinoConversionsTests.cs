using System;
using System.Collections.Generic;
using GsaGH.Components;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.Import;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Helpers.Export.GH {
  [Collection("GrasshopperFixture collection")]
  public class RhinoConversionsTests {
    [Fact]
    public void BuildArcLineCurveFromPtsAndTopoTypeTestArc1d() {
      var topolist = new List<Point3d>() {
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
      var topolist = new List<Point3d>() {
        new Point3d(0, 0, 0),
        new Point3d(2, 2, 0),
        new Point3d(4, 0, 0)
      };
      var crvs = new PolyCurve();
      crvs.Append(new Arc(topolist[0], topolist[1], topolist[2]));

      Tuple<PolyCurve, List<Point3d>, List<string>> mem1d = RhinoConversions.ConvertMem1dCrv(
        crvs);

      Assert.True(mem1d.Item1.IsArc());
      Assert.Equal(3, mem1d.Item2.Count);
      TestPoint(topolist[0], mem1d.Item2[0]);
      TestPoint(topolist[1], mem1d.Item2[1]);
      TestPoint(topolist[2], mem1d.Item2[2]);
      Assert.Equal(string.Empty, mem1d.Item3[0]);
      Assert.Equal("A", mem1d.Item3[1]);
      Assert.Equal(string.Empty, mem1d.Item3[2]);
    }

    private void TestPoint(Point3d expected, Point3d actual) {
      Assert.Equal(expected.X, actual.X, 10);
      Assert.Equal(expected.Y, actual.Y, 10);
      Assert.Equal(expected.Z, actual.Z, 10);
    }
  }
}
