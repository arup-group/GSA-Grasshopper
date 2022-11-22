using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using OasysUnits.Units;
using OasysUnits;
using Xunit;
using static GsaGH.Parameters.GsaMaterial;

namespace GsaGHTests.Components
{
  [Collection("GrasshopperFixture collection")]
  public class CreateProp3dTests
  {
    public static GH_OasysComponent ComponentMother()
    {
      var comp = new CreateProp3d();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, CreateMaterialTests.ComponentMother(), 0);

      return comp;
    }

    [Fact]
    public void CreateComponent()
    {
      // Arrange & Act
      var comp = ComponentMother();

      // Assert
      GsaProp3dGoo output = (GsaProp3dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(MatType.CONCRETE, output.Value.Material.MaterialType);
    }
  }
}
