using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaGridLineTest {
    [Fact]
    public void ConstructorTest() {
      var gridLine = new GsaGridLine(new GsaAPI.GridLine("label"));

      Assert.NotNull(gridLine._gridLine);
      Assert.Equal("label", gridLine._gridLine.Label);
    }

    [Fact]
    public void DuplicateTest() {
      var original = new GsaGridLine(new GsaAPI.GridLine("label1"));

      var duplicate = (GsaGridLine)original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate._gridLine = new GridLine("label2");

      Assert.NotNull(original._gridLine);
      Assert.Equal("label1", original._gridLine.Label);
    }
  }
}
