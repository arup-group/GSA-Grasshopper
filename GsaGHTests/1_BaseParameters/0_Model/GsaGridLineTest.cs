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

      Assert.NotNull(gridLine._gridLine);
      Assert.Equal("label", gridLine._gridLine.Label);
    }

    [Fact]
    public void CloneTest() {
      var original = new GsaGridLine(new GridLine("label1"), new PolyCurve());

      var duplicate = (GsaGridLine)original.Duplicate();

      Assert.Equal(original, duplicate);
    }

    [Fact]
    public void DuplicateTest() {
      var original = new GsaGridLine(new GridLine("label1"), new PolyCurve());

      var duplicate = (GsaGridLine)original.Clone();

      Duplicates.AreEqual(original, duplicate);

      duplicate._gridLine = new GridLine("label2");
      duplicate._curve = new PolyCurve();

      Assert.NotNull(original._gridLine);
      Assert.Equal("label1", original._gridLine.Label);
    }
  }
}
