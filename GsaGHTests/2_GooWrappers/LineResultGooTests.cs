using System.Drawing;

using GsaGH.Parameters;

using OasysUnits;

using Rhino.Geometry;

using Xunit;

using Line = Rhino.Geometry.Line;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class LineResultGooTests {
    [Fact]
    public void LineResultGooTest() {
      var ln = new Line(new Point3d(0, 0, 0), new Point3d(1, 0, 0));
      var res = new Force(10, OasysUnits.Units.ForceUnit.Kilonewton);
      Color col = Color.AliceBlue;
      var goo = new LineResultGoo(ln, res, res, col, col, 1, 1, 1);

      Assert.Equal(1, goo.Boundingbox.Diagonal.Length);
      Assert.Equal(1, goo.ClippingBox.Diagonal.Length);
      Assert.Equal("A GSA result line type.", goo.TypeDescription);
      Assert.Equal("Result Line", goo.TypeName);
      Assert.StartsWith("LineResult: ", goo.ToString());

      var mesh = new Mesh();
      Assert.False(goo.CastTo(ref mesh));
    }
  }
}
