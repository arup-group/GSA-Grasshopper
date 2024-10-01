using Grasshopper.Kernel.Types;

using GsaGH.Components;

using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class MemberEndRestraintInfoTests {

    [Theory]
    [InlineData("Pinned")]
    [InlineData("Fixed")]
    [InlineData("Free")]
    [InlineData("FullRotational")]
    [InlineData("PartialRotational")]
    [InlineData("TopFlangeLateral")]
    public void ChangeShortcutDropdownTest(string expected) {
      var comp = new MemberEndRestraintInfo();
      ComponentTestHelper.SetInput(comp, expected);
      var output = (GH_String)ComponentTestHelper.GetOutput(comp);
      Assert.True(comp.RuntimeMessageLevel == Grasshopper.Kernel.GH_RuntimeMessageLevel.Blank);
    }
  }
}
