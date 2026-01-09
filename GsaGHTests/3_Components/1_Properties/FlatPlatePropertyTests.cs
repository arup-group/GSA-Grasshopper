using GsaAPI;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysUnits;

using Xunit;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class FlatPlatePropertyTests {
    private readonly Create2dProperty _component;

    public FlatPlatePropertyTests() {
      _component = CreateProp2dTests.GetFlatPlateProperty();
    }

    [Fact]
    public void CreateFlatComponent() {
      var output = (GsaProperty2dGoo)ComponentTestHelper.GetOutput(_component);
      Assert.Equal(Property2D_Type.PLATE, output.Value.ApiProp2d.Type);
      Assert.Equal(new Length(14, LengthUnit.Inch), output.Value.Thickness);
    }
  }
}
