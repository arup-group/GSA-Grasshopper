using GsaGH.Components;
using GsaGH.Helpers;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;
using OasysGH.Parameters;

using OasysUnits;
using OasysUnits.Units;

using Xunit;

namespace GsaGHTests.Properties {
  [Collection("GrasshopperFixture collection")]
  public class EditOffsetTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new EditOffset();
      comp.CreateAttributes();
      return comp;
    }

    [Theory]
    [InlineData(6.7, 4.5, -4, -3.7, LengthUnit.Centimeter)]
    [InlineData(500, 400, -350, -300.7, LengthUnit.Millimeter)]
    public void EditValuesFromNewComponent(
      double x1, double x2, double y, double z, LengthUnit unit) {
      var offset = new GsaOffset();
      GH_OasysComponent comp = ComponentMother();

      int i = 0;
      var offsetGoo = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, i++);
      var x1Out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var x2Out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var yOut = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var zOut = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i);

      Duplicates.AreEqual(offset, offsetGoo.Value);
      Assert.NotEqual(offset, offsetGoo.Value);
      Assert.Equal(0, x1Out.Value.As(unit));
      Assert.Equal(0, x2Out.Value.As(unit));
      Assert.Equal(0, yOut.Value.As(unit));
      Assert.Equal(0, zOut.Value.As(unit));

      i = 1;
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new Length(x1, unit)), i++);
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new Length(x2, unit)), i++);
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new Length(y, unit)), i++);
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new Length(z, unit)), i);

      i = 0;
      offsetGoo = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, i++);
      x1Out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      x2Out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      yOut = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      zOut = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i);

      Assert.NotEqual(offset, offsetGoo.Value);

      Assert.NotEqual(0, x1Out.Value.As(unit), DoubleComparer.Default);
      Assert.NotEqual(0, x2Out.Value.As(unit), DoubleComparer.Default);
      Assert.NotEqual(0, yOut.Value.As(unit), DoubleComparer.Default);
      Assert.NotEqual(0, zOut.Value.As(unit), DoubleComparer.Default);

      Assert.Equal(x1, x1Out.Value.As(unit), DoubleComparer.Default);
      Assert.Equal(x2, x2Out.Value.As(unit), DoubleComparer.Default);
      Assert.Equal(y, yOut.Value.As(unit), DoubleComparer.Default); ;
      Assert.Equal(z, zOut.Value.As(unit), DoubleComparer.Default); ;
    }

    [Theory]
    [InlineData(6.7, 4.5, -4, -3.7, LengthUnit.Centimeter)]
    [InlineData(500, 400, -350, -300.7, LengthUnit.Millimeter)]
    public void GetValuesFromExistingComponent(
      double x1, double x2, double y, double z, LengthUnit unit) {
      var offset = new GsaOffset(x1, x2, y, z, unit);

      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, new GsaOffsetGoo(offset));

      int i = 0;
      var offsetGoo = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, i++);
      var x1Out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var x2Out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var yOut = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var zOut = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i);

      Duplicates.AreEqual(offset, offsetGoo.Value);
      Assert.NotEqual(offset, offsetGoo.Value);
      Assert.Equal(x1, x1Out.Value.As(unit), DoubleComparer.Default);
      Assert.Equal(x2, x2Out.Value.As(unit), DoubleComparer.Default);
      Assert.Equal(y, yOut.Value.As(unit), DoubleComparer.Default);
      Assert.Equal(z, zOut.Value.As(unit), DoubleComparer.Default);
    }
  }
}
