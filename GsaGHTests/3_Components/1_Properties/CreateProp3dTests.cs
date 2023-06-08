using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;
using static GsaGH.Parameters.GsaMaterial;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class CreateProp3dTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new CreateProp3d();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, 
        (GsaMaterialGoo)ComponentTestHelper.GetOutput(CreateMaterialTests.ComponentMother()));

      return comp;
    }

    [Fact]
    public void CreateComponent() {
      GH_OasysComponent comp = ComponentMother();

      var output = (GsaProp3dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(MatType.Timber, output.Value.Material.MaterialType);
    }
  }
}
