using GsaAPI;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class CreateProp2dTests {
    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateProp2d();
      comp.CreateAttributes();

      comp.SetSelected(0, 2); // set type to "Flat Plate"
      comp.SetSelected(1, 3); // set measure to "in"

      ComponentTestHelper.SetInput(comp, 14, 0);
      ComponentTestHelper.SetInput(comp, CreateCustomMaterialTests.ComponentMother(), 1);

      return comp;
    }

    [Fact]
    public void CreateComponent() {
      GH_OasysDropDownComponent comp = ComponentMother();

      var output = (GsaProp2dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(Property2D_Type.PLATE, output.Value.Type);
      Assert.Equal(new Length(14, LengthUnit.Inch), output.Value.Thickness);
    }
  }
}
