using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class CreateBucklingFactorsTests {
    public static GH_OasysComponent ComponentMother() {
      var comp = new CreateBucklingFactors();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, 0.1, 0);
      ComponentTestHelper.SetInput(comp, 0.2, 1);
      ComponentTestHelper.SetInput(comp, 1.0, 2);

      return comp;
    }

    [Fact]
    public void CreateComponent() {
      GH_OasysComponent comp = ComponentMother();

      var output = (GsaBucklingLengthFactorsGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(0.1, output.Value.MomentAmplificationFactorStrongAxis);
      Assert.Equal(0.2, output.Value.MomentAmplificationFactorWeakAxis);
      Assert.Equal(1.0, output.Value.EquivalentUniformMomentFactor);
    }
  }
}
