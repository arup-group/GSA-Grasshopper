using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.Components
{
  [Collection("GrasshopperFixture collection")]
  public class CreateBucklingFactorsTests
  {
    public static GH_OasysComponent ComponentMother()
    {
      var comp = new CreateBucklingFactors();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void CreateComponent()
    {
      var comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, 0.1, 0);
      ComponentTestHelper.SetInput(comp, 0.2, 1);
      ComponentTestHelper.SetInput(comp, 1.0, 2);

      GsaBucklingLengthFactorsGoo output = (GsaBucklingLengthFactorsGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(0.1, output.Value.MomentAmplificationFactorStrongAxis);
      Assert.Equal(0.2, output.Value.MomentAmplificationFactorWeakAxis);
      Assert.Equal(1.0, output.Value.LateralTorsionalBucklingFactor);
      Assert.False(output.Value.LengthIsSet);
    }
  }
}
