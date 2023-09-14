using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Rhino.Geometry;
using Xunit;

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
  }
}
