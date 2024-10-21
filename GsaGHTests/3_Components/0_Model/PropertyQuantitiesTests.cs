using Grasshopper.Kernel.Data;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helper;
using GsaGHTests.Helpers;

using OasysGH.Parameters;

using Xunit;

namespace GsaGHTests.Model {
  [Collection("GrasshopperFixture collection")]
  public class PropertyQuantitiesTests {
    [Fact]
    public static void PropertyQuantitiesTest() {
      // Assemble
      var comp = new PropertyQuantities();
      var m = new GsaAPI.Model(GsaFile.Element2dMultiPropsParentMember);
      var model = new GsaModel(m);

      // Act
      ComponentTestHelper.SetInput(comp, new GsaModelGoo(model));
      comp.SetSelected(0, 0); // analysis layer
      var quanity1 = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 1, new GH_Path(1), 0);
      var quanity2 = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 1, new GH_Path(2), 0);

      // Assert
      Assert.Equal(26.54, quanity1.Value.Value, 2);
      Assert.Equal(9.46, quanity2.Value.Value, 2);

      comp.SetSelected(0, 1); // design layer
      var quanity3 = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 1, new GH_Path(1), 0, true);
      Assert.Equal(26.54 + 9.46, quanity3.Value.Value, 2);
    }
  }
}
