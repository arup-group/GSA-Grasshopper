using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;
using static GsaGH.Parameters.GsaMaterial;

namespace GsaGHTests.Components.Properties
{
  [Collection("GrasshopperFixture collection")]
  public class CreateElement2dTests
  {
    public static GH_OasysComponent ComponentMother()
    {
      var comp = new CreateElement1d();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void CreateComponentTest()
    {
      var comp = ComponentMother();

      GsaMaterialGoo output = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(0, output.Value.AnalysisProperty);
      Assert.Equal(1, output.Value.GradeProperty);
      Assert.Equal(MatType.TIMBER, output.Value.MaterialType);
    }
  }
}
