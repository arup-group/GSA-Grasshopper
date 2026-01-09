using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using OasysUnits;
using OasysUnits.Units;

using Xunit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class CreateOffsetTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateOffset();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, new Length(0.5, LengthUnit.Kilometer));
      ComponentTestHelper.SetInput(comp, new Length(-0.75, LengthUnit.Meter), 1);
      ComponentTestHelper.SetInput(comp, new Length(1.99, LengthUnit.Meter), 2);
      ComponentTestHelper.SetInput(comp, new Length(0.99, LengthUnit.Meter), 3);

      return comp;
    }

    [Fact]
    public void CreateComponent() {
      GH_OasysDropDownComponent comp = ComponentMother();

      var output = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(500, output.Value.X1.As(LengthUnit.Meter));
      Assert.Equal(-0.75, output.Value.X2.As(LengthUnit.Meter));
      Assert.Equal(1.99, output.Value.Y.As(LengthUnit.Meter));
      Assert.Equal(0.99, output.Value.Z.As(LengthUnit.Meter));
    }
  }
}
