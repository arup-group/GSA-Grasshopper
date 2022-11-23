using Grasshopper.Kernel.Types;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;
using static GsaGH.Parameters.GsaMaterial;

namespace GsaGHTests.Components.Properties
{
  [Collection("GrasshopperFixture collection")]
  public class CreateSectionTests
  {
    public static GH_OasysComponent ComponentMother()
    {
      var comp = new CreateSection();
      comp.CreateAttributes();

      GH_String profile = (GH_String)ComponentTestHelper.GetOutput(CreateProfileTests.ComponentMother(), 0);
      GsaMaterialGoo material = (GsaMaterialGoo)ComponentTestHelper.GetOutput(CreateCustomMaterialTests.ComponentMother(), 0);

      ComponentTestHelper.SetInput(comp, profile, 0);
      ComponentTestHelper.SetInput(comp, material, 1);

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
      Assert.Equal(MatType.TIMBER, output.Value.Material.MaterialType);
    }
  }
}
