using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaGridLineTest {
    [Fact]
    public void ConstructorTest() {
      var gridLine = new GsaGridLine(7, new GsaAPI.GridLine("label"));

      Assert.Equal(7, gridLine.Id);
      Assert.Equal("label", gridLine._gridLine.Label);
    }

    [Fact]
    public void DuplicateTest() {
      var original = new GsaGridLine(7, new GsaAPI.GridLine("label1"));

      var duplicate = (GsaGridLine)original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate._gridLine = new GridLine("label2");

      Assert.Equal(7, original.Id);
      Assert.Equal("label1", original._gridLine.Label);
    }
  }
}
