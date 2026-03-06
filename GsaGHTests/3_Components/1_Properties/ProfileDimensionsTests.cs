using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Components;

using GsaGHTests.Helpers;

using OasysGH.Components;
using OasysGH.Parameters;

using OasysUnits.Units;

using Xunit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class ProfileDimensionsTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new ProfileDimensions();
      comp.CreateAttributes();
      return comp;
    }

    [Theory]
    [InlineData("STD A(cm) 25 30 4.5 6.7", 25, 30, 30, 30, 6.7, 6.7, 4.5, -1, -1, LengthUnit.Centimeter)]
    [InlineData("STD CH 100 200 10 15", 100, 200, 200, 200, 15, 15, 10, -1, -1, LengthUnit.Millimeter)]
    [InlineData("STD CHS 100 20", 100, -1, -1, -1, -1, -1, 20, -1, -1, LengthUnit.Millimeter)]
    [InlineData("STD C 100", 100, -1, -1, -1, -1, -1, -1, -1, -1, LengthUnit.Millimeter)]
    [InlineData("STD X 100 200 7 8", 100, 200, 200, 200, 8, 8, 7, -1, -1, LengthUnit.Millimeter)]
    [InlineData("STD OVAL 100 200 3", 100, 200, 200, 200, 3, 3, 3, -1, -1, LengthUnit.Millimeter)]
    [InlineData("STD E 100 200 2", 100, 200, 200, 200, -1, -1, -1, -1, -1, LengthUnit.Millimeter)]
    [InlineData("STD GC 100 200 7 8", 100, 200, 200, 200, 7, 7, 8, -1, -1, LengthUnit.Millimeter)]
    [InlineData("STD GZ 300 150 200 15 10 5", 300, 350, 150, 200, 15, 10, 5, -1, -1, LengthUnit.Millimeter)]
    [InlineData("STD GI 900 200 400 15 40 30", 900, 400, 200, 400, 40, 30, 15, -1, -1, LengthUnit.Millimeter)]
    [InlineData("STD CB 900 400 15 40 400 1500", 900, 400, 400, 400, 40, 40, 15, 400, 1500, LengthUnit.Millimeter)]
    [InlineData("STD I 1000 200 7 8", 1000, 200, 200, 200, 8, 8, 7, -1, -1, LengthUnit.Millimeter)]
    [InlineData("STD RHS 400 200 7 8", 400, 200, 200, 200, 8, 8, 7, -1, -1, LengthUnit.Millimeter)]
    [InlineData("STD R 250 300", 250, 300, 300, 300, -1, -1, -1, -1, -1, LengthUnit.Millimeter)]
    [InlineData("STD RE 400 250 350 200 2", 400, 350, 250, 200, -1, -1, -1, -1, -1, LengthUnit.Millimeter)]
    [InlineData("STD SP 250 150 3", 250, 550, -1, -1, -1, -1, -1, -1, 150, LengthUnit.Millimeter)]
    [InlineData("STD SPW 250 150 4", 250, 600, -1, -1, -1, -1, -1, -1, 150, LengthUnit.Millimeter)]
    [InlineData("STD SHT 500 600 150 250 15 10", 500, 600, 150, 250, 15, 15, 10, -1, -1, LengthUnit.Millimeter)]
    [InlineData("STD RC 250 300", 250, 300, -1, -1, -1, -1, -1, -1, -1, LengthUnit.Millimeter)]
    [InlineData("STD TR 250 150 300", 250, 300, 150, 300, -1, -1, -1, -1, -1, LengthUnit.Millimeter)]
    [InlineData("STD T 250 150 7 15", 250, 150, 150, 7, 15, -1, 7, -1, -1, LengthUnit.Millimeter)]
    [InlineData("CAT BSI-IPE IPE100", 100.0, 55, 55, 55, 5.7, 5.7, 4.1, 7, -1, LengthUnit.Millimeter)]
    [InlineData("CAT EN-SHS SHS400x400x16", 400, 400, 400, 400, -1, -1, 16, 0, -1, LengthUnit.Millimeter)]
    [InlineData("CAT EN-RHS RHS400X200X7.1", 400, 200, 200, 200, -1, -1, 7.1, 0, -1, LengthUnit.Millimeter)]
    [InlineData("CAT PX PX0.5", 21.336, 3.7338, -1, -1, -1, -1, 3.7338, -1, -1, LengthUnit.Millimeter)]
    public void CreateComponent(
      string profile, double expectedDepth, double expectedWidth, double expectedWidthTop, double expectedWidthBottom,
      double expectedFlngThkTop, double expectedFlngThkBtm, double expectedWebThk, double expectedRootRadius,
      double expectedSpacing, LengthUnit unit) {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, profile);

      int i = 0;
      var depth = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var width = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var widthTop = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var widthBottom = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var flngThkTop = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var flngThkBot = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var webThk = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var radius = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var spacing = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      string type = ((GH_String)ComponentTestHelper.GetOutput(comp, i)).Value;
      string expectedType = profile.Split(' ')[1];
      if (profile.StartsWith("CAT")) {
        expectedType = "CAT " + expectedType;
      }

      Assert.Equal(expectedDepth, depth.Value.As(unit), 6);
      if (expectedWidth >= 0) {
        Assert.Equal(expectedWidth, width.Value.As(unit), 6);
      } else {
        Assert.Null(width);
      }

      if (expectedWidthTop >= 0) {
        Assert.Equal(expectedWidthTop, widthTop.Value.As(unit), 6);
      } else {
        Assert.Null(widthTop);
      }

      if (expectedWidthBottom >= 0) {
        Assert.Equal(expectedWidthBottom, widthBottom.Value.As(unit), 6);
      } else {
        Assert.Null(widthBottom);
      }

      if (expectedFlngThkTop >= 0) {
        Assert.Equal(expectedFlngThkTop, flngThkTop.Value.As(unit), 6);
      } else {
        Assert.Null(flngThkTop);
      }

      if (expectedFlngThkBtm >= 0) {
        Assert.Equal(expectedFlngThkBtm, flngThkBot.Value.As(unit), 6);
      } else {
        Assert.Null(flngThkBot);
      }

      if (expectedWebThk >= 0) {
        Assert.Equal(expectedWebThk, webThk.Value.As(unit), 6);
      } else {
        Assert.Null(webThk);
      }

      if (expectedRootRadius >= 0) {
        Assert.Equal(expectedRootRadius, radius.Value.As(unit), 6);
      } else {
        Assert.Null(radius);
      }

      if (expectedSpacing >= 0) {
        Assert.Equal(expectedSpacing, spacing.Value.As(unit), 6);
      } else {
        Assert.Null(spacing);
      }

      Assert.StartsWith(type, expectedType);
    }

    [Theory]
    [InlineData("EXP 8 2000 400 200 400 600")]
    [InlineData("invalid")]
    public void CreateInvalidComponent(string profile) {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, profile);
      ComponentTestHelper.GetOutput(comp);

      Assert.NotEmpty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void UpdateCustomUITest() {
      var comp = (ProfileDimensions)ComponentMother();
      comp.Update("ft");
      Assert.Equal("ft", comp.Message);
    }
  }
}
