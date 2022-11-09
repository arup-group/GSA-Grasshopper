using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;
using static GsaGH.Parameters.GsaMaterial;

namespace GsaGHTests.Components
{
  [Collection("GrasshopperFixture collection")]
  public class CreateSectionTests
  {
    public static GH_OasysComponent ComponentMother()
    {
      var comp = new CreateSection();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, CreateProfileTests.ComponentMother(), 0);
      ComponentTestHelper.SetInput(comp, CreateCustomMaterialTests.ComponentMother(), 1);

      return comp;
    }

    [Fact]
    public void CreateComponent()
    {
      // Arrange & Act
      var comp = ComponentMother();

      // Assert
      GsaSectionGoo output = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("STD CH(ft) 1 2 3 4", output.Value.Profile);
      Assert.Equal(MatType.CONCRETE, output.Value.Material.MaterialType);
    }
  }
}
