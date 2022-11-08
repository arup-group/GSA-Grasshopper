using Grasshopper.Kernel.Types;
using GsaGH.Components;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.Components
{
  [Collection("GrasshopperFixture collection")]
  public class CreateProfileTests
  {
    public static GH_OasysDropDownComponent ComponentMother()
    {
      var comp = new CreateProfile();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void CreateComponent()
    {
      var comp = ComponentMother();

      comp.SetSelected(0, 2); // set profile type to "Channel"
      comp.SetSelected(1, 4); // set measure to "ft"
      comp.UpdateUI();

      ComponentTestHelper.SetInput(comp, 1, 0);
      ComponentTestHelper.SetInput(comp, 2, 1);
      ComponentTestHelper.SetInput(comp, 3, 2);
      ComponentTestHelper.SetInput(comp, 4, 3);

      GH_String output = (GH_String)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("STD CH(ft) 1 2 3 4", output.Value);
    }
  }
}
