using System.Collections.Generic;

using GsaGH.Parameters;

using OasysUnits;

using Rhino.Collections;
using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class MeshResultGooTests {
    [Fact]
    public void MeshResultGooTest() {
      var pts = new Point3dList {
        new Point3d(-3, -4, 0),
        new Point3d(5, -2, 0),
        new Point3d(6, 7, 0),
        new Point3d(-1, 2, 0),
      };
      pts.Add(pts[0]);
      var pol = new Polyline(pts);
      var mesh = Mesh.CreateFromPlanarBoundary(pol.ToPolylineCurve(),
        MeshingParameters.DefaultAnalysisMesh, 0.001);
      var res = new List<IList<IQuantity>>(){
        new List<IQuantity>(){
          new Force(10, OasysUnits.Units.ForceUnit.Kilonewton)
          }
        };
      var ptList = new List<Point3dList>() {
        new Point3dList(pts)
      };
      var ids = new List<int> { 1, 2, 3 };
      var goo = new MeshResultGoo(mesh, res, ptList, ids);

      Assert.Equal(14.21, goo.Boundingbox.Diagonal.Length, 2);
      Assert.Equal(14.21, goo.ClippingBox.Diagonal.Length, 2);
      Assert.Equal("A GSA result mesh type.", goo.TypeDescription);
      Assert.Equal("Result Mesh", goo.TypeName);
      Assert.StartsWith("MeshResult: ", goo.ToString());

      var pt = new Point3d(-3, -4, 0);
      Assert.False(goo.CastTo(ref pt));
    }
  }
}
