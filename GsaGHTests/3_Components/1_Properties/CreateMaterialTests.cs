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
      // Arrange & Act
      GH_OasysDropDownComponent comp = ComponentMother();

      // Assert
      var output = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(0, output.Value.AnalysisProperty);
      Assert.Equal(1, output.Value.GradeProperty);
      Assert.Equal(MatType.Timber, output.Value.MaterialType);
    }

    //[Fact]
    //public void CreateComponentWithInputs1()
    //{
    //  var comp = ComponentMother();

    //  ComponentTestHelper.SetInput(comp, 6, 0);
    //  ComponentTestHelper.SetInput(comp, 7, 1);

    //  GsaMaterialGoo output = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp);
    //  Assert.Equal(6, output.Value.AnalysisProperty);
    //  Assert.Equal(7, output.Value.GradeProperty);
    //  Assert.Equal(MatType.UNDEF, output.Value.MaterialType);
    //}
  }
}
