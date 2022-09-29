using Xunit;
using ComposGHTests.Helpers;
using OasysGH.Components;
using GsaGH.Components;
using GsaGH.Parameters;
using static GsaGH.Parameters.GsaMaterial;
using OasysGH.Helpers;

namespace ComposGHTests.Slab
{
  [Collection("GrasshopperFixture collection")]
  public class CreateMaterialTestss
  {
    public static GH_OasysComponent ComponentMother()
    {
      var comp = new CreateMaterial();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void CreateComponent()
    {
      var comp = ComponentMother();

      GsaMaterialGoo output = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(0, output.Value.AnalysisProperty);
      Assert.Equal(1, output.Value.GradeProperty);
      Assert.Equal(MatType.UNDEF, output.Value.MaterialType);
    }

    [Fact]
    public void CreateComponentWithInputs1()
    {
      var comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, 6, 0);
      ComponentTestHelper.SetInput(comp, 7, 1);

      GsaMaterialGoo output = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(6, output.Value.AnalysisProperty);
      Assert.Equal(7, output.Value.GradeProperty);
      Assert.Equal(MatType.UNDEF, output.Value.MaterialType);
    }
  }
}
