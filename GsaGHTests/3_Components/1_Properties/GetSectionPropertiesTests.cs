using Grasshopper.Kernel.Types;

using GsaGH.Components;

using GsaGHTests.Helpers;

using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units.Helpers;

using OasysUnits.Units;

using Xunit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class GetSectionPropertiesTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new SectionProperties();
      comp.CreateAttributes();
      return comp;
    }

    [Theory]
    [InlineData("STD I 1000 500 15 25",
      0.03925,
      0.00701443,
      521.101E-6,
      0.0,
      0.00701443,
      521.101E-6,
      0.0,
      0.530786,
      0.357855,
      0.530786,
      0.357855,
      6.10239E-6,
      244.096E-6,
      0.0140289,
      0.00208440,
      0.0155719,
      0.00317844,
      0.0,
      0.0,
      0.422743,
      0.115223,
      3.97000,
      0.03925,
      LengthUnit.Meter)]
    public void CreateComponent(string profile, double expectedArea, double expectedIyy,
      double expectedIzz, double expectedIyz, double expectedIuu, double expectedIvv,
      double expectedAngle, double expectedKyy, double expectedKzz, double expectedKuu,
      double expectedKvv, double expectedJ, double expectedC, double expectedZy,
      double expectedZz, double expectedZpy, double expectedZpz, double expectedCy,
      double expectedCz, double expectedRy, double expectedRz, double expectedSperL,
      double expectedVperL, LengthUnit lengthUnit) {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, profile);

      var area = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 0);
      var iyy = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 1);
      var izz = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 2);
      var iyz = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 3);
      var iuu = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 4);
      var ivv = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 5);
      var angle = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 6);
      var kyy = (GH_Number)ComponentTestHelper.GetOutput(comp, 7);
      var kzz = (GH_Number)ComponentTestHelper.GetOutput(comp, 8);
      var kuu = (GH_Number)ComponentTestHelper.GetOutput(comp, 9);
      var kvv = (GH_Number)ComponentTestHelper.GetOutput(comp, 10);
      var j = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 11);
      var c = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 12);
      var zy = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 13);
      var zz = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 14);
      var zpy = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 15);
      var zpz = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 16);
      var cy = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 17);
      var cz = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 18);
      var ry = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 19);
      var rz = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 20);
      var sl = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 21);
      var vl = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 22);

      AreaUnit areaUnit = UnitsHelper.GetAreaUnit(lengthUnit);
      AreaMomentOfInertiaUnit inertiaUnit = UnitsHelper.GetAreaMomentOfInertiaUnit(lengthUnit);
      SectionModulusUnit sectionModulusUnit = UnitsHelper.GetSectionModulusUnit(lengthUnit);
      VolumePerLengthUnit volumePerLengthUnit = UnitsHelper.GetVolumePerLengthUnit(lengthUnit);

      Assert.Equal(expectedArea, area.Value.As(areaUnit), 5);
      Assert.Equal(expectedIyy, iyy.Value.As(inertiaUnit), 8);
      Assert.Equal(expectedIzz, izz.Value.As(inertiaUnit), 9);
      Assert.Equal(expectedIyz, iyz.Value.As(inertiaUnit), 6);
      Assert.Equal(expectedIuu, iuu.Value.As(inertiaUnit), 8);
      Assert.Equal(expectedIvv, ivv.Value.As(inertiaUnit), 9);
      Assert.Equal(expectedAngle, angle.Value.Value, 6);

      Assert.Equal(expectedKyy, kyy.Value, 6);
      Assert.Equal(expectedKzz, kzz.Value, 6);
      Assert.Equal(expectedKuu, kuu.Value, 6);
      Assert.Equal(expectedKvv, kvv.Value, 6);

      Assert.Equal(expectedJ, j.Value.As(inertiaUnit), 12);
      Assert.Equal(expectedC, c.Value.As(sectionModulusUnit), 9);

      Assert.Equal(expectedZy, zy.Value.As(sectionModulusUnit), 7);
      Assert.Equal(expectedZz, zz.Value.As(sectionModulusUnit), 7);
      Assert.Equal(expectedZpy, zpy.Value.As(sectionModulusUnit), 7);
      Assert.Equal(expectedZpz, zpz.Value.As(sectionModulusUnit), 7);

      Assert.Equal(expectedCy, cy.Value.As(lengthUnit), 6);
      Assert.Equal(expectedCz, cz.Value.As(lengthUnit), 6);
      Assert.Equal(expectedRy, ry.Value.As(lengthUnit), 6);
      Assert.Equal(expectedRz, rz.Value.As(lengthUnit), 6);

      Assert.Equal(expectedSperL, sl.Value.As(lengthUnit), 5);
      Assert.Equal(expectedVperL, vl.Value.As(volumePerLengthUnit), 5);
    }
  }
}
