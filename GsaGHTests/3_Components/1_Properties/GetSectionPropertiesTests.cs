using Grasshopper.Kernel.Types;
using GsaGH.Components;
using GsaGHTests.Helpers;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysUnits.Units;
using Xunit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class GetSectionPropertiesTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new GetSectionProperties();
      comp.CreateAttributes();
      return comp;
    }

    [Theory]
    [InlineData("STD I 1000 500 15 25", 392.5, 701443, 52110.1, 0, 610.239, 0.530786, 
      0.357855, 397.00, 0.03925, LengthUnit.Centimeter)]
    public void CreateComponent(
      string profile, double expectedArea, double expectedIyy, double expectedIzz,
      double expectedIyz, double expectedJ, double expectedKy,
      double expectedKz, double expectedSperL, double expectedVperL, LengthUnit unit) {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, profile);

      int i = 0;
      var area = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var iyy = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var izz = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var iyz = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var j = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var ky = (GH_Number)ComponentTestHelper.GetOutput(comp, i++);
      var kz = (GH_Number)ComponentTestHelper.GetOutput(comp, i++);
      var sl = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var vl = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);

      AreaUnit areaUnit = OasysGH.Units.Helpers.UnitsHelper.GetAreaUnit(unit);
      Assert.Equal(expectedArea, area.Value.As(areaUnit), 6);
      
      AreaMomentOfInertiaUnit inertiaUnit = 
        OasysGH.Units.Helpers.UnitsHelper.GetAreaMomentOfInertiaUnit(unit);
      Assert.Equal(expectedIyy, iyy.Value.As(inertiaUnit), 0);
      Assert.Equal(expectedIzz, izz.Value.As(inertiaUnit), 0);
      Assert.Equal(expectedIyz, iyz.Value.As(inertiaUnit), 0);
      Assert.Equal(expectedJ, j.Value.As(inertiaUnit), 3);
      
      Assert.Equal(expectedKy, ky.Value, 6);
      Assert.Equal(expectedKz, kz.Value, 6);

      Assert.Equal(expectedSperL, sl.Value.Value, 2);
      Assert.Equal(expectedVperL, vl.Value.Value, 5);
    }
  }
}
