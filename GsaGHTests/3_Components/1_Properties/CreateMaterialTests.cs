using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;
using static GsaGH.Parameters.GsaMaterial;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class CreateMaterialTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateMaterial();
      comp.CreateAttributes();

      comp.SetSelected(0, 3); // set dropdown to "Timber"

      return comp;
    }

    [Fact]
    public void CreateComponent() {
      GH_OasysDropDownComponent comp = ComponentMother();

      var output = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(0, output.Value.Id);
      Assert.Equal(1, output.Value.Id);
      Assert.Equal(MaterialType.Timber, output.Value.MaterialType);
    }
  }
}
