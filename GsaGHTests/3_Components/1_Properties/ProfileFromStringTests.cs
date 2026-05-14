using GsaGH.Components;

using GsaGHTests.Helpers;

using OasysGH.Components;
using OasysGH.Parameters;

using Xunit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class ProfileFromStringTests {

    public static GH_OasysComponent ComponentMother(string description, string unit = null) {
      var comp = new ProfileFromString();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, description, 0);
      if (unit != null) {
        ComponentTestHelper.SetInput(comp, unit, 1);
      }

      return comp;
    }

    [Theory]
    [InlineData("STD I(m) 0.6 0.3 0.012 0.02", null, "STD I(m) 0.6 0.3 0.012 0.02")]
    [InlineData("STD R(mm) 300 300", null, "STD R(mm) 300 300")]
    [InlineData("STD CHS(cm) 20 1.5", null, "STD CHS(cm) 20 1.5")]
    [InlineData("CAT HE HE200.B", null, "CAT HE HE200.B")]
    public void ProfileFromStringTest(string description, string unit, string expectedString) {
      var comp = ComponentMother(description, unit);

      var output = (OasysProfileGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(expectedString, output.ToString());
    }

    [Theory]
    [InlineData("STD I 0.6 0.3 0.012 0.02", "m", "STD I(m) 0.6 0.3 0.012 0.02")]
    [InlineData("STD R 300 300", "mm", "STD R(mm) 300 300")]
    public void ProfileFromStringWithFallbackUnitTest(string description, string unit, string expectedString) {
      var comp = ComponentMother(description, unit);

      var output = (OasysProfileGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(expectedString, output.ToString());
    }
  }
}
