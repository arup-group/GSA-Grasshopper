using Grasshopper.Kernel.Types;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class EffectiveLengthInfoTests {

    [Theory]
    [InlineData(0, "Automatic")]
    [InlineData(1, "InternalRestraints")]
    [InlineData(2, "UserSpecified")]
    public void AutomaticTypeTest(int index, string expected) {
      var comp = new EffectiveLengthInfo();
      comp.CreateAttributes();

      var leffComponent = new CreateEffectiveLengthOptions();
      leffComponent.CreateAttributes();
      leffComponent.SetSelected(0, index);
      var input = (GsaEffectiveLengthGoo)ComponentTestHelper.GetOutput(leffComponent);

      ComponentTestHelper.SetInput(comp, input);
      var output = (GH_String)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(expected, output.Value);
    }
  }
}
