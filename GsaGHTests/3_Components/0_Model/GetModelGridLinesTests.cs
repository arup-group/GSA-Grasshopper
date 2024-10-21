using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.Model {
  [Collection("GrasshopperFixture collection")]
  public class GetModelGridLinesTests {
    [Fact]
    public static void GetModelGridLinesTest() {
      // Assemble
      var comp = new GetModelGridLines();

      // Act
      ComponentTestHelper.SetInput(comp, CreateGridLineTest.CreateModelWithGridlines());
      var outputGoo = (GsaGridLineGoo)ComponentTestHelper.GetOutput(comp);

      // Assert
      Assert.NotNull(outputGoo);
      Assert.Equal(0, outputGoo.Value.GridLine.X, 12);
      Assert.Equal(0, outputGoo.Value.GridLine.Y, 12);
      Assert.Equal(1.4142135623730951, outputGoo.Value.GridLine.Length, 12);
      Assert.Equal(45, outputGoo.Value.GridLine.Theta1, 12);
    }
  }
}
