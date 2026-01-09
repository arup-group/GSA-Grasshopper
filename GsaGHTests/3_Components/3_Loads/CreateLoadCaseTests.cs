using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Components.Loads {
  [Collection("GrasshopperFixture collection")]
  public class CreateLoadCaseTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateLoadCase();
      comp.CreateAttributes();
      comp.SetSelected(0, 4); // live
      ComponentTestHelper.SetInput(comp, 5, 0);
      ComponentTestHelper.SetInput(comp, "Live load", 1);

      return comp;
    }

    [Fact]
    public void CreateComponentTest() {
      GH_OasysDropDownComponent comp = ComponentMother();

      var output = (GsaLoadCaseGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(5, output.Value.Id);
      Assert.Equal("Live", output.Value.LoadCase.CaseType.ToString());
      Assert.Equal("Live load", output.Value.LoadCase.Name);
    }
  }
}
