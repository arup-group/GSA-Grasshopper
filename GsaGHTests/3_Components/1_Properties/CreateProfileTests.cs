using GsaGH.Components;

using GsaGHTests.Helpers;

using OasysGH.Components;
using OasysGH.Parameters;

using Xunit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class CreateProfileTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateProfile();
      comp.CreateAttributes();

      comp.SetSelected(0, 2); // set profile type to "Channel"
      comp.SetSelected(1, 4); // set measure to "ft"

      ComponentTestHelper.SetInput(comp, 1, 0);
      ComponentTestHelper.SetInput(comp, 2, 1);
      ComponentTestHelper.SetInput(comp, 3, 2);
      ComponentTestHelper.SetInput(comp, 4, 3);

      return comp;
    }

    [Fact]
    public void CreateComponentTest1() {
      GH_OasysDropDownComponent comp = ComponentMother();

      var output = (OasysProfileGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("STD CH(ft) 1 2 3 4", output.ToString());
    }

    [Theory]
    [UseCulture("zh-cn")]
    [InlineData(0, "STD CH(mm) 1 2 3 4")]
    [InlineData(1, "STD CH(cm) 1 2 3 4")]
    [InlineData(2, "STD CH(m) 1 2 3 4")]
    [InlineData(3, "STD CH(in) 1 2 3 4")]
    [InlineData(4, "STD CH(ft) 1 2 3 4")]
    public void CreateComponentTest2(int j, string profile) {
      GH_OasysDropDownComponent comp = ComponentMother();
      comp.SetSelected(1, j);

      var output = (OasysProfileGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(profile, output.ToString());
    }
  }
}
