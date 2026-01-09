using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class CreateProp3dTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new Create3dProperty();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp,
        (GsaMaterialGoo)ComponentTestHelper.GetOutput(CreateMaterialTests.ComponentMother()));

      return comp;
    }

    [Fact]
    public void CreateComponent() {
      GH_OasysComponent comp = ComponentMother();

      var output = (GsaProperty3dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(MatType.Concrete, output.Value.Material.MaterialType);
    }
  }
}
