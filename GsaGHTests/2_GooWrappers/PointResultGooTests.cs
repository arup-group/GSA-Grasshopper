using System.Drawing;

using GsaGH.Parameters;

using OasysUnits;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class PointResultGooTests {
    [Fact]
    public void PointResultGooTest() {
      var pt = new Point3d(0, 0, 0);
      var res = new Force(10, OasysUnits.Units.ForceUnit.Kilonewton);
      Color col = Color.AliceBlue;
      var goo = new PointResultGoo(pt, res, col, 1, 1);

      Assert.Equal(3.46, goo.Boundingbox.Diagonal.Length, 2);
      Assert.Equal(3.46, goo.ClippingBox.Diagonal.Length, 2);
      Assert.Equal("A GSA result point type.", goo.TypeDescription);
      Assert.Equal("Result Point", goo.TypeName);
      Assert.StartsWith("PointResult: ", goo.ToString());

      var mesh = new Mesh();
      Assert.False(goo.CastTo(ref mesh));
    }
  }
}
