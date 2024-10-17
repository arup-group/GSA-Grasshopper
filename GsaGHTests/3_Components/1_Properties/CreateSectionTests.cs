using GsaAPI;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class CreateSectionTests {

    public static GH_OasysComponent ComponentMother(string profile) {
      var comp = new CreateSection();
      comp.CreateAttributes();

      var material
        = (GsaMaterialGoo)ComponentTestHelper.GetOutput(CreateCustomMaterialTests.ComponentMother(), 0);

      ComponentTestHelper.SetInput(comp, profile, 0);
      ComponentTestHelper.SetInput(comp, material, 1);

      return comp;
    }

    [Theory]
    [InlineData("STD CH(m) 40 30 2 1")]
    [InlineData("STD CH(cm) 40 30 2 1")]
    [InlineData("STD CH(mm) 40 30 2 1")]
    [InlineData("STD CH(ft) 40 30 2 1")]
    [InlineData("STD CH(in) 40 30 2 1")]
    public void CreateComponentTest1(string profile) {
      GH_OasysComponent comp = ComponentMother(profile);

      var output = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(profile, output.Value.ApiSection.Profile);
      Assert.Equal(MatType.Timber, output.Value.Material.MaterialType);
      Assert.Equal(BasicOffset.Centroid, output.Value.ApiSection.BasicOffset);
    }
  }
}
