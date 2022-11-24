using Grasshopper.Kernel.Types;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using Xunit;
using static GsaGH.Parameters.GsaMaterial;

namespace GsaGHTests.Properties
{
  [Collection("GrasshopperFixture collection")]
  public class EditOffsetTests
  {
    public static GH_OasysComponent ComponentMother()
    {
      var comp = new EditOffset();
      comp.CreateAttributes();
      return comp;
    }

    [Theory]
    [InlineData(6.7, 4.5, -4, -3.7, LengthUnit.Centimeter)]
    [InlineData(500, 400, -350, -300.7, LengthUnit.Millimeter)]
    public void GetValuesFromExistingComponent(double x1, double x2, double y, double z, LengthUnit unit)
    {
      GsaOffset offset = new GsaOffset(x1, x2, y, z, unit);

      var comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, new GsaOffsetGoo(offset), 0);

      int i = 0;
      GsaOffsetGoo offsetGoo = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber x1_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber x2_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber y_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber z_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);

      Duplicates.AreEqual(offset, offsetGoo.Value);
      Assert.NotEqual(offset, offsetGoo.Value);
      Assert.Equal(x1, x1_out.Value.As(unit), 6);
      Assert.Equal(x2, x2_out.Value.As(unit), 6);
      Assert.Equal(y, y_out.Value.As(unit), 6);
      Assert.Equal(z, z_out.Value.As(unit), 6);
    }

    [Theory]
    [InlineData(6.7, 4.5, -4, -3.7, LengthUnit.Centimeter)]
    [InlineData(500, 400, -350, -300.7, LengthUnit.Millimeter)]
    public void EditValuesFromNewComponent(double x1, double x2, double y, double z, LengthUnit unit)
    {
      GsaOffset offset = new GsaOffset();
      var comp = ComponentMother();

      int i = 0;
      GsaOffsetGoo offsetGoo = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber x1_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber x2_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber y_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber z_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);

      Duplicates.AreEqual(offset, offsetGoo.Value);
      Assert.NotEqual(offset, offsetGoo.Value);
      Assert.Equal(0, x1_out.Value.As(unit));
      Assert.Equal(0, x2_out.Value.As(unit));
      Assert.Equal(0, y_out.Value.As(unit));
      Assert.Equal(0, z_out.Value.As(unit));

      i = 1;
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new Length(x1, unit)), i++);
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new Length(x2, unit)), i++);
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new Length(y, unit)), i++);
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new Length(z, unit)), i++);

      i = 0;
      offsetGoo = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, i++);
      x1_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      x2_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      y_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      z_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);

      Assert.NotEqual(offset, offsetGoo.Value);

      Assert.NotEqual(0, x1_out.Value.As(unit));
      Assert.NotEqual(0, x2_out.Value.As(unit));
      Assert.NotEqual(0, y_out.Value.As(unit));
      Assert.NotEqual(0, z_out.Value.As(unit));

      Assert.Equal(x1, x1_out.Value.As(unit), 6);
      Assert.Equal(x2, x2_out.Value.As(unit), 6);
      Assert.Equal(y, y_out.Value.As(unit), 6);
      Assert.Equal(z, z_out.Value.As(unit), 6);
    }
  }
}
