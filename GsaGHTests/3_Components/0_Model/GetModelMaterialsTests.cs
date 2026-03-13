using Grasshopper.Kernel;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helper;
using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.Model {
  [Collection("GrasshopperFixture collection")]
  public class GetModelMaterialsTests {
    [Fact(Skip = "Requires a GSA file with unsupported materials (fabric/orthotropic). File to be provided later.")]
    public void ShouldWarnForUnsupportedMaterials() {
      // Arrange
      var comp = new GetModelMaterials();
      comp.CreateAttributes();
      var model = new GsaModel();
      model.ApiModel.Open(GsaFile.UnsupportedMaterials);

      // Act
      ComponentTestHelper.SetInput(comp, new GsaModelGoo(model));
      ComponentTestHelper.ComputeOutput(comp);

      // Assert
      var warnings = comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning);
      Assert.NotEmpty(warnings);
      foreach (string warning in warnings) {
        Assert.Contains("was not imported", warning);
      }
    }
  }
}
