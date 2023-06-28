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
      return comp;
    }

    [Fact]
    public void CreateComponent() {
      GH_OasysDropDownComponent comp = ComponentMother();

      var output = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(MatType.Concrete, output.Value.MaterialType);

      var expected = new GsaMaterial(MatType.Concrete, "C30/37", "EC2-1-1");

      Duplicates.AreEqual(expected, output.Value);
    }
  }
}
